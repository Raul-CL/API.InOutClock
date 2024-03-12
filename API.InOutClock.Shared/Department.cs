using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace API.InOutClock.Shared
{
    public class Department
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "La propiedad {0} es requerida")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El tamaño de la descripcion debe de ser entre {2} y {1} caracteres")]
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
    }
}
