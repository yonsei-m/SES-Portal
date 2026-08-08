using Microsoft.EntityFrameworkCore;
using SES_Portal.Data;
using SES_Portal.Models;

namespace SES_Portal.Services;

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