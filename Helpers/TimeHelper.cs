namespace MongoBackupBackend.Helpers;

public static class TimeHelper
{
    public static DateTime GetStartTime(string? timeString)
    {
        var dailyTime = TimeSpan.Parse(timeString ?? "00:00");
        var now = DateTime.Now;
        var nextRun = now.Date.Add(dailyTime);
        if (nextRun <= now)
            nextRun = nextRun.AddDays(1);

        return nextRun;
    }
}