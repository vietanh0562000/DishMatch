

public class ScreenShotManager : PersistentSingleton<ScreenShotManager>
{
    public ScreenShotMapController screenShotPrefab;
    private ScreenShotMapController _instanceSS;

    public byte[] ScreenShot(string level_data)
    {
        if (screenShotPrefab != null && _instanceSS == null)
        {
            _instanceSS = Instantiate(screenShotPrefab);
        }
        
        if (_instanceSS != null) { return _instanceSS.ScreenShot(level_data); }

        return null;
    }
}