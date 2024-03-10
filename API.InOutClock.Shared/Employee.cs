using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.InOutClock.Shared
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string PayrollId { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(200)]
        public string NormalizedName
        {
            get
            {//Importante no dejar espacios en blanco
                return $"{Name.ToUpper()}{LastName.ToUpper()}".TrimEnd().TrimStart();
            }
            set
            {

            }
        }
        [Required]
        public int ShiftId { get; set; }
        [Required]
        public int DepartmentId { get; set; }

    }
}
