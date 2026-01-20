namespace PuzzleGames
{
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;

    public class AddSlotResource : PowerUpResource
    {
        public override int Price  => ServerConfig.Instance<ValueRemoteConfig>().addSlotPrice;
        public override int Amount => ServerConfig.Instance<ValueRemoteConfig>().amountBuyAddSlot;

        public override ResourceType Type => ResourceType.Powerup_AddSlot;
    }
}