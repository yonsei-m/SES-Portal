namespace SES_Portal.Enums;
using System.ComponentModel.DataAnnotations;
public enum HistoryActionType
{
    [Display(Name = "登録")]
    Create = 1,

    [Display(Name = "更新")]
    Update = 2,

    [Display(Name = "削除")]
    Delete = 3,

    [Display(Name = "復元")]
    Restore = 4
}