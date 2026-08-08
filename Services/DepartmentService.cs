using Microsoft.EntityFrameworkCore;
using 社員_求人管理アプリ.Data;
using 社員_求人管理アプリ.Models;

namespace 社員_求人管理アプリ.Services;

public class DepartmentService
{
    private readonly AppDbContext _context;

    public DepartmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetAllAsync()
    {
        return await _context.Departments
            .OrderBy(d => d.Name)
            .ToListAsync();
    }
}