using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Barricade.Data;
using Barricade.Logic;

namespace Barricade.Shell
{
    class Drawer
    {
        private readonly Loader _loader;
        readonly Saver _saver;
        public Drawer(Loader loader)
        {
            _loader = loader;
            _saver = new Saver(loader.Kaart);
        }

        public void Start()
        {
            foreach (var veld in _loader.Spel.Spelers.SelectMany(speler => speler.Pionnen))
            {
                veld.PositieWijziging += (a, b) => Draw();
            }
            Draw();
        }

        public void Draw()
        {
//            while (true)
//            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine(_saver.Output(true));
//                Thread.Sleep(100);
//            }
        }
    }
}
