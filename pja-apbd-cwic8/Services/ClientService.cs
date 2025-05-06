using System.Data;
using Microsoft.Data.SqlClient;
using pja_apbd_cwic8.DTOs;
using pja_apbd_cwic8.Models;

namespace pja_apbd_cwic8.Services;

public class ClientService
{
    private readonly string _connection;

    public ClientService(IConfiguration cfg)
    {
        _connection = cfg.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();
    }

    public async Task<Client> GetClientById(int id)
    {
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();

        //select client by id
        var getClient = new SqlCommand("SELECT * FROM Client WHERE Client.IdClient = @Id", connection);
        getClient.Parameters.AddWithValue("@Id", id);

        await using var reader = await getClient.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return new Client
            {
                Id = reader.GetInt32(0),
                Email = reader.GetString("Email"),
                Telephone = reader.GetString("Telephone"),
                Pesel = reader.GetString("Pesel"),
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName")
            };

        throw new KeyNotFoundException("No such client");
    }

    public async Task<int> AddClient(ClientPost c)
    {
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();

        //insert new client
        var command = new SqlCommand(
            "INSERT INTO Client(FirstName, LastName, Email, Telephone, Pesel) OUTPUT INSERTED.IdClient VALUES (@name, @surname, @email, @phone, @pesel)"
            , connection);
        command.Parameters.AddWithValue("@name", c.FirstName);
        command.Parameters.AddWithValue("@surname", c.LastName);
        command.Parameters.AddWithValue("@email", c.Email);
        command.Parameters.AddWithValue("@phone", c.Telephone);
        command.Parameters.AddWithValue("@pesel", c.Pesel);

        var result = await command.ExecuteScalarAsync();

        if (result == null) throw new Exception("Couldn't insert");

        return Convert.ToInt32(result);
    }
}