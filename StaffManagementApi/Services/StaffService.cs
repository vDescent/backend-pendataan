using Microsoft.EntityFrameworkCore;
using StaffManagementApi.Data;
using StaffManagementApi.Dtos;
using StaffManagementApi.Entities;

namespace StaffManagementApi.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _context;

        public StaffService(AppDbContext context)
        {
            _context = context;
        }
        // Comment Gaperlu si haruse sg perlu cmn details e tok wae, cmn tambahi Start sama EndDate
        // Comment, cmn kalo pengen tampilin 1 data staff komplit emg perlu

        private async Task AutoTerminateExpiredStaffAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var expiredStaff = await _context.Staffs
                .Where(s => s.EndDate < today && s.IsActive)
                .ToListAsync();

            foreach (var staff in expiredStaff)
            {
                staff.IsActive = false;
            }

            if (expiredStaff.Any())
                await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StaffSummaryDto>> GetAllStaffSummaryAsync()
        {
            await AutoTerminateExpiredStaffAsync();

            return await _context.Staffs
                .Select(s => new StaffSummaryDto
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    NIM = s.NIM,
                    BinusianId = s.BinusianId,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        // UPDATE tambah start date sama end date

        public async Task<StaffDetailsDto> GetStaffByIdAsync(int id)
        {
            await AutoTerminateExpiredStaffAsync();

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return null!;

            return new StaffDetailsDto
            {
                Id = staff.Id,
                FullName = staff.FullName,
                NIM = staff.NIM,
                BinusianId = staff.BinusianId,
                Gender = staff.Gender,
                Email = staff.Email,
                PhoneNumber = staff.PhoneNumber,
                ActiveSemester = staff.ActiveSemester,
                BinusianStatus = staff.BinusianStatus,
                NIK = staff.NIK,
                DateOfBirth = staff.DateOfBirth,
                Address = staff.Address,
                NPWP = staff.NPWP,
                BankAccountNumber = staff.BankAccountNumber,
                BankBranch = staff.BankBranch,
                AccountHolderName = staff.AccountHolderName,
                ParentGuardianName = staff.ParentGuardianName,
                ParentGuardianPhone = staff.ParentGuardianPhone,
                EmergencyContact = staff.EmergencyContact,
                EmergencyRelation = staff.EmergencyRelation,
                IsActive = staff.IsActive,
                StartDate = staff.StartDate,
                EndDate = staff.EndDate
            };
        }

        // UPDATE kasih StartDate sama EndDate

        public async Task<StaffDetailsDto> CreateStaffAsync(CreateStaffDto staffDto)
        {
            await AutoTerminateExpiredStaffAsync();
            var existingStaff = await _context.Staffs
                .FirstOrDefaultAsync(s => s.NIM == staffDto.NIM || s.BinusianId == staffDto.BinusianId);
            if (existingStaff != null)
            {
                throw new InvalidOperationException("Staff with this NIM or BinusianId already exists.");
            }
    
            var staff = new Staff
            {
                FullName = staffDto.FullName,
                NIM = staffDto.NIM,
                BinusianId = staffDto.BinusianId,
                Gender = staffDto.Gender,
                Email = staffDto.Email,
                PhoneNumber = staffDto.PhoneNumber,
                ActiveSemester = staffDto.ActiveSemester,
                BinusianStatus = staffDto.BinusianStatus,
                NIK = staffDto.NIK,
                DateOfBirth = staffDto.DateOfBirth,
                Address = staffDto.Address,
                NPWP = staffDto.NPWP,
                BankAccountNumber = staffDto.BankAccountNumber,
                BankBranch = staffDto.BankBranch,
                AccountHolderName = staffDto.AccountHolderName,
                ParentGuardianName = staffDto.ParentGuardianName,
                ParentGuardianPhone = staffDto.ParentGuardianPhone,
                EmergencyContact = staffDto.EmergencyContact,
                EmergencyRelation = staffDto.EmergencyRelation,
                IsActive = true,
                StartDate = staffDto.StartDate,
                EndDate = staffDto.EndDate
            };

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            return await GetStaffByIdAsync(staff.Id);
        }


        public async Task<bool> UpdateStaffAsync(int id, UpdateStaffDto staffDto)
        {
            await AutoTerminateExpiredStaffAsync();

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return false;

            staff.FullName = staffDto.FullName;
            staff.NIM = staffDto.NIM;
            staff.BinusianId = staffDto.BinusianId;
            staff.Gender = staffDto.Gender;
            staff.Email = staffDto.Email;
            staff.PhoneNumber = staffDto.PhoneNumber;
            staff.ActiveSemester = staffDto.ActiveSemester;
            staff.BinusianStatus = staffDto.BinusianStatus;
            staff.NIK = staffDto.NIK;
            staff.DateOfBirth = staffDto.DateOfBirth;
            staff.Address = staffDto.Address;
            staff.NPWP = staffDto.NPWP;
            staff.BankAccountNumber = staffDto.BankAccountNumber;
            staff.BankBranch = staffDto.BankBranch;
            staff.AccountHolderName = staffDto.AccountHolderName;
            staff.ParentGuardianName = staffDto.ParentGuardianName;
            staff.ParentGuardianPhone = staffDto.ParentGuardianPhone;
            staff.EmergencyContact = staffDto.EmergencyContact;
            staff.EmergencyRelation = staffDto.EmergencyRelation;
            staff.IsActive = staffDto.IsActive;
            staff.StartDate = staffDto.StartDate;
            staff.EndDate = staffDto.EndDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            await AutoTerminateExpiredStaffAsync();

            await AutoTerminateExpiredStaffAsync();

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return false;

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TerminateStaffAsync(int id)
        {
            await AutoTerminateExpiredStaffAsync();

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return false;

            staff.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UnTerminateStaffAsync(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return false;

            staff.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StaffSummaryDto>> SearchStaffAsync(string keyword)
        {
            await AutoTerminateExpiredStaffAsync();

            return await _context.Staffs
                .Where(s => s.FullName.Contains(keyword) || s.Email.Contains(keyword))
                .Select(s => new StaffSummaryDto
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    NIM = s.NIM,
                    BinusianId = s.BinusianId,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StaffSummaryDto>> SearchStaffByNIMAsync(string nim)
        {
            await AutoTerminateExpiredStaffAsync();

            return await _context.Staffs
                .Where(s => s.NIM.Contains(nim))
                .Select(s => new StaffSummaryDto
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    NIM = s.NIM,
                    BinusianId = s.BinusianId,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StaffDashboardDto>> GetLast10StaffAsync()
        {
            await AutoTerminateExpiredStaffAsync();

            return await _context.Staffs
                .OrderByDescending(s => s.Id)
                .Take(10)
                .Select(s => new StaffDashboardDto
                {
                    FullName = s.FullName,
                    NIM = s.NIM,
                    BinusianId = s.BinusianId,
                    BinusianStatus = s.BinusianStatus,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        } 
    }
}
