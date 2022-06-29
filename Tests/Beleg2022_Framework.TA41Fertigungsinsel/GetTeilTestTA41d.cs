using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Beleg2022.Tests
{
    [TestFixture]
    public class GetTeilTestTa41d: TestsTa41Base
    {
        [Test]
        public void Test_GetTeil_AktuellesTeilNull_NullTeil()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.ABHOLBEREIT); // das sollte in der Kombi grundsätzlich nicht
                                                                    // vorkommen (abolbereit und null) und durch
                                                                    // andere Tests behandelt werden,
                                                                    // aber für den Fall, so wird eine Meldung erzeugt.
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(-12));
            ForceTeil(ref MyFertigungsinsel, null);
            // Act
            Teil? t = MyFertigungsinsel?.GetTeil();
            // Assert
            if (t is null)
            {
                Console.WriteLine("Bestanden");
                Assert.Pass("Bestanden");
            }
            else
            {
                Console.WriteLine("Wenn dieser Test fehlschlägt kann das mehrere Ursachen haben. Vergewissern Sie " +
                    "sich, das sie im ErmittleStatus und im GetTeil keine sinnlosen Variablenbelegungen zulassen");
                Assert.Fail("Wenn dieser Test fehlschlägt kann das mehrere Ursachen haben. Vergewissern Sie " +
                    "sich, das sie im ErmittleStatus und im GetTeil keine sinnlosen Variablenbelegungen zulassen");
            }
        }
        
        [Test]
        public void Test_GetTeil_AktuellesTeiNotlNull_Teil()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.ABHOLBEREIT);
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(-12));
            ForceTeil(ref MyFertigungsinsel, TeilNoneValid);
            // Act
            Teil? t = MyFertigungsinsel?.GetTeil();
            // Assert
            if (t is { } && t.Equals(TeilNoneValid)) // hier kann man nicht auf Teilgleichheit prüfen um 100% abzudecken, mal sehen ob es knallt
            {
                Console.WriteLine("Bestanden");
                Assert.Pass("Bestanden");
            }
            else
            {
                Console.WriteLine("Nachdem Sie ein Teil zurückgeben, sollte das aktuelle Teil null sein.");
                Assert.Fail("Nachdem Sie ein Teil zurückgeben, sollte das aktuelle Teil null sein.");
            }
        }

        [Test]
        public void Test_GetTeil_AktuellesTeiNotlNull_StatusEMPFANGSBEREIT()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.ABHOLBEREIT);
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(-12));
            ForceTeil(ref MyFertigungsinsel, TeilNoneValid);
            // Act
            MyFertigungsinsel?.GetTeil();
            // Assert
            Status sAfter = (Status)(MyFertigungsinselAktuellerStatusFi?.GetValue(MyFertigungsinsel) ?? throw new InvalidOperationException("Neither Status nor its Field info should be empty at this time."));
            if (sAfter is Status.EMPFANGSBEREIT)
            {
                Console.WriteLine("Bestanden");
                Assert.Pass("Bestanden");
            }
            else
            {
                Console.WriteLine("Nachdem Sie ein Teil zurückgeben, sollte der Status wieder EMPFANGSBEREIT sein.");
                Assert.Fail("Nachdem Sie ein Teil zurückgeben, sollte der Status wieder EMPFANGSBEREIT sein.");
            }
        }

        [Test]
        public void Test_GetTeil_AktuellesTeiNotlNull_AktuellesTeilNull()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.ABHOLBEREIT);
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(-12));
            ForceTeil(ref MyFertigungsinsel, TeilNoneValid);
            // Act
            MyFertigungsinsel?.GetTeil();
            // Assert

            if (MyFertigungsinselAktuellesTeilFi?.GetValue(MyFertigungsinsel) is not Teil)
            {
                Console.WriteLine("Bestanden");
                Assert.Pass("Bestanden");
            }
            else
            {
                Console.WriteLine("Nachdem Sie ein Teil zurückgeben, sollte das aktuelle Teil null sein.");
                Assert.Fail("Nachdem Sie ein Teil zurückgeben, sollte das aktuelle Teil null sein.");
            }
        }

        [Test]
        public void Test_GetTeil_AktuellerStatusNotAbholbereit_NullTeil()
        {
            // Arrange
            ForceStatus(ref MyFertigungsinsel, Status.BELEGT);
            ForceBelegtBis(ref MyFertigungsinsel, DateTime.Now.AddSeconds(12));
            ForceTeil(ref MyFertigungsinsel, TeilNextIsValid);
            // Act
            Teil? t = MyFertigungsinsel?.GetTeil();
            // wenn jemand an den TestTeilen spielt kann der Test hier brechen
            // Assert
            if (t is { })
            {
                Console.WriteLine("Auf einer Nicht Abholbereiten Fertigungsinsel sollten Sie nichts abholen.");
                Assert.Fail("Auf einer Nicht Abholbereiten Fertigungsinsel sollten Sie nichts abholen.");
            }else
            {
                Console.WriteLine("Bestanden");
                Assert.Pass("Bestanden");
            }
        }
    }
}
