using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AplicacionPoe.Models
{
    public class employeeO
    {
      
        public int employee_id { get; set; }

       
        public string e_name { get; set; }

       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? e_admission_date { get; set; }
        public string e_active { get; set; }
        public int e_supervisor { get; set; }
        public string e_observation { get; set; }
        public string e_certificate { get; set; }
        public byte[] e_image { get; set; }

      
     

    }
}