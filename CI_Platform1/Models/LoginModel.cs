using System.ComponentModel.DataAnnotations;

using Xunit;

namespace CI_Platform1.Models
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,3}$", ErrorMessage = "Please Provide Valid Email")]
        public string? Email { get; set; }

        //[Required(ErrorMessage = "Enter Password")]
        //[DataType(DataType.Password)]
        //[MinLength(8, ErrorMessage = "Password should contain atleast 8 charachter")]
        //[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Password should contain atleast one Capital letter , one small case letter, one Digit and one special symbol")]
        public string? Password { get; set; }

        //[Required]
        ////[StringLength(20, MinimumLength = 2, ErrorMessage = "First Name Should be min 2 and max 20 length")]
        //public string FirstName { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Last Name")]
        ////[StringLength(20, MinimumLength = 5, ErrorMessage = "First Name Should be min 5 and max 20 length")]
        //public string LastName { get; set; }

        //[Required(ErrorMessage = "Phone Number is required")]
        ////[StringLength(10, MinimumLength = 10)]
        ////[RegularExpression(@"^(\d{10})$", ErrorMessage = "Phone Number not valid")]
        //public long PhoneNumber { get; set; }

        //[Required]
        ////[DataType(DataType.Password)]
        ////[Display(Name = "Confirm Password")]
        ////[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; } = null!;


    }
}
