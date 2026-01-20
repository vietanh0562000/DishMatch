using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;

public class ValueRemoteConfig : ServerConfig
{
    public int levelUnlockBooserAddSlot = 5;
    public int levelUnlockBooserUFO     = 15;
    public int levelUnlockBooserRainbow = 10;

    public int coinRewardPassLevel = 20;

    public int  numLevelToShowBanner           = 19;
    public long  numLevelToShowInterstitial     = 9999999999999999;
    public bool showInterstitialWhenRetry      = false;
    public bool showInterstitialWhenGoHome     = false;
    public long  numLevelToShowRewarded         = 9999999999999999;
    public int  numInterstitialToShowRemoveAds = 3;
    public int  numSecondsToReShowAds          = 90;
    public int  numLevelToGoHomeAfterPassLevel = 10;


    public int minLevelForRandomLoad = 20;
    public int maxLevelForRandomLoad = 100;

    public int number_of_load_levels_after_win   = 2;
    public int number_of_load_levels_after_login = 10;

    public readonly int revivePrice           = 450;
    public readonly int revivePriceSecondTime = 450;

    public int addSlotPrice     = 450;
    public int amountBuyAddSlot = 3;
    public int UFOPrice         = 600;
    public int amountBuyUFO     = 3;
    public int rainbowPrice     = 600;
    public int amountBuyRainbow = 3;

    public float x150Chance = 0.3f;
    public float x200Chance = 0.25f;
    public float x300Chance = 0.2f;
    public float x400Chance = 0.15f;
    public float x500Chance = 0.1f;

    public int resetTimeAfterFailed = 2;
    public int cooldownShowInterAds = 120;
    public int levelRate1 =10;
    public int levelRate2 =50;
}