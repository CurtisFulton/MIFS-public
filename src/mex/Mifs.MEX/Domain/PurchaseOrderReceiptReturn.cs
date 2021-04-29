using System;

namespace Mifs.MEX.Domain
{
    public class PurchaseOrderReceiptReturn
    {
        public int PurchaseOrderReceiptReturnID { get; set; }
        public int? PurchaseOrderLineID { get; set; }
        public int ReceivedReturnedByContactID { get; set; }
        public DateTime ReceivedReturnedDateTime { get; set; }
        public decimal ReceivedReturnedQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int? TaxID { get; set; }
        public decimal TaxPercentage { get; set; }
        public string DeliveryDocketNumber { get; set; }
        public string NonMexPurchaseOrderNumber { get; set; }
        public int? NonMexReceivedReturnedAccountCodeID { get; set; }
        public string ReceivedReturnedActionName { get; set; }
        public int? RelatedPurchaseOrderReceiptReturnID { get; set; }
        public int? RelatedAdjustmentNotePurchaseOrderID { get; set; }
        public string Comment { get; set; }
        public decimal UOMConversionFactor { get; set; }
        public int? CurrencyTypeID { get; set; }
        public decimal ExchangeRatePercentage { get; set; }
        public DateTime? UserDefinedDateTime { get; set; }
        public bool IsProcessed { get; set; }
        public int CreatedByContactID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ModifiedByContactID { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int RegionID { get; set; }
        public decimal? FreightAmount { get; set; }
    }
}
