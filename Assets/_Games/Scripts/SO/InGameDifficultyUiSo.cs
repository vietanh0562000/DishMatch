using UnityEngine;

namespace PuzzleGames
{
	using System;
	using System.Collections.Generic;
	using Sirenix.OdinInspector;

	[CreateAssetMenu(fileName = "InGameDifficultyUI", menuName = "In Game Difficulty UI")]
	public class InGameDifficultyUiSo : ScriptableObject
	{
		public List<DifficultySprite>   DifficultySprites;
		public List<DifficultyMaterial> DifficultyMaterials;

		public Sprite GetSpriteDifficulty(string spriteName, LevelDifficulty difficulty)
		{
			foreach (var difficultySprite in DifficultySprites)
			{
				if (spriteName == difficultySprite.Name)
				{
					return difficultySprite.Get(difficulty);
				}
			}

			return null;
		}

		public Material GetMaterialDifficulty(string name, LevelDifficulty difficulty)
		{
			foreach (var difficultyMaterial in DifficultyMaterials)
			{
				if (name == difficultyMaterial.Name)
				{
					return difficultyMaterial.Get(difficulty);
				}
			}

			return null;
		}
	}

	[Serializable]
	public class DifficultySprite : LevelDiff<Sprite>
	{
	}

	[Serializable]
	public class DifficultyMaterial : LevelDiff<Material>
	{
	}

	[Serializable]
	public class LevelDiff<T>
	{
		public string Name;

		[SerializeField] private List<LevelItem<T>> _items = new();

		private Dictionary<LevelDifficulty, T> _dict;

		public T Get(LevelDifficulty difficulty)
		{
			if (_dict == null)
			{
				_dict = new();
				foreach (var item in _items)
				{
					_dict[item.Difficulty] = item.Details;
				}
			}

			return _dict.GetValueOrDefault(difficulty);
		}

		[Serializable]
		private class LevelItem<TItem>
		{
			[HorizontalGroup("Row", 150)] public LevelDifficulty Difficulty;

			[VerticalGroup("Row/Details")] [HideLabel] [PreviewField(50)]
			public TItem Details;
		}
	}
}