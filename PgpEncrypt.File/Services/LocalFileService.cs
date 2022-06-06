using Microsoft.Extensions.Logging;

namespace PgpEncrypt.File.Services
{
    public class LocalFileService : IFileService
    {
        private readonly ILogger<LocalFileService> _logger;

        public LocalFileService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LocalFileService>();
        }

        public async Task DeleteFileAsync(string directory, string fileName)
        {
            System.IO.File.Delete(Path.Combine(directory, fileName));
        }

        public async Task<bool> GetFileExistsAsync(string directory, string fileName)
        {
            return System.IO.File.Exists(Path.Combine(directory, fileName));
        }

        public async Task<long> GetFileSizeAsync(string directory, string fileName)
        {
            return new FileInfo(Path.Combine(directory, fileName)).Length;
        }

        public async Task<Stream> GetFileStreamAsync(string directory, string fileName)
        {
            return System.IO.File.OpenRead(Path.Combine(directory, fileName));
        }

        public async IAsyncEnumerable<string> ListFilesAsync(string directory, string prefix = "*")
        {
            foreach (string file in Directory.GetFiles(directory, prefix))
            {
                yield return file;
            }
        }

        public async Task SetFileStreamAsync(Stream inputStream, string directory, string fileName)
        {

            using (FileStream fileStream = System.IO.File.Create(Path.Combine(directory, fileName)))
            {
                await inputStream.CopyToAsync(fileStream);
            }
        }
    }
}
