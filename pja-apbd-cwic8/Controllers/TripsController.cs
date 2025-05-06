using Microsoft.AspNetCore.Mvc;
using pja_apbd_cwic8.DTOs;
using pja_apbd_cwic8.Services;

namespace pja_apbd_cwic8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly TripsService _service;

    public TripsController(TripsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetTripsAsync()
    {
        List<TripGet> trips = await _service.GetAllTripsAsync();
        return Ok(trips);
    }

    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientsTripsAsync(int id)
    {
        List<TripGet> trips = await _service.GetTripsByClientAsync(id);
        return Ok(trips);
    }
}