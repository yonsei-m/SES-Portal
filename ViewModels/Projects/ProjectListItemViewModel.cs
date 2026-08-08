namespace SES_Portal.ViewModels.Projects;

public class ProjectListItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string DepartmentName { get; set; } = "";

    public string Status { get; set; } = "";

    public string? Location { get; set; }

    public string? PriceRange { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsFavorite { get; set; }
}