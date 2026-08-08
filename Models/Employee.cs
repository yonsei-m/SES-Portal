using 社員_求人管理アプリ.Models;
using 社員_求人管理アプリ.Enums;

public class Employee
{
    // 社員Id
    public int Id { get; set; }
    // 社員番号
    public string EmployeeNumber { get; set; } = "";
    // 氏名
    public string Name { get; set; } = "";
    // 部署Id
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    // 稼働状況
    public EmployeeStatus Status { get; set; }
    // メールアドレス
    public string Email { get; set; } = "";
    // 入社日
    public DateTime HireDate { get; set; }
    // プロフィール画像パス
    public string? ImagePath { get; set; }
    // ユーザーId
    public string? UserId { get; set; }
    // 自己紹介
    public string? SelfIntroduction { get; set; }
    // 経験年数
    public int? ExperienceYears { get; set; }
    // 削除フラグ
    public bool IsDeleted { get; set; } 
    public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
    public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
    public ICollection<AnnouncementRead> AnnouncementReads { get; set; } = new List<AnnouncementRead>();
}