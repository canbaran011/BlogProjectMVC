using BlogProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogProject.ViewModels.Account
{
    public class AccountViewModel
    {
        public int ID { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
    }
}