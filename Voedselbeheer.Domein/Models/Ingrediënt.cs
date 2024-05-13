namespace Voedselbeheer.Domein;

public class Ingrediënt
{
    public int Id { get; set; }
    public string Naam { get; set; }
    public double Hoeveelheid { get; set; } // Hoeveelheid in gram
    public int GerechtId { get; set; } // Verwijzing naar het gerecht
}