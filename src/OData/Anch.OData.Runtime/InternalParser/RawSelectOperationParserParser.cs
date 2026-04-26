using System.Collections.Immutable;
using System.Globalization;
using Anch.OData.Domain;
using Anch.OData.Domain.QueryLanguage;
using Anch.Parsing;

namespace Anch.OData.InternalParser;

public class RawSelectOperationParserParser(CultureInfo culture, ParameterExpression rootParameter) : CharParsers(culture)
{
    private readonly LambdaExpressionInternalParser rootLambdaExpressionParser = new(culture, rootParameter, [rootParameter]);

    private LambdaExpression CreateRootLambda(Expression body) => new(body, [rootParameter]);


    public Parser<SharedMemoryString, SelectOperation> MainParser =>

        from result in this.OfTable(

            this.GetElementParser("filter", this.GetLazy(() => this.FilterParser))
                .ToRow(() => SelectOperation.Default.Filter),

            this.GetElementParser("orderby", this.GetLazy(() => this.OrdersParser))
                .ToRow(() => SelectOperation.Default.Orders),

            this.GetElementParser("expand", this.GetLazy(() => this.ExpandsParser))
                .ToRow(() => SelectOperation.Default.Expands),

            this.GetElementParser("select", this.GetLazy(() => this.SelectsParser))
                .ToRow(() => SelectOperation.Default.Selects),

            this.GetElementParser("skip", this.Int32Parser)
                .ToRow(() => SelectOperation.Default.SkipCount),

            this.GetElementParser("top", this.Int32Parser)
                .ToRow(() => SelectOperation.Default.TakeCount),

            this.PreSpaces(this.Char('&')),

            (filter, orders, expands, selects, skipCount, takeCount) =>

                new SelectOperation(filter, [.. orders], skipCount, takeCount) { Expands = [.. expands], Selects = [.. selects] })

        from _ in this.PreSpaces(this.Eof)

        select result;

    private Parser<SharedMemoryString, T> GetElementParser<T>(SharedMemoryString name, Parser<SharedMemoryString, T> itemParser) =>

        from _ in this.BetweenSpaces(this.Char('$'))

        from __ in this.StringIgnoreCase(name)

        from ___ in this.BetweenSpaces(this.Char('='))

        from item in itemParser

        select item;

    private Parser<SharedMemoryString, Expression> RootBodyParser => this.rootLambdaExpressionParser.RootBodyParser;

    private Parser<SharedMemoryString, LambdaExpression> FilterParser => this.RootBodyParser.Select(this.CreateRootLambda);

    private Parser<SharedMemoryString, SelectOrder> OrderParser =>

        from expr in this.RootBodyParser

        from orderType in this.PreSpaces(this.OrderTypeParser).Or(this.Return(OrderType.Asc))

        select new SelectOrder(this.CreateRootLambda(expr), orderType);

    private Parser<SharedMemoryString, ImmutableArray<SelectOrder>> OrdersParser => this.SepBy1(this.OrderParser, ',');

    private Parser<SharedMemoryString, ImmutableArray<LambdaExpression>> ExpandsParser => this.SepBy1(this.RootLambdaExpressionParser, ',');

    private Parser<SharedMemoryString, ImmutableArray<LambdaExpression>> SelectsParser =>

        this.SepBy1(this.rootLambdaExpressionParser.PropertyPathParser.Select(this.CreateRootLambda), ',');

    private Parser<SharedMemoryString, LambdaExpression> RootLambdaExpressionParser => this.RootBodyParser.Select(this.CreateRootLambda);

    private Parser<SharedMemoryString, OrderType> OrderTypeParser =>
        this.StringIgnoreCase("desc").Select(_ => OrderType.Desc)
            .Or(() => this.StringIgnoreCase("asc").Select(_ => OrderType.Asc));
}