using Microsoft.EntityFrameworkCore;
using 社員_求人管理アプリ.Data;
namespace 社員_求人管理アプリ.Services;
public class SidebarService
{
    private readonly AppDbContext _context;

    public SidebarService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<int> GetUnreadChatCountAsync(string userId)
    {
        return await _context.ChatMessages
            .CountAsync(m =>
                !m.IsRead &&
                m.SenderId != userId &&
                (m.Room!.User1Id == userId || m.Room.User2Id == userId));
    }


    public async Task<int> GetUnreadAnnouncementCountAsync(string userId)
    {
        var employeeId = await _context.Employees
            .Where(e => e.UserId == userId)
            .Select(e => (int?)e.Id)
            .FirstOrDefaultAsync();

        if (employeeId == null)
        {
            return 0;
        }

        return await _context.Announcements
            .Where(a => a.IsPublished && !a.IsDeleted)
            .CountAsync(a => !a.AnnouncementReads
                .Any(r => r.EmployeeId == employeeId));
    }
}