using Microsoft.EntityFrameworkCore;
using 社員_求人管理アプリ.Data;
using 社員_求人管理アプリ.Models;

namespace 社員_求人管理アプリ.Services;

public class FavoriteProjectService
{
    private readonly AppDbContext _context;

    public FavoriteProjectService(AppDbContext context)
    {
        _context = context;
    }


    // お気に入り追加
    public async Task AddFavoriteAsync(int employeeId, int projectId)
    {
        var exists =
            await _context.FavoriteProjects
                .AnyAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.ProjectId == projectId);

        if (exists)
        {
            return;
        }

        var favorite = new FavoriteProject
        {
            EmployeeId = employeeId,
            ProjectId = projectId,
            CreatedAt = DateTime.Now
        };

        _context.FavoriteProjects.Add(favorite);

        await _context.SaveChangesAsync();
    }

    // お気に入り解除
    public async Task RemoveFavoriteAsync(int employeeId, int projectId)
    {
        var favorite =
            await _context.FavoriteProjects
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.ProjectId == projectId);

        if (favorite == null)
        {
            return;
        }

        _context.FavoriteProjects.Remove(favorite);

        await _context.SaveChangesAsync();
    }

    // お気に入り登録済み判定
    public async Task<bool> IsFavoriteAsync(int employeeId, int projectId)
    {
        return await _context.FavoriteProjects
            .AnyAsync(x =>
                x.EmployeeId == employeeId &&
                x.ProjectId == projectId);
    }

    // 社員のお気に入り案件取得
    public async Task<List<FavoriteProject>> GetFavoritesByEmployeeAsync(int employeeId)
    {
        return await _context.FavoriteProjects
            .Include(x => x.Project)
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }


    // 案件一覧の星表示用
    public async Task<List<int>> GetFavoriteProjectIdsAsync(int employeeId)
    {
        return await _context.FavoriteProjects
            .Where(x => x.EmployeeId == employeeId)
            .Select(x => x.ProjectId)
            .ToListAsync();
    }
}