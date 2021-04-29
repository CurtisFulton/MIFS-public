using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Mifs.MEX.Domain
{
    public class Extension_PurchaseOrder
    {
        public int PurchaseOrderID { get; set; }
        public bool? IsStatusChanging { get; set; }
        public bool? UnlimitedBudget { get; set; }
        public bool HasBeenReceipted { get; set; }
        public decimal GST { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCostCurrencyAdjusted { get; set; }
        public decimal DiscountCost { get; set; }
        public string CurrencyTypeCode { get; set; }
        public int? CurrencyTypeDecimalPlaces { get; set; }
        public decimal? CurrencyTypeExchangeRatePercentage { get; set; }
        public string PurchaserName { get; set; }
        public string PurchaserAttentionContactFullName { get; set; }
        public string PurchaserAddress1 { get; set; }
        public string PurchaserAddress2 { get; set; }
        public string PurchaserCity { get; set; }
        public string PurchaserState { get; set; }
        public string PurchaserCountry { get; set; }
        public string PurchaserPostCode { get; set; }
        public string PurchaserABN { get; set; }
        public string PurchaserDeliveryInstructions { get; set; }
        public string SupplierName { get; set; }
        public string SupplierFirstName { get; set; }
        public string SupplierLastName { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierContactPhoneNumber { get; set; }
        public string SupplierContactFaxNumber { get; set; }
        public string SupplierAttentionContactFullName { get; set; }
        public string SupplierAttentionContactPhoneNumber { get; set; }
        public string SupplierAttentionContactFaxNumber { get; set; }
        public string SupplierAttentionContactEmail { get; set; }
        public string SupplierAddress1 { get; set; }
        public string SupplierAddress2 { get; set; }
        public string SupplierCity { get; set; }
        public string SupplierState { get; set; }
        public string SupplierPostCode { get; set; }
        public string SupplierCountry { get; set; }
        public string SupplierABN { get; set; }
        public string RaisedByContactFullName { get; set; }
        public string RaisedByContactCountry { get; set; }
        public string CancelledByContactFullName { get; set; }
        public string FreightContactFirstName { get; set; }
        public string FreightTaxName { get; set; }
        public string PurchaseOrderUserDefined1Name { get; set; }
        public string PurchaseOrderNumber_ForAdjustmentNote { get; set; }
        public string LastApprovalStatusName { get; set; }
        public string LastApprovalStatusSetByContactFullName { get; set; }
        public DateTime? LastApprovalStatusSetDateTime { get; set; }
        public bool IsApprovalsUsedForPurchasing { get; set; }
        public bool? IsEditable { get; set; }
        public string RequisitionedBy { get; set; }
        public string RegionList { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        private class Configuration : IEntityTypeConfiguration<Extension_PurchaseOrder>
        {
            public void Configure(EntityTypeBuilder<Extension_PurchaseOrder> builder)
            {
                builder.ToView(nameof(Extension_PurchaseOrder))
                       .HasKey(x => x.PurchaseOrderID);
            }
        }
    }
}