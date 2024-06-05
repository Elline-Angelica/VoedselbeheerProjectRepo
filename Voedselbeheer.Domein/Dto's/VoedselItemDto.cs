namespace Voedselbeheer.Domein.Dto_s;

public record VoedselItemDto
(
     int Id ,
     string Naam ,
     string FotoUrl ,
     int VoedselgroepId , 
     int SubVoedselgroepId
     //double Hoeveelheid , 
     //DateTime StockDatum
     //DateTime ExpiryDate  
);