using Aurora;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine(Config.CoreConfig.UpdateAttemptLimit);
        Config.CoreConfig.UpdateAttemptLimit = 1;
        var _ = Updater.DownloadNorthstar(new Progress<float>(Console.WriteLine)).Result;
        for (;;) ;
    }
}