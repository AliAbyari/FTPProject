using Microsoft.AspNetCore.Mvc;
using FTPProject.Services;
using System.Net;
using ContractsManagement.Api.Helpers;
using FTPProject.Domain.DTOs;
using Microsoft.Extensions.Options;

namespace FTPProject.Areas.Application.Controllers
{
    [Route("application/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "application")]
    public class FTPController : ControllerBase
    {
        private readonly FtpService _ftpService;
        private readonly AppSetting _appSettings;


        public FTPController(FtpService ftpService, IOptions<AppSetting> appSettings)
        {
            _ftpService = ftpService;
            _appSettings = appSettings.Value;

        }

        [HttpPost("UploadFiles")]
        public IActionResult UploadFiles()
        {
            try
            {
                _ftpService.UploadFiles();
                return Ok(new
                {
                    TimeStamp = DateTime.Now,
                    ResponseCode = HttpStatusCode.OK,
                    Message = "فایل با موفقیت ارسال شد",
                    Value = "",
                    Error = ""
                });
            }
            catch (FileNotFoundException ex)
            {
                SecurityHelpers.InsertToLog(_appSettings.InsertErrorLog, "Upload_FileNotFoundException - " + ex.ToString());

                return Ok(new
                {
                    TimeStamp = DateTime.Now,
                    ResponseCode = HttpStatusCode.NotFound,
                    Message = "در این پوشه فایلی وجود ندارد.",
                    Value = "",
                    Error = ""
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                SecurityHelpers.InsertToLog(_appSettings.InsertErrorLog, "Upload_UnauthorizedAccessException - " + ex.ToString());

                return Ok(new
                {
                    TimeStamp = DateTime.Now,
                    ResponseCode = HttpStatusCode.Unauthorized,
                    Message = "نام کاربری ویا رمز عبور نادرست است.",
                    Value = "",
                    Error = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                SecurityHelpers.InsertToLog(_appSettings.InsertErrorLog, "Upload_InvalidOperationException - " + ex.ToString());

                return Ok(new
                {
                    TimeStamp = DateTime.Now,
                    ResponseCode = HttpStatusCode.BadRequest,
                    Message = "خطا در دستور FTP.",
                    Value = "",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                SecurityHelpers.InsertToLog(_appSettings.InsertErrorLog, "Upload_Exception - " + ex.ToString());
                return Ok(new
                {

                    TimeStamp = DateTime.Now,
                    ResponseCode = HttpStatusCode.InternalServerError,
                    Message = "خطای داخلی سرور رخ داده است",
                    Value = "",
                    Error = ex.ToString()
                });
            }
        }
    }
}
