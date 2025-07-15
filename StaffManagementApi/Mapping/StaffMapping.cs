using AutoMapper;
using StaffManagementApi.Dtos;
using StaffManagementApi.Entities;


namespace StaffManagementApi.Mapping;

public class StaffMapping : Profile
{
    public StaffMapping(){
        CreateMap<CreateStaffDto, Staff>();
        CreateMap<UpdateStaffDto, Staff>();
        CreateMap<Staff, StaffDetailsDto>();
        CreateMap<Staff, StaffSummaryDto>();
    }
}
