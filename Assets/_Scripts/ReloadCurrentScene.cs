
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace GGJ18
{
	public sealed class ReloadCurrentScene : MonoBehaviour
	{
		public void reload()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
