using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mifs.MEX.Domain
{
    public partial class Extension_PurchaseOrderLine
    {
        public int PurchaseOrderLineID { get; set; }
        public string CurrencyTypeCode { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TotalExcludingTax { get; set; }
        public decimal? TotalIncludingTax { get; set; }
        public string UOMName { get; set; }
        public string TaxName { get; set; }
        public string AccountCodeName { get; set; }
        public string CatalogueNumber { get; set; }
        public string StoreName { get; set; }
        public string BinLocationName { get; set; }
        public string CancelledByContactFullName { get; set; }
        public string PurchaseOrderLineUserDefined1Name { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal ReceiptLineUnitCost { get; set; }
        public int? WorkOrderID { get; set; }
        public string WorkOrderNumber { get; set; }
        public string RegionAssignments { get; set; }
        public string CatalogueSupplierStockNumber { get; set; }
        public bool IsCatalogued { get; set; }
        public bool IsStocked { get; set; }
        public string RequisitionNumber { get; set; }
        public int TransactionCount { get; set; }
        public string RegionIDs { get; set; }
        public bool IsNonInventory { get; set; }

        public PurchaseOrderLine PurchaseOrderLine { get; set; }

        private class Configuration : IEntityTypeConfiguration<Extension_PurchaseOrderLine>
        {
            public void Configure(EntityTypeBuilder<Extension_PurchaseOrderLine> builder)
            {
                builder.ToView(nameof(Extension_PurchaseOrderLine))
                       .HasKey(x => x.PurchaseOrderLineID);
            }
        }
    }
}