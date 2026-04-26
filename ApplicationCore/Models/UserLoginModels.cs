using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class UserLoginModel
    {
        [Required (ErrorMessage="Email is required.")]
        [EmailAddress(ErrorMessage="Invalid email.")]
        [StringLength(50, ErrorMessage= "Email cannot exceed 50 characters.")]
        public string Email { get; set; }
        [Required (ErrorMessage="Password is required.")]
        public string Password { get; set; }

    }
}