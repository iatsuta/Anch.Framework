namespace Anch.Core;

public interface IServiceProxyFactory
{
    TService Create<TService>(params object[] args) => this.Create<TService, TService>(args);

    TService Create<TService, TServiceImpl>(params object[] args)
        where TServiceImpl : TService
        => this.Create<TService>(typeof(TServiceImpl), args);

    TService Create<TService>(Type instanceServiceType, params object[] args);
}