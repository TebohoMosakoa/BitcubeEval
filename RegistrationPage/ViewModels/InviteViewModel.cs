using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationPage.ViewModels
{
    public class InviteViewModel
    {
        public int Id { get; set; }
        public string User_Id { get; set; }
        public string Friend_Id { get; set; }
        public bool is_accepted { get; set; }
    }
}
