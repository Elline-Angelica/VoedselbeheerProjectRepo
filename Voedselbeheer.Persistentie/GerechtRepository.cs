using Voedselbeheer.Domein.Dto_s;
using Voedselbeheer.Domein.Interface;
using Microsoft.Data.SqlClient;
using Voedselbeheer.Domein;


namespace Voedselbeheer.Persistentie;

public class GerechtRepository : IGerechtRepository
{
    private readonly string _connectionString;

    public GerechtRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public Gerecht GetById(int id)
    {
        Gerecht gerecht = null;
            
       
            string query = @"
                SELECT g.Id, g.Naam, g.Foto, vi.Id as VoedselItemID, vi.Naam as VoedselItemNaam, vi.Foto, vi.VoedselGroep, gv.Hoeveelheid, gv.Eenheid
                FROM Gerechten g
                JOIN Gerechten_VoedselItems gv
                ON g.Id = gv.GerechtID
                JOIN VoedselItem vi
                ON vi.Id = gv.VoedselItemID
                WHERE g.Id = @Id";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (gerecht == null)
                            {
                            gerecht = new Gerecht {
                                Id = (int)reader["Id"],
                                Naam = (string)reader["Naam"],
                                FotoUrl = reader["Foto"] as string,
                                Ingredienten = new Dictionary<VoedselItem, double> ()
                            };
                            }

                            var voedselItem = new VoedselItem {
                                Id = (int)reader["VoedselItemID"],
                                Naam = (string)reader["VoedselItemNaam"],
                                FotoUrl = "",
                                VoedselGroep = (string)reader["VoedselGroep"],
                                Eenheid = (string)reader["Eenheid"]
                            };

                            double hoeveelheid = (double)reader["Hoeveelheid"];
                            gerecht.Ingredienten.Add(voedselItem, hoeveelheid);
                        }
                    }
                }
            }
        return gerecht;
    }

    public List<Gerecht> GetAll()
    {
        List<Gerecht> gerechten = new List<Gerecht>();

        string query = @"
        SELECT g.Id, g.Naam, g.Foto, vi.Id as VoedselItemID, vi.Naam as VoedselItemNaam, vi.Foto as VoedselItemFoto, vi.VoedselGroep, gv.Hoeveelheid, gv.Eenheid
        FROM Gerechten g
        JOIN Gerechten_VoedselItems gv ON g.Id = gv.GerechtID
        JOIN VoedselItem vi ON vi.Id = gv.VoedselItemID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Dictionary<int, Gerecht> gerechtDict = new Dictionary<int, Gerecht>();

                    while (reader.Read())
                    {
                        int gerechtId = (int)reader["Id"];
                        if (!gerechtDict.ContainsKey(gerechtId))
                        {
                            Gerecht nieuwGerecht = new Gerecht {
                                Id = gerechtId,
                                Naam = (string)reader["Naam"],
                                FotoUrl = reader["Foto"] as string,
                                Ingredienten = new Dictionary<VoedselItem, double>()
                            };
                            gerechtDict[gerechtId] = nieuwGerecht;
                        }

                        var voedselItem = new VoedselItem {
                            Id = (int)reader["VoedselItemID"],
                            Naam = (string)reader["VoedselItemNaam"],
                            FotoUrl = reader["VoedselItemFoto"] as string,
                            VoedselGroep = (string)reader["VoedselGroep"],
                            Eenheid = (string)reader["Eenheid"]
                        };

                        double hoeveelheid = (double)reader["Hoeveelheid"];
                        gerechtDict[gerechtId].Ingredienten.Add(voedselItem, hoeveelheid);
                    }

                    gerechten = gerechtDict.Values.ToList();
                }
            }
        }

        return gerechten;
    }

    public void Insert(Gerecht gerecht)
    {
        string insertGerechtQuery = @"
        INSERT INTO Gerechten (Naam, Foto)
        OUTPUT INSERTED.Id
        VALUES (@Naam, @Foto)";

        string instretVoedselItemQuery = @"
        INSERT INTO VoedselItem (Naam, Foto, VoedselGroep)
        OUTPUT INSERTED.Id
        VALUES (@Naam, @Foto, @VoedselGroep)";

        string insertGerechtVoedselItemQuery = @"
        INSERT INTO Gerechten_VoedselItems (GerechtID, VoedselItemID, Hoeveelheid, Eenheid)
        VALUES (@GerechtID, @VoedselItemID, @Hoeveelheid, @Eenheid)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    int gerechtId;
                    int voedselItemId;

                    // Insert the Gerecht
                    using (SqlCommand command = new SqlCommand(insertGerechtQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Naam", gerecht.Naam);
                        command.Parameters.AddWithValue("@Foto", (object)gerecht.FotoUrl ?? DBNull.Value);

                        gerechtId = (int)command.ExecuteScalar();
                    }

                    foreach (var item in gerecht.Ingredienten)
                    {

                        using (SqlCommand command = new SqlCommand(instretVoedselItemQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Naam", item.Key.Naam);
                            command.Parameters.AddWithValue("@Foto", "");
                            command.Parameters.AddWithValue("@VoedselGroep", item.Key.VoedselGroep);

                            voedselItemId = (int)command.ExecuteScalar();

                        }

                        using (SqlCommand command = new SqlCommand(insertGerechtVoedselItemQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@GerechtID", gerechtId);
                            command.Parameters.AddWithValue("@VoedselItemID", voedselItemId);
                            command.Parameters.AddWithValue("@Hoeveelheid", item.Value);
                            command.Parameters.AddWithValue("@Eenheid", item.Key.Eenheid);

                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public void Update(Gerecht gerecht)
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

    public void Delete(int id)
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

    public Gerecht GetByName(string naam)
    {
        Gerecht gerecht = null;
        naam = naam.ToLower().Trim();

        string query = @"
                SELECT g.Id, g.Naam, g.Foto, vi.Id as VoedselItemID, vi.Naam as VoedselItemNaam, vi.Foto, vi.VoedselGroep, gv.Hoeveelheid, gv.Eenheid
                FROM Gerechten g
                JOIN Gerechten_VoedselItems gv
                ON g.Id = gv.GerechtID
                JOIN VoedselItem vi
                ON vi.Id = gv.VoedselItemID
                WHERE g.Naam = @Naam";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Naam", naam);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (gerecht == null)
                        {
                            gerecht = new Gerecht {
                                Id = (int)reader["Id"],
                                Naam = (string)reader["Naam"],
                                FotoUrl = reader["Foto"] as string,
                                Ingredienten = new Dictionary<VoedselItem, double>()
                            };
                        }

                        var voedselItem = new VoedselItem {
                            Id = (int)reader["VoedselItemID"],
                            Naam = (string)reader["VoedselItemNaam"],
                            FotoUrl = "",
                            VoedselGroep = (string)reader["VoedselGroep"],
                            Eenheid = (string)reader["Eenheid"]
                        };

                        double hoeveelheid = (double)reader["Hoeveelheid"];
                        gerecht.Ingredienten.Add(voedselItem, hoeveelheid);
                    }
                }
            }
        }
        return gerecht;
    }
}