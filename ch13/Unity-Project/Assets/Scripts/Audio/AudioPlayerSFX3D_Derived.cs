using UnityEngine;
using AudioType = AudioManager.AudioType;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayerSFX3D_Derived : AudioPlayerBase
{
    public override AudioType PlayAudioType => AudioType.SFX;

    [Range(0f, 1f)]
    [SerializeField] private float _volume = 1f;

    [Tooltip("0 = 2D, 1 = 3D"), Range(0f, 1f)]
    [SerializeField] private float _blend2Dto3D;

    [SerializeField] private AudioSource _audioSource;

    private void OnValidate()
        => _audioSource = GetComponent<AudioSource>();

    /// If we only wanted to play a sound, we're done with the above code.
    /// We're overriding the Play() and PlaySound() methods because we're changing the base functionality.

    public override void Play() =>
        AudioManager.Instance.PlayAudio(this, _audioSource);

    public override void PlaySound(AudioSource source)
    {
        source.spatialBlend = _blend2Dto3D;
        source.volume = _volume;
        source.PlayOneShot(_audioClip, _volume);
    }
}