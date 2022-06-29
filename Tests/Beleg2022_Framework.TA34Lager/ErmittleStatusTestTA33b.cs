using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Beleg2022;
using System.Reflection;


namespace Beleg2022.Tests
{
    [TestFixture]
    public class ErmittleStatusTestTA33b
    {
        [Test]
        public void Test_ErmittleStatus_InvokeOnNewlyCreatedLager_EMPFANGSBEREIT()
        {
            //Arrange
            //Act
            // Das geht natürlich nur wenn die Lager irgendwie da sind ... sollten sie aber an der Stelle
            Ausgangslager l = new Ausgangslager("TestAusgangslager", "Somewhere");

            //Assert
            if (l.GetStatus() == Status.EMPFANGSBEREIT)
            {
                Console.WriteLine("An dieser Stelle ist der Status wie erwartet");
                Assert.Pass("An dieser Stelle ist der Status wie erwartet");
            }else
            {
                Console.WriteLine("Nach dem Anlegen der Lager stimmt etwas mit dem erwarteten Status nicht.");
                Assert.Fail("Nach dem Anlegen der Lager stimmt etwas mit dem erwarteten Status nicht.");

            }
        }

        [Test]
        public void Test_ErmittleStatus_InvokeOnNewlyCreatedLager_InternalStatusEQAktuellerStatus()
        {
            // Arrange
            Lager l = new Ausgangslager("TestAusgangslager", "Somewhere");
            // Act
            Status lStatus = l.GetStatus();
            Status lIStatus = (Status)(l.GetType().
                GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(l) ??
                                       throw new InvalidOperationException("Neither AktuellerStatus nor its Field info should be null at this point."));
            // Assert
            if (lIStatus != lStatus)
            {
                Console.WriteLine("Das hätte nicht passieren dürfen. Der Status von _AktuellerStatus und Laqer.GetStatus() sind nicht identisch");
                Assert.Fail("Das hätte nicht passieren dürfen. Der Status von _AktuellerStatus und Lager.GetStatus() sind nicht identisch");
            }
            else
            {
                Console.WriteLine("Alles ok");
                Assert.Pass("Alles ok");
            }
        }
    }
}
