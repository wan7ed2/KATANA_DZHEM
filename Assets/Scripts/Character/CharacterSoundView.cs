using System;
using UnityEngine;

[Serializable]
public class CharacterSoundView
{
    [SerializeField] private SoundClip _hitSound;
    [SerializeField] private SoundClip _fallSadSound;
    [SerializeField] private SoundClip _jumpSound;

    [SerializeField] private float _hitSleepTime = 2f;
    [SerializeField] private float _distanceFallToSound = 12f;

    [SerializeField] private TyazheloSoundController _tyazheloSoundController;
    
    public void Init(Player player, GroundChecker groundChecker)
    {
        _player = player;
        _groundChecker = groundChecker;
        if (Systems.TryGet<SoundSystem>(out var soundSystem))
            _soundSystem = soundSystem;
    }

    public void Start()
    {
        if (_soundSystem == null)
            return;
        
        _player.OnHit += PlayHit;
        _player.MovementController.OnJump += HandleJump;
        _tyazheloSoundController.OnTyazhelo += HandleTyazhelo;
    }

    public void Stop()
    {
        _player.MovementController.OnJump -= HandleJump;
        _tyazheloSoundController.OnTyazhelo -= HandleTyazhelo;
        _player.OnHit -= PlayHit;
    }

    private Player _player;
    private GroundChecker _groundChecker;
    private SoundSystem _soundSystem;

    private float _lastHitSoundTime;
    private float _lastFlyPosY = -1000;

    private void PlayHit()
    {
        var currentTime = Time.timeSinceLevelLoad;
        if ((currentTime - _lastHitSoundTime) < _hitSleepTime)
            return;
        
        _lastHitSoundTime = currentTime;
        _soundSystem.PlayOneShot(_hitSound);
    }
    
    private void HandleFly()
    {
        _lastFlyPosY = _player.Rigidbody.position.y;
    }

    private void HandleGrounded()
    {
        var currentPosY = _player.Rigidbody.position.y;
        if ((_lastFlyPosY - currentPosY) < _distanceFallToSound)
            return;
        
        _soundSystem.PlayOneShot(_fallSadSound);
    }
    
    private void HandleJump()
    {
        if (!_groundChecker.IsGrounded)
            return;
        
        _soundSystem.PlayOneShot(_jumpSound);
    }

    private void HandleTyazhelo()
    {
        _soundSystem.PlayOneShot(_fallSadSound);
    }
    
}