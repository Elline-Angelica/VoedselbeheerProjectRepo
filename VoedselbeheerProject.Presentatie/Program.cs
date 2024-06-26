﻿using CsvHelper;
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

        //Gerecht gerecht = dc.GetGerechtByNaam("Spaghetti Bolognese");

        //Console.WriteLine(gerecht.Naam, gerecht.Id, gerecht.FotoUrl);

        //foreach (var i in gerecht.Ingredienten)
        //{
        //    Console.WriteLine(i.Key.Id.ToString() + i.Key.Eenheid + i.Key.Naam + i.Value);
        //}

        //List<Gerecht> gerechten = dc.GetAlleGerechten();
        //foreach (Gerecht gerecht in gerechten)
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
        g.Naam = "new testinsert";
        g.FotoUrl = "no";
        g.Ingredienten = new Dictionary<VoedselItem, double> { { new VoedselItem { Eenheid = "g", Naam="paprika", VoedselGroep="groente" }, 500 },
                                                                { new VoedselItem { Eenheid = "ml", Naam="melk", VoedselGroep="zuivel" }, 500 }  };


        dc.InsertGerecht(g);
        //Gerecht g = dc.GetGerechtById(1);
        //g.Naam = "nieuweTestnaam";

        //dc.UpdateGerecht(dc.GetGerechtById(1), g);

        //VoedselItem vi = dc.GetVoedselItemByNaam("paprika");
        //List<VoedselItem> test = dc.GetAllVoedselItems();

        //VoedselItem vi = new VoedselItem() { Eenheid = "bla", Naam = "geenzin", VoedselGroep = "ongezond", FotoUrl = "" };
        //dc.InsertVoedselItem(vi);
        //var test = dc.GetVoorraad();
        //var test = dc.GetVoedselItemsOuderDan3Maand();
        //List<VoedselItem> ingredienten = dc.GetAllVoedselItems().Where(v => v.Naam == "paprika" || v.Naam.ToLower() == "gehakt").ToList();
        //var test = dc.GetGerechtenByIngredienten(ingredienten);

        var newTest = dc.GetGerechtenMetIngredientenInVoorraadOuderDan3Maand();
    }
    

}