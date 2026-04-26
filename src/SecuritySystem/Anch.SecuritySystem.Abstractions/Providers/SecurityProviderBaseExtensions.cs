using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.SecuritySystem.AccessDenied;
using Anch.SecuritySystem.SecurityAccessor;

namespace Anch.SecuritySystem.Providers;

public static class SecurityProviderBaseExtensions
{
    extension<TDomainObject>(ISecurityProvider<TDomainObject> securityProvider)
    {
        public ISecurityProvider<TDomainObject> OverrideAccessDeniedResult(Func<AccessResult.AccessDeniedResult, AccessResult.AccessDeniedResult> selector) =>
            new OverrideAccessDeniedResultSecurityProvider<TDomainObject>(securityProvider, selector);

        public ISecurityProvider<TDomainObject> And(Expression<Func<TDomainObject, bool>> securityFilter,
            IExpressionEvaluatorStorage expressionEvaluatorStorage) =>
            securityProvider.And(new ConditionSecurityProvider<TDomainObject>(securityFilter, expressionEvaluatorStorage));

        public ISecurityProvider<TDomainObject> Or(Expression<Func<TDomainObject, bool>> securityFilter,
            IExpressionEvaluatorStorage expressionEvaluatorStorage) =>
            securityProvider.Or(new ConditionSecurityProvider<TDomainObject>(securityFilter, expressionEvaluatorStorage));

        public ISecurityProvider<TDomainObject> And(ISecurityProvider<TDomainObject> otherSecurityProvider) =>
            new CompositeSecurityProvider<TDomainObject>(securityProvider, otherSecurityProvider, true);

        public ISecurityProvider<TDomainObject> Or(ISecurityProvider<TDomainObject> otherSecurityProvider) =>
            new CompositeSecurityProvider<TDomainObject>(securityProvider, otherSecurityProvider, false);

        public ISecurityProvider<TDomainObject> Negate() =>
            new NegateSecurityProvider<TDomainObject>(securityProvider);

        public ISecurityProvider<TDomainObject> Except(
            ISecurityProvider<TDomainObject> otherSecurityProvider) =>
            securityProvider.And(otherSecurityProvider.Negate());

        public async Task CheckAccessAsync(
            TDomainObject domainObject,
            IAccessDeniedExceptionService accessDeniedExceptionService,
            CancellationToken cancellationToken = default)
        {
            switch (await securityProvider.GetAccessResultAsync(domainObject, cancellationToken))
            {
                case AccessResult.AccessDeniedResult accessDenied:
                    throw accessDeniedExceptionService.GetAccessDeniedException(accessDenied);

                case AccessResult.AccessGrantedResult:
                    break;

                default:
                    throw new InvalidOperationException("unknown access result");
            }
        }
    }

    extension<TDomainObject>(IEnumerable<ISecurityProvider<TDomainObject>> securityProviders)
    {
        public ISecurityProvider<TDomainObject> And() =>
            securityProviders.Match(
                () => new DisabledSecurityProvider<TDomainObject>(),
                single => single,
                many => many.Aggregate((v1, v2) => v1.And(v2)));

        public ISecurityProvider<TDomainObject> Or() =>
            securityProviders.Match(
                () => new AccessDeniedSecurityProvider<TDomainObject>(),
                single => single,
                many => many.Aggregate((v1, v2) => v1.Or(v2)));
    }

    private class CompositeSecurityProvider<TDomainObject>(
        ISecurityProvider<TDomainObject> securityProvider,
        ISecurityProvider<TDomainObject> otherSecurityProvider,
        bool isAnd)
        : ISecurityProvider<TDomainObject>
    {
        public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) =>
            isAnd
                ? securityProvider.Inject(queryable).Pipe(otherSecurityProvider.Inject)
                : securityProvider.Inject(queryable).Union(otherSecurityProvider.Inject(queryable));

        public async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken cancellationToken) =>
            isAnd
                ? (await securityProvider.GetAccessResultAsync(domainObject, cancellationToken)).And(
                    await otherSecurityProvider.GetAccessResultAsync(domainObject, cancellationToken))
                : (await securityProvider.GetAccessResultAsync(domainObject, cancellationToken)).Or(
                    await otherSecurityProvider.GetAccessResultAsync(domainObject, cancellationToken));

        public async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken) =>
            isAnd
                ? await securityProvider.HasAccessAsync(domainObject, cancellationToken) &&
                  await otherSecurityProvider.HasAccessAsync(domainObject, cancellationToken)
                : await securityProvider.HasAccessAsync(domainObject, cancellationToken) ||
                  await otherSecurityProvider.HasAccessAsync(domainObject, cancellationToken);

        public async ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken)
        {
            var left = await securityProvider.GetAccessorDataAsync(domainObject, cancellationToken);

            var right = await otherSecurityProvider.GetAccessorDataAsync(domainObject, cancellationToken);

            return isAnd
                ? new SecurityAccessorData.AndSecurityAccessorData(left, right)
                : new SecurityAccessorData.OrSecurityAccessorData(left, right);
        }
    }

    private class NegateSecurityProvider<TDomainObject>(ISecurityProvider<TDomainObject> securityProvider)
        : ISecurityProvider<TDomainObject>
    {
        public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) =>
            queryable.Except(securityProvider.Inject(queryable));

        public async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken cancellationToken)
        {
            switch (await securityProvider.GetAccessResultAsync(domainObject, cancellationToken))
            {
                case AccessResult.AccessDeniedResult:
                    return AccessResult.AccessGrantedResult.Default;

                case AccessResult.AccessGrantedResult:
                    return AccessResult.AccessDeniedResult.Create(domainObject);

                default:
                    throw new InvalidOperationException("unknown access result");
            }
        }

        public async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken) =>
            !await securityProvider.HasAccessAsync(domainObject, cancellationToken);

        public async ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken)
        {
            var baseResult = await securityProvider.GetAccessorDataAsync(domainObject, cancellationToken);

            return new SecurityAccessorData.NegateSecurityAccessorData(baseResult);
        }
    }
}