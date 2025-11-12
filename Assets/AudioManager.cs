using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource), typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioClip defaultButtonClickSound, defaultButtonHoverSound;
    public AudioClip mainMenuMusic;
    #region Singleton Pattern

    public static AudioManager Instance { get; private set; }
    private float minPitch = 0.9f;
    private float maxPitch = 1.1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAudioSources();
    }

    #endregion

    #region Audio Sources
    
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private List<AudioSource> _sfxPool = new List<AudioSource>();

    private void InitializeAudioSources()
    {
        _musicSource.loop = true;
    }

    #endregion

    #region Public Playback Methods
    
    public void PlayMusic(AudioClip musicClip)
    {
        if (_musicSource.clip == musicClip && _musicSource.isPlaying)
        {
            return;
        }

        _musicSource.clip = musicClip;
        _musicSource.Play();
    }
    
    public void PlayMainMenuMusic()
    {
        
        _musicSource.clip = mainMenuMusic;
        _musicSource.Play();
    }
    
    public void StopMusic()
    {
        _musicSource.Stop();
    }
    
    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null) return;
        _sfxSource.PlayOneShot(sfxClip);
    }

    private float temporaryVolume;
    public void PlaySFX(AudioClip sfxClip, float volume)
    {
        temporaryVolume = _sfxSource.volume;
        _sfxSource.volume = volume;
        if (sfxClip == null) return;
        _sfxSource.PlayOneShot(sfxClip);
        _sfxSource.volume = temporaryVolume;
    }
    
    public void PlaySFXAtPosition(AudioClip clip, Vector3 position)
    {
        AudioSource available = _sfxPool.FirstOrDefault(s => !s.isPlaying);
        if (available != null)
        {
            available.transform.position = position;
            available.PlayOneShot(clip);
        }
    }
    
    public void StopSFX()
    {
        _sfxSource.Stop();
    }
    
    public void PlayButtonClickSound()
    {
        _sfxSource.PlayOneShot(defaultButtonClickSound);
    }
    
    public void PlayButtonHoverSound()
    {
        _sfxSource.PlayOneShot(defaultButtonHoverSound);
    }

    #endregion

    #region Volume Control
    public void SetMusicVolume(float volume)
    {
        _musicSource.volume = Mathf.Clamp01(volume);
    }
    
    public void SetSfxVolume(float volume)
    {
        _sfxSource.volume = Mathf.Clamp01(volume);
    }
    
    public void EnableMusic(bool enable)
    {
        _musicSource.enabled = enable;
    }
    
    public void EnableSfx(bool enable)
    {
        _sfxSource.enabled = enable;
    }

    #endregion
}
