public class FavoriteProject
{
    // お気に入りプロジェクトId
    public int Id { get; set; }
    // 社員Id
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public int ProjectId { get; set; }
    // プロジェクトId
    public Project Project { get; set; } = null!;
    // 登録日時
    public DateTime CreatedAt { get; set; }
}