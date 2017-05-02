using System;

namespace Diploma.Core.Models
{
    public class AuditEntry
    {
        public int Id { get; set; }

        public string Source { get; set; }

        public string Details { get; set; }

        public DateTime DateTime { get; set; }

        public LogLevel LogLevel { get; set; }
    }
}
