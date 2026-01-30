using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VITRACK.Api.DTOs.Users;
using VITRACK.Application.Interfaces;
using VITRACK.Infrastructure.Data;

namespace VITRACK.Application.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserFullDataDTO>> GetAllUsersWithDetailsAsync(bool? isDeleted = false)
    {
        var result = await _context.Users
            .Where(u => u.IsDeleted == isDeleted)
            .Join(
                _context.UserRoles,
                user => user.Id,
                userRole => userRole.UserId,
                (user, userRole) => new { user, userRole }
            )
            .Join(
                _context.Roles,
                ur => ur.userRole.RoleId,
                role => role.Id,
                (ur, role) => new { ur.user, ur.userRole, role }
            )
            .GroupJoin(
                _context.Departments,
                ur => ur.user.DepartmentId,
                d => (int?)d.Id,
                (ur, departments) => new { ur.user, ur.role, departments }
            )
            .SelectMany(
                x => x.departments.DefaultIfEmpty(),
                (x, department) => new { x.user, x.role, department }
            )
            .GroupJoin(
                _context.WorkSchedules,
                ur => ur.user.WorkScheduleId,
                w => (int?)w.Id,
                (ur, workSchedules) => new { ur.user, ur.role, ur.department, workSchedules }
            )
            .SelectMany(
                x => x.workSchedules.DefaultIfEmpty(),
                (x, workSchedule) => new { x.user, x.role, x.department, workSchedule }
            )
            .Select(x => new UserFullDataDTO
            {
                Id = x.user.Id,
                Firstname = x.user.Firstname,
                Lastname = x.user.Surname,
                Role = x.role.Name,
                Login = x.user.UserName,
                Department = x.department != null ? new DepartmentSimpleDTO
                {
                    Id = x.department.Id,
                    Name = x.department.Name
                } : null,
                WorkSchedule = x.workSchedule != null ? new WorkScheduleSimpleDTO
                {
                    Id = x.workSchedule.Id,
                    Name = x.workSchedule.Name
                } : null
            })
            .ToListAsync();

        return result;
    }

    public async Task<UserFullDataDTO?> GetUserWithDetailsByIdAsync(string id)
    {
        var result = await _context.Users
            .Where(u => u.Id == id)
            .Join(
                _context.UserRoles,
                user => user.Id,
                userRole => userRole.UserId,
                (user, userRole) => new { user, userRole }
            )
            .Join(
                _context.Roles,
                ur => ur.userRole.RoleId,
                role => role.Id,
                (ur, role) => new { ur.user, ur.userRole, role }
            )
            .GroupJoin(
                _context.Departments,
                ur => ur.user.DepartmentId,
                d => (int?)d.Id,
                (ur, departments) => new { ur.user, ur.role, departments }
            )
            .SelectMany(
                x => x.departments.DefaultIfEmpty(),
                (x, department) => new { x.user, x.role, department }
            )
            .GroupJoin(
                _context.WorkSchedules,
                ur => ur.user.WorkScheduleId,
                w => (int?)w.Id,
                (ur, workSchedules) => new { ur.user, ur.role, ur.department, workSchedules }
            )
            .SelectMany(
                x => x.workSchedules.DefaultIfEmpty(),
                (x, workSchedule) => new { x.user, x.role, x.department, workSchedule }
            )
            .Select(x => new UserFullDataDTO
            {
                Id = x.user.Id,
                Firstname = x.user.Firstname,
                Lastname = x.user.Surname,
                Role = x.role.Name,
                Login = x.user.UserName,
                Department = x.department != null ? new DepartmentSimpleDTO
                {
                    Id = x.department.Id,
                    Name = x.department.Name
                } : null,
                WorkSchedule = x.workSchedule != null ? new WorkScheduleSimpleDTO
                {
                    Id = x.workSchedule.Id,
                    Name = x.workSchedule.Name,
                    StartTime = x.workSchedule.StartTime,
                    EndTime = x.workSchedule.EndTime
                } : null
            })
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task UpdateAsync(UserEditDTO userDto)
    {
        var user = await _context.Users.FindAsync(userDto.Id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.Firstname = userDto.Firstname;
        user.Surname = userDto.Lastname;
        user.UserName = userDto.Login;
        user.DepartmentId = userDto.DepartmentId;
        user.WorkScheduleId = userDto.WorkScheduleId;
        user.NormalizedUserName = userDto.Login.ToUpper();

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

}
