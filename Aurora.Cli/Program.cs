using Aurora;
using Mono.Options;

class Program
{
    public static void Main(string[] args)
    {
        var gamePath = string.Empty;
        var showHelp = false;
        var options = new OptionSet
        {
            {
                "g|gamepath", "Path to the game's installation directory. Overrides autodetection if used.",
                s => { gamePath = s ?? string.Empty; }
            },
            {
                "p|platform",
                "Whether or not the game is installed on \"steam\" or \"origin\". Used for autodetection.", s => { }
            },
            {
                "f|force", "Force installation whenever possible.", s => { _forceInstall = s != null; }
            },
            { "h|help", "Show this message and exit.", s => { showHelp = s != null; } },
        };

        var extras = options.Parse(args);

        if (showHelp)
        {
            Console.WriteLine("Usage: aurora [OPTIONS] [COMMAND]");
            Console.WriteLine("Manage and interface with a Northstar installation for Titanfall 2.");
            Console.WriteLine();
            Console.WriteLine("Commands:");

            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
            return;
        }
    }

    private static bool _forceInstall;

    private static readonly OptionSet _commands = new()
    {
        { "update", "Update northstar to the latest available version.", UpdateNorthstar }
    };

    private static void UpdateNorthstar(string obj)
    {
        throw new NotImplementedException();
    }
}