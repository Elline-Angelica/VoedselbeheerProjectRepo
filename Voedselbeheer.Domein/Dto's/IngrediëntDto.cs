namespace Voedselbeheer.Domein.Dto_s;

public record IngrediëntDto
(
    Guid Id,
    string Naam,
    double Hoeveelheid,// Hoeveelheid in gram
    int GerechtId // Verwijzing naar het gerecht
    
);