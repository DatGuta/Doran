namespace DR.Database.ExtendModels;

public class EmailReport {
    public int Hour { get; set; }
    public bool Day { get; set; }
    public bool Week { get; set; }
    public bool Month { get; set; }
    public bool IsSend { get; set; }
    public List<string> Emails { get; set; } = new();
}
