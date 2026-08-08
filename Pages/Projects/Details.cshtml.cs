using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SES_Portal.Services;

namespace SES_Portal.Pages.Projects;
public class DetailsModel : PageModel
{
    private readonly ProjectService _projectService;
    private readonly FavoriteProjectService _favoriteProjectService;
    private readonly CurrentUserService _currentUserService;
    public DetailsModel(
        ProjectService projectService,
        FavoriteProjectService favoriteProjectService,
        CurrentUserService currentUserService)
    {
        _projectService = projectService;
        _favoriteProjectService = favoriteProjectService;
        _currentUserService = currentUserService;
    }
    public Project Project { get; set; } = null!;
    // 必須スキル表示用
    public List<string> SkillList { get; set; } = new();
    public List<Employee> Members { get; set; } = new();
    public bool IsFavorite { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        Project = project;
        var favoriteProjectIds = await GetFavoriteProjectIdsAsync();
        IsFavorite = favoriteProjectIds.Contains(Project.Id);

        Members = Project.Members
            .Select(pm => pm.Employee)
            .ToList();


        // C#,SQL,AWS のような文字列を分割
        if (!string.IsNullOrWhiteSpace(Project.RequiredSkills))
        {
            SkillList = Project.RequiredSkills
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
        }
        return Page();
    }
    public async Task<IActionResult> OnPostFavoriteAsync(int id)
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee == null)
        {
            return RedirectToPage(new { id });
        }
        var isFavorite = await _favoriteProjectService.IsFavoriteAsync(employee.Id, id);
        if (isFavorite)
        {
            await _favoriteProjectService.RemoveFavoriteAsync(employee.Id, id);
        }
        else
        {
            await _favoriteProjectService.AddFavoriteAsync(employee.Id, id);
        }
        return RedirectToPage(new { id });
    }
    private async Task<HashSet<int>> GetFavoriteProjectIdsAsync()
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee == null)
        {
            return new HashSet<int>();
        }
        return (await _favoriteProjectService.GetFavoriteProjectIdsAsync(employee.Id))
            .ToHashSet();
    }
}