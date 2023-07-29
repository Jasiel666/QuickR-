using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplicacionPoe.Models
{
    public class LoginUsers
    {

        public int id { get; set; }

        public string UserLogin { get; set; }

        public string UserPassword { get; set; }

        public string confirmPassword { get; set; }

    }
}