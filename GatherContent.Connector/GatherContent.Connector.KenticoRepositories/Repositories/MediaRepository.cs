namespace GatherContent.Connector.KenticoRepositories.Repositories
{
    using System;
    using System.Net;

    using CMS.Base;
    using CMS.Core;
    using CMS.DocumentEngine;
    using CMS.Helpers;
    using CMS.IO;
    using CMS.MediaLibrary;
    using CMS.Membership;
    using CMS.SiteProvider;

    using GatherContent.Connector.Entities;
    using GatherContent.Connector.IRepositories.Interfaces;
    using GatherContent.Connector.IRepositories.Models.Import;

    using File = GatherContent.Connector.IRepositories.Models.Import.File;
    using FileMode = CMS.IO.FileMode;
    using FileStream = CMS.IO.FileStream;
    using MemoryStream = System.IO.MemoryStream;
    using Path = System.IO.Path;
    using SeekOrigin = System.IO.SeekOrigin;
    using Stream = System.IO.Stream;

    public class MediaRepository : IMediaRepository<MediaFileInfo, TreeNode>
    {
        private const string GC_CONTENT_ID = "GC.ContentId";

        protected GCAccountSettings GcAccountSettings;

        private readonly string currentSiteName;

        private readonly TreeProvider treeProvider;

        public MediaRepository(GCAccountSettings gcAccountSettings)
        {
            this.GcAccountSettings = gcAccountSettings;

            var currentUser = (UserInfo)MembershipContext.AuthenticatedUser;
            this.treeProvider = new TreeProvider(currentUser);
            this.currentSiteName = SiteContext.CurrentSiteName;
        }

        public string ResolveMediaPath(CmsItem item, TreeNode createdItem, CmsField cmsField)
        {
            var path = string.IsNullOrEmpty(cmsField.TemplateField.FieldName)
                           ? string.Format("GatherContent/{0}/", item.Title)
                           : string.Format("GatherContent/{0}/{1}/", item.Title, cmsField.TemplateField.FieldName);

            return path;

            // string dataSourcePath = "/GatherContent";

            // var parent = createdItem.Parent;
            // string subfolder = string.Empty;
            // while (parent != null && parent.DocumentCustomData[GC_CONTENT_ID] != null && !String.IsNullOrEmpty(parent.DocumentCustomData[GC_CONTENT_ID].ToString()))
            // {
            // subfolder = "/" + parent.DocumentName + subfolder;

            // parent = parent.Parent;
            // }

            // dataSourcePath = dataSourcePath + subfolder;

            // var path = string.IsNullOrEmpty(cmsField.TemplateField.FieldName)
            // ? string.Format("{0}/{1}/", dataSourcePath, item.Title)
            // : string.Format("{0}/{1}/{2}/", dataSourcePath, item.Title, cmsField.TemplateField.FieldName);
            return path;
        }

        public void SaveBinaryDataToDisk(string filePath, byte[] fileData)
        {
            FileStream fileStream = null;
            if (fileData == null) return;
            if (string.IsNullOrEmpty(filePath)) return;
            try
            {
                fileStream = FileStream.New(filePath, FileMode.Create);
                fileStream.Write(fileData, 0, fileData.Length);
                fileStream.Flush();
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        public void SaveFileToDisk(string filePath, BinaryData data, bool closeStream = true)
        {
            if (data.SourceStream != null) this.SaveStreamToDisk(filePath, data.SourceStream, closeStream);
            else this.SaveBinaryDataToDisk(filePath, data.Data);
        }

        public void SaveStreamToDisk(string filePath, Stream str, bool closeStream = true)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = FileStream.New(filePath, FileMode.Create);
                var length = str.Length;
                if (length <= 0L) return;
                var count1 = 65536;
                if (count1 > length) count1 = (int)length;
                var buffer = new byte[count1];
                while (length > 0L)
                {
                    var count2 = str.Read(buffer, 0, count1);
                    fileStream.Write(buffer, 0, count2);
                    fileStream.Flush();
                    length -= count2;
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }

                if ((str != null) && closeStream)
                {
                    str.Close();
                    str.Dispose();
                }
            }
        }

        public MediaFileInfo UploadFile(string targetPath, File fileInfo)
        {
            var uri = fileInfo.Url.StartsWith("http") ? fileInfo.Url : "https://gathercontent.s3.amazonaws.com/" + fileInfo.Url;

            var extension = string.Empty;
            if (fileInfo.FileName.Contains("."))
            {
                extension = fileInfo.FileName.Substring(fileInfo.FileName.LastIndexOf('.') + 1);
            }

            var request = (HttpWebRequest)WebRequest.Create(uri);
            var resp = (HttpWebResponse)request.GetResponse();
            var stream = resp.GetResponseStream();
            if (stream != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(memoryStream);

                    if (memoryStream.Length > 0)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        var media = this.CreateMedia(targetPath, fileInfo, extension, memoryStream);
                        return media;
                    }
                }
            }

            return null;
        }

        protected virtual MediaFileInfo CreateMedia(string rootPath, File mediaFile, string extension, Stream mediaStream)
        {
            var gatherContentLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo(this.GcAccountSettings.MediaLibraryName, SiteContext.CurrentSiteName);

            var fileName = Path.GetFileName(mediaFile.Url);

            MediaFileInfo mediaFileInfo;
            using (var streamWrapper = StreamWrapper.New(mediaStream))
            {
                mediaFileInfo = new MediaFileInfo
                                    {
                                        FileGUID = Guid.NewGuid(),
                                        FileCreatedByUserID = MembershipContext.AuthenticatedUser.UserID,
                                        FileModifiedByUserID = MembershipContext.AuthenticatedUser.UserID,
                                        FileSiteID = gatherContentLibrary.LibrarySiteID,
                                        FileLibraryID = gatherContentLibrary.LibraryID,
                                        FileTitle = fileName,
                                        FilePath = rootPath,
                                        FileExtension = "." + extension,
                                        FileName = Path.GetFileNameWithoutExtension(fileName),
                                        FileDescription = fileName,
                                        FileBinaryStream = streamWrapper,
                                        FileMimeType = MimeTypeHelper.GetMimetype(extension),
                                        FileSize = mediaStream.Length,
                                        FileModifiedWhen = DateTime.UtcNow
                                    };

                MediaFileInfoProvider.SetMediaFileInfo(mediaFileInfo);

                // SetMediaFileInfoInternal(mediaFileInfo, true, CMSActionContext.CurrentUser.UserID, false);
            }

            return mediaFileInfo;
        }

        protected virtual string SaveFileToDiskInternal(
            string siteName,
            string libraryFolder,
            string librarySubFolderPath,
            string fileName,
            string fileExtension,
            Guid fileGuid,
            BinaryData fileData,
            bool synchronization,
            bool ensureUniqueFileName,
            bool skipChecks,
            string filePath,
            string fileSubFolderPath)
        {
            if (!ensureUniqueFileName && synchronization)
            {
                var mediaFileInfo = MediaFileInfoProvider.GetMediaFileInfo(fileGuid, siteName);
                try
                {
                    MediaFileInfoProvider.DeleteMediaFileThumbnails(mediaFileInfo);
                }
                catch
                {
                }
            }

            if (!skipChecks)
            {
                fileSubFolderPath = this.CheckAndEnsureFilePath(siteName, libraryFolder, librarySubFolderPath, fileName, fileExtension, ensureUniqueFileName, out filePath);
                fileSubFolderPath = MediaLibraryHelper.EnsurePhysicalPath(fileSubFolderPath);
            }

            if (fileData != null)
            {
                this.SaveFileToDisk(filePath, fileData, false);
                if (!synchronization && CMSActionContext.CurrentLogWebFarmTasks)
                    WebFarmHelper.CreateIOTask(
                        "UPDATEMEDIAFILE",
                        filePath,
                        fileData,
                        "mediafileupload",
                        siteName,
                        libraryFolder,
                        librarySubFolderPath,
                        fileName,
                        fileExtension,
                        fileGuid.ToString());
                fileData.Close();
                if (synchronization)
                {
                    CacheHelper.TouchKey("mediafile|" + fileGuid.ToString().ToLowerCSafe(), false, false);
                    CacheHelper.TouchKey("mediafilepreview|" + fileGuid.ToString().ToLowerCSafe(), false, false);
                }
            }

            return CMS.IO.Path.EnsureSlashes(fileSubFolderPath, false);
        }

        protected void SetMediaFileInfoInternal(MediaFileInfo mediaFile, bool saveFileToDisk, int userId, bool ensureUniqueFileName)
        {
            if (mediaFile == null) return;
            var flag = false;
            string filePath1 = null;
            SiteInfo siteInfo = null;
            MediaLibraryInfo mediaLibraryInfo = null;
            var librarySubFolderPath1 = string.Empty;
            if (saveFileToDisk && ((mediaFile.FileBinaryStream != null) || (mediaFile.FileBinary != null)))
            {
                mediaLibraryInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(mediaFile.FileLibraryID);
                if (mediaLibraryInfo != null)
                {
                    siteInfo = SiteInfoProvider.GetSiteInfo(mediaFile.FileSiteID);
                    if (siteInfo != null)
                    {
                        flag = true;
                        var length = mediaFile.FilePath.LastIndexOfCSafe('/');
                        if (length > 0)
                        {
                            librarySubFolderPath1 = mediaFile.FilePath.Substring(0, length);
                        }

                        mediaFile.FilePath = this.CheckAndEnsureFilePath(
                            siteInfo.SiteName,
                            mediaLibraryInfo.LibraryFolder,
                            librarySubFolderPath1,
                            mediaFile.FileName,
                            mediaFile.FileExtension,
                            ensureUniqueFileName,
                            out filePath1);
                        mediaFile.FileName = CMS.IO.Path.GetFileNameWithoutExtension(mediaFile.FilePath);
                    }
                }
            }

            if (ImageHelper.IsImage(mediaFile.FileExtension) && (mediaFile.FileImageWidth == 0) && (mediaFile.FileImageHeight == 0))
            {
                mediaLibraryInfo = mediaLibraryInfo ?? MediaLibraryInfoProvider.GetMediaLibraryInfo(mediaFile.FileLibraryID);
                if (mediaLibraryInfo != null)
                {
                    siteInfo = siteInfo ?? SiteInfoProvider.GetSiteInfo(mediaFile.FileSiteID);
                    if (siteInfo != null)
                    {
                        var str = DirectoryHelper.CombinePath(
                            MediaLibraryInfoProvider.GetMediaLibraryFolderPath(siteInfo.SiteName, mediaLibraryInfo.LibraryFolder, null),
                            mediaFile.FilePath);
                        if (CMS.IO.File.Exists(str))
                        {
                            var image = new ImageHelper().GetImage(str);
                            if (image != null)
                            {
                                mediaFile.FileImageHeight = image.Height;
                                mediaFile.FileImageWidth = image.Width;
                            }
                        }
                    }
                }
            }
            else if (!ImageHelper.IsImage(mediaFile.FileExtension) && ((mediaFile.FileImageWidth > 0) || (mediaFile.FileImageHeight > 0)))
            {
                mediaFile.FileImageHeight = 0;
                mediaFile.FileImageWidth = 0;
            }

            if (mediaFile.FileID > 0)
            {
                mediaFile.FileModifiedByUserID = userId;
            }
            else
            {
                mediaFile.FileCreatedWhen = DateTime.Now;
                mediaFile.FileCreatedByUserID = userId;
            }

            // using (CMSTransactionScope transactionScope = this.NewTransaction())
            // {
            try
            {
                // if (!mediaFile.IsFilePathUnique())
                // throw new MediaFilePathNotUniqueException(string.Format(CoreServices.Localization.GetString("mediafile.filepathnotunique", (string)null, true), (object)mediaFile.FilePath));
                // this.SetInfo(mediaFile);
                if (flag)
                {
                    var mediaFileInfo = mediaFile;
                    var siteName = siteInfo.SiteName;
                    var libraryFolder = mediaLibraryInfo.LibraryFolder;
                    var librarySubFolderPath2 = librarySubFolderPath1;
                    var fileName = mediaFile.FileName;
                    var fileExtension = mediaFile.FileExtension;
                    var fileGuid = mediaFile.FileGUID;
                    var fileBinary = mediaFile.FileBinary;
                    var fileData = fileBinary != null ? fileBinary : (BinaryData)mediaFile.FileBinaryStream;
                    var num1 = 0;
                    var num2 = ensureUniqueFileName ? 1 : 0;
                    var num3 = 1;
                    var filePath2 = filePath1;
                    var filePath3 = mediaFile.FilePath;
                    var str = this.SaveFileToDiskInternal(
                        siteName,
                        libraryFolder,
                        librarySubFolderPath2,
                        fileName,
                        fileExtension,
                        fileGuid,
                        fileData,
                        num1 != 0,
                        num2 != 0,
                        num3 != 0,
                        filePath2,
                        filePath3);
                    mediaFileInfo.FilePath = str;
                    MediaFileInfoProvider.DeleteMediaFileThumbnails(mediaFile);
                }
            }
            finally
            {
                if (mediaFile.FileBinaryStream != null) mediaFile.FileBinaryStream.Close();
            }

            // transactionScope.Commit();
            // }
            if (!mediaFile.Generalized.TouchCacheDependencies) return;
            CacheHelper.TouchKeys(MediaFileInfoProvider.GetDependencyCacheKeys(mediaFile, false));
        }

        private string CheckAndEnsureFilePath(
            string siteName,
            string libraryFolder,
            string librarySubFolderPath,
            string fileName,
            string fileExtension,
            bool ensureUniqueFileName,
            out string filePath)
        {
            var libraryFolderPath = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(siteName, libraryFolder, null);
            if (string.IsNullOrEmpty(libraryFolderPath)) throw new Exception("[MediaFileInfoProvider.CheckAndEnsureFilePath]: Physical library path doesn't exist.");
            var path1 = libraryFolderPath;
            librarySubFolderPath = librarySubFolderPath.TrimStart('\\');
            if (!string.IsNullOrEmpty(librarySubFolderPath)) path1 = DirectoryHelper.CombinePath(libraryFolderPath, librarySubFolderPath);
            if (!DirectoryHelper.CheckPermissions(path1)) throw new PermissionException(string.Format("[MediaFileInfoProvider.CheckAndEnsureFilePath]: Access to the path '{0}' is denied.", libraryFolderPath));
            filePath = DirectoryHelper.CombinePath(path1, fileName) + fileExtension;
            if (ensureUniqueFileName) filePath = MediaLibraryHelper.EnsureUniqueFileName(filePath);
            var fileName1 = CMS.IO.Path.GetFileName(filePath);
            string str;
            if (!(librarySubFolderPath != string.Empty)) str = fileName1;
            else str = DirectoryHelper.CombinePath(librarySubFolderPath, fileName1);
            var path2 = str;
            DirectoryHelper.EnsureDiskPath(filePath, MediaLibraryHelper.GetMediaRootFolderPath(siteName, null));
            return CMS.IO.Path.EnsureSlashes(path2, false);
        }
    }
}