using Amazon.Runtime;
using Amazon.S3;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using PgpEncrypt.File.Services;
using Renci.SshNet;

namespace PgpEncrypt.File.Extensions
{
    public static class FileServiceCollectionExtensions
    {
        public static IServiceCollection AddSourceS3FileService(this IServiceCollection collection, string accessKey, string secretKey)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (accessKey == null) throw new ArgumentNullException(nameof(accessKey));
            if (secretKey == null) throw new ArgumentNullException(nameof(secretKey));

            // Create an S3 client object.
            BasicAWSCredentials awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            AmazonS3Client s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.EUWest2);

            return collection
                .AddSingleton<IAmazonS3>(s3Client)
                .AddSingleton<ISourceFileService, S3FileService>();
        }

        public static IServiceCollection AddDestinationS3FileService(this IServiceCollection collection, string accessKey, string secretKey)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (accessKey == null) throw new ArgumentNullException(nameof(accessKey));
            if (secretKey == null) throw new ArgumentNullException(nameof(secretKey));

            // Create an S3 client object.
            BasicAWSCredentials awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            AmazonS3Client s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.EUWest2);

            return collection
                .AddSingleton<IAmazonS3>(s3Client)
                .AddSingleton<IDestinationFileService, S3FileService>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="storageUrl">The URL of the storage container</param>
        /// <param name="tenantId">The tennant ID of the the user accessing the resource</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddSourceBlobStorageFileService(this IServiceCollection collection, string storageUrl, string tenantId = null)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (storageUrl == null) throw new ArgumentNullException(nameof(storageUrl));

            if (tenantId != null)
                Environment.SetEnvironmentVariable("AZURE_TENANT_ID", tenantId);

            // Add blob storage client
            collection.AddAzureClients(builder =>
            {
                // Add a storage account client
                builder.AddBlobServiceClient(new Uri(storageUrl));

                // Select the appropriate credentials based on enviroment
                builder.UseCredential(new DefaultAzureCredential());
            });

            return collection
                .AddSingleton<ISourceFileService, BlobStorageFileService>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="storageUrl">The URL of the storage container</param>
        /// <param name="tenantId">The tennant ID of the the user accessing the resource</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddDestinationBlobStorageFileService(this IServiceCollection collection, string storageUrl, string tenantId = null)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (storageUrl == null) throw new ArgumentNullException(nameof(storageUrl));
            if (tenantId == null) throw new ArgumentNullException(nameof(tenantId));

            if (tenantId != null)
                Environment.SetEnvironmentVariable("AZURE_TENANT_ID", tenantId);

            // Add blob storage client
            collection.AddAzureClients(builder =>
            {
                // Add a storage account client
                builder.AddBlobServiceClient(new Uri(storageUrl));

                // Select the appropriate credentials based on enviroment
                builder.UseCredential(new DefaultAzureCredential());
            });

            return collection
                .AddSingleton<IDestinationFileService, BlobStorageFileService>();
        }

        public static IServiceCollection AddSourceSftpFileService(this IServiceCollection collection, string host, string userName, string password)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));

            // Initialise connection info
            ConnectionInfo connectionInfo = new ConnectionInfo(host, userName, new PasswordAuthenticationMethod(userName, password));

            return AddSourceSftpFileService(collection, connectionInfo);
        }

        public static IServiceCollection AddSourceSftpFileService(this IServiceCollection collection, ConnectionInfo connectionInfo)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (connectionInfo == null) throw new ArgumentNullException(nameof(connectionInfo));

            // Create an sftp client object.
            SftpClient sftpClient = new SftpClient(connectionInfo);

            return collection
                .AddSingleton<SftpClient>(sftpClient)
                .AddSingleton<ISourceFileService, SftpFileService>();
        }

        public static IServiceCollection AddDestinationSftpFileService(this IServiceCollection collection, string host, string userName, string password)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));

            // Initialise connection info
            ConnectionInfo connectionInfo = new ConnectionInfo(host, userName, new PasswordAuthenticationMethod(userName, password));

            return AddDestinationSftpFileService(collection, connectionInfo);
        }

        public static IServiceCollection AddDestinationSftpFileService(this IServiceCollection collection, ConnectionInfo connectionInfo)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (connectionInfo == null) throw new ArgumentNullException(nameof(connectionInfo));

            // Create an sftp client object.
            SftpClient sftpClient = new SftpClient(connectionInfo);

            return collection
                .AddSingleton<SftpClient>(sftpClient)
                .AddSingleton<IDestinationFileService, SftpFileService>();
        }

        public static IServiceCollection AddSourceLocalFileService(this IServiceCollection collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            return collection
                .AddSingleton<ISourceFileService, LocalFileService>();
        }

        public static IServiceCollection AddDestinationLocalFileService(this IServiceCollection collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            return collection
                .AddSingleton<IDestinationFileService, LocalFileService>();
        }
    }
}
