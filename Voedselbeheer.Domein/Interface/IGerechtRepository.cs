using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voedselbeheer.Domein.Dto_s;

namespace Voedselbeheer.Domein.Interface
{
    public interface IGerechtRepository
    {
        // CRUD operaties
        Gerecht GetGerechtById(int id);
        List<Gerecht> GetAllGerechten();
        void InsertGerecht(Gerecht gerecht);
        void UpdateGerecht(Gerecht gerecht, Gerecht nieuwGerecht);
        void Delete(int id);
        Gerecht GetGerechtByName(string naam);
        VoedselItem GetVoedselItemByName(string v);
        List<VoedselItem> GetAllVoedselItems();
        void InsertVoedselItem(VoedselItem vi);
    }
}
