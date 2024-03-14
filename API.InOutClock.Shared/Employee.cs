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
        [StringLength(12, MinimumLength = 3, ErrorMessage = "El tamaño de {0} debe de ser entre {2} y {1} caracteres")]
        public string PayrollId { get; set; }
        [Required(ErrorMessage = "La propiedad {0} es requerida")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El tamaño de {0} debe de ser entre {2} y {1} caracteres")]
        public string Name { get; set; }
        [Required(ErrorMessage = "La propiedad {0} es requerida")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El tamaño de {0} debe de ser entre {2} y {1} caracteres")]
        public string LastName { get; set; }
        [Required]        
        public string NormalizedName
        {
            get
            {//Importante no dejar espacios en blanco
                return $"{Name.ToUpper().Replace(" ", "")}{LastName.ToUpper().Replace(" ", "")}";
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
