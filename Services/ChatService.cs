using Microsoft.EntityFrameworkCore;
using 社員_求人管理アプリ.Data;
using 社員_求人管理アプリ.Models;
using 社員_求人管理アプリ.ViewModels;


namespace 社員_求人管理アプリ.Services;

public class ChatService
{
    private readonly AppDbContext _context;
    private readonly AttendanceService _attendanceService;
    public ChatService(
        AppDbContext context, 
        AttendanceService attendanceService)
    {
        _context = context;
        _attendanceService = attendanceService;
    }


    public async Task<List<ChatMessage>> GetMessagesAsync(int roomId)
    {
        return await _context.ChatMessages
            .Where(x => x.RoomId == roomId)
            .OrderBy(x => x.SentAt)
            .ToListAsync();
    }
    public async Task<ChatMessage> SendMessageAsync(int roomId, string senderId, string senderName, string message, string? fileName, string? filePath, long? fileSize, string? contentType)
    {
        var chatMessage = new ChatMessage
        {
            RoomId = roomId,
            SenderId = senderId,
            SenderName = senderName,
            Message = message,
            SentAt = DateTime.UtcNow,
            IsRead = false,

            FileName = fileName,
            FilePath = filePath,
            FileSize = fileSize,
            ContentType = contentType
        };

        _context.ChatMessages.Add(chatMessage);

        var room = await _context.ChatRooms.FindAsync(roomId);

        if (room != null)
        {
            room.LastMessage = message;
            room.LastMessageAt = chatMessage.SentAt;
        }

        await _context.SaveChangesAsync();

        return chatMessage;
    }

    public async Task<ChatRoom> GetOrCreateRoomAsync(string currentUserId, string targetUserId)
    {
        var room = await _context.ChatRooms.FirstOrDefaultAsync(r =>
            (r.User1Id == currentUserId && r.User2Id == targetUserId) ||
            (r.User1Id == targetUserId && r.User2Id == currentUserId));

        if (room != null)
        {
            return room;
        }

        room = new ChatRoom
        {
            User1Id = currentUserId,
            User2Id = targetUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatRooms.Add(room);
        await _context.SaveChangesAsync();

        return room;
    }

    public async Task<List<ChatRoomListItemViewModel>> GetRoomsAsync(string currentUserId)
    {
        var rooms = await _context.ChatRooms
            .Where(r =>
                r.User1Id == currentUserId ||
                r.User2Id == currentUserId)
            .OrderByDescending(r => r.LastMessageAt)
            .ToListAsync();

        var result = new List<ChatRoomListItemViewModel>();

        foreach (var room in rooms)
        {
            var partnerUserId =
                room.User1Id == currentUserId
                ? room.User2Id
                : room.User1Id;

            var employee = await _context.Employees .FirstOrDefaultAsync(e => e.UserId == partnerUserId);

            if (employee == null)
                continue;
            var isWorking = await _attendanceService.IsWorkingAsync(employee.Id);
            result.Add(new ChatRoomListItemViewModel
            {
                RoomId = room.Id,
                PartnerEmployeeId = employee.Id,
                PartnerName = employee.Name,
                PartnerImagePath = employee.ImagePath,
                LastMessage = room.LastMessage,
                LastMessageAt = room.LastMessageAt,
                IsWorking = isWorking,
                UnreadCount = await _context.ChatMessages
                    .CountAsync(m =>
                        m.RoomId == room.Id &&
                        m.SenderId != currentUserId &&
                        !m.IsRead)
            });
        }

        return result;
    }
    public async Task MarkAsReadAsync(int roomId, string currentUserId)
    {
        var unreadMessages = await _context.ChatMessages
                .Where(m =>
                    m.RoomId == roomId &&
                    m.SenderId != currentUserId &&
                    !m.IsRead)
                .ToListAsync();


        foreach(var message in unreadMessages)
        {
            message.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(int roomId, string userId)
    {
        return await _context.ChatMessages
            .CountAsync(m =>
                m.RoomId == roomId &&
                m.SenderId != userId &&
                !m.IsRead);
    }

    public async Task<List<SharedFileViewModel>> GetSharedFilesAsync(int roomId)
    {
        return await _context.ChatMessages
            .Where(m =>
                m.RoomId == roomId &&
                !string.IsNullOrEmpty(m.FilePath))
            .OrderByDescending(m => m.SentAt)
            .Select(m => new SharedFileViewModel
            {
                FileName = m.FileName!,
                FilePath = m.FilePath!,
                FileSize = m.FileSize ?? 0,
                ContentType = m.ContentType ?? "",
                SentAt = m.SentAt,
                SenderName = m.SenderName
            })
            .ToListAsync();
    }

    public async Task<List<ChatMemo>> GetMemosAsync(int roomId)
    {
        return await _context.ChatMemos
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.UpdatedAt)
            .ToListAsync();
    }    
    public async Task CreateMemoAsync(int roomId, string title, string content, string userId)
    {
        var memo = new ChatMemo
        {
            RoomId = roomId,
            Title = title,
            Content = string.IsNullOrWhiteSpace(content)
                ? null
                : content,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ChatMemos.Add(memo);

        await _context.SaveChangesAsync();
    }
    public async Task UpdateMemoAsync(int memoId, string title, string content)
    {
        var memo = await _context.ChatMemos
            .FirstOrDefaultAsync(m => m.Id == memoId);

        if (memo == null)
        {
            return;
        }

        memo.Title = title;
        memo.Content = content;
        memo.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
    public async Task DeleteMemoAsync(int memoId)
    {
        var memo = await _context.ChatMemos
            .FirstOrDefaultAsync(m => m.Id == memoId);

        if (memo == null)
        {
            return;
        }

        _context.ChatMemos.Remove(memo);

        await _context.SaveChangesAsync();
    }
}
