namespace SpaceInvaders.Services
{
    public interface ISoundService
    {
        void PlaySound(string soundName);
        void SetMute(bool isMuted);
    }
}
