using System.ComponentModel.DataAnnotations;

namespace CI_Platform1.Models
{
    public class ForgetModel
    {
        //[Required(ErrorMessage = "please enter password")]
        //public string? Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,3}$", ErrorMessage = "Please Provide Valid Email")]
        public string? Email { get; set; } = null!;

    }
}
