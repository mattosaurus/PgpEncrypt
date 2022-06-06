using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PgpCore;
using PgpEncrypt.File.Services;

namespace PgpEncrypt.Function
{
    public class PgpFunction
    {
        private readonly ILogger<PgpFunction> _logger;
        private readonly ISourceFileService _sourceService;
        private readonly IDestinationFileService _destinationService;

        public PgpFunction(ISourceFileService sourceService, IDestinationFileService destinationService, ILoggerFactory loggerFactory)
        {
            _sourceService = sourceService;
            _destinationService = destinationService;
            _logger = loggerFactory.CreateLogger<PgpFunction>();
        }

        [Function("Encrypt")]
        public async Task<HttpResponseData> EncryptAsync([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req)
        {
            // Ideally we'd do this nativly but there doesn't seem to be an easy way yet.
            // https://github.com/Azure/azure-functions-dotnet-worker/issues/366
            var parsedFormBody = await MultipartFormDataParser.ParseAsync(req.Body);

            // Load keys
            EncryptionKeys encryptionKeys = new EncryptionKeys(await _sourceService.GetFileStreamAsync("pgp", @"Keys/publicKey.asc"));
            PGP pgp = new PGP(encryptionKeys);

            foreach (FilePart file in parsedFormBody.Files)
            {
                // Encrypt
                using (Stream outputStream = new MemoryStream())
                {
                    await pgp.EncryptStreamAsync(file.Data, outputStream);
                    outputStream.Position = 0;
                    await _destinationService.SetFileStreamAsync(outputStream, "pgp", file.FileName + ".pgp");
                }
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }

        [Function("Decrypt")]
        public async Task<HttpResponseData> DecryptAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            // Load keys
            EncryptionKeys encryptionKeys = new EncryptionKeys(await _sourceService.GetFileStreamAsync("pgp", @"Keys/privateKey.asc"), "password");
            PGP pgp = new PGP(encryptionKeys);

            var response = req.CreateResponse(HttpStatusCode.OK);

            // Decrypt
            using (Stream inputFileStream = await _sourceService.GetFileStreamAsync(@"pgp", @"Content/encryptedContent.pgp"))
            {
                await pgp.DecryptStreamAsync(inputFileStream, response.Body);
                return response;
            }
        }
    }
}
