using System.ComponentModel.DataAnnotations;

namespace SES_Portal.Enums;

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