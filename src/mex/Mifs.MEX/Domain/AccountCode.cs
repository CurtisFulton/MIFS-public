using System;
namespace Mifs.MEX.Domain{
    public partial class AccountCode : IEntityWithId
    {
        public int Id => this.AccountCodeID;
        public int AccountCodeID { get; set; }
        public string AccountCodeName { get; set; }
        public string AccountCodeDescription { get; set; }
        public decimal AnnualBudget { get; set; }
        public decimal January { get; set; }
        public decimal February { get; set; }
        public decimal March { get; set; }
        public decimal April { get; set; }
        public decimal May { get; set; }
        public decimal June { get; set; }
        public decimal July { get; set; }
        public decimal August { get; set; }
        public decimal September { get; set; }
        public decimal October { get; set; }
        public decimal November { get; set; }
        public decimal December { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByContactID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ModifiedByContactID { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ValidationType { get; set; }
    }}