using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
	public void ExitGame()
	{
		Application.Quit();
	}
	public void StartGame()
	{
		SceneManager.LoadScene("DevView");
	}
}
