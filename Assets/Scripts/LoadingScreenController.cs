using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreenController : MonoBehaviour
{
    public GameObject loadingPanel; 
    public Slider slider; 
    public TextMeshProUGUI loadingText; 
    public float duration = 2.5f;

    private void Awake()
    {
        StartCoroutine(LoadingIn());
    }
    private void Start()
    {
    }

    IEnumerator LoadingIn()
    {
        loadingPanel.SetActive(true);
        slider.value = 0f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            slider.value = progress;
            loadingText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        slider.value = 1f;
        loadingText.text = "100%";

        loadingPanel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
