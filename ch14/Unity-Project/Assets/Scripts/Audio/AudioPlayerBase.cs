using UnityEngine;
using AudioType = AudioManager.AudioType;

public abstract class AudioPlayerBase : MonoBehaviour, IPlaySound
{
    [SerializeField] protected AudioClip _audioClip;

    public abstract AudioType PlayAudioType { get; }

    public virtual void PlaySound(AudioSource source)
        => source.PlayOneShot(_audioClip);

    public virtual void Play()
        => AudioManager.Instance.PlayAudio(this);
}