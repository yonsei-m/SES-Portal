using System.ComponentModel.DataAnnotations;

namespace 社員_求人管理アプリ.Enums;

public enum EmployeeStatus
{
    [Display(Name = "稼働中")]
    Working = 1,

    [Display(Name = "待機中")]
    Waiting = 2,

    [Display(Name = "休職中")]
    Leave = 3,

    [Display(Name = "退職済")]
    Retired = 4
}