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
        GerechtDto GetById(Guid id);
        IEnumerable<GerechtDto> GetAll();
        void Insert(GerechtDto gerecht);
        void Update(GerechtDto gerecht);
        void Delete(Guid id);
    }
}
