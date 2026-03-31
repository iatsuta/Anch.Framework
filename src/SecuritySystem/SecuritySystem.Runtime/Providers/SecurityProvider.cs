using System.Linq.Expressions;

using CommonFramework;
using CommonFramework.ExpressionEvaluate;

using SecuritySystem.SecurityAccessor;

namespace SecuritySystem.Providers
{
    public abstract class SecurityProvider<TDomainObject> : ISecurityProvider<TDomainObject>
    {
        private readonly Lazy<Func<TDomainObject, bool>> lazyHasAccessFunc;

        private readonly Lazy<IExpressionEvaluator> lazyExpressionEvaluator;


        protected SecurityProvider(IExpressionEvaluatorStorage expressionEvaluatorStorage)
        {
            this.lazyExpressionEvaluator = LazyHelper.Create(() => expressionEvaluatorStorage.GetForType(this.GetType()));

            this.lazyHasAccessFunc = LazyHelper.Create(() => this.ExpressionEvaluator.Compile(this.SecurityFilter));
        }

        protected IExpressionEvaluator ExpressionEvaluator => this.lazyExpressionEvaluator.Value;


        public abstract Expression<Func<TDomainObject, bool>> SecurityFilter { get; }

        public virtual IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) => queryable.Where(this.SecurityFilter);

        public virtual async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken) => this.lazyHasAccessFunc.Value(domainObject);

        public abstract ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken);
    }
}
