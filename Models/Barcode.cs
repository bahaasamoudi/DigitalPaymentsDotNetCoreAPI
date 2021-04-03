using DigitalPayments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPayments.Models
{
    public class Barcode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public ushort Amount { get; set; }
        public DateTime GeneratedDate { get; set; }
        public bool Active { get; set; } // اذا كان تم الشراء به او انتهى وقته لا يعتبر اكتف 
        public string UserFor { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}