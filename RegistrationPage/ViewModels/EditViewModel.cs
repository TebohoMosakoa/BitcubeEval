using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationPage.ViewModels
{
    public class EditViewModel
    {
        public string Id { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
