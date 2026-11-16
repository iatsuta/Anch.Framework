using System.Data;

using NHibernate;

namespace ExampleApp.Infrastructure.Services;

public sealed class NHibAutoCommitSession : IAsyncDisposable, IDisposable
{
    private readonly ITransaction nhibTransaction;

    private bool closed;

    public NHibAutoCommitSession(ISessionFactory sessionFactory)
    {
        this.NativeSession = sessionFactory.OpenSession();
        this.NativeSession.FlushMode = FlushMode.Manual;

        this.nhibTransaction = this.NativeSession.BeginTransaction();

        this.Transaction = GetDbTransaction(this.nhibTransaction, this.NativeSession);
    }

    public ISession NativeSession { get; }

    public IDbTransaction Transaction { get; }

    private static IDbTransaction GetDbTransaction(ITransaction transaction, ISession session)
    {
        using var dbCommand = session.Connection.CreateCommand();
        dbCommand.Cancel();
        transaction.Enlist(dbCommand);
        return dbCommand.Transaction!;
    }

    public void Dispose() => this.DisposeAsync().GetAwaiter().GetResult();

    public ValueTask DisposeAsync() => this.CloseAsync(CancellationToken.None);

    public async ValueTask CloseAsync(CancellationToken ct)
    {
        if (this.closed)
        {
            return;
        }

        this.closed = true;

        using (this.NativeSession)
        {
            using (this.nhibTransaction)
            {
                await this.NativeSession.FlushAsync(ct);

                await this.nhibTransaction.CommitAsync(ct);
            }
        }
    }
}