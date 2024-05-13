namespace Voedselbeheer.Domein.Dto_s;

public record GerechtDto
(
     Guid Id ,
     string Naam ,
     string FotoUrl //wordt soms gewoon 'Foto' genoemd
     //List<VoedselItem> Ingredienten
     );