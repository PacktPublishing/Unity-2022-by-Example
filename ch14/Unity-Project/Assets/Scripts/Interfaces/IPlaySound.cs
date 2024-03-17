using UnityEngine;
using AudioType = AudioManager.AudioType;

public interface IPlaySound
{
    AudioType PlayAudioType { get; }
    void PlaySound(AudioSource source);
}