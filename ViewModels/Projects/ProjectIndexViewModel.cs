using SES_Portal.Models;

namespace SES_Portal.ViewModels.Projects;

public class ProjectIndexViewModel
{
    public List<ProjectListItemViewModel> Projects { get; set; } = new();
    public ProjectSummaryViewModel Summary { get; set; } = new();

}