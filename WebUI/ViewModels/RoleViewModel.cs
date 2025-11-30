using System.ComponentModel.DataAnnotations;

namespace WebUI.ViewModels
{
    public class RoleViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(256, ErrorMessage = "Role name must not exceed 256 characters.")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }

}
