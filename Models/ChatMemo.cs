namespace 社員_求人管理アプリ.Models;
public class ChatMemo
{
    // チャットメモId
    public int Id { get; set; }
    // チャットルームId
    public int RoomId { get; set; }
    public ChatRoom Room { get; set; } = null!;
    // メモタイトル
    public string Title { get; set; } = "";
    // 本文
    public string? Content { get; set; }
    // 作成者Id
    public string CreatedBy { get; set; } = "";
    // 作成日時
    public DateTime CreatedAt { get; set; }
    // 更新日時
    public DateTime UpdatedAt { get; set; }
}