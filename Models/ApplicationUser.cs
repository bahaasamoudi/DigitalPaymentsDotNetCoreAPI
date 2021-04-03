using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string IdNumber { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public string UserImage { get; set; }
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public decimal? Balance { get; set; }
        public DateTime RegisteredDate { get; set; }
        public int Gender { get; set; }
        public DateTime Birthdate { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }
    }
}
