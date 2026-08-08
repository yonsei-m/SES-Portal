using System.ComponentModel.DataAnnotations;
using SES_Portal.Enums;

namespace SES_Portal.Models;

public class Announcement
{
    // お知らせID
    public int Id { get; set; }
    // タイトル
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    // 本文
    [Required]
    public string Content { get; set; } = string.Empty;
    // お知らせ区分
    public AnnouncementCategory Category { get; set; }
    public ICollection<AnnouncementRead> AnnouncementReads { get; set; } = new List<AnnouncementRead>();
    // 添付ファイルパス
    public string? AttachmentPath { get; set; }
    // 添付ファイル名
    public string? AttachmentFileName { get; set; }
    // 添付ファイルサイズ
    public long? AttachmentFileSize { get; set; }
    // 公開フラグ
    public bool IsPublished { get; set; } = true;
    // 削除フラグ
    public bool IsDeleted { get; set; }
    // 作成日時
    public DateTime CreatedAt { get; set; } = DateTime.Now;

}