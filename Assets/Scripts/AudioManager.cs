using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM Sources")]
    public AudioSource bgMenu;
    public AudioSource bgLevel;

    [Header("SFX Sources")]
    public AudioSource sfxPlayerHit;
    public AudioSource sfxEnemyHit;
    public AudioSource sfxBoom;
    public AudioSource sfxButton;
    public AudioSource sfxGameOver;
    public AudioSource sfxWinLevel;

    private AudioSource currentBGM;
    private bool bgmEnabled = true;
    private bool sfxEnabled = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        PlayerPrefs.SetInt("BGM_Enabled", 1);
        PlayerPrefs.SetInt("SFX_Enabled", 1);
        bgmEnabled = PlayerPrefs.GetInt("BGM_Enabled", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFX_Enabled", 1) == 1;

        ApplyAudioSettings();

        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayMenuBGM();
        Debug.Log("AnhNTT SAound");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayMenuBGM();
                break;
            case "GamePlay":
                PlayLevel1BGM();
                break;
            default:
                StopAllBGM();
                break;
        }

    }


    public void PlayBGM(AudioSource bgm)
    {
        if (!bgmEnabled) return;
        if (currentBGM == bgm) return;

        StopAllBGM();
        currentBGM = bgm;
        bgm.Play();
    }

    public void StopAllBGM()
    {
        bgMenu.Stop();
        bgLevel.Stop();
    }

    public void PlayMenuBGM() => PlayBGM(bgMenu);
    public void PlayLevel1BGM() => PlayBGM(bgLevel);

    public void PlayPlayerHit() { if (sfxEnabled) sfxPlayerHit.PlayOneShot(sfxPlayerHit.clip); }
    public void PlayEnemyHit() { if (sfxEnabled) sfxEnemyHit.PlayOneShot(sfxEnemyHit.clip); }
    public void PlaysfxBoom() { if (sfxEnabled) sfxBoom.PlayOneShot(sfxBoom.clip); }
    public void PlayButton() { if (sfxEnabled) sfxButton.PlayOneShot(sfxButton.clip); }
    public void PlayGameOver() { if (sfxEnabled) sfxGameOver.PlayOneShot(sfxGameOver.clip); }
    public void PlayWinLevel() { if (sfxEnabled) sfxWinLevel.PlayOneShot(sfxWinLevel.clip); }

    public void OnBGMToggleChanged(bool isOn)
    {
        bgmEnabled = isOn;
        PlayerPrefs.SetInt("BGM_Enabled", isOn ? 1 : 0);
        PlayerPrefs.Save();
        ApplyAudioSettings();
    }

    public void OnSFXToggleChanged(bool isOn)
    {
        sfxEnabled = isOn;
        PlayerPrefs.SetInt("SFX_Enabled", isOn ? 1 : 0);
        PlayerPrefs.Save();
        ApplyAudioSettings();
    }

    private void ApplyAudioSettings()
    {
        if (bgmEnabled)
        {
            if (currentBGM != null && !currentBGM.isPlaying)
                currentBGM.Play();
        }
        else
        {
            StopAllBGM();
        }

        float sfxVolume = sfxEnabled ? 1f : 0f;
        sfxPlayerHit.volume = sfxVolume;
        sfxEnemyHit.volume = sfxVolume;
        sfxBoom.volume = sfxVolume;
        sfxButton.volume = sfxVolume;
        //sfxGameOver.volume = sfxVolume;
        sfxWinLevel.volume = sfxVolume;
    }
}
