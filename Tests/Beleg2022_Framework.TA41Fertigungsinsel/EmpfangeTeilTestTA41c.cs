using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Beleg2022.Tests
{
    [TestFixture]
    public class EmpfangeTeilTestTa41C : TestsTa41Base
    {
        [Test]
        public void Test_EmpfangeTeil_StatusNotEMPFANGSBEREIT_ReturnFALSE()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.BELEGT);
            ForceTeil(ref MyFertigungsinsel, TeilNextIsValid);
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(30));
            // Act
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            bool ret = MyFertigungsinsel.EmpfangeTeil(TeilNextIsValid);
            // Assert
            if (!ret)
            {
                Console.WriteLine("Test bestanden");
                Assert.Pass("Test bestanden");
            }
            else
            {
                Console.WriteLine("Der Rückgabewert ist unerwartet");
                Assert.Fail("Der Rückgabewert ist unerwartet");
            }
        }

        /// um den nicht zu bestehen muss man schon nahezu absichtlich mist bauen
        [Test]
        public void Test_EmpfangeTeil_StatusNotEMPFANGSBEREIT_AktuellesTeilNull()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.BELEGT);
            ForceTeil(ref MyFertigungsinsel, TeilNextIsNotValid);
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(30));
            // Act
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            MyFertigungsinsel.EmpfangeTeil(TeilNextIsValid);
            Teil t = MyFertigungsinselAktuellesTeilFi?.GetValue(MyFertigungsinsel) as Teil ??
                     throw new InvalidOperationException(
                         "If you see this Exception, you likely messed with the NUnit Setup method or previous tests failed.");
            // Assert
            if (t.Equals(TeilNextIsNotValid))
            {
                Console.WriteLine("Test bestanden");
                Assert.Pass("Test bestanden");
            }
            else
            {
                Console.WriteLine("Sie haben ein Teil übernommen, obwohl der Status bereits BELEGT war.");
                Assert.Fail("Sie haben ein Teil übernommen, obwohl der Status bereits BELEGT war.");
            }
        }

        [Test]
        public void Test_EmpfangeTeil_ParameterNull_ReturnFALSE()
        {
            // Arrange
            Teil? t = null;
            // Act
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            bool ret = MyFertigungsinsel.EmpfangeTeil(t);
            if (!ret)
            {
                Console.WriteLine("Test bestanden");
                Assert.Pass("Test bestanden");
            }
            else
            {
                Console.WriteLine("Der Rückgabewert ist unerwartet");
                Assert.Fail("Der Rückgabewert ist unerwartet");
            }
        }

        /// um den nicht zu bestehen muss man schon nahezu absichtlich mist bauen, z.B. ... an den Haaren herbeigezogen 
        [Test]
        public void Test_EmpfangeTeil_ParameterNull_AktuellesTeilNull()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.EMPFANGSBEREIT);
            Teil? teilIn = null;
            // Act
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            MyFertigungsinsel.EmpfangeTeil(teilIn);
            // Assert
            if (MyFertigungsinselAktuellesTeilFi?.GetValue(MyFertigungsinsel) is not Teil)
            {
                Console.WriteLine("Test bestanden");
                Assert.Pass("Test bestanden");
            }
            else
            {
                Console.WriteLine("Was auch immer Sie getan haben, das aktuelle Teil hätte null sein müssen. Es schlagen vermutlich auch Tests bei GetTeil() fehl!");
                Assert.Fail("Was auch immer Sie getan haben, das aktuelle Teil hätte null sein müssen. Es schlagen vermutlich auch Tests bei GetTeil() fehl!");
            }
        }

        [Test]
        public void Test_EmpfangeTeil_NaechterSchrittNotInFaehigkeiten_Returnfalse()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.EMPFANGSBEREIT);
            // Act
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            bool ret = MyFertigungsinsel.EmpfangeTeil(TeilNextIsNotValid);
            // Assert
            if (!ret)
            {
                Console.WriteLine("Test bestanden");
                Assert.Pass("Test bestanden");
            }
            else
            {
                Console.WriteLine("Der Rückgabewert ist unerwartet");
                Assert.Fail("Der Rückgabewert ist unerwartet");
            }

        }

        [Test]
        public void Test_EmpfangeTeil_NaechterSchrittNotInFaehigkeiten_AktuellesTeilNull()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.EMPFANGSBEREIT);
            // Act
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            MyFertigungsinsel.EmpfangeTeil(TeilNextIsNotValid);
            // Assert
            if (MyFertigungsinselAktuellesTeilFi?.GetValue(MyFertigungsinsel) is not Teil)
            {
                Console.WriteLine("Test bestanden");
                Assert.Pass("Test bestanden");
            }
            else
            {
                Console.WriteLine("Sie haben ein Teil übernommen, obwohl das Teil nicht bearbeitet werden kann.");
                Assert.Fail("Sie haben ein Teil übernommen, obwohl das Teil nicht bearbeitet werden kann.");
            }
        }
        
        [Test]
        public void Test_EmpfangeTeil_ValidInvoke_AktuellesTeilNotNull()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.EMPFANGSBEREIT);
            // Act
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            MyFertigungsinsel.EmpfangeTeil(TeilNextIsValid);
            // Assert
            if (MyFertigungsinselAktuellesTeilFi?.GetValue(MyFertigungsinsel) is Teil t && t.Equals(TeilNextIsValid))
            {
                Console.WriteLine("Test bestanden");
                Assert.Pass("Test bestanden");
            }
            else
            {
                Console.WriteLine("Sie haben ein Teil übernommen, dieses ist danach jedoch nicht das aktuelle Teil.");
                Assert.Fail("Sie haben ein Teil übernommen, dieses ist danach jedoch nicht das aktuelle Teil.");
            }
        }

        [Test]
        public void Test_EmpfangeTeil_ValidInvoke_StatusBELEGT()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.EMPFANGSBEREIT);
            // Act
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            MyFertigungsinsel.EmpfangeTeil(TeilNextIsValid);
            //Status s = (Status)MyFertigungsinselAktuellerStatusFi?.GetValue(MyFertigungsinsel);
            // Assert
            //if (s == Status.BELEGT)
            if (MyFertigungsinselAktuellerStatusFi?.GetValue(MyFertigungsinsel) is Status.BELEGT)
            {
                Console.WriteLine("Test bestanden");
                Assert.Pass("Test bestanden");
            }
            else
            {
                Console.WriteLine("Sie haben ein Teil übernommen, Danach ist der Status allerdings nicht BELEGT.");
                Assert.Fail("Sie haben ein Teil übernommen, Danach ist der Status allerdings nicht BELEGT.");
            }
        }

        ///Bewertungstest
        [Test]
        public void Test_EmpfangeTeil_ValidInvoke_BearbeiteTeilInvokedONCE()
        {
            // Arrange          
            
            DateTime now = DateTime.Now;
            // Act
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            MyFertigungsinsel.EmpfangeTeil(TeilNextIsValid);
            DateTime myFertigungsinselBelegtBis = (DateTime)(MyFertigungsinselBelegtBisFi?.GetValue(MyFertigungsinsel) ?? DateTime.MinValue);
            //Assert
            TimeSpan difftime = myFertigungsinselBelegtBis - now;
            if (difftime.Seconds is >= 10 and < 20)
            {
                Console.WriteLine("Es sieht so aus, als würde sich die Methode so verhalten wie erwartet.");
                Assert.Pass("Es sieht so aus, als würde sich die Methode so verhalten wie erwartet.");
            }
            else
            {
                Console.WriteLine("Haben Sie die Aufgabestellung komplett umgesetzt ? Es sieht so aus, als würden Sie die Bearbeitung nicht starten.");
                Assert.Fail("Haben Sie die Aufgabestellung komplett umgesetzt? Es sieht so aus, als würden Sie die Bearbeitung nicht starten.");
            }
        }
    }
}
