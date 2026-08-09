namespace SES_Portal.Models;

public class ChatRoom
{
    // チャットルームId
    public int Id { get; set; }
    // ユーザー1Id
    public string User1Id { get; set; } = "";
    // ユーザー2Id
    public string User2Id { get; set; } = "";
    // 最終メッセージ
    public string? LastMessage { get; set; }
    // 最終送信日時
    public DateTime? LastMessageAt { get; set; }
    // メッセージ一覧
    public List<ChatMessage> Messages { get; set; } = new();
    // 共有メモ一覧
    public List<ChatMemo> Memos { get; set; } = new();
    // 作成日時
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}