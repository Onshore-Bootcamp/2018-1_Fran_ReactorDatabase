using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReactorDatabaseWebApp.Models
{
    public class LoginPO
    {
        [StringLength(20, MinimumLength = 6)]
        [RegularExpression("^[A-Za-z_][A-Za-z0-9_]*$",
            ErrorMessage = "Must not contain spaces, symbols, or start with a number.")]
        [Required]
        public string Username { get; set; }

        [StringLength(20, MinimumLength = 6)]
        [RegularExpression("^[A-Za-z0-9$@&#]*$",
            ErrorMessage = "Allowed characters: letters, numbers, $, @, &, #. " +
                           "Must not contain spaces.")]
        [Required]
        public string Password { get; set; }
    }
}