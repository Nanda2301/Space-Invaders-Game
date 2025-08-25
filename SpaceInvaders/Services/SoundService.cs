using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using SpaceInvaders.Models;

namespace SpaceInvaders.Services
{
    public class SoundService : ISoundService
    {
        private MediaPlayer _mediaPlayer = new MediaPlayer();
        private bool _isMuted = false;

        public async void PlaySound(string soundName)
        {
            if (_isMuted) return;

            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(
                    new System.Uri($"ms-appx:///Assets/Sounds/{soundName}"));
                
                _mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                _mediaPlayer.Play();
            }
            catch (System.Exception ex)
            {
                // Handle exception (file not found, etc.)
                System.Diagnostics.Debug.WriteLine($"Sound file not found: {soundName} - Error: {ex.Message}");
            }
        }

        public void SetMute(bool isMuted)
        {
            _isMuted = isMuted;
            _mediaPlayer.Volume = isMuted ? 0 : 1;
        }

        public void PlaySound(SoundEffects soundEffect)
        {
            string soundFile = soundEffect switch
            {
                SoundEffects.PlayerShoot => "player_shoot.wav",
                SoundEffects.EnemyShoot => "enemy_shoot.wav",
                SoundEffects.Explosion => "explosion.wav",
                SoundEffects.RedEnemyAppear => "red_enemy_appear.wav",
                SoundEffects.RedEnemyKilled => "red_enemy_killed.wav",
                SoundEffects.GameOver => "game_over.wav",
                SoundEffects.ExtraLife => "extra_life.wav",
                _ => "player_shoot.wav"
            };

            PlaySound(soundFile);
        }

        public void StopAllSounds()
        {
            try
            {
                _mediaPlayer?.Pause();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping sounds: {ex.Message}");
            }
        }
    }
}
