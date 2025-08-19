using System.Collections.Generic;
using System.Threading.Tasks;
using StaffManagementApi.Dtos;

namespace StaffManagementApi.Services
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffSummaryDto>> GetAllStaffSummaryAsync();
        Task<StaffDetailsDto> GetStaffByIdAsync(int id);
        Task<StaffDetailsDto> CreateStaffAsync(CreateStaffDto staffDto);
        Task<bool> UpdateStaffAsync(int id, UpdateStaffDto staffDto);
        Task<bool> DeleteStaffAsync(int id);
        Task<bool> TerminateStaffAsync(int id);
        Task<bool> UnTerminateStaffAsync(int id);
        Task<IEnumerable<StaffSummaryDto>> SearchStaffAsync(string keyword);
        Task<IEnumerable<StaffSummaryDto>> SearchStaffByNIMAsync(string nim);
        Task<IEnumerable<StaffDashboardDto>> GetLast10StaffAsync();
    }
}