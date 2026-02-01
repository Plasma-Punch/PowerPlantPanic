using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource SfxSource { get; private set; } //For sound effects
    public AudioSource MusicSource { get; private set; } //For background music

    public AudioSource AmbianceSource { get; private set; } //For background music
    public AudioClip AbiantSound;

    private Dictionary<string, AudioClip> _soundClips;
    private List<AudioClip> _activeClips = new List<AudioClip>();

    private float _masterVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Initialize audio
        SfxSource = gameObject.AddComponent<AudioSource>();
        MusicSource = gameObject.AddComponent<AudioSource>();
        AmbianceSource = gameObject.AddComponent<AudioSource>();
        AmbianceSource.clip = AbiantSound;
        AmbianceSource.loop = true;
        AmbianceSource.Play();
        MusicSource.enabled = true;
        _soundClips = new Dictionary<string, AudioClip>();
    }

    public void SetMasterVolume(Component sender, object obj)
    {
        float? volume = obj as float?;
        _masterVolume = volume.Value;

        MusicSource.volume = _masterVolume;
        SfxSource.volume = _masterVolume;
        AmbianceSource.volume = _masterVolume;
    }

    // Load an audio clip from Resource folder
    public void LoadSound(string name, string filePath)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);
        if (clip != null)
        {
            _soundClips[name] = clip;
            Debug.Log($"Loaded sound: {name}");
        }
        else
        {
            Debug.Log($"Failed to load sound at path: {filePath}");
        }
    }

    public void LoadSoundWithOutPath(string name, AudioClip audio)
    {
        AudioClip clip = audio;
        if (clip != null)
        {
            _soundClips[name] = clip;
            Debug.Log($"Loaded sound: {name}");
        }
        else
        {
            Debug.Log($"Failed to load sound: {audio}");
        }
    }
    
    // Play sound effect 
    public void PlaySound(string name)
    {

        if (_soundClips.ContainsKey(name))
        {
            AudioClip clip = _soundClips[name];

            if (_activeClips.Contains(clip)) return;
            
            _activeClips.Add(clip);
            SfxSource.PlayOneShot(clip);

            StartCoroutine(RemoveClipAfterPlaying(clip, clip.length));
        }
        else
        {
            Debug.Log($"Sound {name} not found!");
        }
    }

    // Play background music
    public void PlayMusic(string name)
    {
        if (_soundClips.ContainsKey(name))
        {
            MusicSource.clip = _soundClips[name];
            MusicSource.Play();
        }
        else
        {
            Debug.Log($"Music {name} not found!");
        }
    }

    // Stop all sound effects
    public void StopSound()
    {
        SfxSource.Stop();
    }

    // Stop background music
    public void StopMusic()
    {
        MusicSource.Stop();
    }

    // Set volume for sound effects
    public void SetSFXVolume(float volume)
    {
        float tempVolume = Mathf.Clamp01(volume);
        SfxSource.volume = tempVolume * _masterVolume;
    }

    // Set volume for background music
    public void SetMusicVolume(float volume)
    {
        float tempVolume = Mathf.Clamp01(volume);
        MusicSource.volume = tempVolume * _masterVolume;
    }

    private IEnumerator RemoveClipAfterPlaying(AudioClip clip, float duration)
    {
        yield return new WaitForSeconds(duration);
        _activeClips.Remove(clip);
    }
}
