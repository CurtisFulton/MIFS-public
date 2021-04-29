using System;

namespace Mifs.MEX.Domain
{
    public class Contact
    {
        public int ContactID { get; set; }
        public int? ParentContactID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Comment { get; set; }
        public int? DepartmentID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebAddress { get; set; }
        public bool IsPrimaryContact { get; set; }
        public string ABN { get; set; }
        public int? CurrencyTypeID { get; set; }
        public int? TaxID { get; set; }
        public string TaxExemptionNumber { get; set; }
        public bool IsTaxExempt { get; set; }
        public int? AccountCodeID { get; set; }
        public int? PaymentTermID { get; set; }
        public decimal CustomerDiscountPercentage { get; set; }
        public decimal CataloguedMarkUpCeiling { get; set; }
        public decimal NonCataloguedMarkUpCeiling { get; set; }
        public decimal CataloguedMarkupPercentage { get; set; }
        public decimal NonCataloguedMarkUpPercentage { get; set; }
        public string EmployeeNumber { get; set; }
        public decimal WorkingDayHours { get; set; }
        public DateTime? EmployedDateTime { get; set; }
        public DateTime? TerminatedDateTime { get; set; }
        public decimal ExpectedFreightPrice { get; set; }
        public int? CustomerTypeID { get; set; }
        public Byte[] ContactImage { get; set; }
        public string ContactTypeName { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByContactID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ModifiedByContactID { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingCountry { get; set; }
        public string BillingPostCode { get; set; }
        public DateTime AddedDateTime { get; set; } = DateTime.Now;
        public string IRDNumber { get; set; }
        public string ContactSignature { get; set; }
        public int? CustomControlFile1ID { get; set; }

        public AccountCode AccountCode { get; set; }
    }
}
