using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.InOutClock.Shared
{
    public class Check
    {
        public int Id { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public DateTime Record { get; set; }
        [Required]
        [Range(0, 1)]//Se utiliza int y no bool, por que a futuro puedo requerir valores de entrada y salida a comer
        public int TypeOfCheck { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public int ShiftId { get; set; }
        [Required]
        public bool RecordEvaluation { get; set; }
    }
}
