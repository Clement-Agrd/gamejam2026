using UnityEngine;

public class AudioSourceGroup : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource[] audioSources;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;
    public float pitch = 1f;

    void Awake()
    {
        ApplySettings();
    }

    public void PlayAll()
    {
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
                source.Play();
        }
    }

    public void StopAll()
    {
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
                source.Stop();
        }
    }

    public void PauseAll()
    {
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
                source.Pause();
        }
    }

    public void ApplySettings()
    {
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
            {
                source.volume = volume;
                source.pitch = pitch;
            }
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        ApplySettings();
    }

    public void SetPitch(float newPitch)
    {
        pitch = newPitch;
        ApplySettings();
    }
}