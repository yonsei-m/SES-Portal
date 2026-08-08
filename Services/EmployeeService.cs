using Microsoft.EntityFrameworkCore;
using SES_Portal.Data;
using SES_Portal.Models;
using Microsoft.AspNetCore.Http;
using SES_Portal.ViewModels;
using SES_Portal.Enums;

namespace SES_Portal.Services
{
    public class EmployeeService
    {
        private readonly AppDbContext _context;
        private readonly ImageService _imageService;

        public EmployeeService(
            AppDbContext context,
            ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }
        // 詳細
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // 論理削除
        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (employee == null) return;

            employee.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<(List<Employee> Employees, int TotalCount)> GetEmployeesAsync(string? searchName,int? searchDepartmentId, EmployeeStatus? searchStatus, List<int>? searchSkillIds, string? sortColumn,string? sortOrder,int pageNumber,int pageSize)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill)
                .Where(e => !e.IsDeleted)
                .AsQueryable();
            // 検索
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(x => x.Name.Contains(searchName));
            }

            if (searchDepartmentId.HasValue)
            {
                query = query.Where(x => x.DepartmentId == searchDepartmentId);
            }
            if (searchStatus.HasValue)
            {
                query = query.Where(x =>
                    x.Status == searchStatus.Value);
            }
            if (searchSkillIds != null && searchSkillIds.Any())
            {
                query = query.Where(e =>
                    e.EmployeeSkills.Any(es =>
                        searchSkillIds.Contains(es.SkillId)));
            }

            // デフォルト
            if (string.IsNullOrEmpty(sortColumn))
            {
                sortColumn = "empNo";
                sortOrder = "asc";
            }

            sortOrder ??= "asc";

            switch (sortColumn)
            {
                case "name":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(x => x.Name)
                        : query.OrderBy(x => x.Name);
                    break;

                case "empNo":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(x => x.EmployeeNumber)
                        : query.OrderBy(x => x.EmployeeNumber);
                    break;

                case "dept":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(x => x.Department.Name)
                        : query.OrderBy(x => x.Department.Name);
                    break;

                default:
                    query = query.OrderBy(x => x.Id);
                    break;
            }

            var totalCount = await query.CountAsync();

            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (employees, totalCount);
        }
        public async Task<bool> UpdateAsync(Employee employee)
        {
            var entity = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employee.Id && !e.IsDeleted);

            if (entity == null)
            {
                return false;
            }

            entity.EmployeeNumber = employee.EmployeeNumber;
            entity.Name = employee.Name;
            entity.DepartmentId = employee.DepartmentId;
            entity.Email = employee.Email;
            entity.HireDate = employee.HireDate;
            entity.SelfIntroduction = employee.SelfIntroduction;
            entity.ExperienceYears = employee.ExperienceYears;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Skill>> GetActiveSkillsAsync()
        {
            return await _context.Skills
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();
        }
        public async Task<List<int>> GetEmployeeSkillIdsAsync(int employeeId)
        {
            return await _context.EmployeeSkills
                .Where(es => es.EmployeeId == employeeId)
                .Select(es => es.SkillId)
                .ToListAsync();
        }
        public async Task UpdateEmployeeSkillsAsync(int employeeId, List<int> skillIds)
        {
            var currentSkills = await _context.EmployeeSkills
                .Where(es => es.EmployeeId == employeeId)
                .ToListAsync();

            _context.EmployeeSkills.RemoveRange(currentSkills);

            foreach(var skillId in skillIds)
            {
                _context.EmployeeSkills.Add(new EmployeeSkill
                {
                    EmployeeId = employeeId,
                    SkillId = skillId
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}