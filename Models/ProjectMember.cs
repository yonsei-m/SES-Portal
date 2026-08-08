using System.ComponentModel.DataAnnotations.Schema;

namespace SES_Portal.Models;

public class ProjectMember
{
    // 案件メンバーId
    public int Id { get; set; }
    // プロジェクトId
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    // 社員Id
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    // 参画日
    public DateTime JoinedAt { get; set; } = DateTime.Now;
}