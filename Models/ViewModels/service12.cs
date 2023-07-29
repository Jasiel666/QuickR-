using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AplicacionPoe.Models
{
    public class service12
    {
        
        public int service_id { get; set; }

     
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? service_date { get; set; }

        public int? employee_attend { get; set; }
        public int? client_attended { get; set; }

        public string activity { get; set; }
    }
}