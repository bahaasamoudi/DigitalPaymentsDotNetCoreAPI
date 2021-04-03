using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models
{
    public class Bill
    {
        public int Id { get; set; }

        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }

        [Required]
        public decimal? Amount { get; set; }

        [Required]
        public bool Paid { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
      
        public string ShopId { get; set; }
        public virtual Shop Shop { get; set; }

    }
}
