using DigitalPayments.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalPayments.Models
{
    public class Shop
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // You Can Put Shop Id Manulay [we Will Put It = user id]
        public string Id { get; set; }

        [Required]
        public string ShopName { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string TypeOfService { get; set; }
        [Required]
        public string WebSite { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public bool Accepted { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}