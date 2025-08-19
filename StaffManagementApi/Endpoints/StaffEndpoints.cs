using Microsoft.AspNetCore.Mvc;
using StaffManagementApi.Dtos;
using StaffManagementApi.Services;

namespace StaffManagementApi.Endpoints;

[ApiController]
[Route("api/staff")]
public class StaffEndpoints : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly IFileService _fileService;

    public StaffEndpoints(IStaffService staffService, IFileService fileService)
    {
        _staffService = staffService;
        _fileService = fileService;
    }

    // GET semua data
    [HttpGet("all")]
    public async Task<IActionResult> GetAllStaff()
    {
        var result = await _staffService.GetAllStaffSummaryAsync();
        return Ok(result);
    }
    
    [HttpGet("last")]
    public async Task<IActionResult> GetLast10Staff()
    {
        var result = await _staffService.GetLast10StaffAsync();
        return Ok(result);
    }


    // Search 1 data orang
    [HttpGet("search")]
    public async Task<IActionResult> SearchStaff([FromQuery] string keyword)
    {
        var result = await _staffService.SearchStaffAsync(keyword);
        return Ok(result);
    }

    [HttpGet("search-nim")]
    public async Task<IActionResult> SearchStaffByNIM([FromQuery] string nim)
    {
        var result = await _staffService.SearchStaffByNIMAsync(nim);
        return Ok(result);
    }


    // GET 1 ID 
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStaffById(int id)
    {
        var result = await _staffService.GetStaffByIdAsync(id);
        return result != null ? Ok(result) : NotFound();
    }

    // Add data
    [HttpPost("add")]
    public async Task<IActionResult> AddStaff([FromBody] CreateStaffDto staffDto)
    {
        try
        {
            var result = await _staffService.CreateStaffAsync(staffDto);
            return CreatedAtAction(nameof(GetStaffById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex) 
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex) // error lain
        {
            return StatusCode(500, new { message = "Terjadi kesalahan di server.", detail = ex.Message });
        }
    }   

    // Update
    [HttpPut("edit/{id}")]
    public async Task<IActionResult> EditStaff(int id, [FromBody] UpdateStaffDto staffDto)
    {
        var success = await _staffService.UpdateStaffAsync(id, staffDto);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        Console.WriteLine("Deleting");
        var success = await _staffService.DeleteStaffAsync(id);
        Console.WriteLine("Delete success");
        return success ? Ok() : NotFound();
    }

    [HttpPost("terminate/{id}")]
    public async Task<IActionResult> TerminateStaff(int id)
    {
        var success = await _staffService.TerminateStaffAsync(id);
        return success ? Ok() : NotFound();
    }

    [HttpPost("unterminate/{id}")]
    public async Task<IActionResult> UnTerminateStaff(int id)
    {
        var success = await _staffService.UnTerminateStaffAsync(id);
        return success ? Ok() : NotFound();
    }

    [HttpPost("upload/excel")]
    public async Task<IActionResult> UploadExcel([FromForm] IFormFile file)
    {
        var result = await _fileService.UploadExcelAsync(file);
        return Ok(result);
    }

    [HttpPost("upload/photo/{id}")]
    public async Task<IActionResult> UploadPhoto(int id, [FromForm] IFormFile photo)
    {
        var result = await _fileService.UploadPhotoAsync(photo, id.ToString());
        return Ok(new { filePath = result });
    }
}
