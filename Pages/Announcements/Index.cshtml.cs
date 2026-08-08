using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SES_Portal.Models;
using SES_Portal.Services;
using SES_Portal.Enums;

namespace SES_Portal.Pages.Announcements;
public class IndexModel : PageModel
{
    private readonly AnnouncementService _announcementService;
    private readonly CurrentUserService _currentUserService;

    public IndexModel(
        AnnouncementService announcementService,
        CurrentUserService currentUserService)
    {
        _announcementService = announcementService;
        _currentUserService = currentUserService;
    }
    public List<Announcement> Announcements { get; set; } = new List<Announcement>();
    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }
    [BindProperty(SupportsGet = true)]
    public AnnouncementCategory? Category { get; set; }
    [BindProperty(SupportsGet = true)]
    public string Sort { get; set; } = "new";
    public HashSet<int> ReadAnnouncementIds { get; set; } = new();
    [BindProperty(SupportsGet = true)]
    public bool UnreadOnly { get; set; }
    public async Task OnGetAsync()
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee != null)
        {
            ReadAnnouncementIds =
                (await _announcementService
                    .GetReadAnnouncementIdsAsync(employee.Id))
                .ToHashSet();

            Announcements =
                await _announcementService
                    .GetListAsync(Keyword, Category, Sort, UnreadOnly, employee.Id);
        } 
    }
}