using System.Collections.Generic;

namespace ClientApp.Models
{
    public class StoreResult
    {
        public long? EmployeeId { get; set; }

        public List<string> Errors { get; set; } = new();

        public string Message { get; set; } = string.Empty;

        public bool ShowPayrollWarning { get; set; }

        public bool Success { get; set; }
    }
}
