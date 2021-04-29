using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;using System.Collections.Generic;

namespace Mifs.MEX.Domain{
    public class PurchaseOrder : IEntityWithId
    {
        public int Id => this.PurchaseOrderID;

        public int PurchaseOrderID { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public int SupplierContactID { get; set; }
        public int? SupplierAttentionContactID { get; set; }
        public int? CurrencyTypeID { get; set; }
        public string PurchaseOrderTypeName { get; set; }
        public string PurchaseOrderStatusName { get; set; }
        public string InvoiceMatchStatusName { get; set; }
        public bool IsInvoiceReceived { get; set; }
        public bool IsInvoicePaid { get; set; }
        public int RegionID { get; set; }
        public int? PurchaserAttentionContactID { get; set; }
        public string SpecialInstruction { get; set; }
        public DateTime RaisedDateTime { get; set; }
        public DateTime? DueDateTime { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public DateTime? StandingPurchaseOrderExpiryDateTime { get; set; }
        public decimal StandingPurchaseOrderBudget { get; set; }
        public int RaisedByContactID { get; set; }
        public string QuoteNumber { get; set; }
        public string SalesTaxNumber { get; set; }
        public int? FreightContactID { get; set; }
        public decimal FreightAmount { get; set; }
        public int? FreightTaxID { get; set; }
        public decimal FreightTaxPercentage { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsFaxed { get; set; }
        public bool IsEmailed { get; set; }
        public int? PurchaseOrderUserDefined1ID { get; set; }
        public string PurchaseOrderUserDefinedTextBox1 { get; set; }
        public DateTime? PurchaseOrderUserDefinedDateTime1 { get; set; }
        public DateTime? PurchaseOrderUserDefinedDateTime2 { get; set; }
        public int? PurchaseOrderIDForAdjustmentNote { get; set; }
        public bool IsCancelled { get; set; }
        public int? CancelledByContactID { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public int CreatedByContactID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ModifiedByContactID { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int PurchaserID { get; set; }
        public Extension_PurchaseOrder Extension_PurchaseOrder { get; set; }
        public Contact SupplierContact { get; set; }
        public CurrencyType CurrencyType { get; set; }

        public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; }

        private class Configuration : IEntityTypeConfiguration<PurchaseOrder>
        {
            public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
            {
                builder.HasOne(x => x.Extension_PurchaseOrder)
                       .WithOne(x => x.PurchaseOrder)
                       .HasForeignKey<Extension_PurchaseOrder>(x => x.PurchaseOrderID)
                       .OnDelete(DeleteBehavior.ClientSetNull);
            }
        }
    }}