using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Reflection;

namespace Beleg2022.Tests
{
    [TestFixture]
    public class ConstructorTestTa31B : TestTa31Base
    {
        // Einfache Arrays wobei jeder Eintrag einen Konstruktor bedingt, jeweils mit der genannten Anzahl an Parametern
        private readonly Tuple<String, int[]> _lagerNumberOfConstructorParam = new("Lager", new int[] { 2 });
        private readonly Tuple<String, int[]> _fertigungsinselNumberOfConstructorParam = new("Fertigungsinsel", new int[] { 2 });
        private readonly Tuple<String, int[]> _eingangslagerNumberOfConstructorParam = new("Eingangslager", new int[] { 2 });
        private readonly Tuple<String, int[]> _ausgangslagerNumberOfConstructorParam = new("Ausgangslager", new int[] { 2 });
        private readonly Tuple<String, int[]> _teilNumberOfConstructorParam = new("Teil", new int[] { 2 });
        private readonly Tuple<String, int[]> _transportfahrzeugNumberOfConstructorParam = new("Transportfahrzeug", new int[] { 2 });

        //Predefined per Framework
        private readonly Tuple<String, int[]> _produktionseinrichtungNumberOfConstructorParam = new("Produktionseinrichtung", new int[] { 2 });
        private readonly Tuple<String, int[]> _transportsystemNumberOfConstructorParam = new("Transportsystem", new int[] { 2 });
        private readonly Tuple<String, int[]> _abteilungssteuerungNumberOfConstructorParam = new("Abteilungssteuerung", new int[] { 0 });



        private Dictionary<string, int[]>? _constructorsPerClass;

        [OneTimeSetUp]
        public void AssembleDataStructure()
        {
            _constructorsPerClass = new()
            {
                { _lagerNumberOfConstructorParam.Item1, _lagerNumberOfConstructorParam.Item2 },
                { _fertigungsinselNumberOfConstructorParam.Item1, _fertigungsinselNumberOfConstructorParam.Item2 },
                { _eingangslagerNumberOfConstructorParam.Item1, _eingangslagerNumberOfConstructorParam.Item2 },
                { _ausgangslagerNumberOfConstructorParam.Item1, _ausgangslagerNumberOfConstructorParam.Item2 },
                { _teilNumberOfConstructorParam.Item1, _teilNumberOfConstructorParam.Item2 },
                { _transportfahrzeugNumberOfConstructorParam.Item1, _transportfahrzeugNumberOfConstructorParam.Item2 },
                { _produktionseinrichtungNumberOfConstructorParam.Item1, _produktionseinrichtungNumberOfConstructorParam.Item2 },
                { _transportsystemNumberOfConstructorParam.Item1, _transportsystemNumberOfConstructorParam.Item2 },
                { _abteilungssteuerungNumberOfConstructorParam.Item1, _abteilungssteuerungNumberOfConstructorParam.Item2 }
            };
        }

        /// <summary>
        /// Dieser Test schaut ob Ihre Klassen einen Konstruktor aufweisen, der die geforderte Anzahl an Parametern hat
        /// Ob es noch weitere Konstruktoren gibt oder ob die Typen der Parameter korrekt nach EKD sind, wird hier nicht
        /// betrachtet
        /// </summary>
        [Test]
        public void Test_Structure_CheckIfConstructorWithExpectedNumberOfParametersIsPresent_AllArePresent()
        {
            bool failed = false;
            String msg = "";
            foreach (Type t in TypesExistingInCode.Where(t => t.IsClass).Except(new[] { typeof(Extenions) }.ToList()))
            {
                if (_constructorsPerClass != null && _constructorsPerClass.ContainsKey(t.Name))
                {
                    ConstructorInfo[] con = t.GetConstructors();
                    if (!con.Any(c => _constructorsPerClass[t.Name].Any(nc => c.GetParameters().Length == nc)))
                    {
                        failed = true;
                        msg += "In der Klasse " + t.Name + " konnte kein Konstruktor gefunden werden, der " +
                            "exakt die im EKD vorgegebene Anzahl an Parametern aufweist.\n";
                    }
                }
                else
                {
                    msg += "Die von Ihnen definierte Klasse " + t.Name + " wurde nicht auf Konstruktoren überprüft." +
                            "Das kann entweder bedeuten, dass Sie eine zusätzliche Klasse definiert haben oder das ein Schreibfehler" +
                            "vorliegt. In beiden Fällen sollten Sie Ihren Code überprüfen\n";
                    failed = true;
                }

            }
            if (!failed)
            {
                Assert.Pass("Alles bislang von Ihnen implementierten Klassen weisen wahrscheinlich die erwarteten Konstruktoren auf " + msg + "\nm");
            }
            else
            {
                Assert.Fail("In den von Ihnen bislang implementierten Klassen, scheinen noch nicht alle geforderten Konstruktoren vorhanden zu sein.\n" + msg);
            }

        }
    }

}
