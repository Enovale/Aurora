namespace Aurora;

public class CoreConfig
{
    public string NorthstarRepository
    {
        get => _northstarRepository;
        set => UpdateProperty(ref _northstarRepository, value);
    }

    public string[] PreservedPaths
    {
        get => _preservedPaths;
        set => UpdateProperty(ref _preservedPaths, value);
    }

    public int UpdateAttemptLimit
    {
        get => _updateAttemptLimit;
        set => UpdateProperty(ref _updateAttemptLimit, value);
    }

    private string _northstarRepository = "R2Northstar/Northstar";
    private string[] _preservedPaths = new[]
    {
        "ns_startup_args.txt", 
        "ns_startup_args_dedi.txt"
    };
    private int _updateAttemptLimit = 5;

    private void UpdateProperty<TSource>(ref TSource property, TSource value) where TSource : IEquatable<TSource>
    {
        var oldVal = property;
        property = value;
        if (!oldVal.Equals(value))
            Config.Save();
    }

    private void UpdateProperty<TSource>(ref TSource[] property, TSource[] value) where TSource : IEquatable<TSource>
    {
        var oldVal = property;
        property = value;
        if (!oldVal.SequenceEqual(property))
            Config.Save();
    }
}