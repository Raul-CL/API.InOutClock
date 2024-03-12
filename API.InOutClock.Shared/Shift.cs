using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.InOutClock.Shared
{
    public class Shift
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Description { get; set; }
        public string NormalizedDescription
        {
            get
            {//Importante no dejar espacios en blanco
                return $"{Description.ToUpper()}".TrimEnd().TrimStart();
            }
            set
            {

            }
        }
        [Required]        
        public TimeOnly In { get; set; }
        [Required]        
        public TimeOnly Out { get; set; }
    }
}
