using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Models
{
    public class ReportFilters
    {
        public DateTime? StartDate { get; set; } = DateTime.Today.AddMonths(-1);
        public DateTime? EndDate { get; set; } = DateTime.Today;

        public string? Vendor { get; set; }
        public string? Warehouse { get; set; }
        public string? Store { get; set; }
        public string? Class { get; set; }
        public string? Type { get; set; }
    }
}
