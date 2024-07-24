using AutoMapper;
using Preventyon.Models;
using Preventyon.Models.DTO;
using Preventyon.Models.DTO.AdminDTO;
using Preventyon.Models.DTO.Employee;
using Preventyon.Models.DTO.Incidents;


namespace Preventyon
{
    public class MappingConfig:Profile
    {
        public MappingConfig() {

            CreateMap<CreateIncidentDTO, Incident>().ReverseMap();
            CreateMap<Employee, CreateEmployeeDTO>().ReverseMap();
            CreateMap<Employee, GetEmployeesDTO>();
            CreateMap<Employee, GetEmployeeRoleWithIDDTO>();
            CreateMap<Employee, UpdateEmployeeRoleDTO>().ReverseMap();
            CreateMap<Role, RoleDTO>();
            CreateMap<Permission, PermissionDTO>();
            CreateMap<Incident, UpdateIncidentUserDto>().ReverseMap();
            CreateMap<Incident, UpdateIncidentDTO>().ReverseMap();
            CreateMap<Admin, CreateAdminDTO>().ReverseMap();
            CreateMap<Incident, TableFetchIncidentsDto>().ReverseMap();
        }
    }
}
