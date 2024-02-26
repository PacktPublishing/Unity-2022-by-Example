using UnityEngine;
using AudioType = AudioManager.AudioType;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayerSFX3D : MonoBehaviour, IPlaySound
{
    public AudioType PlayAudioType => AudioType.SFX;

    [SerializeField] private AudioClip _audioClip;

    [Range(0f, 1f)]
    [SerializeField] private float _volume = 1f;

    [Tooltip("0 = 2D, 1 = 3D"), Range(0f, 1f)]
    [SerializeField] private float _blend2Dto3D;

    [SerializeField] private AudioSource _audioSource;

    // Only using OnValidate because we're requiring the component type exists.
    private void OnValidate()
        => _audioSource = GetComponent<AudioSource>();

    public void Play() =>
        AudioManager.Instance.PlayAudio(this, _audioSource);

    public void PlaySound(AudioSource source)
    {
        source.spatialBlend = _blend2Dto3D;
        source.PlayOneShot(_audioClip, _volume);
    }
}