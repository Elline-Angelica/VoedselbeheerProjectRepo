using Voedselbeheer.Domein.Dto_s;
using Voedselbeheer.Domein.Interface;
using Microsoft.Data.SqlClient;


namespace Voedselbeheer.Persistentie;

public class GerechtRepository : IRepository
{
    private readonly string _connectionString;

    public GerechtRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public GerechtDto GetById(Guid id)
    {
        GerechtDto gerecht = null;
            
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT g.Id, g.Naam, g.Foto
                FROM Gerechten g
                WHERE g.Id = @Id";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {

                //moet er hier een reader.Read() staan
                gerecht = new GerechtDto
                (
                    reader.GetGuid(reader.GetOrdinal("id")), 
                    reader.GetString(reader.GetOrdinal("naam")),
                    reader.GetString(reader.GetOrdinal("foto"))
                );
            }
        }

        return gerecht;
    }

    public IEnumerable<GerechtDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public void Insert(GerechtDto gerecht)
    {
        throw new NotImplementedException();
    }

    public void Update(GerechtDto gerecht)
    {
        throw new NotImplementedException();
    }

    public void Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}