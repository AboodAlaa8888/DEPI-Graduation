using Infrastructure.Data.DbContexts;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WebUI.Validations
{
    public class UniqueNurseAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success; // rely on [Required] if needed

            var context = (NursingServicesDbContext?)validationContext.GetService(typeof(NursingServicesDbContext));
            if (context is null)
                throw new InvalidOperationException("NursingServicesDbContext is not available in DI container.");

            var newName = value.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(newName)) return ValidationResult.Success;

            // Try to get current entity Id (for Edit forms)
            var currentObject = validationContext.ObjectInstance;
            var idProp = currentObject.GetType().GetProperty("Id");
            var currentId = idProp != null ? (int?)Convert.ToInt32(idProp.GetValue(currentObject) ?? 0) : null;

            var exists = context.Nurses
                .Any(n =>
                    n.FullName.ToLower() == newName.ToLower() &&
                    (currentId == null));

            return exists
                ? new ValidationResult("Nurse name already exists!")
                : ValidationResult.Success;
        }
    }
}
