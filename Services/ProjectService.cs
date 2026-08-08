using Microsoft.EntityFrameworkCore;
using 社員_求人管理アプリ.Data;
using 社員_求人管理アプリ.Models;

namespace 社員_求人管理アプリ.Services;

public class ProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }
    private IQueryable<Project> GetProjectQuery()
    {
        return _context.Projects
            .Include(p => p.Department)
            .Where(p => !p.IsDeleted);
    }

    public async Task<(List<Project> Projects, int TotalCount)> GetProjectsAsync(string? searchTitle, string? searchPrefecture, string? searchStatus, string? sortColumn, string? sortOrder, int pageNumber, int pageSize)
    {
        var query = GetProjectQuery();

        query = ApplySearch(query, searchTitle, searchPrefecture, searchStatus);

        query = ApplySort(query, sortColumn, sortOrder);

        var totalCount = await query.CountAsync();

        var projects = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (projects, totalCount);
    }
    public async Task<Project?> GetByIdAsync(int id)
    {
            return await _context.Projects
                .Include(p => p.Department)
                .Include(p => p.Members)
                    .ThenInclude(pm => pm.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Project project)
    {
        var existing = await _context.Projects.FindAsync(project.Id);

        if (existing == null)
            return false;

        existing.Title = project.Title;
        existing.Description = project.Description;
        existing.DepartmentId = project.DepartmentId;
        existing.Status = project.Status;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
            return false;

        project.IsDeleted = true;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
            return false;

        project.Status = project.Status == "Open"
            ? "Closed"
            : "Open";

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<List<Project>> GetDeletedProjectsAsync()
    {
        return await _context.Projects
            .Include(p => p.Department)
            .Where(p => p.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> RestoreAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
        {
            return false;
        }

        project.IsDeleted = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetOpenCountAsync()
    {
        return await GetProjectQuery()
            .CountAsync(p => p.Status == "Open");
    }

    public async Task<int> GetClosedCountAsync()
    {
        return await GetProjectQuery()
            .CountAsync(p => p.Status == "Closed");
    }

    private IQueryable<Project> ApplySearch(IQueryable<Project> query, string? title, string? searchPrefecture, string? status)
    {
        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(p => p.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(searchPrefecture))
        {
            query = query.Where(p =>
                p.Location != null &&
                p.Location.StartsWith(searchPrefecture));
        }

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.Status == status);

        return query;
    }

    private IQueryable<Project> ApplySort(IQueryable<Project> query, string? sortColumn, string? sortOrder)
    {
        sortColumn ??= "created";
        sortOrder ??= "desc";

        return (sortColumn, sortOrder) switch
        {
            ("title", "asc") => query.OrderBy(p => p.Title),
            ("title", "desc") => query.OrderByDescending(p => p.Title),

            ("dept", "asc") => query.OrderBy(p => p.Department.Name),
            ("dept", "desc") => query.OrderByDescending(p => p.Department.Name),

            ("created", "asc") => query.OrderBy(p => p.CreatedAt),

            _ => query.OrderByDescending(p => p.CreatedAt)
        };
    }
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Projects
            .Where(p => !p.IsDeleted)
            .CountAsync();
    }
}
