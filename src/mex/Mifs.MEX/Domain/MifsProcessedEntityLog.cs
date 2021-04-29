using System;

namespace Mifs.MEX.Domain
{
    public class MifsProcessedEntityLog
    {
        public int MifsProcessedEntityLogId { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public string IntegrationName { get; set; }
        public DateTime? ProcessedDateTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}
