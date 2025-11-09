using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AudioClipData
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(-3f, 3f)] public float pitch = 1f;
    public bool loop = false;
}

public class AudioManager : MonoBehaviour
{
    [Header("Audio Settings")] [SerializeField]
    private AudioSource musicSource;

    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")] [SerializeField]
    private List<AudioClipData> musicClips = new List<AudioClipData>();

    [SerializeField] private List<AudioClipData> sfxClips = new List<AudioClipData>();

    private Dictionary<string, AudioClipData> musicClipDictionary = new Dictionary<string, AudioClipData>();
    private Dictionary<string, AudioClipData> sfxClipDictionary = new Dictionary<string, AudioClipData>();

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeAudioDictionaries();

    }
    

    private void InitializeAudioDictionaries()
    {
        // Create dictionaries for faster lookup
        foreach (AudioClipData clipData in musicClips)
        {
            if (clipData.clip != null && !string.IsNullOrEmpty(clipData.name))
            {
                if (!musicClipDictionary.ContainsKey(clipData.name))
                {
                    musicClipDictionary.Add(clipData.name, clipData);
                }
            }
        }

        foreach (AudioClipData clipData in sfxClips)
        {
            if (clipData.clip != null && !string.IsNullOrEmpty(clipData.name))
            {
                if (!sfxClipDictionary.ContainsKey(clipData.name))
                {
                    sfxClipDictionary.Add(clipData.name, clipData);
                }
            }
        }
    }

    #region Music Functions

    public void PlayMusic(string musicName, bool loop = true)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("Music AudioSource is not assigned in AudioManager!");
            return;
        }

        if (musicClipDictionary.TryGetValue(musicName, out AudioClipData clipData))
        {
            musicSource.clip = clipData.clip;
            musicSource.volume = clipData.volume;
            musicSource.pitch = clipData.pitch;
            musicSource.loop = loop;

            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }

            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip '{musicName}' not found in AudioManager!");
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = Mathf.Clamp01(volume);
        }
    }

    #endregion

    #region SFX Functions

    public void PlaySFX(string sfxName)
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("SFX AudioSource is not assigned in AudioManager!");
            return;
        }

        if (sfxClipDictionary.TryGetValue(sfxName, out AudioClipData clipData))
        {
            sfxSource.PlayOneShot(clipData.clip, clipData.volume);
        }
        else
        {
            Debug.LogWarning($"SFX clip '{sfxName}' not found in AudioManager!");
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("SFX AudioSource is not assigned in AudioManager!");
            return;
        }

        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlaySFXAtPosition(string sfxName, Vector3 position)
    {
        if (sfxClipDictionary.TryGetValue(sfxName, out AudioClipData clipData))
        {
            PlaySFXAtPosition(clipData.clip, position, clipData.volume);
        }
        else
        {
            Debug.LogWarning($"SFX clip '{sfxName}' not found in AudioManager!");
        }
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null in PlaySFXAtPosition!");
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    #endregion

    #region Utility Functions

    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }

    public bool IsSFXPlaying()
    {
        return sfxSource != null && sfxSource.isPlaying;
    }

    #endregion
}