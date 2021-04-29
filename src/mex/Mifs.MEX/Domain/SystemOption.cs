using System;

namespace Mifs.MEX.Domain
{
    public class SystemOption : IEntityWithId
    {
        public int Id => this.SystemOptionID;

        public int SystemOptionID { get; set; }
        public string DatabaseVersion { get; set; }
        public int LastWorkOrderNumber { get; set; }
        public int LastInvoiceNumber { get; set; }
        public int LastPolicyNumber { get; set; }
        public int LastRequestNumber { get; set; }
        public int LastReservationNumber { get; set; }
        public int LastRequisitionNumber { get; set; }
        public int LastPurchaseOrderNumber { get; set; }
        public int LastCatalogueNumber { get; set; }
        public int LastLogSheetNumber { get; set; }
        public int LastHireOrderNumber { get; set; }
        public int LastTransactionNumber { get; set; }
        public bool IsAutomaticPurchaseOrderNumberOn { get; set; }
        public bool IsAutomaticCatalogueNumberOn { get; set; }
        public bool IsAutomaticCostUpdateOn { get; set; }
        public int LastSystemWorkOrderStatus { get; set; }
        public bool IsLockEstimatedQuantityWorkOrderSpareOn { get; set; }
        public string SalesTaxNumber { get; set; }
        public bool IsAllowReceiveMoreThanOrderedOn { get; set; }
        public bool IsAllowNegativeStockOnHandOn { get; set; }
        public string StoresCostingMethod { get; set; }
        public decimal? UserLevel { get; set; }
        public decimal? Trailer { get; set; }
        public decimal? WorkOrderFactor { get; set; }
        public decimal? AssetIDNumber { get; set; }
        public decimal? WorkOrderIDNumber { get; set; }
        public decimal? FloatB { get; set; }
        public decimal? CurrentUsers { get; set; }
        public string License { get; set; }
        public int? StoresLevel { get; set; }
        public int? Crit { get; set; }
        public int? FloatX { get; set; }
        public decimal? PkgLevel { get; set; }
        public decimal? UTE { get; set; }
        public decimal? FactorX { get; set; }
        public decimal? FloatC { get; set; }
        public int? RequestLevel { get; set; }
        public int? ByteString { get; set; }
        public int? FloatY { get; set; }
        public decimal? OpsAssetIDNumber { get; set; }
        public decimal? OpsWorkOrderIDNumber { get; set; }
        public int BudgetYearStartDay { get; set; }
        public string BudgetYearStartMonth { get; set; }
        public string StraightLineDepreciationTimeFrame { get; set; }
        public bool IsSecurityOn { get; set; }
        public bool IsSecurityAuditOn { get; set; }
        public bool IsSecurityWarningsOn { get; set; }
        public bool IsInvoiceMatchingOn { get; set; }
        public bool IsAuditOn { get; set; }
        public bool IsApprovalsUsedForPurchasing { get; set; }
        public bool IsMexOpsSecurityOn { get; set; }
        public int MexOpsTimeoutMinutes { get; set; }
        public string MexOpsScrollingBarText { get; set; }
        public bool IsSpecified { get; set; }
        public int CreatedByContactID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ModifiedByContactID { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string WebLevel { get; set; }
        public int? BossCompanyID { get; set; }
        public bool IsFleetSystem { get; set; }
        public DateTime? LastCleanDateTime { get; set; }
        public bool IsMovingRegionAssetInUseFlagOn { get; set; }
        public int TimeOutMinutes { get; set; }
        public int LastAssetNumber { get; set; }
        public string MEXcitedDatabaseVersion { get; set; }
        public Guid ConnectivityTest { get; set; }
        public int LastQuoteNumber { get; set; }
        public int? UserPortalClientID { get; set; }
        public string UserPortalPassword { get; set; }
        public Guid? UserPortalGUID { get; set; }
        public string V14DataServerAddress { get; set; }
        public string DatabaseBuild { get; set; }
        public bool IsEnforceStrongPasswords { get; set; }
        public string StrongPasswordValidateList { get; set; }
        public int LastStocktakeNumber { get; set; }
        public int ShowXCharsInListingField { get; set; }
        public bool IsAddNewUsersToContactsForMEXOpsOn { get; set; }
        public bool IsEnableMEXOpsForRequestsOn { get; set; }
        public bool IsSecurityActiveDirectoryOn { get; set; }
        public bool IsUseNotificationService { get; set; }
        public DateTime? LastNotificationServicePollDateTime { get; set; }
        public string ScheduledTaskUserName { get; set; }
        public string ScheduledTaskPassword { get; set; }
        public bool IsDynamicGridOn { get; set; }
        public bool IsEncryptPasswordsOn { get; set; }
        public bool IsSecurityActiveDirectoryStorePasswordOn { get; set; }
        public string ExtensionServerAddress { get; set; }
        public int QuoteValidDays { get; set; }
        public string LastActiveDirectoryName { get; set; }
        public string WebBasedActiveDirectoryConnection { get; set; }
        public int ArchiveXMonths { get; set; }
        public DateTime? LastArchiveDateTime { get; set; }
    }
}
