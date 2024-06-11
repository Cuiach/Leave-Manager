using Leave_Manager.Leave_Manager.Core.Entities;

namespace Leave_Manager.Leave_Manager.Core.Interfaces
{
    internal interface IEmployeeRepository
    {
        List<Employee> GetAllUsers();
    }
}