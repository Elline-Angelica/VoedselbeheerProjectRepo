namespace Voedselbeheer.Domein;

public class Voedselgroep
{
    public int Id { get; set; }
    public string Naam { get; set; }
    public List<SubVoedselgroep> SubVoedselgroepen { get; set; }
}
