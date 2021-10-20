using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

namespace SubjectbookIntegration
{
    internal class GoogleDriveConnector
    {
        private static string[] Scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "OTACS client";

        public static Dictionary<string, string> RetrieveAllFolders(DriveService service)
        {
            var foldersDictionary = new Dictionary<string, string>();
            var listRequest = service.Files.List();
            listRequest.PageSize = 200;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType)";
            var files = listRequest.Execute().Files;
            if (files == null || files.Count <= 0) return foldersDictionary;
            // loop through files.
            foreach (var file in files)
            {
                if (file.MimeType.Contains("folder"))
                {
                    foldersDictionary.Add(file.Id, file.Name);
                }
            }
            return foldersDictionary;
        }

        public static Dictionary<string, string> GetFiles(DriveService service, string folderId)
        {

            string pageToken = null;
            var foldersDictionary = new Dictionary<string, string>();
            do
            {
                try
                {
                    var request = service.Files.List();
                    request.PageSize = 200;
                    request.Q = "trashed=false"; // undeleted items
                    request.Q += string.Format(" and '{0}' in parents", folderId); // https://developers.google.com/drive/search-parameters
                    request.OrderBy = "name";
                    request.PageToken = pageToken;
                    //request.Spaces = "drive";
                    //request.Fields = "nextPageToken, files(id, name, mimeType, parents)";
                    var result = request.Execute();
                    foreach (var file in result.Files)
                    {
                        foldersDictionary.Add(file.Id, file.Name);
                    }
                    pageToken = result.NextPageToken;
                }
                catch
                {
                    break;
                }
                
            } while (pageToken != null);

            return foldersDictionary;
        }


        public static Dictionary<string, string> GetFolders(DriveService service, string folderId)
        {
            string pageToken = null;
            var foldersDictionary = new Dictionary<string, string>();
            do
            {
                var request = service.Files.List();
                request.PageSize = 200;
                request.Q = "mimeType = 'application/vnd.google-apps.folder' and '" + folderId + "' in parents";
                request.PageToken = pageToken;
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name, mimeType, parents)";
                request.OrderBy = "name";
                var result = request.Execute();
                foreach (var file in result.Files)
                {
                    foldersDictionary.Add(file.Id, file.Name);
                }
                pageToken = result.NextPageToken;
            } while (pageToken != null);

            return foldersDictionary;
        }




        public static string UploadFile(DriveService service, string driveFolderId, string localFilePath, string fileName, string mimeType)
        {
            var fileMetadata = new File
            {
                Name = fileName,
                Parents = new List<string> { driveFolderId }
            };
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(localFilePath, System.IO.FileMode.Open))
            {
                request = service.Files.Create(
                    fileMetadata, stream, mimeType);
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;
            Console.WriteLine(@"File ID: " + file.Id);
            return file.Id;
        }

        public static void ClearCredentials()
        {
            var credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
            credPath = Path.Combine(credPath, ".credentials");
            try
            {
                Directory.Delete(credPath, true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static DriveService Connect()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine(@"Credential file saved to: " + credPath);
            }
            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            return service;
        }

        public static File UpdateFile(DriveService _service, string _uploadFile, string _fileId, string newMimeType)
        {

            if (System.IO.File.Exists(_uploadFile))
            {
                File body = new File();
                //body.Title = "OTACS";
                body.Description = "File updated by OTACS";
                body.MimeType = newMimeType;
                //body.Parents = new List() { new ParentReference() { Id = _parent } };

                // File's content.
                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.UpdateMediaUpload request = _service.Files.Update(body, _fileId, stream, newMimeType);
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("File does not exist: " + _uploadFile);
                return null;
            }

        }

        

        public static File UpdateFileWithStream(DriveService _service, System.IO.MemoryStream stream, string _fileId, string newMimeType)
        {

            File body = new File();
            //body.Title = "OTACS";
            body.Description = "File updated by OTACS";
            body.MimeType = newMimeType;
            //body.Parents = new List() { new ParentReference() { Id = _parent } };

            try
            {
                FilesResource.UpdateMediaUpload request = _service.Files.Update(body, _fileId, stream, newMimeType);
                request.Upload();
                return request.ResponseBody;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return null;
            }

        }

    }
}


