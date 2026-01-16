using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VITRACK.Application.Interfaces;
using VITRACK.Infrastructure.Data;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Application.Repositories;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _db;

    public DepartmentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _db.Departments.AsNoTracking().ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _db.Departments.FindAsync(id);
    }

    public async Task<Department> CreateAsync(Department department)
    {
        var entry = await _db.Departments.AddAsync(department);
        await _db.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task UpdateAsync(Department department)
    {
        _db.Departments.Update(department);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _db.Departments.FindAsync(id);
        if (existing is null) return;
        _db.Departments.Remove(existing);
        await _db.SaveChangesAsync();
    }
}
