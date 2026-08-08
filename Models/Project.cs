using SES_Portal.Models;

public class Project
{
    // プロジェクト名
    public int Id { get; set; }
    // プロジェクトコード
    public string ProjectCode { get; set; } = string.Empty;
    // プロジェクト名
    public string Title { get; set; } = string.Empty;
    // プロジェクト概要
    public string? Description { get; set; }
    // 所属部署
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    // 開始日
    public DateTime? StartDate { get; set; }
    // 終了予定日
    public DateTime? EndDate { get; set; }
    // 勤務地
    public string? Location { get; set; }
    // 単価
    public string? PriceRange { get; set; }
    // 必須スキル
    public string? RequiredSkills { get; set; }
    // 公開状況
    public string Status { get; set; } = "Open";
    // 登録日時
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    // 削除フラグ
    public bool IsDeleted { get; set; }
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}