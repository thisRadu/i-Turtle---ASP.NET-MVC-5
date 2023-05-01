using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace i_Turtle.Models
{
    public partial class Patient
    {


        public void GetRiskScaleString()
        {
            switch (RiscScale)
            {
                case "1":
                    RiscScale = "Low";
                    break;
                case "2":
                    RiscScale = "Moderate";
                    break;
                case "3":
                    RiscScale = "High";
                    break;
                default:
                    RiscScale = "Unknown";
                    break;
            }
        }
        /*
                private TurtleDbContext _context;

                public Patient(TurtleDbContext context)
                {
                    _context = context; 
                }

                public User GetDoctorName()
                {
                    if (DoctorId == null) return null;  
                    return _context.Users.FirstOrDefault(x => x.Id == DoctorId);
                }
                public IEnumerable<User> GetParentsName()
                {
                    if (ParentId == null) return null;

                    List<User> parents = new List<User>();
                    foreach (var item in ParentId.Split(",").ToArray())
                    {
                       parents.Add(_context.Users.FirstOrDefault(x => x.Id == Convert.ToInt32(item)));
                    }

                    return parents;
                }*/
    }
    public class PatientPaginationViewModel
    {
        public string SortBy { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PatientsPerPage { get; set; }
        public int TotalPatients { get; set; }
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }
        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
    }
}



