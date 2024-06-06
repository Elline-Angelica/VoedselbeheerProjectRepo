using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voedselbeheer.Domein.Models
{
    public class VoorraadItem
    {
        public VoedselItem Item { get; set; }
        public double Hoeveelheid { get; set; }
        public DateTime DatumInVoorraad {  get; set; }
    }
}
