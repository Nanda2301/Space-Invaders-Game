using Windows.Storage;
using System.IO;
using System.Threading.Tasks;

namespace SpaceInvaders.Utilities
{
    public static class FileHelper
    {
        public static async Task<string> ReadFileAsync(string fileName)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(fileName);
                return await FileIO.ReadTextAsync(file);
            }
            catch
            {
                return null;
            }
        }

        public static async Task WriteFileAsync(string fileName, string content)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, content);
            }
            catch
            {
                // Handle exception
            }
        }
    }
}
