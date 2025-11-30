using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Patient
    {
       
        public string Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Gender { get; set; }
      
        public int Age { get; set; }
       
        public string Address { get; set; }

        public List<Order> Orders { get; set; }


    }
}
