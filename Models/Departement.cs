namespace 社員_求人管理アプリ.Models;

public class Department
{
    // 部署Id
    public int Id { get; set; }
    // 部署名
    public string Name { get; set; } = "";
    public List<Employee> Employees { get; set; } = new();
}