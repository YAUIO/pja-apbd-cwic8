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

        //select a trip with joined country name
        var command = new SqlCommand(
            "SELECT Trip.IdTrip, Trip.Name AS TripName, Trip.Description, Trip.DateFrom, Trip.MaxPeople, Trip.DateTo, c.Name AS CountryName FROM Trip JOIN Country_Trip ct on ct.IdTrip = Trip.IdTrip JOIN Country c on ct.IdCountry = c.IdCountry"
            , connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var trip = trips.FirstOrDefault(t => t.Id == reader.GetInt32(0));

            if (trip == null) trips.Add(new TripGet
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString("TripName"),
                Description = reader.GetString("Description"),
                DateFrom = reader.GetDateTime("DateFrom"),
                DateTo = reader.GetDateTime("DateTo"),
                MaxPeople = reader.GetInt32("MaxPeople"),
                Countries = new List<string>{reader.GetString("CountryName")}
            });
            else trip.Countries.Add(reader.GetString("CountryName"));
        }

        return trips;
    }

    public async Task<List<TripClientGet>> GetTripsByClientAsync(int clientId)
    {
        var trips = new List<TripClientGet>();

        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();

        //select a trip by client
        var command =
            new SqlCommand("SELECT * FROM Trip JOIN Client_Trip ct on ct.IdTrip = Trip.IdTrip WHERE ct.IdClient = @Id",
                connection);

        command.Parameters.AddWithValue("@Id", clientId);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            trips.Add(new TripClientGet
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString("Name"),
                Description = reader.GetString("Description"),
                DateFrom = reader.GetDateTime("DateFrom"),
                DateTo = reader.GetDateTime("DateTo"),
                MaxPeople = reader.GetInt32("MaxPeople"),
                PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate")) ? null : reader.GetInt32("PaymentDate"),
                RegisteredAt = reader.GetInt32("RegisteredAt")
            });

        return trips;
    }
    
    public async Task<TripGet> GetTripById(int id)
    {
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();

        //select trip by id
        var getClient = new SqlCommand("SELECT * FROM Trip WHERE Trip.IdTrip = @Id", connection);
        getClient.Parameters.AddWithValue("@Id", id);

        await using var reader = await getClient.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return new TripGet()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString("Name"),
                Description = reader.GetString("Description"),
                DateFrom = reader.GetDateTime("DateFrom"),
                DateTo = reader.GetDateTime("DateTo"),
                MaxPeople = reader.GetInt32("MaxPeople")
            };

        throw new KeyNotFoundException("No such trip");
    }

    public async Task<int> GetRegisteredPeopleCountByTripIdAsync(int tripId)
    {
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();

        //select client_trip by trip_id
        var command = new SqlCommand("SELECT count(*) FROM Client_Trip ct WHERE ct.IdTrip = @Id", connection);
        command.Parameters.AddWithValue("@Id", tripId);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return reader.GetInt32(0);
        }

        throw new KeyNotFoundException("No such trip");
    }

    public async Task PutClientOnTripAsync(int clientId, int tripId)
    {
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();

        //insert new client
        var command = new SqlCommand(
            "INSERT INTO Client_Trip(IdClient, IdTrip, RegisteredAt) OUTPUT INSERTED.IdClient VALUES (@idClient, @idTrip, @registered)"
            , connection);
        command.Parameters.AddWithValue("@idClient", clientId);
        command.Parameters.AddWithValue("@idTrip", tripId);
        command.Parameters.AddWithValue("@registered", int.Parse(DateTime.Now.ToString("yyyyMMdd")));

        var result = await command.ExecuteScalarAsync();

        if (result == null) throw new Exception("Couldn't insert");
    }

    public async Task DeleteClientsRegistrationAsync(int clientId, int tripId)
    {
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();

        //delete a client_trip record
        var command = new SqlCommand(
            "DELETE FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip"
            , connection);
        command.Parameters.AddWithValue("@IdClient", clientId);
        command.Parameters.AddWithValue("@IdTrip", tripId);
        
        var result = await command.ExecuteNonQueryAsync();

        if (result == 0)
        {
            throw new KeyNotFoundException("Couldn't find the registration to delete");
        }
    }
}