using 社員_求人管理アプリ.Models;

namespace 社員_求人管理アプリ.ViewModels.Projects;

public class ProjectIndexViewModel
{
    public List<ProjectListItemViewModel> Projects { get; set; } = new();
    public ProjectSummaryViewModel Summary { get; set; } = new();

}