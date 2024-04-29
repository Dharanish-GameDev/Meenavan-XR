using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    [SerializeField] private AudioClipsSO audioClips;
    public AudioClipsSO AudioClips { get { return audioClips; } }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this.gameObject);

        DontDestroyOnLoad(this);
    }

    #region Public Methods

    public void PlayOneShot(AudioClip clip,AudioSource source,float volume)
    {
        source.volume = volume;
        source.PlayOneShot(clip);
    }
    public void SetVolume(AudioSource audioSource,float volume)
    {
        audioSource.volume = volume;
    }
    #endregion
}
