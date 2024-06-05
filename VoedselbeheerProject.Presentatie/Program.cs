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
    }
}