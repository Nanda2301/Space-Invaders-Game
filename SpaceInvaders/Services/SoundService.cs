using System;
using System.Collections.Generic;
using Windows.Media.Playback;
using static SpaceInvaders.Models.SoundEffects;

namespace SpaceInvaders.Services
{
    public class SoundService : ISoundService, IDisposable
    {
        private readonly Dictionary<SoundEffects, MediaPlayer> _mediaPlayers;
        private bool _isMuted = false;

        public SoundService()
        {
            _mediaPlayers = new Dictionary<SoundEffects, MediaPlayer>();
            InitializeSounds();
        }

        private void InitializeSounds()
        {
            var sounds = new[]
            {
                SoundEffects.PlayerShoot,
                SoundEffects.EnemyShoot,
                SoundEffects.Explosion,
                SoundEffects.RedEnemyAppear,
                SoundEffects.RedEnemyKilled,
                SoundEffects.GameOver,
                SoundEffects.ExtraLife
            };

            foreach (var sound in sounds)
            {
                _mediaPlayers[sound] = new MediaPlayer();
            }
        }

        public void PlaySound(SoundEffects sound)
        {
            if (_isMuted || !_mediaPlayers.ContainsKey(sound)) return;

            try
            {
                // Aqui você pode tocar o som real via MediaPlayer
                // Para simplificação, usamos Console.Beep como exemplo
                switch (sound)
                {
                    case SoundEffects.PlayerShoot: PlayBeep(800, 100); break;
                    case SoundEffects.EnemyShoot: PlayBeep(400, 150); break;
                    case SoundEffects.Explosion: PlayBeep(200, 300); break;
                    case SoundEffects.RedEnemyAppear: PlayBeep(1000, 200); break;
                    case SoundEffects.RedEnemyKilled: PlayBeep(600, 250); break;
                    case SoundEffects.GameOver: PlayBeep(150, 500); break;
                    case SoundEffects.ExtraLife: PlayBeep(1200, 300); break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing sound {sound}: {ex.Message}");
            }
        }

        public void StopAllSounds()
        {
            foreach (var player in _mediaPlayers.Values)
            {
                player?.Pause();
                player?.Dispose(); // opcional, dependendo da implementação
            }
            _mediaPlayers.Clear();
        }

        public void PlaySound(string playerShootWav)
        {
            throw new NotImplementedException();
        }

        private void PlayBeep(int frequency, int duration)
        {
#if WINDOWS
            try { Console.Beep(frequency, duration); }
            catch { }
#endif
        }

        public void SetMute(bool isMuted)
        {
            _isMuted = isMuted;
            foreach (var player in _mediaPlayers.Values)
            {
                player.Volume = isMuted ? 0 : 1;
            }
        }

        public void Dispose()
        {
            StopAllSounds();
        }
    }
}
