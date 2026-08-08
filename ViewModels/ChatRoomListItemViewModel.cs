namespace SES_Portal.ViewModels;

public class ChatRoomListItemViewModel
{
    public int RoomId { get; set; }
    public int PartnerEmployeeId { get; set; }
    public string PartnerName { get; set; } = "";
    public string? PartnerImagePath { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public bool IsWorking { get; set; }
}