using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private Slider volume;

    public void UpdateVolume()
    {
        AudioManager.Instance.SetMasterVolume(Mathf.Lerp(-80, 0, volume.value)); // Set the master volume to the value of the slider
    }
}
