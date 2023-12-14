using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public enum AudioType
    { Music, SFX, Ambient };

    [SerializeField] private AudioMixerGroup _groupMusic;
    [SerializeField] private AudioMixerGroup _groupSFX;
    [SerializeField] private AudioMixerGroup _groupAmbient;

    private AudioSource _audioSource2D, _audioSourceMusic;

    private AudioSource AudioSourcePlaySFX
    {
        get
        {
            if (_audioSource2D == null)
            {
                _audioSource2D = new GameObject().AddComponent<AudioSource>();
                _audioSource2D.spatialBlend = 0f; // 2D
                _audioSource2D.name = "New 2D AudioSource";
            }
            return _audioSource2D;
        }
    }

    private AudioMixerGroup GetAudioMixerGroup(AudioType audioType)
        => audioType switch
        {
            AudioType.SFX => _groupSFX,
            AudioType.Music => _groupMusic,
            AudioType.Ambient => _groupAmbient,
            _ => throw new ArgumentOutOfRangeException(nameof(AudioType), $"Not expected audioTpe value: {audioType}"),
        };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlayAudio(IPlaySound player, AudioSource source = null)
    {
        if (source == null)
            source = AudioSourcePlaySFX;

        source.outputAudioMixerGroup = GetAudioMixerGroup(player.PlayAudioType);
        player.PlaySound(source);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (_audioSourceMusic == null)
            _audioSourceMusic = gameObject.AddComponent<AudioSource>();

        _audioSourceMusic.outputAudioMixerGroup = _groupMusic;
        _audioSourceMusic.clip = clip;
        _audioSourceMusic.spatialBlend = 0f; // 2D
        _audioSourceMusic.bypassReverbZones = true;
        _audioSourceMusic.loop = true;
        _audioSourceMusic.Play();
    }
}