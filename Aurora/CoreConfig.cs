namespace Aurora;

public class CoreConfig
{
    public string NorthstarRepository { get; set; } = "R2Northstar/Northstar";

    public string[] PreservedPaths { get; set; } = new[]
    {
        "ns_startup_args.txt", 
        "ns_startup_args_dedi.txt"
    };

    public int UpdateAttemptLimit { get; set; } = 5;
}