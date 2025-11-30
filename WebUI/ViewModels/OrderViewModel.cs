using Core.Models;
using WebUI.Validations;
using System.ComponentModel.DataAnnotations;

namespace WebUI.ViewModels
{
    public class OrderViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Order Title is required.")]
        [StringLength(150, ErrorMessage = "Order Title must be 150 character or fewer.")]
        [Display(Name = "Order Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Order Duration is required.")]
        [Range(1, 5, ErrorMessage = "Duration Must Be Between 1 & 5")]
        [Display(Name = "Order Duration")]
        public double Duration { get; set; }


        [Required(ErrorMessage = "Order Address is required.")]
        [StringLength(150, ErrorMessage = "Order Address must be 150 character or fewer.")]
        [Display(Name = "Order Address")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Patient Age is required.")]
        [Range(0, 120, ErrorMessage = "Patient Age must be between 0 and 120.")]
        [Display(Name = "Patient Age")]
        public int PatientAge { get; set; }


        [Required(ErrorMessage = "Gender Age is required.")]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Order Status is required.")]
        [Display(Name = "Order Status")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;


        [Required(ErrorMessage = "Order Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }


        [Required(ErrorMessage = "Nurse is required.")]
        [Display(Name = "Nurse")]
        public int NurseId { get; set; }

        [Required(ErrorMessage = "Patient Id is required.")]
        [Display(Name = "User")]
        public string PatientId { get; set; }
    }
}
