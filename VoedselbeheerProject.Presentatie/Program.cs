using CsvHelper;
using CsvHelper.Configuration;
using System.Formats.Asn1;
using System.Globalization;
using Voedselbeheer.Domein;
using Voedselbeheer.Domein.Interface;
using Voedselbeheer.Persistentie;

namespace VoedselbeheerProject;

class Program
{
    static void Main(string[] args)
    {
        string connectionstring = @"Data Source=.\SQLEXPRESS;Initial Catalog=VoedselvoorraadDb;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        IGerechtRepository gerechtRepo = new GerechtRepository(connectionstring);
        DomeinController dc = new DomeinController(gerechtRepo);

        //Gerecht gerecht = dc.GetGerechtByNaam("Pasta Carbonara");

        //Console.WriteLine(gerecht.Naam, gerecht.Id, gerecht.FotoUrl);

        //foreach(var i in gerecht.Ingredienten)
        //{
        //    Console.WriteLine(i.Key.Id.ToString() + i.Key.Eenheid + i.Key.Naam + i.Value);
        //}

        //List<Gerecht> gerechten = dc.GetAlleGerechten();
        //foreach(Gerecht gerecht in gerechten)
        //{
        //    Console.WriteLine(gerecht.Naam, gerecht.Id, gerecht.FotoUrl);

        //    foreach (var i in gerecht.Ingredienten)
        //    {
        //        Console.WriteLine(i.Key.Id.ToString() + i.Key.Eenheid + i.Key.Naam + i.Value);
        //    }
        //}
        //string csvFilePath = "path_to_your_csv_file.csv";

        //var config = new CsvConfiguration(CultureInfo.InvariantCulture) {
        //    Delimiter = ",",
        //    HasHeaderRecord = true
        //};

        //using (var reader = new StreamReader(csvFilePath))
        //using (var csv = new CsvReader(reader, config))
        //{
        //    var records = csv.GetRecords<Gerecht>().ToList();
        //    foreach(Gerecht record in records)
        //    {
        //        dc.InsertGerecht(record);
        //    }
        //}

        Gerecht g = new Gerecht();
        g.Naam = "TestInsert";
        g.FotoUrl = "haha";
        g.Ingredienten = new Dictionary<VoedselItem, double> { { new VoedselItem { Eenheid = "g", Naam="paprika", VoedselGroep="groente" }, 500 },
                                                                { new VoedselItem { Eenheid = "ml", Naam="melk", VoedselGroep="zuivel" }, 500 }  };


        dc.InsertGerecht(g);
    }
    

}