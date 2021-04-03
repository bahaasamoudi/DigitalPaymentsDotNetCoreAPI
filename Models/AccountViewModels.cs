using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Balance")]
        public decimal? Balance { get; set; }

        [Display(Name = "User Image")]
        public string UserImage { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }


        [Display(Name = "Gender")]
        public int Gender { get; set; } // 0 = male, 1 = female
        [Required]
        [Display(Name = "Birth Date")]
        public string BirthDate { get; set; }

        [Required]
        public string IdNumber { get; set; }
    }


    public class ChangePersonalViewModel
    {


        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Gender")]
        [Required]
        public int Gender { get; set; } // 0 = male, 1 = female
        [Required]
        public string Country { get; set; }
        [Required]
        public string IdNumber { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }

    }

    public class ChangeShopInformationViewModel
    {

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

    }

    public class ChangePasswordVM
    {


        [Required]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }

    }

    public class AddShopViewModel
    {


        [Required]
        public string ShopName { get; set; }

        [Required]
        public string TypeOfService { get; set; }

        [Required]
        public string ShopWebsite { get; set; }

        [Required]
        public string ShopPhone { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ShopLocation { get; set; }

    }

    public class AcceptShopViewModel
    {

        [Required]
        public string ShopId { get; set; }

    }




}
