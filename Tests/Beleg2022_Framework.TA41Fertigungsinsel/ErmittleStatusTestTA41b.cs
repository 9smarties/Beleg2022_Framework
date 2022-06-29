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
    public class ErmittleStatusTestTa41B : TestsTa41Base
    {
        [Test]
        public void Test_ErmittleStatus_AfterInselCreation_EMPFANGSBEREIT()
        {
            // Arrange// Act 
            // Assert 
            if (MyFertigungsinselAktuellerStatusFi?.GetValue(MyFertigungsinsel) is Status.EMPFANGSBEREIT)
            {
                Console.WriteLine("Bestanden");
                Assert.Pass("Bestanden");
            }
            else
            {
                Console.WriteLine("Nach dem Anlegen der Fertigungsinsel sollte der Status, EMPFANGSBEREIT sein.");
                Assert.Fail("Nach dem Anlegen der Fertigungsinsel sollte der Status, EMPFANGSBEREIT sein.");
            };
        }

        [Test]
        public void Test_ErmittleStatus_DuringProcessing_BELEGT()
        {
            // Arrange
            ForceTeil(ref MyFertigungsinsel, TeilNextIsValid);
            ForceStatus(ref MyFertigungsinsel, Status.BELEGT);
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(10));
            // Act 
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            Status s = MyFertigungsinsel.ErmittleStatus();
            // Assert
            if (s == Status.BELEGT)
            {
                Console.WriteLine("Bestanden");
                Assert.Pass("Bestanden");
            }
            else
            {
                Console.WriteLine("Der Status sollte zu diesem Punkt BELEGT sein.");
                Assert.Fail("Der Status sollte zu diesem Punkt BELEGT sein."); 
            }
        }

        [Test]
        public void Test_ErmittleStatus_ProcessingFinishedNextStepNotPossible_ABHOLBEREIT()
        {
            // Arrange
            ForceTeil(ref MyFertigungsinsel, TeilNoneValid);
            ForceStatus(ref MyFertigungsinsel, Status.BELEGT);
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(-50));
            // Act 
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            Status s = MyFertigungsinsel.ErmittleStatus();
            // Assert
            if (s == Status.ABHOLBEREIT)
            {
                Console.WriteLine("Bestanden");
                Assert.Pass("Bestanden");
            }
            else
            {
                Console.WriteLine("Der Status sollte zu diesem Punkt ABHOLBEREIT sein.");
                Assert.Fail("Der Status sollte zu diesem Punkt ABHOLBEREIT sein.");
            }
        }

        [Test]
        public void Test_ErmittleStatus_ProcessingFinishedNextStepPossible_BELEGT()
        {
            // Arrange
            ForceTeil(ref MyFertigungsinsel, TeilNextIsNotValid); // die Namen sind nicht optimal, der nächste
                                                                  // geht zwar nicht, der wird ja aber auch
                                                                  // entfernt bevor geprüft werden soll, der
                                                                  // übernächste geht wieder bei dme Teil
            ForceStatus(ref MyFertigungsinsel, Status.BELEGT);
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(-50));
            // Act 
            if (MyFertigungsinsel is null) throw new InvalidOperationException("If you see this Exception, you likely messed with the NUnit Setup method.");
            Status s = MyFertigungsinsel.ErmittleStatus();
            // Assert
            if (s == Status.BELEGT)
            {
                Console.WriteLine("Bestanden");
                Assert.Pass("Bestanden");
            }
            else
            {
                Console.WriteLine("Der Status sollte zu diesem Punkt wieder BELEGT sein.");
                Assert.Fail("Der Status sollte zu diesem Punkt wieder BELEGT sein.");
            }
        }
    }
}
