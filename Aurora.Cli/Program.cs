using Aurora;

class Program
{
    public static void Main(string[] args)
    {
        var _ = Updater.DownloadNorthstar(new Progress<float>(Console.WriteLine)).Result;
        for (;;) ;
    }
}