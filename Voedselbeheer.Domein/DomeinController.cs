using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
using Voedselbeheer.Domein.Interface;

namespace Voedselbeheer.Domein
{
    public class DomeinController(IGerechtRepository gerechtRepo)
    {
        private IGerechtRepository _gerechtRepo { get; } = gerechtRepo;


=======

namespace Voedselbeheer.Domein
{
    internal class DomeinController
    {
>>>>>>> branch_iva
    }
}
