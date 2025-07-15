using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ClosedXML.Excel;
using StaffManagementApi.Entities;
using StaffManagementApi.Data;

namespace StaffManagementApi.Services
{
    public class FileService : IFileService
    {
        private readonly AppDbContext _context;
        private readonly string _photoDirectory;
        private readonly string _excelDirectory;

        public FileService(IConfiguration configuration, AppDbContext context)
        {
            _context = context;
            _photoDirectory = configuration["FilePaths:PhotoUploads"] ?? "Uploads/Photos";
            _excelDirectory = configuration["FilePaths:ExcelUploads"] ?? "Uploads/ExcelFiles";
        }

        

        

        public async Task<string> UploadPhotoAsync(IFormFile file, string staffId)
        {
            if (file == null || file.Length == 0) return null!;

            var fileName = $"{staffId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_photoDirectory, fileName);

            Directory.CreateDirectory(_photoDirectory);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public async Task<string> UploadExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return "No file uploaded.";

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            var staffList = new List<Staff>();

            for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
            {
                var wsRow = worksheet.Row(row);

                var staff = new Staff
                {
                    FullName = wsRow.Cell(1).GetString(),
                    NIM = wsRow.Cell(2).GetString(),
                    BinusianId = wsRow.Cell(3).GetString(),
                    Gender = wsRow.Cell(4).GetString(),
                    Email = wsRow.Cell(5).GetString(),
                    PhoneNumber = wsRow.Cell(6).GetString(),
                    ActiveSemester = int.TryParse(wsRow.Cell(7).GetString(), out var semester) ? semester : 0,
                    BinusianStatus = wsRow.Cell(8).GetString(),
                    NIK = wsRow.Cell(9).GetString(),
                    DateOfBirth = DateOnly.FromDateTime(wsRow.Cell(10).GetDateTime()),
                    Address = wsRow.Cell(11).GetString(),
                    NPWP = wsRow.Cell(12).GetString(),
                    BankAccountNumber = wsRow.Cell(13).GetString(),
                    BankBranch = wsRow.Cell(14).GetString(),
                    AccountHolderName = wsRow.Cell(15).GetString(),
                    ParentGuardianName = wsRow.Cell(16).GetString(),
                    ParentGuardianPhone = wsRow.Cell(17).GetString(),
                    EmergencyContact = wsRow.Cell(18).GetString(),
                    EmergencyRelation = wsRow.Cell(19).GetString(),
                    // IsActive = bool.TryParse(wsRow.Cell(20).GetString(), out var isActive) && isActive
                    IsActive = wsRow.Cell(20).GetValue<int>() == 1,
                };

                try
                {
                    if (wsRow.Cell(21).DataType == XLDataType.DateTime)
                        staff.StartDate = DateOnly.FromDateTime(wsRow.Cell(21).GetDateTime());
                    else if (DateOnly.TryParse(wsRow.Cell(21).GetString(), out var start))
                        staff.StartDate = start;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Row {row}: Gagal parsing StartDate - {ex.Message}");
                }

                try
                {
                    if (wsRow.Cell(22).DataType == XLDataType.DateTime)
                        staff.EndDate = DateOnly.FromDateTime(wsRow.Cell(22).GetDateTime());
                    else if (DateOnly.TryParse(wsRow.Cell(22).GetString(), out var end))
                        staff.EndDate = end;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Row {row}: Gagal parsing EndDate - {ex.Message}");
                }

                staffList.Add(staff);
            }

            _context.Staffs.AddRange(staffList);
            await _context.SaveChangesAsync();

            return $"Data pada excel sejumlah {staffList.Count} sudah masuk di database.";
        }


        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var filePath = Path.Combine(_photoDirectory, fileName);
            if (!File.Exists(filePath)) filePath = Path.Combine(_excelDirectory, fileName);
            if (!File.Exists(filePath)) return null!;

            return await Task.FromResult(File.OpenRead(filePath));
        }
    }
}
