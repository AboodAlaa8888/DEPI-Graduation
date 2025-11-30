using Core.Models;
using System.ComponentModel.DataAnnotations;

namespace WebUI.ViewModels
{
    public class NurseRegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Geneder is required.")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120.")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Nurse Experience Year is required.")]
        [Range(3, 40, ErrorMessage = "Nurse Experience Year must be greater than 2 year.")]
        [Display(Name = "Nurse Experience Year")]
        public int Experience_years { get; set; }

        [Required(ErrorMessage = "Nurse Description is required.")]
        [StringLength(1000, ErrorMessage = "Nurse Name must be 1000 character or fewer.")]
        [Display(Name = "Nurse Description")]
        public string Description { get; set; }

        [Display(Name = "Nurse Picture URL")]
        public string? PictureUrl { get; set; }

        [Display(Name = "Nurse Picture File")]
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        //public List<Order>? Orders { get; set; }

    }
}
