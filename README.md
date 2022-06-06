# PgpEncrypt
This is an example Azure function which demonstrates uploading and encrypting a file to Azure blob storage and downloading and decrypting a file from Azure blob storage.

## Sources/Destinations
The following sources and destinations are available and can be selected using the relevant startup extension method.
- Azure blob storage
- Local file storage
- AWS S3 storage
- SFTP server

## Usage
The upload reads input data from from the multipart-form body as represented by the following cURL command.

```
curl --location --request PUT 'http://localhost:7071/api/Encrypt' --form '=@"/C:/TEMP/Content/content.txt"'
```

## Authentication
MSI authentication is used for connecting to Azure blob storage. If running in Visual Studio/VS Code you'll need to be logged in as a user with rights to the storage container, you can also use credentials stored in the enviroment or local.settings.json. When running in Azure the function will need to be assigned an identity and granted access.

The Azure user will need to have "Storage Blob Data Contributor" rights at the storage account level.

## Notes
- This is an example, don't hardcode your settings in a real app.
