using System.ComponentModel.DataAnnotations;

namespace 社員_求人管理アプリ.Enums;

public enum AnnouncementCategory
{
    [Display(Name = "全体向け")]
    General = 1,

    [Display(Name = "人事")]
    HR = 2,

    [Display(Name = "システム")]
    System = 3,

    [Display(Name = "社内イベント")]
    Event = 4,

    [Display(Name = "研修")]
    Training = 5
}