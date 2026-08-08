using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using 社員_求人管理アプリ.Models;
using 社員_求人管理アプリ.Services;
using 社員_求人管理アプリ.ViewModels;

namespace 社員_求人管理アプリ.Pages.Chat;

public class IndexModel : PageModel
{
    private readonly EmployeeService _employeeService;
    private readonly ChatService _chatService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IWebHostEnvironment _environment;
    private readonly AttendanceService _attendanceService;
    private readonly CurrentUserService _currentUserService;
    public IndexModel(
        EmployeeService employeeService, 
        ChatService chatService, 
        UserManager<IdentityUser> userManager, 
        IHubContext<ChatHub> hubContext, 
        IWebHostEnvironment environment, 
        AttendanceService attendanceService,
        CurrentUserService currentUserService)
    {
        _employeeService = employeeService;
        _chatService = chatService;
        _userManager = userManager;
        _hubContext = hubContext;
        _environment = environment;
        _attendanceService = attendanceService;
        _currentUserService = currentUserService;
    }

    public Employee? Employee { get; private set; }
    public ChatRoom? Room { get; private set; }
    public List<ChatMessage> Messages { get; private set; } = new();
    public List<ChatRoomListItemViewModel> Rooms { get; private set; } = [];
    public List<SharedFileViewModel> SharedFiles { get; set; } = [];
    public string CurrentUserId { get; private set; } = "";

    [BindProperty]
    public string Message { get; set; } = "";
    [BindProperty]
    public IFormFile? Attachment { get; set; }
    public List<ChatMemo> Memos { get; set; } = new();
    [BindProperty]
    public int MemoId { get; set; }

    [BindProperty]
    public string MemoTitle { get; set; } = "";

    [BindProperty]
    public string MemoContent { get; set; } = "";
    public string AttendanceStatusText { get; set; } = "退勤済み";
    public bool IsWorking { get; set; }
    public async Task<IActionResult> OnGetAsync(int? id, bool markRead = true)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if(currentUser == null)
        {
            return Unauthorized();
        }

        CurrentUserId = currentUser.Id;
        Rooms = await _chatService.GetRoomsAsync(currentUser.Id);

        // 相手未選択の場合
        if(!id.HasValue)
        {
            return Page();
        }
        Employee = await _employeeService.GetByIdAsync(id.Value);
        
        if (Employee == null)
        {
            return NotFound();
        }

        if(string.IsNullOrEmpty(Employee!.UserId))
        {
            return BadRequest();
        }
        if(Employee != null)
        {
            var todayAttendance = await _attendanceService
                .GetTodayAttendanceAsync(Employee.Id);


            if(todayAttendance?.ClockIn != null &&
            todayAttendance.ClockOut == null)
            {
                AttendanceStatusText = "勤務中";
                IsWorking = true;
            }
            else
            {
                AttendanceStatusText = "退勤済み";
                IsWorking = false;
            }
        }
        Room = await _chatService.GetOrCreateRoomAsync(currentUser.Id, Employee.UserId);
        Messages = await _chatService.GetMessagesAsync(Room.Id);
        SharedFiles = await _chatService.GetSharedFilesAsync(Room.Id);
        Memos = await _chatService.GetMemosAsync(Room.Id);
        
        // DBを既読化
        if(markRead)
        {
            await _chatService.MarkAsReadAsync(
                Room.Id,
                currentUser.Id
            );
        }
        await _hubContext.Clients
            .Group(Room.Id.ToString())
            .SendAsync(
                "ReceiveRead",
                Room.Id,
                currentUser.Id
            );

        Rooms = await _chatService.GetRoomsAsync(currentUser.Id);

