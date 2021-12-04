using UnityEngine;
using UnityEngine.Audio;

public class AudioMenuController : MonoBehaviour
{
    #region Attributes
    [SerializeField] AudioMixer mainMixer;
    #endregion

    #region Normal Methods
    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("volume", volume);
    }
    #endregion
}
