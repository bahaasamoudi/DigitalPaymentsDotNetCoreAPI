using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models
{
    public class ReportViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public decimal? Money { get; set; }
    }

    public class ProfitsViewModel
    {
        public int BillId { get; set; }
        public string ShopId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string ShopName { get; set; }
        public string ShopPhone { get; set; }
  
        public decimal? Receivables { get; set; }
    }
}
