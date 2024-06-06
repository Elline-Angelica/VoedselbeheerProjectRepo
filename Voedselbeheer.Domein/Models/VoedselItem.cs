namespace Voedselbeheer.Domein;

public class VoedselItem
{
    public int Id { get; set; }
    public string Naam { get; set; }
    public string FotoUrl { get; set; }
    //public int VoedselgroepId { get; set; } // Verwijzing naar de voedselgroep
    //public int SubVoedselgroepId { get; set; } // Verwijzing naar de subvoedselgroep
    //public double Hoeveelheid { get; set; } // Hoeveelheid in gram
    //public DateTime StockDatum { get; set; } // Datum in voorraad --> zit al op de VoorraadItem
    //public DateTime ExpiryDate { get; set; } // Houdbaarheidsdatum --> moesten enkel met datum in voorraad werken
    public string VoedselGroep { get; set; } // bv gerecht-vlees-vis-groente-fruit-snacks
}