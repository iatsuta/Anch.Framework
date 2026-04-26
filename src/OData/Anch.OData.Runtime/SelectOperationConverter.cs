using Anch.Core;
using Anch.OData.Domain;

namespace Anch.OData;

public class SelectOperationConverter(ILambdaExpressionConverter lambdaExpressionConverter) : ISelectOperationConverter
{
    public SelectOperation<TDomainObject> Convert<TDomainObject>(SelectOperation rawSelectOperation)
    {
        var typedFilter = lambdaExpressionConverter.Convert<Func<TDomainObject, bool>>(rawSelectOperation.Filter);

        var typedOrders = rawSelectOperation.Orders.Select(this.ToTypedOrder<TDomainObject>);

        return new SelectOperation<TDomainObject>(typedFilter, [.. typedOrders], rawSelectOperation.SkipCount, rawSelectOperation.TakeCount)
        {
            Expands = rawSelectOperation.Expands,
            Selects = rawSelectOperation.Selects
        };
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