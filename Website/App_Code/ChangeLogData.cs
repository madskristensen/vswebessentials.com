using System;

public class ChangeLogData
{
    private DateTime expirationDate;

    public string ChangeLogOutput { get; set; }

    public bool IsGitHubApiSuccessed { get; set; }

    public DateTime ExpirationDateTime
    {
        get
        {
            return expirationDate;
        }
    }

    internal void UpdateExpiration()
    {
        int minRange, maxRange;
        if (this.IsGitHubApiSuccessed)
        {
            minRange = 50; maxRange = 60;
        }
        else
        {
            minRange = 1; maxRange = 5;
        }

        // So the same person will not wait for both output (WE2013 and WE2015) to regenerate, (depending of number of user per hour):
        expirationDate = DateTime.Now.AddMinutes(new Random().Next(minRange, maxRange));
    }
}