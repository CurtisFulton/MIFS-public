using System;
namespace Mifs.MEX{
    public partial class CurrencyType : IEntityWithId
    {
        public int CurrencyTypeID { get; set; }
        public string CurrencyTypeCode { get; set; }
        public string CurrencyTypeDescription { get; set; }
        public decimal ExchangeRatePercentage { get; set; }
        public int DecimalPlaces { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByContactID { get; set; } = 1;
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public int ModifiedByContactID { get; set; } = 1;
        public DateTime ModifiedDateTime { get; set; } = DateTime.Now;

        public int Id => this.CurrencyTypeID;

        public CurrencyType()
        {
            DecimalPlaces = 2;
            ExchangeRatePercentage = 1;
        }
    }}