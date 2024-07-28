using System.ComponentModel.DataAnnotations.Schema;

namespace Preventyon.Models
{
    public class Admin
    {
        // chnage to Id
        public int AdminId { get; set; }

        public int EmployeeId { get; set; }

        public int AssignedBy { get; set; }

        public DateTime AssignedOn { get; set; }

        public Boolean Status { get; set; }

        public Employee Employee { get; set; }

        public Admin()
        {
            AssignedOn = DateTime.UtcNow;
        }
    }
}
