namespace 社員_求人管理アプリ.ViewModels;

public class SharedFileViewModel
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string SenderName { get; set; } = string.Empty;
}