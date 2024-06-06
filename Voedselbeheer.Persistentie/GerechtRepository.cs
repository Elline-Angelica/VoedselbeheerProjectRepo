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
    public Gerecht GetGerechtById(int id)
    {
        Gerecht gerecht = null;


        string query = @"
                SELECT g.Id, g.Naam, g.Foto, vi.Id as VoedselItemID, vi.Naam as VoedselItemNaam, vi.Foto, vi.VoedselGroep, gv.Hoeveelheid, vi.Eenheid
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


    public List<Gerecht> GetAllGerechten()
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

    public void InsertGerecht(Gerecht gerecht)
    {
        if (GetGerechtByName(gerecht.Naam) != null)
        {
            string insertGerechtQuery = @"
        INSERT INTO Gerechten (Naam, Foto)
        OUTPUT INSERTED.Id
        VALUES (@Naam, @Foto)";

            string instretVoedselItemQuery = @"
        INSERT INTO VoedselItem (Naam, Foto, VoedselGroep, Eenheid)
        OUTPUT INSERTED.Id
        VALUES (@Naam, @Foto, @VoedselGroep, @Eenheid)";

            string insertGerechtVoedselItemQuery = @"
        INSERT INTO Gerechten_VoedselItems (GerechtID, VoedselItemID, Hoeveelheid)
        VALUES (@GerechtID, @VoedselItemID, @Hoeveelheid)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int gerechtId;
                        int voedselItemId;

                        // InsertGerecht the Gerecht
                        using (SqlCommand command = new SqlCommand(insertGerechtQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Naam", gerecht.Naam);
                            command.Parameters.AddWithValue("@Foto", (object)gerecht.FotoUrl ?? DBNull.Value);

                            gerechtId = (int)command.ExecuteScalar();
                        }

                        foreach (var item in gerecht.Ingredienten)
                        {
                            VoedselItem vi = GetVoedselItemByName(item.Key.Naam);
                            if (vi == null)
                            {

                                using (SqlCommand command = new SqlCommand(instretVoedselItemQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Naam", item.Key.Naam);
                                    command.Parameters.AddWithValue("@Foto", "");
                                    command.Parameters.AddWithValue("@VoedselGroep", item.Key.VoedselGroep);
                                    command.Parameters.AddWithValue("@Eenheid", item.Key.Eenheid);

                                    voedselItemId = (int)command.ExecuteScalar();

                                }

                                using (SqlCommand command = new SqlCommand(insertGerechtVoedselItemQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@GerechtID", gerechtId);
                                    command.Parameters.AddWithValue("@VoedselItemID", voedselItemId);
                                    command.Parameters.AddWithValue("@Hoeveelheid", item.Value);

                                    command.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (SqlCommand command = new SqlCommand(insertGerechtVoedselItemQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@GerechtID", gerechtId);
                                    command.Parameters.AddWithValue("@VoedselItemID", vi.Id);
                                    command.Parameters.AddWithValue("@Hoeveelheid", item.Value);

                                    command.ExecuteNonQuery();
                                }
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
    }

    public void UpdateGerecht(Gerecht oudGerecht, Gerecht nieuwGerecht)
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

                string updateIngredientenQuery = @"
                UPDATE VoedselItem
                SET
                    Naam = @Naam,
                    Foto = @Foto
                    VoedselGroep = @VoedselGroep";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Id", oudGerecht.Id);
                    command.Parameters.AddWithValue("@Naam", nieuwGerecht.Naam);
                    command.Parameters.AddWithValue("@Foto", nieuwGerecht.FotoUrl);
                    command.ExecuteNonQuery();
                }

                foreach(var ingredient in oudGerecht.Ingredienten)
                {
                    //DeleteIngredient(ingredient);
                    //using (SqlCommand command = new SqlCommand(updateIngredientenQuery, connection, transaction))
                    //{
                    //    command.Parameters.AddWithValue("@Naam", );
                    //    command.Parameters.AddWithValue("@Foto", nieuwGerecht.FotoUrl);
                    //    command.ExecuteNonQuery();
                    //}
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
    public List<VoedselItem> GetAllVoedselItems()
    {
        List<VoedselItem> voedselItems = new List<VoedselItem>();

        string query = @"
            SELECT Id , Naam, Foto, VoedselGroep, Eenheid
            FROM VoedselItem";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var voedselitem = new VoedselItem {
                            Id = (int)reader["Id"],
                            Naam = (string)reader["Naam"],
                            FotoUrl = reader["Foto"] as string,
                            Eenheid = reader["Eenheid"] as string,
                            VoedselGroep = reader["VoedselGroep"] as string
                        };
                        voedselItems.Add(voedselitem);
                    }
                }
            }
        }
        return voedselItems;
    }

    public VoedselItem GetVoedselItemByName(string naam)
    {
        VoedselItem voedselitem = null;
        naam = naam.ToLower().Trim();

        string query = @"
                SELECT Id , Naam, Foto, VoedselGroep, Eenheid
                FROM VoedselItem 
                WHERE Naam = @Naam";
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
                        if (voedselitem == null)
                        {
                            voedselitem = new VoedselItem {
                                Id = (int)reader["Id"],
                                Naam = (string)reader["Naam"],
                                FotoUrl = reader["Foto"] as string,
                                Eenheid = reader["Eenheid"] as string,
                                VoedselGroep = reader["VoedselGroep"] as string
                            };
                        }

                        var voedselItem = new VoedselItem {
                            Id = (int)reader["Id"],
                            Naam = (string)reader["Naam"],
                            FotoUrl = "",
                            VoedselGroep = (string)reader["VoedselGroep"],
                            Eenheid = (string)reader["Eenheid"]
                        };                       
                    }
                }
            }
        }
        return voedselitem;
    }
    public void InsertVoedselItem(VoedselItem voedselItem)
    {
        // Controleren of het voedselitem al bestaat
        VoedselItem existingItem = GetVoedselItemByName(voedselItem.Naam);
        if (existingItem != null)
        {
            // Voedselitem bestaat al, je kunt hier eventueel een exception gooien of een andere actie ondernemen
            return;
        }

        // Als het voedselitem niet bestaat, voeg het dan in
        string insertQuery = @"
        INSERT INTO VoedselItem (Naam, Foto, VoedselGroep, Eenheid)
        VALUES (@Naam, @Foto, @VoedselGroep, @Eenheid)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Naam", voedselItem.Naam);
                command.Parameters.AddWithValue("@Foto", (object)voedselItem.FotoUrl ?? DBNull.Value);
                command.Parameters.AddWithValue("@VoedselGroep", voedselItem.VoedselGroep);
                command.Parameters.AddWithValue("@Eenheid", voedselItem.Eenheid);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
    public Gerecht GetGerechtByName(string naam)
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