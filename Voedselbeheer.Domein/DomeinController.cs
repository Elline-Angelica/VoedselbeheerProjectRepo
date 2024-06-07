using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voedselbeheer.Domein.Dto_s;
using Voedselbeheer.Domein.Interface;
using Voedselbeheer.Domein.Models;

namespace Voedselbeheer.Domein;

public class DomeinController(IGerechtRepository gerechtRepo)
{
    private IGerechtRepository _gerechtRepo { get; } = gerechtRepo;

    public List<Gerecht> GetAlleGerechten()
    {
        return _gerechtRepo.GetAllGerechten();
    }

    public List<VoedselItem> GetAllVoedselItems()
    {
        return _gerechtRepo.GetAllVoedselItems();
    }

    public Gerecht GetGerechtById(int id)
    {
        Gerecht gerecht = _gerechtRepo.GetGerechtById(id);

        return gerecht;
    }

    public Gerecht GetGerechtByNaam(string naam)
    {
        Gerecht gerecht = _gerechtRepo.GetGerechtByName(naam);
        return gerecht;
    }

    public VoedselItem GetVoedselItemByNaam(string v)
    {
        return _gerechtRepo.GetVoedselItemByName(v);
    }

    public void InsertGerecht(Gerecht record)
    {
        _gerechtRepo.InsertGerecht(record);
    }

    public void InsertVoedselItem(VoedselItem vi)
    {
        _gerechtRepo.InsertVoedselItem(vi);
    }

    public void UpdateGerecht(Gerecht gerecht, Gerecht nieuwGerecht)
    {
        _gerechtRepo.UpdateGerecht(gerecht, nieuwGerecht);
    }
    public List<Gerecht> GetGerechtenByIngredienten(List<VoedselItem> ingredienten)
    {
        return _gerechtRepo.GetGerechtenByIngredienten(ingredienten);
    }
    //logica nog niet in orde
    public List<Gerecht> GetGerechtenMetIngredientenInVoorraadOuderDan3Maand()
    {
        List<VoorraadItem> ouderDan3Maand = GetVoedselItemsOuderDan3Maand();
        List<Gerecht> gerechten = new();
        List<VoedselItem> ingredienten = new();
        HashSet<int> gebruikteGerechtIds = new HashSet<int>();
        foreach (VoorraadItem vi in ouderDan3Maand)
        {
            List<Gerecht> gevondenGerechten = _gerechtRepo.GetGerechtenByIngredienten(new List<VoedselItem> { vi.Item });

            foreach (Gerecht gerecht in gevondenGerechten)
            {
                if (!gebruikteGerechtIds.Contains(gerecht.Id))
                {
                    gerechten.Add(gerecht);
                    gebruikteGerechtIds.Add(gerecht.Id);
                }
            }
        }
        return gerechten;
    }
    public List<VoorraadItem> GetVoedselItemsOuderDan3Maand()
    {
        List<VoorraadItem> voorraad = GetVoorraad();
        DateTime now = DateTime.Now;
        List<VoorraadItem> ouderDan3Maand = voorraad.Where(v => v.DatumInVoorraad.AddMonths(3) < now).ToList();
        return ouderDan3Maand;
    }

    public List<VoorraadItem> GetVoorraad()
    {
        return _gerechtRepo.GetAllVoedselItemsInVoorraad();
    }
}


