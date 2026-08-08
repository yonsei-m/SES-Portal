using System.ComponentModel.DataAnnotations;

namespace SES_Portal.Models;

public class EmployeeSkill
{
    // 社員スキルId
    public int Id { get; set; }
    // 社員Id
    [Required]
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    // スキルId
    [Required]
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    // 経験年数
    public int? ExperienceYears { get; set; }
    // 登録日時
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}