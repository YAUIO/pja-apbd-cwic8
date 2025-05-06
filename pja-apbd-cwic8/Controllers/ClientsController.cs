using Microsoft.AspNetCore.Mvc;
using pja_apbd_cwic8.DTOs;
using pja_apbd_cwic8.Services;

namespace pja_apbd_cwic8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly ClientService _clientService;
    private readonly TripsService _tripsService;

    public ClientsController(TripsService tripsService, ClientService clientService)
    {
        _tripsService = tripsService;
        _clientService = clientService;
    }

    [HttpGet("{id}")] //api/clients/id - get client by id
    public async Task<IActionResult> GetClientById(int id)
    {
        if (id < 0) return BadRequest();
        try
        {
            return Ok(await _clientService.GetClientById(id));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost] //api/clients/ - create new client
    public async Task<IActionResult> PostClient(ClientPost client)
    {
        try
        {
            var id = await _clientService.AddClient(client);
            return Created("", id);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}/trips")] //api/clients/id/trips - get client's trips
    public async Task<IActionResult> GetClientsTripsAsync(int id)
    {
        try
        {
            await _clientService.GetClientById(id);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }

        List<TripClientGet> trips = await _tripsService.GetTripsByClientAsync(id);
        return Ok(trips);
    }

    [HttpPut("{clientId}/trips/{tripId}")] //api/clients/id/trips/id - put a new trip on client's trip list
    public async Task<IActionResult> PutClientOnTripAsync(int clientId, int tripId)
    {
        try
        {
            await _clientService.GetClientById(clientId);
            var trip = await _tripsService.GetTripById(tripId);
            if (trip.MaxPeople - await _tripsService.GetRegisteredPeopleCountByTripIdAsync(tripId) <= 0) 
                return Conflict("Maximum amount of sign-ups reached");
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }

        try
        {
            await _tripsService.PutClientOnTripAsync(clientId, tripId);
            return Created();
        }
        catch (Exception e)
        {
            return Conflict(e.Message);
        }
    }

    [HttpDelete("{clientId}/trips/{tripId}")] //api/clients/id/trips/id - delete client's registration for a trip
    public async Task<IActionResult> DeleteClientTripRegistrationAsync(int clientId, int tripId)
    {
        try
        {
            await _clientService.GetClientById(clientId);
            await _tripsService.GetTripById(tripId);
            List<TripClientGet> trips = await _tripsService.GetTripsByClientAsync(clientId);
            var result = trips.FirstOrDefault(t => t.Id.Equals(tripId));
            if (result == null) throw new KeyNotFoundException("No such registration found");
            await _tripsService.DeleteClientsRegistrationAsync(clientId, tripId);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }

        return NoContent();
    }
}