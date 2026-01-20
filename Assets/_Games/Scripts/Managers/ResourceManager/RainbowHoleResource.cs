namespace PuzzleGames
{
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;

    public class RainbowHoleResource : PowerUpResource
    {
        public override int Price  => ServerConfig.Instance<ValueRemoteConfig>().rainbowPrice;
        public override int Amount => ServerConfig.Instance<ValueRemoteConfig>().amountBuyRainbow;

        public override ResourceType Type => ResourceType.Powerup_RainbowHole;
    }
}