using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Mifs.MEX.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX
{
    public interface IMEXDbContext : IDisposable
    {
        DbSet<AccountCode> AccountCodes { get; }
        DbSet<Contact> Contacts { get; }
        DbSet<CurrencyType> CurrencyTypes { get; }
        DbSet<Extension_PurchaseOrder> Extension_PurchaseOrders { get; }
        DbSet<Extension_PurchaseOrderLine> Extension_PurchaseOrderLines { get; }
        DbSet<PurchaseOrder> PurchaseOrders { get; }
        DbSet<PurchaseOrderLine> PurchaseOrderLines { get; }
        DbSet<PurchaseOrderReceiptReturn> PurchaseOrderReceiptReturns { get; }
        DbSet<RecordApproval> RecordApprovals { get; }
        DbSet<SystemOption> SystemOptions { get; }
        DbSet<WorkOrder> WorkOrders { get; }

        DbSet<MifsProcessedEntityLog> MifsProcessedEntityLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DbSet<TSet> Set<TSet>() where TSet : class;

        EntityEntry Attach(object entity);
        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;

        IModel Model { get; }
    }
}
