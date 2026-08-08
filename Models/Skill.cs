using System.ComponentModel.DataAnnotations;

namespace SES_Portal.Models;

public class Skill
{
    // スキルId
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    // スキル名
    public string Name { get; set; } = string.Empty;
    [StringLength(50)]
    // スキル区分
    public string? Category { get; set; }
    // 表示順
    public int SortOrder { get; set; }
    // 有効フラグ
    public bool IsActive { get; set; } = true;
    public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
}