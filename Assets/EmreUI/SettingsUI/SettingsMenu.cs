using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SettingsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Button exit;
    //  public Slider sfxSlider;
    //  public Slider carSlider;

    private void Start()
    {
        if (musicSlider != null)
        {
            // Slider değerlerini ayarlamak için mevcut ses seviyelerini alabilirsiniz
            musicSlider.value = AudioManager.instance.musicSource.volume;
            //  sfxSlider.value = AudioManager.instance.sfxSource.volume;
            // carSlider.value = AudioManager.instance.carSource.volume;

            // Slider değerleri değiştiğinde çağrılacak fonksiyonları ayarlayın
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            // sfxSlider.onValueChanged.AddListener(SetSfxVolume);
            //  carSlider.onValueChanged.AddListener(SetCarVolume);
        }
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Credits")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
    public void SetMusicVolume(float volume)
    {
        AudioManager.instance.SetMusicVolume(volume);
    }

    public void SetSfxVolume(float volume)
    {
        AudioManager.instance.SetSfxVolume(volume);
    }

    public void SetCarVolume(float volume)
    {
        AudioManager.instance.SetCarVolume(volume);
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
