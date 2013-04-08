using System.Windows;
using Barricade.Logic;
using Barricade.Logic.Velden;

namespace Barricade.Presentation.Statisch
{


	public interface IElement 
	{
        UIElement BerekenPunt(Logic.Pion pion);
        UIElement BerekenPunt(Logic.Barricade barricade);
	    void WisselLicht(bool status);
	    IVeld Veld { get; }
	}
}

