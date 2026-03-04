using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var patients = await _patientService.GetAllAlphabeticalAsync(page, pageSize);
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null) return NotFound();
        return Ok(patient);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        var patients = await _patientService.SearchByNameAsync(name);
        return Ok(patients);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Patient patient)
    {
        try
        {
            var created = await _patientService.CreateAsync(patient);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
    {
        if (id != patient.Id) return BadRequest("L'id ne correspond pas.");

        try
        {
            var updated = await _patientService.UpdateAsync(patient);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _patientService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}