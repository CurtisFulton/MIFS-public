using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mifs.MEX.Domain
{
    [Table(nameof(WorkOrder))]
    public class WorkOrder
    {
        public int WorkOrderID { get; set; }
        public int WorkOrderNumber { get; set; }
        public string WorkOrderSuffix { get; set; }
        public string WorkOrderDescription { get; set; }
        public string WorkOrderHistoryDescription { get; set; }
        public string Instruction { get; set; }
        public string SafetyNote { get; set; }
        public int AssetID { get; set; }
        public bool IsGroupWorkOrder { get; set; }
        public int? GroupWorkOrderID { get; set; }
        public DateTime RaisedDateTime { get; set; }
        public DateTime? DueStartDateTime { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? DueFinishDateTime { get; set; }
        public DateTime? FinishedDateTime { get; set; }
        public bool IsHistoryCreated { get; set; }
        public decimal? DueReading { get; set; }
        public decimal? LastDoneReading { get; set; }
        public decimal OverallDurationHours { get; set; }
        public int WorkOrderStatusID { get; set; }
        public int? PriorityID { get; set; }
        public int? PreventativeMaintenanceID { get; set; }
        public int? InspectionParentWorkOrderID { get; set; }
        public int? AccountCodeID { get; set; }
        public int? JobTypeID { get; set; }
        public decimal EstimatedLabourCost { get; set; }
        public decimal EstimatedMaterialCost { get; set; }
        public decimal EstimatedOtherCost { get; set; }
        public decimal ActualLabourCost { get; set; }
        public decimal ActualMaterialCost { get; set; }
        public decimal ActualOtherCost { get; set; }
        public int? CustomerContactID { get; set; }
        public int? ContractorContactID { get; set; }
        public string ConsignmentNumber { get; set; }
        public int? FreightContactID { get; set; }
        public int? SentByContactID { get; set; }
        public int? ReceivedByContactID { get; set; }
        public DateTime? SentDateTime { get; set; }
        public DateTime? ContractorReceivedDateTime { get; set; }
        public DateTime? ReceivedBackDateTime { get; set; }
        public string CustomerPurchaseOrderNumber { get; set; }
        public string QuoteNumber { get; set; }
        public decimal? QuoteAmount { get; set; }
        public int? DepartmentID { get; set; }
        public bool IsPrinted { get; set; }
        public decimal ProgressIndicatorPercentage { get; set; }
        public string WorkOrderUserDefinedTextBox1 { get; set; }
        public string WorkOrderUserDefinedTextBox2 { get; set; }
        public int? ComponentCodeID { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime? WorkOrderUserDefinedDateTime { get; set; }
        public decimal DownTimeHours { get; set; }
        public decimal RepairTimeHours { get; set; }
        public decimal? DoneReading { get; set; }
        public int? FrequencyTypeID { get; set; }
        public int? AssetMovementID { get; set; }
        public int? CancelledByContactID { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public bool IsQuotedAmountInvoiced { get; set; }
        public int? CheckedInCustomerContactID { get; set; }
        public string CheckedInPhone { get; set; }
        public DateTime? CheckedInDateTime { get; set; }
        public DateTime? CheckedOutDateTime { get; set; }
        public DateTime? DueInDateTime { get; set; }
        public DateTime? DuePickupDateTime { get; set; }
        public int CreatedByContactID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ModifiedByContactID { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ContactSignature { get; set; }
        public decimal? Reading { get; set; }
        public int? WorkOrderClosedContactID { get; set; }
        public decimal ContractorQuoteAmount { get; set; }
        public string ContractorQuoteNumber { get; set; }
        public DateTime? ContractorQuoteProvidedDateTime { get; set; }
        public int? ContractorContactContactID { get; set; }
        public string WorkOrderFormatName { get; set; }
        public bool IsCompletedByContractor { get; set; }
        public int? CompletedByContractorContactID { get; set; }
        public DateTime? CompletedByContractorDateTime { get; set; }
        public int? ContractorPurchaseOrderID { get; set; }
        public bool IsContractorWorkOrder { get; set; }
        public bool IsContractorInvoicePaid { get; set; }
        public bool? IsWorkOrderSentToContractor { get; set; }
        public bool IsAudit { get; set; }
        public int? ParentWorkOrderID { get; set; }
    }
}
