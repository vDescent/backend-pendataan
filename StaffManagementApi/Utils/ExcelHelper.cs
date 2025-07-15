using System.Data;
using ExcelDataReader;
using StaffManagementApi.Dtos;

namespace StaffManagementApi.Utils
{
    public static class ExcelHelper
    {
        public static async Task<List<CreateStaffDto>> ReadExcelFileAsync(Stream fileStream)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var staffList = new List<CreateStaffDto>();

            using (var reader = ExcelReaderFactory.CreateReader(fileStream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true // Gunakan header row
                    }
                });

                var dataTable = result.Tables[0];

                foreach (DataRow row in dataTable.Rows.Cast<DataRow>())
                {
                    var staff = new CreateStaffDto
                    {
                        FullName = row["FullName"]?.ToString() ?? "",
                        NIM = row["NIM"]?.ToString() ?? "",
                        BinusianId = row["BinusianId"]?.ToString() ?? "",
                        Gender = row["Gender"]?.ToString() ?? "",
                        Email = row["Email"]?.ToString() ?? "",
                        PhoneNumber = row["PhoneNumber"]?.ToString() ?? "",
                        ActiveSemester = int.TryParse(row["ActiveSemester"]?.ToString(), out int semester) ? semester : 0,
                        BinusianStatus = row["BinusianStatus"]?.ToString() ?? "",
                        NIK = row["NIK"]?.ToString() ?? "",
                        DateOfBirth = DateOnly.TryParse(row["DateOfBirth"]?.ToString(), out DateOnly dob) ? dob : default,
                        Address = row["Address"]?.ToString() ?? "",
                        NPWP = row["NPWP"]?.ToString() ?? "",
                        BankAccountNumber = row["BankAccountNumber"]?.ToString() ?? "",
                        BankBranch = row["BankBranch"]?.ToString() ?? "",
                        AccountHolderName = row["AccountHolderName"]?.ToString() ?? "",
                        ParentGuardianName = row["ParentGuardianName"]?.ToString() ?? "",
                        ParentGuardianPhone = row["ParentGuardianPhone"]?.ToString() ?? "",
                        EmergencyContact = row["EmergencyContact"]?.ToString() ?? "",
                        EmergencyRelation = row["EmergencyRelation"]?.ToString() ?? ""
                    };
                    staffList.Add(staff);
                }
            }

            return await Task.FromResult(staffList);
        }
    }
}
