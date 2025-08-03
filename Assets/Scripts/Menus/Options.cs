using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private Slider volume;
    [SerializeField] private Slider sfxVolume;
    [SerializeField] private Slider musicVolume;

    public void UpdateVolume()
    {
        AudioManager.Instance.SetMasterVolume(Mathf.Lerp(-80, 0, volume.value)); // Set the master volume to the value of the slider
    }
    public void UpdateMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(Mathf.Lerp(-80, 0, musicVolume.value)); // Set the master volume to the value of the slider
    }
    public void UpdateSFXVolume()
    {
        AudioManager.Instance.SetSFXVolume(Mathf.Lerp(-80, 0, sfxVolume.value)); // Set the master volume to the value of the slider
    }
}
