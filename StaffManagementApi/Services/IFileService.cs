using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StaffManagementApi.Services
{
    public interface IFileService
    {
        Task<string> UploadPhotoAsync(IFormFile file, string staffId);
        Task<string> UploadExcelAsync(IFormFile file);
        Task<Stream> DownloadFileAsync(string fileName);
    }
}