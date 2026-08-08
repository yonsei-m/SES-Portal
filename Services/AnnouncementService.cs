using Microsoft.EntityFrameworkCore;
using SES_Portal.Data;
using SES_Portal.Models;
using SES_Portal.Enums;

namespace SES_Portal.Services;

public class AnnouncementService
{
    private readonly AppDbContext _context;
    public AnnouncementService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Announcement>> GetAllAsync()
    {
        return await _context.Announcements
            .Where(a => !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
    public async Task<List<Announcement>> GetLatestAsync(int count = 5)
    {
        return await _context.Announcements
            .Where(a => a.IsPublished && !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
    public async Task<Announcement?> GetByIdAsync(int id)
    {
        return await _context.Announcements
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }
    public async Task CreateAsync(Announcement announcement)
    {
        announcement.CreatedAt = DateTime.Now;
        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Announcement announcement)
    {
        _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var announcement = await GetByIdAsync(id);
        if (announcement == null)
        {
            return;
        }
        announcement.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Announcement>> GetListAsync(string? keyword, AnnouncementCategory? category, string? sort, bool unreadOnly, int? employeeId)
    {
        var all = await _context.Announcements.ToListAsync();
        Console.WriteLine($"全件:{all.Count}");
        var published = all
            .Where(a => !a.IsDeleted && a.IsPublished)
            .ToList();
        Console.WriteLine($"公開済み:{published.Count}");
                var query = _context.Announcements
                    .Where(a =>
                        !a.IsDeleted &&
                        a.IsPublished)
                    .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(a =>
                a.Title.Contains(keyword) ||
                a.Content.Contains(keyword));
        }

        if (category.HasValue)
        {
            query = query.Where(x => x.Category == category.Value);
        }
        if (unreadOnly && employeeId.HasValue)
        {
            query = query.Where(a =>
                !_context.AnnouncementReads.Any(r =>
                    r.EmployeeId == employeeId.Value &&
                    r.AnnouncementId == a.Id));
        }
        query = sort == "old"
            ? query.OrderBy(a => a.CreatedAt)
            : query.OrderByDescending(a => a.CreatedAt);

        return await query.ToListAsync();
    }


    public async Task MarkAsReadAsync(int announcementId, int employeeId)
    {
        var exists = await _context.AnnouncementReads
            .AnyAsync(ar =>
                ar.AnnouncementId == announcementId &&
                ar.EmployeeId == employeeId);

        if (exists)
        {
            return;
        }

        _context.AnnouncementReads.Add(new AnnouncementRead
        {
            AnnouncementId = announcementId,
            EmployeeId = employeeId,
            ReadAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }
    public async Task<bool> IsReadAsync(int announcementId, int employeeId)
    {
        return await _context.AnnouncementReads
            .AnyAsync(ar =>
                ar.AnnouncementId == announcementId &&
                ar.EmployeeId == employeeId);
    }
    public async Task<List<int>> GetReadAnnouncementIdsAsync(int employeeId)
    {
        return await _context.AnnouncementReads
            .Where(ar => ar.EmployeeId == employeeId)
            .Select(ar => ar.AnnouncementId)
            .ToListAsync();
    }
    public async Task<Announcement?> GetPreviousAsync(int id)
    {
        var current = await GetByIdAsync(id);
        if (current == null)
            return null;

        return await _context.Announcements
            .Where(a =>
                a.IsPublished &&
                !a.IsDeleted &&
                a.CreatedAt > current.CreatedAt)
            .OrderBy(a => a.CreatedAt)
            .FirstOrDefaultAsync();
    }
    public async Task<Announcement?> GetNextAsync(int id)
    {
        var current = await GetByIdAsync(id);
        if (current == null)
            return null;

        return await _context.Announcements
            .Where(a =>
                a.IsPublished &&
                !a.IsDeleted &&
                a.CreatedAt < current.CreatedAt)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();
    }
}