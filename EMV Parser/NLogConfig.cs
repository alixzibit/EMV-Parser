using NLog;
using NLog.Config;

public class NLogConfig
{
    private readonly string _configFilePath;

    public NLogConfig(string configFilePath)
    {
        _configFilePath = configFilePath;
    }

    public void Configure()
    {
        LogManager.Configuration = new XmlLoggingConfiguration(_configFilePath, true);
    }
}
