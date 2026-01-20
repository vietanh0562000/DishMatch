using BasePuzzle.Core.Scripts.Services.GameObjs;
using UnityEngine;

namespace BasePuzzle.Core.Scripts.Utils.Singletons
{
	using BasePuzzle.Core.Scripts.Services.GameObjs;

	public abstract class FMonoSingleton<T> : MonoBehaviour where T : FMonoSingleton<T>, new()
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if( _instance == null )
				{
					_instance = FGameObj.Instance.AddIfNotExist<T>();
				}
				return _instance;
			}

			protected set { _instance = value; }
		}

		protected virtual void Awake()
		{
			DontDestroyOnLoad( gameObject );
			if( _instance != this && _instance != null )
			{
				Destroy( this );
			}
		}
	}
}