using Mifs.Scheduling;
using System;
namespace Mifs.MEX.Domain{
    public class RecordApproval : IEntityWithId
    {
        public int RecordApprovalID { get; set; }
        public string EntityName { get; set; }
        public int EntityID { get; set; }
        public string ApprovalStatusName { get; set; }
        public DateTime ApprovedDateTime { get; set; }
        public int CreatedByContactID { get; set; }
        public DateTime CreatedByDateTime { get; set; }
        public int ModifiedByContactID { get; set; }
        public DateTime ModifiedByDateTime { get; set; }

        public int Id => this.RecordApprovalID;
    }}