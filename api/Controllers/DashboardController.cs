using HospitalManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("patients/{patientId}")]
    public async Task<IActionResult> GetPatientRecord(int patientId)
    {
        var record = await _dashboardService.GetPatientRecordAsync(patientId);
        if (record is null) return NotFound();
        return Ok(record);
    }

    [HttpGet("doctors/{doctorId}")]
    public async Task<IActionResult> GetDoctorPlanning(int doctorId)
    {
        var planning = await _dashboardService.GetDoctorPlanningAsync(doctorId);
        if (planning is null) return NotFound();
        return Ok(planning);
    }

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartmentStats()
    {
        var stats = await _dashboardService.GetDepartmentStatsAsync();
        return Ok(stats);
    }
}