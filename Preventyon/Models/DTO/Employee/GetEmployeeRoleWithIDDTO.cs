namespace Preventyon.Models.DTO.Employee
{
    public class GetEmployeeRoleWithIDDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }

        public RoleDTO Role { get; set; }
    }
}
