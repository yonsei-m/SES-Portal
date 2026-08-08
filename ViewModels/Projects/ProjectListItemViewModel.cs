namespace 社員_求人管理アプリ.ViewModels.Projects;

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