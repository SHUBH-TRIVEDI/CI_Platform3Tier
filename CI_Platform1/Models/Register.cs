using System.ComponentModel.DataAnnotations;

namespace CI_Platform1.Models
{
    public class Register
    {


        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First Name Should be min 2 and max 20 length")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Last Name")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "First Name Should be min 5 and max 20 length")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Phone Number not valid")]
        public long PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
        //public string? FirstName { get; set; }
        //public string? LastName { get; set; }
        //public string? Email { get; set; }
        //public string? Mobile { get; set; }
        //public string? Password { get; set; }

        //public string? Confirmpassword { get; set; }

    }
}
