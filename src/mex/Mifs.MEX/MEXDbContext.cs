using Microsoft.EntityFrameworkCore;
using Mifs.MEX.Domain;
using System.Linq;

namespace Mifs.MEX
{
    public class MEXDbContext : DbContext, IMEXDbContext
    {
        public DbSet<AccountCode> AccountCodes { get; private set; }
        public DbSet<Contact> Contacts { get; private set; }
        public DbSet<CurrencyType> CurrencyTypes { get; private set; }
        public DbSet<Extension_PurchaseOrder> Extension_PurchaseOrders { get; private set; }
        public DbSet<Extension_PurchaseOrderLine> Extension_PurchaseOrderLines { get; private set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; private set; }
        public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; private set; }
        public DbSet<PurchaseOrderReceiptReturn> PurchaseOrderReceiptReturns { get; private set; }
        public DbSet<RecordApproval> RecordApprovals { get; private set; }
        public DbSet<SystemOption> SystemOptions { get; private set; }
        public DbSet<WorkOrder> WorkOrders { get; private set; }

        public DbSet<MifsProcessedEntityLog> MifsProcessedEntityLogs { get; private set; }

        public MEXDbContext(DbContextOptions<MEXDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.ClrType)
                            .ToTable(entityType.ClrType.Name);

                var decimalProperties = entityType.GetProperties()
                                                  .Where(prop => prop.ClrType == typeof(decimal)
                                                              || prop.ClrType == typeof(decimal?));

                foreach (var property in decimalProperties)
                {
                    property.SetColumnType("decimal(18, 7)");
                }
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MEXDbContext).Assembly);
        }
    }
}
