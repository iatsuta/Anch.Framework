using Anch.Core;
using Anch.OData.Domain;

namespace Anch.OData;

/// <summary>Binds an untyped operation to a domain type: the filter and every sort key become typed expressions.</summary>
/// <param name="lambdaExpressionConverter">Converts the lambdas.</param>
public class SelectOperationConverter(ILambdaExpressionConverter lambdaExpressionConverter) : ISelectOperationConverter
{
    /// <inheritdoc/>
    public SelectOperation<TDomainObject> Convert<TDomainObject>(SelectOperation rawSelectOperation)
    {
        var typedFilter = lambdaExpressionConverter.Convert<Func<TDomainObject, bool>>(rawSelectOperation.Filter);

        var typedOrders = rawSelectOperation.Orders.Select(this.ToTypedOrder<TDomainObject>);

        return new SelectOperation<TDomainObject>(typedFilter, [.. typedOrders], rawSelectOperation.SkipCount, rawSelectOperation.TakeCount);
    }

    private SelectOrder<TDomainObject> ToTypedOrder<TDomainObject>(SelectOrder selectOrder)
    {
        var orderKeyType = selectOrder.Path.ExtractTargetType<TDomainObject>();

        return new Func<SelectOrder, SelectOrder<TDomainObject, object>>(this.ToTypedOrder<TDomainObject, object>)
            .CreateGenericMethod(typeof(TDomainObject), orderKeyType)
            .Invoke<SelectOrder<TDomainObject>>(this, selectOrder);
    }

    private SelectOrder<TDomainObject, TOrderKey> ToTypedOrder<TDomainObject, TOrderKey>(SelectOrder selectOrder) =>
        new(lambdaExpressionConverter.Convert<Func<TDomainObject, TOrderKey>>(selectOrder.Path))
        { OrderType = selectOrder.OrderType };
}