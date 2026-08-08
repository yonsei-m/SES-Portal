namespace SES_Portal.Helpers;

public static class ChatDateHelper
{
    public static string Format(DateTime? date)
    {
        if(date == null)
        {
            return "";
        }


        var localDate = date.Value.ToLocalTime();

        var now = DateTime.Now;


        // 今日
        if(localDate.Date == now.Date)
        {
            return localDate.ToString("HH:mm");
        }


        // 昨日
        if(localDate.Date == now.Date.AddDays(-1))
        {
            return $"昨日 {localDate:HH:mm}";
        }


        // 今年
        if(localDate.Year == now.Year)
        {
            return localDate.ToString("M/d HH:mm");
        }


        // それ以前
        return localDate.ToString("yyyy/M/d HH:mm");
    }
        // メッセージ横表示用
    public static string FormatTime(DateTime? date)
    {
        if(date == null)
        {
            return "";
        }

        return date.Value
            .ToLocalTime()
            .ToString("HH:mm");
    }
}