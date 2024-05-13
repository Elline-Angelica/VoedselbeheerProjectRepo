namespace Voedselbeheer.Domein;

public class VoedselItem
{
    public int Id { get; set; }
    public string Naam { get; set; }
    public string FotoUrl { get; set; }
    public int VoedselgroepId { get; set; } // Verwijzing naar de voedselgroep
    public int SubVoedselgroepId { get; set; } // Verwijzing naar de subvoedselgroep
    public double Hoeveelheid { get; set; } // Hoeveelheid in gram
    public DateTime StockDatum { get; set; } // Datum in voorraad
    public DateTime ExpiryDate { get; set; } // Houdbaarheidsdatum
}