using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace SpaceInvaders.Services
{
    public class SoundService : ISoundService
    {
        private MediaPlayer _mediaPlayer = new MediaPlayer();
        private bool _isMuted = false;
        private ISoundService _soundServiceImplementation;

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
            catch
            {
                // Handle exception (file not found, etc.)
                System.Diagnostics.Debug.WriteLine($"Sound file not found: {soundName}");
            }
        }

        public void SetMute(bool isMuted)
        {
            _isMuted = isMuted;
            _mediaPlayer.Volume = isMuted ? 0 : 1;
        }

        public void PlaySound(SoundEffects soundEffect)
        {
            _soundServiceImplementation.PlaySound(soundEffect);
        }

        public void StopAllSounds()
        {
            _soundServiceImplementation.StopAllSounds();
        }
    }
}
