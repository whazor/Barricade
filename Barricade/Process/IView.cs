using System.Collections.Generic;
using System.Threading.Tasks;
using Barricade.Logic;
using Barricade.Logic.Velden;

namespace Barricade.Process
{
    public interface IView
    {
        void Highlight(IEnumerable<Pion> pionnen, bool status);
        void Highlight(IEnumerable<IVeld> mogelijk, bool status);
        void Klem(Logic.Barricade barricade, bool status);

        Task Wacht(int wachttijdBot);
        int Gedobbeld { get; set; }
    }
}