using UnityEngine;

public class AudioPlayerMusic : MonoBehaviour
{
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private bool _playOnStart = true;

    private void Start()
    {
        if (_playOnStart)
            Play();
    }

    private void Play()
        => AudioManager.Instance.PlayMusic(_musicClip);
}