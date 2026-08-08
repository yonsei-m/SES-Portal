using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SES_Portal.Models;
using SES_Portal.Services;

namespace SES_Portal.Pages.Announcements;

public class DetailsModel : PageModel
{
    private readonly AnnouncementService _announcementService;
    private readonly CurrentUserService _currentUserService;
    public DetailsModel(
        AnnouncementService announcementService,
        CurrentUserService currentUserService)
    {
        _announcementService = announcementService;
        _currentUserService = currentUserService;
    }
    public Announcement Announcement { get; set; } = new();
    public Announcement? PreviousAnnouncement { get; set; }
    public Announcement? NextAnnouncement { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var announcement = await _announcementService.GetByIdAsync(id);

        if (announcement == null)
        {
            return NotFound();
        }

        Announcement = announcement;
        PreviousAnnouncement = await _announcementService.GetPreviousAsync(id);
        NextAnnouncement = await _announcementService.GetNextAsync(id);
        
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee != null)
        {
            await _announcementService
                .MarkAsReadAsync(announcement.Id, employee.Id);
        }
        return Page();
    }
}