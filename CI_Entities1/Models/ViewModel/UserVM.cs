using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Entities1.Models.ViewModel
{
    public class UserVM
    {
        public List<User> users { get; set; }
        public long UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide First Name")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First Name Should be min 2 and max 20 length")]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public long PhoneNumber { get; set; }

        public string? Avatar { get; set; }

        public string? WhyIVolunteer { get; set; }

        public string? EmployeeId { get; set; }
        [Required]
        public string? Department { get; set; }

        public long? CityId { get; set; }

        public long? CountryId { get; set; }

        public string? ProfileText { get; set; }

        public string? LinkedInUrl { get; set; }
        [Required]
        public string? Title { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual City? City { get; set; }

        public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

        public virtual Country? Country { get; set; }
    }
}
