using UnityEngine;
using AudioType = AudioManager.AudioType;

public class AudioPlayerSFX : MonoBehaviour, IPlaySound
{
    public AudioType PlayAudioType => AudioType.SFX;

    [SerializeField] private AudioClip _audioClip;

    [Range(0f, 1f)]
    [SerializeField] private float _volume = 1f;

    public void Play() =>
        AudioManager.Instance.PlayAudio(this);

    // Added for footsteps support.
    public void Play(AudioClip clip)
    {
        _audioClip = clip;
        AudioManager.Instance.PlayAudio(this);
    }

    public void PlaySound(AudioSource source)
        => source.PlayOneShot(_audioClip, _volume);
}