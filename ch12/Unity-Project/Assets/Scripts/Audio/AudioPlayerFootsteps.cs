using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioPlayerSFX))]
public class AudioPlayerFootsteps : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _walkInterval = 0.5f;
    [SerializeField] private float _sprintInterval = 0.3f;
    [SerializeField] private AudioClip[] _footstepSounds;

    private AudioPlayerSFX _playerSFX;
    private float _timerStep;
    private bool _isSprinting;

    private void OnValidate() => _playerSFX = GetComponent<AudioPlayerSFX>();

    private void Start() => _timerStep = _walkInterval;

    private void Update()
    {
        if (!_characterController.isGrounded
            || _characterController.velocity.magnitude <= 0)
            return;

        float currentStepInterval =
            _isSprinting ? _sprintInterval : _walkInterval;

        _timerStep -= Time.deltaTime;
        if (_timerStep <= 0)
        {
            _playerSFX.Play(GetRandomFootstepClip());
            _timerStep = currentStepInterval;
        }

        AudioClip GetRandomFootstepClip()
            => _footstepSounds[Random.Range(0, _footstepSounds.Length)];
    }

    public void OnSprint(InputValue value)
        => _isSprinting = value.isPressed;
}