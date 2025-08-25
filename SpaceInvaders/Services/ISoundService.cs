namespace SpaceInvaders.Services
{
    public interface ISoundService
    {
        void PlaySound(SoundEffects soundEffect);
        void StopAllSounds();
        void PlaySound(string playerShootWav);
    }

    public enum SoundEffects
    {
        PlayerShoot,
        EnemyShoot,
        Explosion,
        RedEnemyAppear,
        RedEnemyKilled,
        ExtraLife,
        GameOver
    }
}
