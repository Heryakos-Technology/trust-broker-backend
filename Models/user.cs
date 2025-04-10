

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace broker.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        public string City { get; set; }
        public string Subcity { get; set; }
        public string Kebele { get; set; }
        public string Picture { get; set; }
        public string IdentificationCard { get; set; }
        public string Sex { get; set; }


        public string Role { get; set; }

        public ICollection<Buy> Buys { get; set; }
        public double  Latitude { get; set; }
        public double  Longtiude { get; set; }





    }

}