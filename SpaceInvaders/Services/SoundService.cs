using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace SpaceInvaders.Services
{
    public class SoundService : ISoundService, IDisposable
    {
        private readonly Dictionary<SoundEffects, CachedSound> _sounds;
        private readonly List<IWavePlayer> _activePlayers;
        private bool _isMuted = false;
        private bool _disposed = false;

        public SoundService()
        {
            _sounds = new Dictionary<SoundEffects, CachedSound>();
            _activePlayers = new List<IWavePlayer>();

            // Inicializa em background
            Task.Run(() => InitializeSounds());
        }

        private async Task InitializeSounds()
        {
            var soundFiles = new Dictionary<SoundEffects, string>
            {
                { SoundEffects.PlayerShoot, "player_shoot.wav" },
                { SoundEffects.EnemyShoot, "enemy_shoot.wav" },
                { SoundEffects.Explosion, "explosion.wav" },
                { SoundEffects.RedEnemyAppear, "red_enemy_appear.wav" },
                { SoundEffects.RedEnemyKilled, "red_enemy_killed.wav" },
                { SoundEffects.GameOver, "game_over.wav" },
                { SoundEffects.ExtraLife, "extra_life.wav" }
            };

            try
            {
                StorageFolder assetsFolder =
                    await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                StorageFolder soundsFolder = await assetsFolder.GetFolderAsync("Sounds");

                foreach (var kvp in soundFiles)
                {
                    try
                    {
                        StorageFile file = await soundsFolder.GetFileAsync(kvp.Value);
                        string filePath = file.Path;

                        _sounds[kvp.Key] = new CachedSound(filePath);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Falha ao carregar som {kvp.Value}: {ex.Message}");
                        _sounds[kvp.Key] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Falha ao inicializar sons: {ex.Message}");
            }
        }

        // Toca som pré-carregado (por enum)
        public void PlaySound(SoundEffects sound)
        {
            if (_isMuted || _disposed || !_sounds.ContainsKey(sound) || _sounds[sound] == null)
                return;

            try
            {
                var cachedSound = _sounds[sound];
                var outputDevice = new WaveOutEvent();
                var provider = new CachedSoundSampleProvider(cachedSound);

                outputDevice.Init(provider);
                outputDevice.Volume = 0.7f;

                outputDevice.PlaybackStopped += (sender, e) =>
                {
                    outputDevice.Dispose();
                    lock (_activePlayers) _activePlayers.Remove(outputDevice);
                };

                lock (_activePlayers) _activePlayers.Add(outputDevice);

                outputDevice.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Erro ao tocar som {sound}: {ex.Message}");
                Console.Beep(); // fallback simples
            }
        }

        // Exigido pela interface: tocar som direto por caminho
        public void PlaySound(string filePath)
        {
            if (_isMuted || _disposed) return;

            try
            {
                var reader = new AudioFileReader(filePath);
                var output = new WaveOutEvent();
                output.Init(reader);
                output.Volume = 0.7f;
                output.Play();

                lock (_activePlayers) _activePlayers.Add(output);

                output.PlaybackStopped += (s, e) =>
                {
                    output.Dispose();
                    reader.Dispose();
                    lock (_activePlayers) _activePlayers.Remove(output);
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Erro ao tocar som direto: {ex.Message}");
            }
        }

        // Exigido pela interface
        public void StopAllSounds()
        {
            lock (_activePlayers)
            {
                foreach (var output in _activePlayers.ToList())
                {
                    output.Stop();
                    output.Dispose();
                }
                _activePlayers.Clear();
            }
        }

        public void Mute() => _isMuted = true;
        public void Unmute() => _isMuted = false;

        public void Dispose()
        {
            if (_disposed) return;

            StopAllSounds();
            _disposed = true;
        }
    }

    // Carrega áudio inteiro em memória
    public class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        public CachedSound(string audioFileName)
        {
            using (var reader = new AudioFileReader(audioFileName))
            {
                WaveFormat = reader.WaveFormat;
                var buffer = new List<float>();
                var readBuf = new float[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = reader.Read(readBuf, 0, readBuf.Length)) > 0)
                {
                    for (int i = 0; i < samplesRead; i++)
                        buffer.Add(readBuf[i]);
                }
                AudioData = buffer.ToArray();
            }
        }
    }

    // Provider para tocar CachedSound
    public class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private long position;

        public CachedSoundSampleProvider(CachedSound cachedSound)
        {
            this.cachedSound = cachedSound;
        }

        public WaveFormat WaveFormat => cachedSound.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }
    }
}
