namespace SES_Portal.Models;

public class Department
{
    // 部署Id
    public int Id { get; set; }
    // 部署名
    public string Name { get; set; } = "";
    public List<Employee> Employees { get; set; } = new();
}