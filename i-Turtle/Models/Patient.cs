using System;
using System.Collections.Generic;

#nullable disable

namespace i_Turtle.Models
{
    public partial class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int? DoctorId { get; set; }
        public string RiscScale { get; set; }
        public DateTime? HandlingDate { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
