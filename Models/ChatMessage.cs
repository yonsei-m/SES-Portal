namespace 社員_求人管理アプリ.Models;

public class ChatMessage
{
    // チャットメッセージId
    public int Id { get; set; }
    // チャットルームId
    public int RoomId { get; set; }
    public ChatRoom? Room { get; set; }
    // 送信者Id
    public string SenderId { get; set; } = "";
    // 送信者名
    public string SenderName { get; set; } = "";
    // メッセージ本文
    public string Message { get; set; } = "";
    // 送信日時
    public DateTime SentAt { get; set; }
    // 既読フラグ
    public bool IsRead { get; set; }
    // 添付ファイル名
    public string? FileName { get; set; }
    // 添付ファイルパス
    public string? FilePath { get; set; }
    // 添付ファイルサイズ
    public long? FileSize { get; set; }
    // コンテンツタイプ
    public string? ContentType { get; set; }    
}
