namespace FTPProject.Domain.DTOs
{
    public class AppSetting
    {
        public bool InsertErrorLog { get; set; }
        public bool InsertInfoLog { get; set; }
        public string LogFilePath { get; set; }
        public int LogFileSizeMB { get; set; }
        public string LogFileName { get; set; }
    }
}
