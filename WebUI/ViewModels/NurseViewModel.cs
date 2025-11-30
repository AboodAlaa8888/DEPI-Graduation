using Core.Models;
using WebUI.Validations;
using System.ComponentModel.DataAnnotations;

namespace WebUI.ViewModels
{
    public class NurseViewModel
    {
        public int Id { get; set; }


        //[Required(ErrorMessage = "Nurse name is required.")]
        //[UniqueNurse(ErrorMessage = "Nurse name must be unique.")]
        //[StringLength(150, ErrorMessage = "Nurse Name must be 150 character or fewer.")]
        //[Display(Name = "Nurse Name")]
        //public string Name { get; set; }

        [Required(ErrorMessage = "Nurse name is required.")]
        [UniqueNurse(ErrorMessage = "Nurse name must be unique.")]
        [StringLength(150, ErrorMessage = "Nurse Name must be 150 character or fewer.")]
        [Display(Name = "Nurse FullName")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Nurse Age is required.")]
        [Range(22, 60, ErrorMessage = "Nurse Age must be greater than 22 and fewer than 60.")]
        [Display(Name = "Nurse Age")]
        public int Age { get; set; }


        [Required(ErrorMessage = "Nurse Experience Year is required.")]
        [Range(3, 40, ErrorMessage = "Nurse Experience Year must be greater than 2 year.")]
        [Display(Name = "Nurse Experience Year")]
        public int Experience_years { get; set; }


        [Required(ErrorMessage = "Nurse Address is required.")]
        [StringLength(150, ErrorMessage = "Nurse Address must be 150 character or fewer.")]
        [Display(Name = "Nurse Address")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Nurse Gender is required.")]
        [Display(Name = "Nurse Gender")]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Nurse Description is required.")]
        [StringLength(1000, ErrorMessage = "Nurse Name must be 1000 character or fewer.")]
        [Display(Name = "Nurse Description")]
        public string Description { get; set; }


        [Display(Name = "Nurse Picture URL")]
        public string? PictureUrl { get; set; }

        [Display(Name = "Nurse Picture File")]
        public IFormFile? File { get; set; }


        public List<Order>? Orders { get; set; }

    }
}
