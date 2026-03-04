using HospitalManagement.Domain.DTOs;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultationsController : ControllerBase
{
    private readonly IConsultationService _consultationService;

    public ConsultationsController(IConsultationService consultationService)
    {
        _consultationService = consultationService;
    }

    [HttpPost]
    public async Task<IActionResult> Schedule([FromBody] CreateConsultationDto dto)
    {
        var consultation = new Consultation
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            Date = dto.Date,
            Status = (ConsultationStatus)dto.Status,
            Notes = dto.Notes
        };

        try
        {
            var created = await _consultationService.ScheduleAsync(consultation);
            return CreatedAtAction(nameof(GetUpcomingForPatient),
                new { patientId = created.PatientId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] ConsultationStatus status)
    {
        try
        {
            var updated = await _consultationService.UpdateStatusAsync(id, status);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _consultationService.CancelAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("patient/{patientId}/upcoming")]
    public async Task<IActionResult> GetUpcomingForPatient(int patientId)
    {
        var consultations = await _consultationService.GetUpcomingForPatientAsync(patientId);
        return Ok(consultations);
    }

    [HttpGet("doctor/{doctorId}/today")]
    public async Task<IActionResult> GetTodayForDoctor(int doctorId)
    {
        var consultations = await _consultationService.GetTodayForDoctorAsync(doctorId);
        return Ok(consultations);
    }
}