using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EMS.Models
{
    public class DetailsModel
    {
        [Key]
        public int Id { get; set; }
        public int Sno { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; } = DateTime.Now;
        [Required]
        public string Gender { get; set; }

        [MaxLength(75, ErrorMessage = "Enter mail address within 75 characters")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessage = "Enter valid mail address")]
        //[Remote(action: "EmailCheck", "EMS", AdditionalFields = "Id")]
        [Remote("EmailCheck", "EMS", AdditionalFields = "Id")]
        [Required]
        //[BindProperty]
        public string Email { get; set; }

        [MaxLength(10, ErrorMessage = "Enter a valid 10-digit mobile number")]
        [MinLength(10, ErrorMessage = "Enter a valid 10-digit mobile number")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter numbers only")]
        [Remote("PhoneNumberCheck", "EMS", AdditionalFields = "Id")]
        [Required]
        public string PhoneNumber { get; set; } 

        public string Address { get; set; }

        [Required]
        public string Address1 { get; set; }

        public string? Address2 { get; set; }

        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be 6 digits only")]
        [Required]
        //[MaxLength(6, ErrorMessage = "Pincode must be 6 digits only")]
        public int pincode { get; set; }


        [MaxLength(10, ErrorMessage = "Enter a valid 10-digit employee number")]
        [MinLength(10, ErrorMessage = "Enter a valid 10-digit employee number")]
        [Required]
        public string Empno { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DOJ { get; set; } = DateTime.Now;
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        [Required]
        public bool Status { get; set; }

        [Required]
        public string Designation { get; set; }

        public int Deleteflag { get; set; } = 0;


    }
}
