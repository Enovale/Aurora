using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Aurora;
using Mono.Options;

class Program
{
    public const string SuiteName = "aurora";
    
    public static int Main(string[] args)
    {
        Config.CoreConfig = _config;
        Config.Save(_config);
        CreateConfigOptions();
        Commands.Run(args);
        return _returnValue;
    }

    private static readonly CommandSet Commands = new(SuiteName)
    {
        $"usage: {SuiteName} COMMAND [OPTIONS]+",
        "",
        "Common Options:",
        { "version",
            "Show version info",
            v => _showVersion = v != null },
        { "help",
            "Show this message and exit",
            v => _showHelp = v != null },
        "Northstar Commands:",
        new Command("update", "Update northstar to the latest available version")
        {
            Options = new()
            {
                {
                    "f|force", "Force installation regardless of installed version", s => _forceInstall = s != null
                },
            },
            Run = UpdateNorthstar
        },
        "Configuration Commands:",
        new CommandSet("config")
        {
            new Command("set", "Set a configuration variable permanently")
            {
                Run = SetConfig
            },
            new Command("get", "Print out a configuration variable")
            {
                Run = PrintConfig
            }
        },
    };

    private static bool _forceInstall;

    private static bool _showVersion;
    
    private static bool _showHelp;

    private static readonly CoreConfig _config = Config.LoadConfig();

    private static int _returnValue = 0;

    private static void UpdateNorthstar(IEnumerable<string> extras)
    {
        Console.WriteLine("Updating!");
    }

    private static void PrintConfig(IEnumerable<string> obj)
    {
        var ret = typeof(CoreConfig).GetProperty(obj.First())?.GetValue(Config.CoreConfig);
        Commands.Out.WriteLine(ret);
        _returnValue = ret == null ? 1 : 0;
    }

    private static void SetConfig(IEnumerable<string> obj)
    {
        var args = obj as string[] ?? obj.ToArray();
        var prop = typeof(CoreConfig).GetProperty(args[0]);
        if (prop == null)
        {
            _returnValue = 1;
            return;
        }
        
        var converter = TypeDescriptor.GetConverter(prop.PropertyType);
        if (!converter.IsValid(args[1]))
        {
            _returnValue = 1;
            return;
        }
        
        prop.SetValue(Config.CoreConfig, converter.ConvertFromString(args[1]));
        Config.Save();
    }

    private static void CreateConfigOptions()
    {
        var props = typeof(CoreConfig).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var converter = TypeDescriptor.GetConverter(prop.PropertyType);
            Commands.Add(prop.Name + "=", "Temporarily set the VALUE of this config option.",
                s => prop.SetValue(_config, converter.ConvertFromString(s)));
        }
    }
}