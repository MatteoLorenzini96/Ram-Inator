using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;
    private bool _isMusicPlaying = true; // Tiene traccia dello stato della musica

    public void ToggleMusic()
    {
        if (_isMusicPlaying)
        {
            AudioManager.Instance.ToggleMusic();
        }
        else
        {
            AudioManager.Instance.PlayMusic("MainTheme");
        }

        _isMusicPlaying = !_isMusicPlaying; // Alterna lo stato
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
    }
}
