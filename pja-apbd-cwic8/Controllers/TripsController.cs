using Microsoft.AspNetCore.Mvc;
using pja_apbd_cwic8.DTOs;
using pja_apbd_cwic8.Services;

namespace pja_apbd_cwic8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly TripsService _tripsService;

    public TripsController(TripsService tripsService, ClientService clientService)
    {
        _tripsService = tripsService;
    }

    [HttpGet] //api/trips/ - get all trips
    public async Task<IActionResult> GetTripsAsync()
    {
        List<TripGet> trips = await _tripsService.GetAllTripsAsync();
        return Ok(trips);
    }
}