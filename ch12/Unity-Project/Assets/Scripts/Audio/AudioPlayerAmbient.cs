using UnityEngine;
using AudioType = AudioManager.AudioType;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayerAmbient : MonoBehaviour, IPlaySound
{
    public AudioType PlayAudioType => AudioType.Ambient;

    [SerializeField] private AudioClip _audioClip;

    [SerializeField] private AudioSource _audioSource;

    private void OnValidate()
        => _audioSource = GetComponent<AudioSource>();

    private void Start() => Play();

    public void Play() =>
        AudioManager.Instance.PlayAudio(this, _audioSource);

    public void PlaySound(AudioSource source)
    {
        source.clip = _audioClip;

        // Ambient sounds are always 3D and loop,
        // so, override the Inspector values.
        source.spatialBlend = 1f;   // 3D
        source.loop = true;
        source.Play();
    }
}