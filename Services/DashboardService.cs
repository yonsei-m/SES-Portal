using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using 社員_求人管理アプリ.ViewModels;
using 社員_求人管理アプリ.Data;
using 社員_求人管理アプリ.Models;

namespace 社員_求人管理アプリ.Services;

public class DashboardService
{
    private readonly AppDbContext _context;
    private readonly AnnouncementService _announcementService;
    private readonly FavoriteProjectService _favoriteProjectService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DashboardService(AppDbContext context, AnnouncementService announcementService, FavoriteProjectService favoriteProjectService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _announcementService = announcementService;
        _favoriteProjectService = favoriteProjectService;
        _httpContextAccessor = httpContextAccessor;
    }



    // 公開中求人
    public async Task<int> GetOpenProjectCountAsync()
    {
        return await _context.Projects
            .Where(p => !p.IsDeleted && p.Status == "Open")
            .CountAsync();
    }

    // お気に入り案件数
    public async Task<int> GetFavoriteProjectCountAsync(int employeeId)
    {
        return await _context.FavoriteProjects
            .CountAsync(f => f.EmployeeId == employeeId);
    }
    // 最新求人
    public async Task<List<Project>> GetLatestProjectsAsync(int count = 5)
    {
        return await _context.Projects
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    // 最新お知らせ
    public async Task<List<DashboardAnnouncementViewModel>> GetLatestAnnouncementsAsync(int count = 7)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User.FindFirstValue(ClaimTypes.NameIdentifier);


        var employeeId = await _context.Employees
            .Where(e => e.UserId == userId)
            .Select(e => e.Id)
            .FirstOrDefaultAsync();


        return await _context.Announcements
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .Select(a => new DashboardAnnouncementViewModel
            {
                Id = a.Id,
                Title = a.Title,
                CreatedAt = a.CreatedAt,

                IsRead = a.AnnouncementReads
                    .Any(r => r.EmployeeId == employeeId)
            })
            .ToListAsync();
    }


}