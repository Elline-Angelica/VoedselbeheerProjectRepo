namespace Voedselbeheer.Domein;

public class Gerecht
{
    public int Id { get; set; }
    public string Naam { get; set; }
    public string FotoUrl { get; set; }
    public List<VoedselItem> Ingredienten { get; set; }
}