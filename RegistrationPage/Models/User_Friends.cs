using RegistrationPage.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationPage.Models
{
    public class User_Friends
    {
        [Key]
        public int Id { get; set; }
        public string User_Id { get; set; }
        public string Friend_Id { get; set; }
        public bool is_accepted { get; set; }
    }
}