        return Page();
    }

    public async Task<IActionResult> OnPostSendAsync(int id)
    {
        
        Employee = await _employeeService.GetByIdAsync(id);
        if (Employee == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(Message) && Attachment == null)
        {
            Messages = await _chatService.GetMessagesAsync(id);
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        if(currentUser == null)
        {
            return Unauthorized();
        }

        CurrentUserId = currentUser.Id;

        Room = await _chatService.GetOrCreateRoomAsync(currentUser.Id, Employee.UserId!);

        string? fileName = null;
        string? filePath = null;
        long? fileSize = null;
        string? contentType = null;

        if (Attachment != null && Attachment.Length > 0)
        {
            var uploadFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "chat");

            Directory.CreateDirectory(uploadFolder);

            var savedFileName = $"{Guid.NewGuid()}{Path.GetExtension(Attachment.FileName)}";

            var savePath = Path.Combine(uploadFolder, savedFileName);

            using var stream = new FileStream(savePath, FileMode.Create);

            await Attachment.CopyToAsync(stream);

            fileName = Attachment.FileName;
            filePath = $"/uploads/chat/{savedFileName}";
            fileSize = Attachment.Length;
            contentType = Attachment.ContentType;
        }
                    


        var chatMessage = await _chatService.SendMessageAsync(
            Room.Id,
            currentUser.Id,
            currentUser.UserName ?? "ユーザー",
            Message?.Trim() ?? "",
            fileName,
            filePath,
            fileSize,
            contentType);

        var unreadCount = await _chatService.GetUnreadCountAsync(
            Room.Id,
            currentUser.Id);


        var receiverUserId = Employee.UserId;


        // 送信者へ通知
        await _hubContext.Clients
            .Group($"user_{currentUser.Id}")
            .SendAsync(
                "ReceiveMessage",
                Room.Id,
                chatMessage.SenderId,
                chatMessage.SenderName,
                chatMessage.Message,
                chatMessage.FileName,
                chatMessage.FilePath,
                chatMessage.FileSize,
                chatMessage.ContentType,
                chatMessage.SentAt,
                unreadCount);


        // 受信者へ通知
        await _hubContext.Clients
            .Group($"user_{receiverUserId}")
            .SendAsync(
                "ReceiveMessage",
                Room.Id,
                chatMessage.SenderId,
                chatMessage.SenderName,
                chatMessage.Message,
                chatMessage.FileName,
                chatMessage.FilePath,
                chatMessage.FileSize,
                chatMessage.ContentType,
                chatMessage.SentAt,
                unreadCount);

        
        Messages = await _chatService.GetMessagesAsync(Room.Id);
        Message = string.Empty;

        Rooms = await _chatService.GetRoomsAsync(currentUser.Id);

        return new JsonResult(new
        {
            success = true
        });
    }

    public async Task<IActionResult> OnPostReadAsync(int roomId)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if(currentUser == null)
        {
            return Unauthorized();
        }


        await _chatService.MarkAsReadAsync(
            roomId,
            currentUser.Id);


        await _hubContext.Clients
            .Group(roomId.ToString())
            .SendAsync(
                "ReceiveRead",
                roomId,
                currentUser.Id);


        return new JsonResult(new
        {
            success = true
        });
    }
    public async Task<IActionResult> OnPostCreateMemoAsync(int id)
    {
        var currentUser =
            await _userManager.GetUserAsync(User);

        if(currentUser == null)
        {
            return Unauthorized();
        }


        var room =
            await _chatService.GetOrCreateRoomAsync(
                currentUser.Id,
                (await _employeeService.GetByIdAsync(id))!.UserId!);


        await _chatService.CreateMemoAsync(
            room.Id,
            MemoTitle,
            MemoContent,
            currentUser.Id);


        return RedirectToPage(
            new { 
                id,
                markRead = false
            }
        );
    }    
    public async Task<IActionResult> OnPostUpdateMemoAsync(int id)
    {
        var currentUser =
            await _userManager.GetUserAsync(User);

        if(currentUser == null)
        {
            return Unauthorized();
        }


        if(MemoId == 0)
        {
            var employee =
                await _employeeService.GetByIdAsync(id);

            if(employee == null || string.IsNullOrEmpty(employee.UserId))
            {
                return NotFound();
            }


            var room =
                await _chatService.GetOrCreateRoomAsync(
                    currentUser.Id,
                    employee.UserId);


            await _chatService.CreateMemoAsync(
                room.Id,
                MemoTitle,
                MemoContent,
                currentUser.Id);
        }
        else
        {
            await _chatService.UpdateMemoAsync(
                MemoId,
                MemoTitle,
                MemoContent);
        }


        return RedirectToPage(
            new { 
                id,
                markRead = false
            }
        );
    }
    public async Task<IActionResult> OnPostDeleteMemoAsync(int id)
    {
        await _chatService.DeleteMemoAsync(
            MemoId);


        return RedirectToPage(
            new { 
                id,
                markRead = false
            }
        );
    }
}
