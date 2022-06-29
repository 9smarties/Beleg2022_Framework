using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

namespace Beleg2022.Tests
{
    /// <summary>
    /// Ab dieser Teilaufgabe wird davon ausgegangen, das alle
    /// Klassen wie erwartet vorhanden sind. Wenn Sie die Tests
    /// also im VisualStudio laufen lassen, werden diese nicht 
    /// starten, solange die vorangegangegen Tests nicht bestanden
    /// wurden. Die GitLab CI Pipeline sorgt automatishc dafür, 
    /// das diese Tests hier und alle weiteren, nur dann ausgeführt
    /// werden, wenn die vorangegangenen erfolgreich waren.
    /// 
    /// Ab hier werden sie Tests nach dem Arrange-Act-Assert Schema
    /// organisiert sein. 
    /// </summary>
    [TestFixture]
    public class CSVParseTestTA34a
    {
        /// <summary>
        /// Da davon ausgegangen wird, das die Aufgaben in der vorgegebenen Reihenfolge bearbeitet werden
        /// und die Konstruktoren bereits implementiert sind (für die korrekte Implementierung der Konstruktoren
        /// haben Sie keine UnitTests erhalten!)
        /// </summary>
        [Test]
        //Wenn hier exakt in der Reihenfolge der Aufgabenstellung implementiert wurde,
        //kann es sein, das der Test fehlschlägt auch wenn das Parsing grundsätzlich funktioniert
        //das ist z.B. dann so, wenn der _Bestand über den Setter des base.Lagers bestückt wird
        public void Test_Parser_InvokeInitialisiereBestand_EmptyPartsCreatedInBestandAsExpected()
        {
            // Arrange
            // Wenn das fehlschlägt stimmt die Vererbung nicht, bzw. es wurde Code aus dem Vorgabeprojekt entfernt
            // in allen anderen Fällen sollte der vorgegebene Base Konstruktor hier greifen
            Eingangslager eingangslager = new Eingangslager("EingangslagerName", "XYZ");
            // brauchen wir wahrscheinlich nicht, aber man weiß nie, auf was so alles zugegriffen wird.
            eingangslager.SetFaehigkeiten(new List<Verarbeitungsschritt> { Verarbeitungsschritt.INITIALISIEREN });

            //Act
            IEnumerable<Teil> el = eingangslager.GetType().GetField("_Bestand", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(eingangslager) as IEnumerable<Teil> ?? throw new InvalidOperationException(
                "Neither Bestand nor its Field info should be null at this point. Likely previous Tests already failed ");

            //Assert
            // TODO ... tja, was wollen wir hier eigentlich testen
            // der Test ist aktuelle recht billig, hat aber den Vorteil,
            // das es schon früh grün wird. Sobald hier auf den Inhalt
            // der Liste geprüft werden soll geht das nicht mehr so einfach,
            // denn die könnte einfach leere Elemente haben, z.B. wenn der Konstruktor
            // vom Teil einfach nichts sinnvolles macht (nur Signatur angelegt). 
            if(el.Count() == 9)
            {
                Console.WriteLine(" Der Test verlief wie erwartet.");
                Assert.Pass(" Der Test verlief wie erwartet.");
            }
            else
            {
                Assert.Fail("Wenn dieser Test fehlschlägt wird Ihr Bestand beim Anlegen des Eingangslagers" +
                    " nicht korrekt angelegt. Das kann z.B. daran liegen, das Sie die entsprechende Methode" +
                    " nicht implementiert haben oder auch einfach daran, dass Sie die Methode nie aufrufen.");
            }
        }

        [Test]
        public void Test_Parser_DirectlyInvokeInitialisiereBestand_BestandEQAmountOfTeilInOtherCSV()
        {
            // Arrange
            string pfad = @"../../../Eingangslager2.csv";
            // Wenn das fehlschlägt stimmt die Vererbung nicht, bzw. es wurde Code aus dem Vorgabeprojekt entfernt
            // in allen anderen Fällen sollte der vorgegebene Base Konstruktor hier greifen
            Eingangslager eingangslager = new Eingangslager("EingangslagerName", "XYZ");
            // brauchen wir wahrscheinlich nicht, aber man weiß nie, auf was so alles zugegriffen wird.
            eingangslager.SetFaehigkeiten(new List<Verarbeitungsschritt> { Verarbeitungsschritt.INITIALISIEREN });

            var bestandEingangslagerFI = eingangslager.GetType()
                .GetField("_Bestand", BindingFlags.Instance | BindingFlags.NonPublic);
            bestandEingangslagerFI?.SetValue(eingangslager
                , (Activator.CreateInstance(bestandEingangslagerFI.FieldType) as IEnumerable<Teil>));
            //Act

            MethodInfo refInitBestand = eingangslager.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.Name == "InitialisiereBestand")
                .Where(m => m.GetParameters().Count() == 1)
                .FirstOrDefault(m => m.GetParameters()[0].ParameterType == typeof(string)) ?? throw new InvalidOperationException(
                "If this Exception occurs, other tests already failed, there is no suitable InitialisiereBestand method in Eingangslager");
            refInitBestand.Invoke(eingangslager, new object[] { pfad });

            IEnumerable<Teil> el = eingangslager.GetType()
                .GetField("_Bestand", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(eingangslager) as IEnumerable<Teil> ??
                            throw new InvalidOperationException(
                                "If this fails previous Tests failed already, an Bestand is missing from Lager");
            //Assert
            if(el.Count() == 18)
            {
                Console.WriteLine(" Der Test verlief wie erwartet.");
                Assert.Pass(" Der Test verlief wie erwartet.");
            }
            else
            {
                Assert.Fail("Wenn diese Methode fehlschlägt bedeutet möglicherweise:\n" +
                    "- Sie haben zwar einen Pfad übergeben, nutzen diesen aber nicht\n" +
                    "- Sie laufen nicht wirklich durch das CSV-File sondern legen Objekte manuell an\n" +
                    "- Sie haben etwas konstruiert, das abseits des EKD liegt und so nicht vorhergesehen " +
                    "war, in dem Fall können Sie diesen Fehler ignorieren, wenn Sie wissen was Sie tun.");
            }

        }
        [Test]
        public void Test_Parser_DirectlyInvokeInitialisiereBestand_StatusEQABHOLBEREITorINTERAKTIONSBEREIT()
        {
            // Arrange
            string pfad = @"../../../Eingangslager2.csv";
            // Wenn das fehlschlägt stimmt die Vererbung nicht, bzw. es wurde Code aus dem Vorgabeprojekt entfernt
            // in allen anderen Fällen sollte der vorgegebene Base Konstruktor hier greifen
            Eingangslager eingangslager = new Eingangslager("EingangslagerName", "XYZ");
            // brauchen wir wahrscheinlich nicht, aber man weiß nie, auf was so alles zugegriffen wird.
            eingangslager.SetFaehigkeiten(new List<Verarbeitungsschritt> { Verarbeitungsschritt.INITIALISIEREN });
            // Die Passende Methode holen, die einen Sting nimmt

            //Act

            MethodInfo refInitBestand = eingangslager.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.Name == "InitialisiereBestand")
                .Where(m => m.GetParameters().Count() == 1)
                .FirstOrDefault(m => m.GetParameters()[0].ParameterType == typeof(string)) ?? throw new InvalidOperationException(
                "If this Exception occurs, other tests already failed, there is no suitable InitialisiereBestand method in Eingangslager");
            refInitBestand.Invoke(eingangslager, new object[] { pfad });

            Status s = (Status)(eingangslager.GetType()
                .GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(eingangslager) ??
                                 throw new InvalidOperationException(
                                     "Neither AktuellerStatus nor its Field info should be null at this point"));
            //Assert
            if(s is Status.INTERAKTIONSBEREIT or Status.ABHOLBEREIT)
            {
                Console.WriteLine(" Der Test verlief wie erwartet.");
                Assert.Pass(" Der Test verlief wie erwartet.");
            }
            else
            {
                Assert.Fail("Nach dem Initialisieren, sollte der Status nicht " + s + " sein.");
            }
        }
    }
}
