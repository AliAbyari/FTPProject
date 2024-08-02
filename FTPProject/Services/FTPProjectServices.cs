using FluentFTP;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using FluentFTP.Exceptions;
using Microsoft.Extensions.Logging;
using FTPProject.Domain.DTOs;

namespace FTPProject.Services
{
    public class FtpService
    {
        private readonly List<FtpFolderSettings> _ftpFolders;

        public FtpService(IOptions<AppSettings> appSettings)
        {
            _ftpFolders = appSettings.Value.FtpFolders;

            if (_ftpFolders == null || _ftpFolders.Count == 0)
            {
                throw new ArgumentException("No FTP folder settings configured.");
            }
        }

        public void UploadFiles()
        {
            foreach (var folder in _ftpFolders)
            {
                if (folder == null || folder.FtpSettings == null)
                {
                    continue;
                }

                var ftpClient = new FtpClient(folder.FtpSettings.Host, folder.FtpSettings.Port)
                {
                    Credentials = new NetworkCredential(folder.FtpSettings.Username, folder.FtpSettings.Password)
                };

                ftpClient.Config.EncryptionMode = folder.FtpSettings.Protocol.Equals("ftps", StringComparison.OrdinalIgnoreCase) ? FtpEncryptionMode.Explicit : FtpEncryptionMode.None;
                ftpClient.Config.ValidateAnyCertificate = true; 

                try
                {
                    ftpClient.Connect();

                    var files = Directory.GetFiles(folder.FolderPath);
                    if (files.Length == 0)
                    {
                        throw new FileNotFoundException($"No files found in folder: {folder.FolderPath}");
                    }

                    foreach (var file in files)
                    {
                        var remoteFilePath = $"{folder.FtpSettings.DestinationFolder}/{Path.GetFileName(file)}";
                        ftpClient.UploadFile(file, remoteFilePath);

                        // Archive the file
                        var archivePath = folder.ArchivePath;
                        if (!Directory.Exists(archivePath))
                        {
                            Directory.CreateDirectory(archivePath);
                        }

                        var archiveFileName = Path.Combine(archivePath, $"{Path.GetFileNameWithoutExtension(file)}_archive{Path.GetExtension(file)}");
                        File.Move(file, archiveFileName);
                    }
                }
                catch (FtpAuthenticationException)
                {
                    throw new UnauthorizedAccessException("Invalid FTP credentials provided.");
                }
                catch (FtpCommandException ex)
                {
                    throw new InvalidOperationException($"FTP command error: {ex.Message}");
                }
                catch (FileNotFoundException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while uploading files: {ex.Message}", ex);
                }
                finally
                {
                    if (ftpClient.IsConnected)
                    {
                        ftpClient.Disconnect();
                    }
                }
            }
        }
    }
}
