using System;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Barricade.Testing
{
    [TestClass]
    public class DataTester
    {
        [TestMethod]
        public void LoadGame()
        {
            new Loader();

        }
    }
}
