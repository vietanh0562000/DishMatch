namespace PuzzleGames
{
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;

    public class UFOResource : PowerUpResource
    {
        public override int Price  => ServerConfig.Instance<ValueRemoteConfig>().UFOPrice;
        public override int Amount => ServerConfig.Instance<ValueRemoteConfig>().amountBuyUFO;

        public override ResourceType Type => ResourceType.Powerup_Helidrop;
    }
}