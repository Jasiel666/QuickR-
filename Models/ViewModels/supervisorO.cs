using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AplicacionPoe.Models
{
    public class supervisorO
    {
       
        public int supervisor_id { get; set; }
        public string s_name { get; set; }
        public string s_area { get; set; }
        public string s_active { get; set; }
    }
}