using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voedselbeheer.Domein.Interface;

namespace Voedselbeheer.Domein;

public class DomeinController(IGerechtRepository gerechtRepo)
{
    private IGerechtRepository _gerechtRepo { get; } = gerechtRepo;

    public List<Gerecht> GetAlleGerechten()
    {
        return _gerechtRepo.GetAll();
    }

    public Gerecht GetGerechtById(int id)
    {
        Gerecht gerecht = _gerechtRepo.GetById(id);

        return gerecht;
    }

    public Gerecht GetGerechtByNaam(string naam)
    {
        Gerecht gerecht = _gerechtRepo.GetByName(naam);
        return gerecht;
    }

    public VoedselItem GetVoedselItemByNaam(string v)
    {
        return _gerechtRepo.GetVoedselItemByName(v);
    }

    public void InsertGerecht(Gerecht record)
    {
        _gerechtRepo.Insert(record);
    }

    public void UpdateGerecht(Gerecht gerecht, Gerecht nieuwGerecht)
    {
        _gerechtRepo.Update(gerecht, nieuwGerecht);
    }
}


