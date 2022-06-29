using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beleg2022
{
    public abstract class Lager
    {
        protected Queue<Teil> _Bestand = new Queue<Teil>();
        protected static int _Kapazitaet = 20;


       
    }
}
