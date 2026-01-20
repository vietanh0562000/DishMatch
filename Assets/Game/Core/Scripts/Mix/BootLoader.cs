using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootLoader : MonoBehaviour
{
	[SerializeField] private string nextSceneName = "InitScene";  // <- Change to your real next scene name

	private IEnumerator Start()
	{
		// Optional: Short delay if needed (like 0.5f seconds)
		yield return new WaitForSeconds(0.1f);

		// Load next scene asynchronously (non-blocking)
		SceneManager.LoadSceneAsync(nextSceneName);
	}
}