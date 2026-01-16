using System.Collections.Generic;
using System.Threading.Tasks;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Application.Interfaces;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<Department> CreateAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(int id);
}
