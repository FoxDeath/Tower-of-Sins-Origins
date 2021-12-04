using UnityEngine;

[System.Serializable]
public class Sound
{
    #region Attributes
    public string name;

    public AudioSource source;
    public AudioClip clip;

    [Range(0f,1f)]
    public float volume;
    [Range(-3f, 3f)]
    public float pitch;

    public bool isLooping;
    public bool isPlaying;
    public bool playOnAwake;
    #endregion
}
