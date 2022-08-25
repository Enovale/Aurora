namespace Aurora;

public class CoreConfig
{
    public string NorthstarRepository
    {
        get => _northstarRepository;
        set
        {
            var oldVal = _northstarRepository;
            _northstarRepository = value;
            if(oldVal != value)
                Config.Save();
        }
    }

    private string _northstarRepository = "R2Northstar/Northstar";
}