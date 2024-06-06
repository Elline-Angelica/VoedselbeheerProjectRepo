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
        Gerecht GetById(int id);
        List<Gerecht> GetAll();
        void Insert(Gerecht gerecht);
        void Update(Gerecht gerecht, Gerecht nieuwGerecht);
        void Delete(int id);
        Gerecht GetByName(string naam);
        VoedselItem GetVoedselItemByName(string v);
    }
}
