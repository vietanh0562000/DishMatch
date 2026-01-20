using System;
using BasePuzzle.PuzzlePackages.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace PuzzleGames
{
	using TMPro;

	public class LevelLoader : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] private GameObject _btnBackToLE;
		[SerializeField] private string     _txtLevel;
		[SerializeField] private TMP_Text   _txtLevelEditor;
#endif


		private void Start()
		{
			var levelJson=TempDataHandler.Get<string>(TempDataKeys.CURRENT_LEVEL_JSON_DATA);

#if UNITY_EDITOR
			bool fromEditor=TempDataHandler.Get(TempDataKeys.OPEN_LEVEL_FROM_EDITOR,false);
			if(_btnBackToLE!=null)
				_btnBackToLE.SetActive(fromEditor);

			if(_txtLevelEditor!=null && fromEditor)
			{
				var level=TempDataHandler.Get<string>(TempDataKeys.CURRENT_LEVEL_FROM_LEVEL_EDITOR);
				if(!string.IsNullOrEmpty(level))
				{
					_txtLevelEditor.gameObject.SetActive(true);
					_txtLevelEditor.text=$"Level: {level}";
				}
			}

			if(string.IsNullOrEmpty(levelJson)) levelJson=_txtLevel;
#endif
			var serializerSettings=new JsonSerializerSettings
			{
				NullValueHandling=NullValueHandling.Ignore,
				TypeNameHandling =TypeNameHandling.All // Enables polymorphic deserialization
			};

			/*var levelData=JsonConvert.DeserializeObject<TxtLevelData>(levelJson);
			var mapJson  =JsonCompressing.Decompressing(levelData.mapData);
			var mapData  =JsonConvert.DeserializeObject<LevelData>(mapJson,serializerSettings);

			CreateMap(mapData);

			var currentLevel=TempDataHandler.Get(TempDataKeys.CURRENT_LEVEL_FROM_HOME,0);
			Debug.LogError($"CurrentLevel: {currentLevel}");

			LevelManager.Instance.SetLevelData(currentLevel,mapJson,levelData.difficulty);*/
		}

		public void CreateMapFromTxt(string txt)
		{
			var levelData=JsonConvert.DeserializeObject<TxtLevelData>(txt);
			var mapJson  =JsonCompressing.Decompressing(levelData.mapData);
			var mapData  =JsonConvert.DeserializeObject<LevelData>(mapJson);

			CreateMap(mapData,true);
		}

		private void CreateMap(LevelData data,bool forScreenShot=false) { }

		/// <summary>
		/// Direct call from UIs, Test
		/// </summary>
		public static void LoadLevel(int level)
		{
			Destroy(PoolHolder.PoolTransform.gameObject);
			var levelJson=LoadLevelManager.instance.ReadLevelData(level);
			TempDataHandler.Set(TempDataKeys.CURRENT_LEVEL_JSON_DATA,levelJson);
			TempDataHandler.Set(TempDataKeys.CURRENT_LEVEL_FROM_HOME,level);
			LoadSceneManager.Instance.LoadScene("GamePlay");
		}


#if UNITY_EDITOR
		public void ClickBtnBackToLevelEditor()
		{
			Destroy(PoolHolder.PoolTransform.gameObject);

#if UNITY_ANDROID
			GameViewUtils.SetFullScreen();
#endif
			LoadSceneManager.Instance.LoadScene("LevelEditor");
		}
#endif
	}
}
