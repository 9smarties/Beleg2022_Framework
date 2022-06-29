using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beleg2022
{
    public class Teil
    {
        private string _Id;
        private List<Verarbeitungsschritt> _Rezept;


        public static void Main(string[] args)
        {

        }
        public Teil(string id, List<Verarbeitungsschritt> rezept)
        {
            this._Id = id;
            this._Rezept = rezept;
        }

        public Verarbeitungsschritt GetNaechstenSchritt()
        {

            return this._Rezept[0];
        }

        public void EntferneFertigenSchritt()
        {
            this._Rezept.Remove(0);
            Console.WriteLine(_Rezept);
        }

        public string GetId()
        {
            return this._Id;
        }
    }
}
