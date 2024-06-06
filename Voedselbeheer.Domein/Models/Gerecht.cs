namespace Voedselbeheer.Domein;

public class Gerecht : VoedselItem
{
    //public int Id { get; set; }
    //public string Naam { get; set; }
    //public string FotoUrl { get; set; }
    // dict welke voedselitem + hoeveelheid voor de ingredienten (eenheid zit op de VoedselItem)
    public Dictionary<VoedselItem, double> Ingredienten { get; set; }
}