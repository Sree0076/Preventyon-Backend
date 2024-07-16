using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Preventyon.Models
{
    public class Employee
    {

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Department { get; set; }

        [Required]
        [StringLength(100)]
        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public Role Role { get; set; }

        public ICollection<Incident> Incidents { get; set; }

    }
}
