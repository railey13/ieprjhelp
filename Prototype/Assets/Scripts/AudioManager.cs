using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


// Call AudioManager.Instance.PlaySFX("Attack") or PlayMusic("Theme") from anywhere


[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop = false;

        [Tooltip("Randomize pitch slightly each time this plays (SFX only).")]
        public bool randomizePitch = false;
        [Range(0f, 0.5f)] public float pitchVariance = 0.1f;
    }

    [Header("Sound Library")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    [Header("Mixer (optional)")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicMixerParam = "MusicVolume";
    [SerializeField] private string sfxMixerParam = "SFXVolume";

    [Header("Music")]
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private float defaultCrossfadeDuration = 1.0f;

    [Header("SFX Pool")]
    [SerializeField] private int sfxPoolSize = 10;

    private readonly Dictionary<string, Sound> _soundLookup = new Dictionary<string, Sound>();
    private readonly List<AudioSource> _sfxPool = new List<AudioSource>();
    private int _sfxPoolIndex = 0;

    private AudioSource _activeMusicSource;
    private AudioSource _inactiveMusicSource;
    private Coroutine _crossfadeRoutine;

    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;
    private bool _musicMuted = false;
    private bool _sfxMuted = false;

    private const string PREF_MUSIC_VOL = "AudioManager_MusicVolume";
    private const string PREF_SFX_VOL = "AudioManager_SFXVolume";
    private const string PREF_MUSIC_MUTE = "AudioManager_MusicMuted";
    private const string PREF_SFX_MUTE = "AudioManager_SFXMuted";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildLookup();
        SetupMusicSources();
        SetupSfxPool();
        LoadPrefs();
    }

    private void BuildLookup()
    {
        _soundLookup.Clear();
        foreach (var s in sounds)
        {
            if (s == null || string.IsNullOrEmpty(s.name)) continue;
            if (!_soundLookup.ContainsKey(s.name))
                _soundLookup.Add(s.name, s);
            else
                Debug.LogWarning($"[AudioManager] Duplicate sound name '{s.name}' ignored.");
        }
    }

    private void SetupMusicSources()
    {
        if (musicSourceA == null)
        {
            musicSourceA = gameObject.AddComponent<AudioSource>();
            musicSourceA.playOnAwake = false;
            musicSourceA.loop = true;
        }
        if (musicSourceB == null)
        {
            musicSourceB = gameObject.AddComponent<AudioSource>();
            musicSourceB.playOnAwake = false;
            musicSourceB.loop = true;
        }

        _activeMusicSource = musicSourceA;
        _inactiveMusicSource = musicSourceB;
    }

    private void SetupSfxPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            _sfxPool.Add(src);
        }
    }


    public void PlaySFX(string name)
    {
        if (!_soundLookup.TryGetValue(name, out var sound))
        {
            Debug.LogWarning($"[AudioManager] SFX '{name}' not found.");
            return;
        }

        var src = GetNextSfxSource();
        src.clip = sound.clip;
        src.volume = sound.volume * _sfxVolume * (_sfxMuted ? 0f : 1f);
        src.pitch = sound.randomizePitch
            ? sound.pitch + UnityEngine.Random.Range(-sound.pitchVariance, sound.pitchVariance)
            : sound.pitch;
        src.loop = sound.loop;
        src.Play();
    }

    //Play a one-shot SFX at a world position (spatial audio via a temp AudioSource).
    public void PlaySFXAtPoint(string name, Vector3 position)
    {
        if (!_soundLookup.TryGetValue(name, out var sound))
        {
            Debug.LogWarning($"[AudioManager] SFX '{name}' not found.");
            return;
        }

        float volume = sound.volume * _sfxVolume * (_sfxMuted ? 0f : 1f);
        AudioSource.PlayClipAtPoint(sound.clip, position, volume);
    }

    private AudioSource GetNextSfxSource()
    {
        var src = _sfxPool[_sfxPoolIndex];
        _sfxPoolIndex = (_sfxPoolIndex + 1) % _sfxPool.Count;
        return src;
    }

    // Play a music track immediately (no crossfade).
    public void PlayMusic(string name)
    {
        if (!_soundLookup.TryGetValue(name, out var sound))
        {
            Debug.LogWarning($"[AudioManager] Music '{name}' not found.");
            return;
        }

        _activeMusicSource.clip = sound.clip;
        _activeMusicSource.volume = sound.volume * _musicVolume * (_musicMuted ? 0f : 1f);
        _activeMusicSource.pitch = sound.pitch;
        _activeMusicSource.loop = true;
        _activeMusicSource.Play();
    }

    // Crossfade to a new music track.
    public void CrossfadeMusic(string name, float duration = -1f)
    {
        if (!_soundLookup.TryGetValue(name, out var sound))
        {
            Debug.LogWarning($"[AudioManager] Music '{name}' not found.");
            return;
        }

        if (duration < 0f) duration = defaultCrossfadeDuration;

        if (_crossfadeRoutine != null)
            StopCoroutine(_crossfadeRoutine);

        _crossfadeRoutine = StartCoroutine(CrossfadeRoutine(sound, duration));
    }

    private System.Collections.IEnumerator CrossfadeRoutine(Sound sound, float duration)
    {
        _inactiveMusicSource.clip = sound.clip;
        _inactiveMusicSource.pitch = sound.pitch;
        _inactiveMusicSource.loop = true;
        _inactiveMusicSource.volume = 0f;
        _inactiveMusicSource.Play();

        float targetVol = sound.volume * _musicVolume * (_musicMuted ? 0f : 1f);
        float startVol = _activeMusicSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            _activeMusicSource.volume = Mathf.Lerp(startVol, 0f, lerp);
            _inactiveMusicSource.volume = Mathf.Lerp(0f, targetVol, lerp);
            yield return null;
        }

        _activeMusicSource.Stop();
        _activeMusicSource.volume = 0f;

        var temp = _activeMusicSource;
        _activeMusicSource = _inactiveMusicSource;
        _inactiveMusicSource = temp;
    }

    public void StopMusic()
    {
        _activeMusicSource.Stop();
    }

    public void PauseMusic() => _activeMusicSource.Pause();
    public void ResumeMusic() => _activeMusicSource.UnPause();


    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        ApplyMusicVolume();
        ApplyMixerVolume(musicMixerParam, _musicVolume, _musicMuted);
        PlayerPrefs.SetFloat(PREF_MUSIC_VOL, _musicVolume);
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        ApplyMixerVolume(sfxMixerParam, _sfxVolume, _sfxMuted);
        PlayerPrefs.SetFloat(PREF_SFX_VOL, _sfxVolume);
    }

    public void SetMusicMuted(bool muted)
    {
        _musicMuted = muted;
        ApplyMusicVolume();
        ApplyMixerVolume(musicMixerParam, _musicVolume, _musicMuted);
        PlayerPrefs.SetInt(PREF_MUSIC_MUTE, muted ? 1 : 0);
    }

    public void SetSFXMuted(bool muted)
    {
        _sfxMuted = muted;
        ApplyMixerVolume(sfxMixerParam, _sfxVolume, _sfxMuted);
        PlayerPrefs.SetInt(PREF_SFX_MUTE, muted ? 1 : 0);
    }

    public float GetMusicVolume() => _musicVolume;
    public float GetSFXVolume() => _sfxVolume;
    public bool IsMusicMuted() => _musicMuted;
    public bool IsSFXMuted() => _sfxMuted;

    private void ApplyMusicVolume()
    {
        if (_activeMusicSource != null && _soundLookup.TryGetValue(_activeMusicSource.clip?.name ?? "", out _))
        {
            // Scale relative to the clip's base volume if we can find it, otherwise just apply directly.
        }
        if (_activeMusicSource != null)
            _activeMusicSource.volume = _musicMuted ? 0f : _musicVolume;
    }

    private void ApplyMixerVolume(string param, float linearVolume, bool muted)
    {
        if (audioMixer == null || string.IsNullOrEmpty(param)) return;
        float effective = muted ? 0.0001f : Mathf.Max(linearVolume, 0.0001f);
        float db = Mathf.Log10(effective) * 20f;
        audioMixer.SetFloat(param, db);
    }

    // 

    private void LoadPrefs()
    {
        _musicVolume = PlayerPrefs.GetFloat(PREF_MUSIC_VOL, 1f);
        _sfxVolume = PlayerPrefs.GetFloat(PREF_SFX_VOL, 1f);
        _musicMuted = PlayerPrefs.GetInt(PREF_MUSIC_MUTE, 0) == 1;
        _sfxMuted = PlayerPrefs.GetInt(PREF_SFX_MUTE, 0) == 1;

        ApplyMusicVolume();
        ApplyMixerVolume(musicMixerParam, _musicVolume, _musicMuted);
        ApplyMixerVolume(sfxMixerParam, _sfxVolume, _sfxMuted);
    }
}