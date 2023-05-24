namespace Lct2023.Business;

public class BusinessInit
{
    private BusinessInit(string baseApiPath, string baseCmsPath)
    {
        BaseApiPath = baseApiPath;
        BaseCmsPath = baseCmsPath;
    }

    internal static BusinessInit Instance { get; private set; }

    internal string BaseApiPath { get; }

    internal string BaseCmsPath { get; }

    public static void Init(string baseApiPath, string baseCmsPath)
    {
        Instance = new BusinessInit(baseApiPath, baseCmsPath);
    }
}
