using Voedselbeheer.Domein.Dto_s;
using Voedselbeheer.Domein.Interface;
using Microsoft.Data.SqlClient;


namespace Voedselbeheer.Persistentie;

public class GerechtRepository : IGerechtRepository
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
        List<GerechtDto> gerechten = null;
            
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT *
                FROM Gerechten g
                JOIN Gerechten_VoedselItems gv
                ON g.Id = gv.GerechtID
                JOIN VoedselItem v
                ON gv.VoedselItemID = v.Id
                JOIN VoedselGroepen vg
                ON v.VoedselGroepID = vg.VoedselGroepID";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {

                GerechtDto gerechtDto;

                //moet er hier een reader.Read() staan
                gerechtDto = new GerechtDto
                (
                    reader.GetGuid(reader.GetOrdinal("id")), 
                    reader.GetString(reader.GetOrdinal("naam")),
                    reader.GetString(reader.GetOrdinal("foto"))
                );
                
                gerechten.Add(gerechtDto);
            }
        }

        return gerechten;
    }

    public void Insert(GerechtDto gerecht)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
                
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                string query = "INSERT INTO Gerechten (Id, Naam, Foto) VALUES (@Id, @Naam, @Foto);";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Id", gerecht.Id);
                    command.Parameters.AddWithValue("@Naam", gerecht.Naam);
                    command.Parameters.AddWithValue("@Foto", gerecht.FotoUrl);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"SQL insert failed: {ex.Message}");
            }
                
        }
    }

    public void Update(GerechtDto gerecht)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                string query = @"
                UPDATE Gerechten 
                SET 
                    Naam = @Naam, 
                    Foto = @Foto
                WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Id", gerecht.Id);
                    command.Parameters.AddWithValue("@Naam", gerecht.Naam);
                    command.Parameters.AddWithValue("@Foto", gerecht.FotoUrl);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"SQL update failed: {ex.Message}");
            }
        }
    }

    public void Delete(Guid id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                string query = "DELETE FROM Gerechten WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}