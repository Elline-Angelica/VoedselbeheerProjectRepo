﻿using Voedselbeheer.Domein.Dto_s;
using Voedselbeheer.Domein.Interface;
using Microsoft.Data.SqlClient;
using Voedselbeheer.Domein;


namespace Voedselbeheer.Persistentie;

public class GerechtRepository : IRepository
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
                SELECT g.Id, g.Naam, g.Foto, vi.Id, vi.Naam, vi.Foto, vi.VoedselGroep, gv.Hoeveelheid, gv.Eenheid
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
                                    FotoUrl = reader["Foto"] as string
                                };
                            }

                            var voedselItem = new VoedselItem {
                                Id = (int)reader["VoedselItemId"],
                                Naam = (string)reader["VoedselItemNaam"],
                                FotoUrl = reader["VoedselItemFoto"] as string,
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

    public IEnumerable<GerechtDto> GetAll()
    {
        List<GerechtDto> gerechten = null;
            
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT *
                FROM Gerechten";

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