using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

	public string menuSceneName = "MainMenu";

    public LoadingScreenController loadingScreen;

    public void Retry ()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

	public void Menu ()
	{
        SceneManager.LoadScene(menuSceneName);
	}
    public void NextLevel()
    {
        PlayerData.Instance.Level++;
        Debug.Log("LevelUnlocked 2:" + (PlayerData.Instance.Level));
        PlayerPrefs.SetInt("LevelUnlocked", PlayerData.Instance.Level);
        PlayerData.Instance.SaveData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
