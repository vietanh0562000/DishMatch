using UnityEngine;

namespace PuzzleGames
{
	using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;

	public class AddSlotBtn : BoosterBtn
	{
		protected override void OnClickButton() { base.OnClickButton(); }

		public override void ActivateBooster()
		{
			HapticController.instance.Play();
			GameManager.Instance.SetPlayed();
			AudioController.PlaySound(SoundKind.UseBoosterAddSlot);
			MinusPowerUpCount();
			InGameTracker.UseBooster();
		}
		public override int          LevelUnlock => ServerConfig.Instance<ValueRemoteConfig>().levelUnlockBooserAddSlot;
		public override ResourceType Type        => ResourceType.Powerup_AddSlot;
	}
}
