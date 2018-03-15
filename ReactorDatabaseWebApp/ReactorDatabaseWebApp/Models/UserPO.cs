using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ReactorDatabaseWebApp.Models
{
    public class UserPO
    {
        public int UserID { get; set; }

        public List<SelectListItem> RoleDropDown { get; set; }

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

        [Required]
        public int RoleID { get; set; }

        [DisplayName("First Name")]
        [StringLength(20, MinimumLength = 2)]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Allowed characters: a-z, A-Z")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [StringLength(40, MinimumLength = 2)]
        public string LastName { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a whole number.")]
        public int Age { get; set; }

        [StringLength(50)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [DisplayName("Subscribe")]
        [Required]
        public bool Subscription { get; set; }
    }
}