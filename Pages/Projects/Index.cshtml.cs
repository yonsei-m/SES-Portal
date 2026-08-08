using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SES_Portal.Data;
using SES_Portal.Services;
using SES_Portal.ViewModels.Projects;

namespace SES_Portal.Pages.Projects;

public class IndexModel : PageModel
{
   private readonly ProjectService _projectService;
   private readonly FavoriteProjectService _favoriteProjectService;
   private readonly CurrentUserService _currentUserService;
   public IndexModel(
      ProjectService projectService,
      FavoriteProjectService favoriteProjectService,
      CurrentUserService currentUserService)
   {
      _projectService = projectService;
      _favoriteProjectService = favoriteProjectService;
      _currentUserService = currentUserService;
      // preserve previous default sort order for this page
      SortOrder = "desc";
   }


   public ProjectIndexViewModel ViewModel { get; set; } = new();

   public IReadOnlyList<string> PrefectureList => Constants.Prefectures.All;

   [BindProperty(SupportsGet = true)]
    public string? SearchTitle { get; set; }
   [BindProperty(SupportsGet = true)]
   public string? SearchPrefecture { get; set; }
   [BindProperty(SupportsGet = true)]
   public string? SearchStatus { get; set; }
   [BindProperty(SupportsGet = true)]
   public bool OnlyFavorite { get; set; }
   [BindProperty(SupportsGet = true)]
   public int PageNumber { get; set; } = 1;
   public int PageSize { get; set; } = 20;
   public int TotalCount { get; set; }
   public int TotalPages =>
      (int)Math.Ceiling((double)TotalCount / PageSize);

   [BindProperty(SupportsGet = true)]
   public string? SortColumn { get; set; }
   [BindProperty(SupportsGet = true)]
   public string? SortOrder { get; set; } = "desc";
   public async Task OnGetAsync()
   {
      ViewModel = new ProjectIndexViewModel();
      var (projects, totalCount) = await _projectService.GetProjectsAsync(
         SearchTitle,
         SearchPrefecture,
         SearchStatus,
         SortColumn,
         SortOrder,
         PageNumber,
         PageSize);
         
      TotalCount = totalCount;

      var favoriteProjectIds = await GetFavoriteProjectIdsAsync();
      ViewModel.Summary = new ProjectSummaryViewModel
      {
         TotalCount = await _projectService.GetTotalCountAsync(),
         OpenCount = await _projectService.GetOpenCountAsync(),
         ClosedCount = await _projectService.GetClosedCountAsync(),
         FavoriteCount = favoriteProjectIds.Count
      };

      if (OnlyFavorite)
      {
         projects = projects
            .Where(p => favoriteProjectIds.Contains(p.Id))
            .ToList();
      }
      ViewModel.Projects = projects
         .Select(project => CreateProjectListItem(project, favoriteProjectIds))
         .ToList();
   }

   public string GetSortOrder(string column)
   {
      if (SortColumn != column) return "asc";
      return SortOrder == "asc" ? "desc" : "asc";
   }

   public string GetSortIcon(string column)
   {
      if (SortColumn != column) return "⇅";
      return SortOrder == "asc" ? "▲" : "▼";
   }

   public async Task<IActionResult> OnPostToggleStatusAsync(int id)
   {
      var success = await _projectService.ToggleStatusAsync(id);
      if (!success)
      {
         return NotFound();
      }
      return RedirectToPage();
   }

   public async Task<IActionResult> OnPostFavoriteAsync(int id)
   {
      var employee =
         await _currentUserService.GetCurrentEmployeeAsync(User);

      if (employee == null)
      {
         return RedirectToPage();
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
      return RedirectToPage();
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
   private static ProjectListItemViewModel CreateProjectListItem(Project project, HashSet<int> favoriteProjectIds)
   {
      return new ProjectListItemViewModel
      {
         Id = project.Id,
         Title = project.Title,
         DepartmentName = project.Department.Name,
         Status = project.Status,
         Location = project.Location,
         PriceRange = project.PriceRange,
         CreatedAt = project.CreatedAt,
         IsFavorite = favoriteProjectIds.Contains(project.Id)
      };
   }
}