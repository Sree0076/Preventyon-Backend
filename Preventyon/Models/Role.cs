namespace Preventyon.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int PermissionID { get; set; }

        public Permission Permission { get; set; }

        public ICollection<Employee> Employees { get; set; }

    }
}
