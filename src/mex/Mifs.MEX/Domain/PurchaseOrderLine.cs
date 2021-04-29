using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Runtime.Serialization;

namespace Mifs.MEX.Domain
{
    [DataContract]
    public partial class PurchaseOrderLine : IEntityWithId
    {
        public int Id => this.PurchaseOrderLineID;

        public int PurchaseOrderLineID { get; set; }
        public int PurchaseOrderID { get; set; }
        public int LineNumber { get; set; }
        public string SupplierStockNumber { get; set; }
        public int? AssetID { get; set; }
        public int? CatalogueSupplierID { get; set; }
        public string PurchaseOrderLineDescription { get; set; }
        public decimal OrderedQuantity { get; set; }
        public int? UOMID { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal UnitPrice { get; set; }
        public int? TaxID { get; set; }
        public decimal TaxPercentage { get; set; }
        public int? WorkOrderSpareID { get; set; }
        public int? RequisitionLineID { get; set; }
        public int? AccountCodeID { get; set; }
        public string UserDefinedTextBox { get; set; }
        public int? PurchaseOrderLineUserDefined1ID { get; set; }
        public bool IsCancelled { get; set; }
        public int? CancelledByContactID { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public bool IsPrinted { get; set; }
        public int CreatedByContactID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ModifiedByContactID { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public decimal FreightAmount { get; set; }
        public bool IsTriggered { get; set; }

        public Extension_PurchaseOrderLine Extension_PurchaseOrderLine { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public AccountCode AccountCode { get; set; }

        public decimal TotalExcludingTax => this.OrderedQuantity * this.UnitPrice;
        public decimal TotalTax => this.TotalExcludingTax * this.TaxPercentage;

        private class Configuration : IEntityTypeConfiguration<PurchaseOrderLine>
        {
            public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
            {
                builder.HasOne(x => x.Extension_PurchaseOrderLine)
                       .WithOne(x => x.PurchaseOrderLine)
                       .HasForeignKey<Extension_PurchaseOrderLine>(x => x.PurchaseOrderLineID)
                       .OnDelete(DeleteBehavior.ClientSetNull);
            }
        }
    }
}