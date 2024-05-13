using Voedselbeheer.Domein.Dto_s;

namespace Voedselbeheer.Domein.Interface;

public interface IRepository
{
    // CRUD operaties
    GerechtDto GetById(Guid id);
    IEnumerable<GerechtDto> GetAll();
    void Insert(GerechtDto gerecht);
    void Update(GerechtDto gerecht);
    void Delete(Guid id);
}