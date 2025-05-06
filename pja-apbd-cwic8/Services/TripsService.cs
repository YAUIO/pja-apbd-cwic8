using System.Data;
using Microsoft.Data.SqlClient;
using pja_apbd_cwic8.DTOs;

namespace pja_apbd_cwic8.Services;

public class TripsService
{
    private readonly string _connection;

    public TripsService(IConfiguration cfg)
    {
        _connection = cfg.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();
    }

    public async Task<List<TripGet>> GetAllTripsAsync()
    {
        var trips = new List<TripGet>();

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();

        var command = new SqlCommand("SELECT * FROM Trip", connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            trips.Add(new TripGet
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString("Name"),
                Description = reader.GetString("Description"),
                DateFrom = reader.GetDateTime("DateFrom"),
                DateTo = reader.GetDateTime("DateTo")
            });

        return trips;
    }

    public async Task<List<TripGet>> GetTripsByClientAsync(int clientId)
    {
        var trips = new List<TripGet>();

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();

        var command = new SqlCommand("SELECT * FROM Trip JOIN Client_Trip ct on ct.IdTrip = Trip.IdTrip WHERE ct.IdClient = @Id", connection);
        
        command.Parameters.AddWithValue("@id", clientId);
        
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
            trips.Add(new TripGet
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString("Name"),
                Description = reader.GetString("Description"),
                DateFrom = reader.GetDateTime("DateFrom"),
                DateTo = reader.GetDateTime("DateTo")
            });

        return trips;
    }
}