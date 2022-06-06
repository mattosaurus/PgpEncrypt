using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace PgpEncrypt.File.Services
{
    public class SftpFileService : IFileService
    {
        private readonly SftpClient _sftpClient;
        private readonly ILogger<SftpFileService> _logger;

        public SftpFileService(SftpClient sftpClient, ILoggerFactory loggerFactory)
        {
            _sftpClient = sftpClient;
            _logger = loggerFactory.CreateLogger<SftpFileService>();
        }

        public async Task DeleteFileAsync(string directory, string fileName)
        {
            using (SftpClient client = new SftpClient(_sftpClient.ConnectionInfo))
            {
                client.Connect();
                client.DeleteFile(Path.Combine(directory, fileName));
            }
        }

        public async Task<bool> GetFileExistsAsync(string directory, string fileName)
        {
            bool exists;

            using (SftpClient client = new SftpClient(_sftpClient.ConnectionInfo))
            {
                client.Connect();
                exists = client.Exists(Path.Combine(directory, fileName));
            }

            return exists;
        }

        public async Task<long> GetFileSizeAsync(string directory, string fileName)
        {
            using (SftpClient client = new SftpClient(_sftpClient.ConnectionInfo))
            {
                client.Connect();
                var attributes = client.GetAttributes(Path.Combine(directory, fileName));

                return attributes.Size;
            }
        }

        public async Task<Stream> GetFileStreamAsync(string directory, string fileName)
        {
            Stream memoryStream = new MemoryStream();

            using (SftpClient client = new SftpClient(_sftpClient.ConnectionInfo))
            {
                client.Connect();
                client.DownloadFile(Path.Combine(directory, fileName), memoryStream);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async IAsyncEnumerable<string> ListFilesAsync(string container, string prefix = null)
        {
            using (SftpClient client = new SftpClient(_sftpClient.ConnectionInfo))
            {
                client.Connect();
                foreach (SftpFile file in client.ListDirectory(container))
                {
                    if (!file.IsDirectory)
                        if (file.Name.StartsWith(prefix))
                            yield return file.FullName;
                }
            }
        }

        public async Task SetFileStreamAsync(Stream inputStream, string directory, string fileName)
        {
            using (SftpClient client = new SftpClient(_sftpClient.ConnectionInfo))
            {
                client.Connect();
                client.UploadFile(inputStream, fileName);
            }
        }
    }
}
