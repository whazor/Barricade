using System.Collections.Generic;
using System.Threading.Tasks;
using Barricade.Logic;
using Barricade.Logic.Velden;

namespace Barricade.Process
{
    public interface IView
    {
        Task Wacht(int wachttijdBot);
        int Gedobbeld { get; set; }
    }
}