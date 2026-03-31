using System.Collections.Immutable;
using System.Globalization;

using CommonFramework;
using CommonFramework.Maybe;
using CommonFramework.Parsing;

using OData.Domain.QueryLanguage;
using OData.Domain.QueryLanguage.Constant;
using OData.Domain.QueryLanguage.Constant.Base;
using OData.Domain.QueryLanguage.Operations;

using ExpressionParser = CommonFramework.Parsing.Parser<CommonFramework.Parsing.SharedMemoryString, OData.Domain.QueryLanguage.Expression>;
using MapFunc = System.Func<OData.Domain.QueryLanguage.Expression, OData.Domain.QueryLanguage.Expression>;
using ExpressionMapParser = CommonFramework.Parsing.Parser<CommonFramework.Parsing.SharedMemoryString, System.Func<OData.Domain.QueryLanguage.Expression, OData.Domain.QueryLanguage.Expression>>;

using SExpressions = System.Linq.Expressions;

namespace OData.InternalParser;

public class LambdaExpressionInternalParser(
    CultureInfo culture,
    ParameterExpression currentParameter,
    ImmutableArray<ParameterExpression> usedParameters)
    : CharParsers(culture)
{
    private const char EscapeChar = (char)27;

    private const string PostEscapeChars = "'";


    private ExpressionParser BracketsRootParser => this.BetweenBrackets(this.RootBodyParser);

    public ExpressionParser RootBodyParser => this.MainParser.Pipe(this.InnerRootBodyParser);

    private ExpressionParser MainParser => this.GetMainParser((expression, _) => expression);

    private Parser<SharedMemoryString, TResult> GetMainParser<TResult>(Func<Expression, bool, TResult> resultSelector) =>
        this.OneOfMany(
            this.GetLazy(() => this.BracketsRootParser.Select(res => resultSelector(res, true))),
            this.GetLazy(() => this.UnaryExpressionParser.Select(res => resultSelector(res, false))),
            this.GetLazy(() => this.OtherExpressionParser.Select(res => resultSelector(res, false))));

    private LambdaExpressionInternalParser GetSubParser(ParameterExpression subParameter) =>
        new(culture, subParameter, [.. usedParameters, subParameter]);

    private ExpressionMapParser InnerRootBodyParser =>
        this.PreSpaces(this.BinaryExpressionParser.Compose(this.GetLazy(() => this.InnerRootBodyParser)))
            .Or(this.GetIdentityFunc<Expression>());

    private ExpressionParser UnaryExpressionParser
    {
        get
        {
            Func<UnaryExpression, Expression> applyPriorityFunc = sourceExpr =>
            {
                var req =

                    from changeExpr in (sourceExpr.Operand as BinaryExpression).ToMaybe()

                    where changeExpr.Operation.GetPriority() < sourceExpr.Operation.GetPriority()

                    let newLeft = new UnaryExpression(sourceExpr.Operation, changeExpr.Left)

                    select new BinaryExpression(changeExpr.Operation, newLeft, changeExpr.Right);

                return req.GetValueOrDefault((Expression)sourceExpr);
            };


            return from unaryOperation in this.PreSpaces(this.UnaryOperationParser)

                from rootResult in this.GetMainParser((operand, isRoot) => new { Operand = operand, IsRoot = isRoot })

                from mapOperandFunc in this.PreSpaces(this.BinaryExpressionParser).Or(this.GetIdentityFunc<Expression>())

                from overridePriorityFunc in rootResult.IsRoot ? this.GetIdentityFunc<Expression, UnaryExpression>() : this.Return(applyPriorityFunc)

                let baseExpression = new UnaryExpression(unaryOperation, mapOperandFunc(rootResult.Operand))

                select overridePriorityFunc(baseExpression);
        }
    }

    private ExpressionMapParser BinaryExpressionParser
    {
        get
        {
            Func<BinaryExpression, Expression> applyPriorityFunc = sourceExpr =>
            {
                var req =

                    from changeExpr in (sourceExpr.Right as BinaryExpression).ToMaybe()

                    where changeExpr.Operation.GetPriority() < sourceExpr.Operation.GetPriority()

                    let newLeft = new BinaryExpression(sourceExpr.Operation, sourceExpr.Left, changeExpr.Left)

                    select new BinaryExpression(changeExpr.Operation, newLeft, changeExpr.Right);

                return req.GetValueOrDefault(sourceExpr);
            };


            return from binaryOperation in this.BinaryOperationParser

                from rootResult in this.GetMainParser((right, isRoot) => new { Right = right, IsRoot = isRoot })

                from mapRightFunc in this.PreSpaces(this.BinaryExpressionParser).Or(this.GetIdentityFunc<Expression>())

                from overridePriorityFunc in rootResult.IsRoot ? this.GetIdentityFunc<Expression, BinaryExpression>() : this.Return(applyPriorityFunc)

                select new MapFunc(left =>
                {
                    var baseExpression = new BinaryExpression(binaryOperation, left, mapRightFunc(rootResult.Right));

                    return overridePriorityFunc(baseExpression);
                });
        }
    }

    private ExpressionParser OtherExpressionParser => this.PreSpaces(this.InnerOtherExpressionParser);

    private ExpressionParser InnerOtherExpressionParser =>
        this.StringMethodExpressionParser
            .Or(() => this.CollectionMethodExpressionParser)
            .Or(() => this.GuidConstantExpressionParser)
            .Or(() => this.DateTimeConstantExpressionParser)
            .Or(() => this.BooleanConstantExpressionParser)
            .Or(() => this.DecimalConstantExpressionParser)
            .Or(() => this.Int32ConstantExpressionParser)
            .Or(() => this.Int64ConstantExpressionParser)
            .Or(() => this.StringConstantExpressionParser)
            .Or(() => this.NullConstantExpressionParser)
            .Or(() => this.StringConstantExpressionParser)
            .Or(() => this.DateTimePropertyParser)
            .Or(() => this.LengthPropertyParser)
            .Or(() => this.PropertyExpressionParser);

    private ExpressionParser StringMethodExpressionParser =>

        from methodType in this.StringMethodExpressionTypeParser

        from result in

            this.BetweenBrackets(

                from arg1 in this.RootBodyParser

                from _ in this.PreSpaces(this.Char(','))

                from arg2 in this.RootBodyParser

                select methodType == MethodExpressionType.StringContains

                    ? new MethodExpression(arg2, methodType, [arg1])

                    : new MethodExpression(arg1, methodType, [arg2]))

        select (Expression)result;

    private ExpressionParser GuidConstantExpressionParser =>

        from value in this.Between(this.GuidParser, this.StringIgnoreCase("guid'"), this.Char('\''))

        select (Expression)new GuidConstantExpression(value);

    private ExpressionParser DateTimeConstantExpressionParser =>

        from _ in this.StringIgnoreCase("datetime'")

        from dateStr in this.TakeTo("'")

        from res in this.SpanParser<DateTime>(dateStr)

        select (Expression)new DateTimeConstantExpression(res);


    private ExpressionParser BooleanConstantExpressionParser =>

        from value in this.BooleanParser

        select (Expression)new BooleanConstantExpression(value);

    private ExpressionParser StringConstantExpressionParser =>

        from _ in this.Char('\'')

        from value in this.Many(this.EscapeCharParser.Or(() => this.Char(c => c != '\'')))

        from __ in this.Char('\'')

        select (Expression)new StringConstantExpression(new string(value.AsSpan()));

    private Parser<SharedMemoryString, char> EscapeCharParser =>
        from ___ in this.Char(EscapeChar)

        from c in this.OneOfMany(PostEscapeChars.Select(this.Char))

        select c;

    public ExpressionParser PropertyPathParser
    {
        get
        {
            var propWithAlias = this.BetweenBrackets(
                from propertyName in this.Variable
                from _ in this.Spaces1
                from alias in this.Variable
                select new { PropertyName = propertyName, Alias = (string?)alias },
                '[',
                ']');

            var propWithoutAlias = () => this.BetweenSpaces(this.Variable)
                .Select(propertyName => new { PropertyName = propertyName, Alias = default(string?) });

            return from properties in this.SepBy(propWithAlias.Or(propWithoutAlias), '/')

                select properties.Aggregate(
                    (Expression)currentParameter,
                    (source, propertyPair) =>

                        propertyPair.Alias == null
                            ? new PropertyExpression(source, propertyPair.PropertyName)
                            : new SelectExpression(source, propertyPair.PropertyName, propertyPair.Alias));
        }
    }

    private ExpressionParser PropertyExpressionParser =>
        from startParameterInfo in this.PropertyStartParameterExpressionParser

        from parameterNames in
            startParameterInfo.Item2
                ? this.Many(
                    from _ in this.BetweenSpaces(this.Char('/'))
                    from parameterName in this.Variable
                    select parameterName)
                : this.SepBy(this.PreSpaces(this.Variable), this.Char('/'))

        where startParameterInfo.Item2 || parameterNames.Any()

        select parameterNames.Aggregate((Expression)startParameterInfo.Item1, (expr, parameterName) => new PropertyExpression(expr, parameterName));

    private Parser<SharedMemoryString, Tuple<ParameterExpression, bool>> PropertyStartParameterExpressionParser =>

        this.StringIgnoreCase("it").Or(() => this.StringIgnoreCase("this")).Select(_ => Tuple.Create(currentParameter, true))

            .Or(() => from startElementName in this.PreSpaces(this.Variable)

                let startElementParameter = new ParameterExpression(startElementName)

                where usedParameters.Contains(startElementParameter)

                select Tuple.Create(startElementParameter, true))

            .Or(() => this.Return(Tuple.Create(currentParameter, false)));

    private ExpressionParser CollectionMethodExpressionParser =>

        from collectionMethodType in this.CollectionMethodExpressionTypeParser

        from result in

            this.BetweenBrackets(

                from source in this.RootBodyParser

                from subParameter in this.PreSpaces(this.Variable).Select(aliasName => new ParameterExpression(aliasName))
                    .Or(() => this.Return(
                        this.GenerateAnonymousParameterExpression(
                            (source as PropertyExpression).Maybe(propExpr => propExpr.PropertyName.FromPluralize()) ?? "collection")))

                from _ in this.PreSpaces(this.Char(','))

                from bodyExpr in this.GetSubParser(subParameter).RootBodyParser

                let arg = new LambdaExpression(bodyExpr, [subParameter])

                select new MethodExpression(source, collectionMethodType, [arg]))

        select (Expression)result;

    private ExpressionParser DateTimePropertyParser =>

        this.OneOfMany(

            DateTimeProperties.Select(propertyName =>

                from _ in this.StringIgnoreCase(propertyName)
                from source in this.BetweenBrackets(this.RootBodyParser)
                select (Expression)new PropertyExpression(source, propertyName)));


    private ExpressionParser LengthPropertyParser =>

        from body in this.Pre(this.BetweenBrackets(this.RootBodyParser), this.StringIgnoreCase("length"))

        select (Expression)new PropertyExpression(body, "Length");


    private Parser<SharedMemoryString, decimal> DecimalParser =>

        from isNegate in this.TryString(culture.NumberFormat.NegativeSign)

        from numberText in this.Many1(this.Digit.Or(this.Char(culture.NumberFormat.NumberDecimalSeparator.Contains)))

        from _ in this.Char('m', 'M')

        from preResult in this.MaybeParser(() => Maybe.OfCondition(decimal.TryParse(numberText.AsSpan(), culture, out var result), () => result))

        select isNegate ? -preResult : preResult;

    private ExpressionParser DecimalConstantExpressionParser =>

        from value in this.DecimalParser

        select (Expression)new DecimalConstantExpression(value);

    private ExpressionParser Int32ConstantExpressionParser =>

        from value in this.Int32Parser

        select (Expression)new Int32ConstantExpression(value);

    private ExpressionParser Int64ConstantExpressionParser =>

        from value in this.Int64Parser

        select (Expression)new Int64ConstantExpression(value);

    private ExpressionParser NullConstantExpressionParser =>

        from _ in this.StringIgnoreCase("null")

        select (Expression)NullConstantExpression.Value;

    private Parser<SharedMemoryString, MethodExpressionType> StringMethodExpressionTypeParser =>
        this.FromDictionary(StringMethodExpressions, this.StringIgnoreCase);

    private Parser<SharedMemoryString, MethodExpressionType> CollectionMethodExpressionTypeParser =>
        this.FromDictionary(CollectionMethodExpressions, this.StringIgnoreCase);

    private Parser<SharedMemoryString, UnaryOperation> UnaryOperationParser =>

        from operation in this.FromDictionary(UnaryOperations, this.StringIgnoreCase)

        from _ in this.TestNo(this.Word1)

        select operation;

    private Parser<SharedMemoryString, BinaryOperation> BinaryOperationParser =>

        from operation in this.FromDictionary(BinaryOperations, this.StringIgnoreCase)

        from _ in this.TestNo(this.Word1)

        select operation;

    private ParameterExpression GenerateAnonymousParameterExpression(string baseName)
    {
        var preName = "$" + baseName.ToStartLowerCase();

        var query = from name in new[] { preName }.Concat(Enumerable.InfiniteSequence(0, 1).Select(index => preName + index))

            let parameter = new ParameterExpression(name)

            where !usedParameters.Contains(parameter)

            select parameter;

        return query.First();
    }


    private static readonly Dictionary<SharedMemoryString, MethodExpressionType> StringMethodExpressions =
        new()
        {
            { "startswith", MethodExpressionType.StringStartsWith },
            { "substringof", MethodExpressionType.StringContains },
            { "endswith", MethodExpressionType.StringEndsWith }
        };

    private static readonly Dictionary<SharedMemoryString, MethodExpressionType> CollectionMethodExpressions =
        new()
        {
            { "any", MethodExpressionType.CollectionAny },
            { "all", MethodExpressionType.CollectionAll }
        };

    private static readonly Dictionary<SharedMemoryString, BinaryOperation> BinaryOperations = new()
    {
        { "gt", BinaryOperation.GreaterThan },
        { "lt", BinaryOperation.LessThan },
        { "le", BinaryOperation.LessThanOrEqual },
        { "ge", BinaryOperation.GreaterThanOrEqual },
        { "eq", BinaryOperation.Equal },
        { "ne", BinaryOperation.NotEqual },
        { "and", BinaryOperation.AndAlso },
        { "or", BinaryOperation.OrElse },
        { "add", BinaryOperation.Add },
        { "sub", BinaryOperation.Subtract },
        { "mult", BinaryOperation.Mul },
        { "div", BinaryOperation.Div },
        { "mod", BinaryOperation.Mod }
    };

    private static readonly Dictionary<SharedMemoryString, UnaryOperation> UnaryOperations = new()
    {
        { "not", UnaryOperation.Not },
        { "negate", UnaryOperation.Negate },
        { "plus", UnaryOperation.Plus }
    };

    private static readonly ImmutableArray<string> DateTimeProperties =
    [
        ..new SExpressions.Expression<Func<DateTime, object>>[]
        {
            d => d.Day,
            d => d.Hour,
            d => d.Minute,
            d => d.Month,
            d => d.Second,
            d => d.Year,
            d => d.Date // extra
        }.Select(expr => expr.GetProperty().Name)
    ];
}