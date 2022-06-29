using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Beleg2022;
using System.Runtime.CompilerServices;

namespace Beleg2022.Tests
{
    [TestFixture]
    public class ClassMethodeFieldTestTa31A : TestTa31Base
    {
        // Auflistug aller erwarteten Typen
        private readonly List<String> _expectedNonAbstractTypes = new()
        {
            "Eingangslager",
            "Ausgangslager",
            "Fertigungsinsel",
            "Teil",
            "Transportfahrzeug",
            "Status",
            "Verarbeitungsschritt",
            "Abteilungssteuerung"
        };

        // Auflistug aller erwarteten Typen
        private readonly List<String> _expectedAbstractTypes = new()
        {
            "Lager",
            "Produktionseinrichtung",
            "Transportsystem",
            "Extenions"
        };

        // Liste der Klassen mit Funktionen und Attributen, jeweils nur die lokal implementierten, keine ererbten etc.
        #region studentisch
        private readonly Tuple<String, List<String>, List<String>> _lagerFunctionList =
            new("Lager",
                new List<string>() {
                    "ErmittleStatus",
                    "EmpfangeTeil"
                },
                new List<String>() {
                        "_Kapazitaet",
                        "_Bestand"
                }
            );

        private readonly Tuple<String, List<String>, List<String>> _fertigungsinselFunktionList =
            new("Fertigungsinsel",
                new List<string>() {
                    "ErmittleStatus",
                    "EmpfangeTeil",
                    "BearbeiteTeil",
                    "GetTeil"
                },
                new List<String>() {
                        "_BelegtBis",
                        "_AktuellesTeil"
                }
            );

        private readonly Tuple<String, List<String>, List<String>> _eingangslagerFunctionList =
            new("Eingangslager",
                new List<String>() {
                    "InitialisiereBestand",
                    "GetTeil",
                },
                new List<String>()
                {


                }
            );

        private readonly Tuple<String, List<String>, List<String>> _ausgangslagerFunctionList =
            new("Ausgangslager",
                new List<String>()
                {

                },
                new List<String>()
                {

                }
            );

        private readonly Tuple<String, List<String>, List<String>> _teilFunctionList =
            new("Teil",
                new List<String>()
                {
                    "GetNaechstenSchritt",
                    "EntferneFertigenSchritt",
                    "GetId"
                },
                new List<String>() {
                    "_Id",
                    "_Rezept"
                }
            );

        private readonly Tuple<String, List<String>, List<String>> _transportfahrzeugFunctionList =
            new("Transportfahrzeug",
                new List<String>()
                {
                    "Hauptprozess"
                },
                new List<String>()
                {
                    "_AktuellesTeil"
                }
            );
        #endregion

        #region vorgegeben
        private readonly Tuple<String, List<String>, List<string>> _produktionseinrichtungFunctionList =
            new("Produktionseinrichtung",
                new List<String>()
                {
                    "SetStatus",
                    "SetFaehigkeiten",
                    "GetName",
                    "GetPosition",
                    "HatFaehigkeit",
                    "ErmittleStatus"
                },
                new List<String>()
                {
                    "_AktuellerStatus",
                    "_Faehigkeiten",
                    "_Name",
                    "_Position"
                }
            );

        private readonly Tuple<String, List<String>, List<String>> _abteilungssteuerungFunctionList =
            new("Abteilungssteuerung",
                new List<string>()
                {
                    "InitialisiereAbteilung",
                    "Anlegen"
                },
                new List<string>()
                {
                    "_Transportsystem",
                    "_Produktionseinrichtungen"
                }
            );

        private readonly Tuple<String, List<String>, List<String>> _transportsystemFunctionList =
            new("Transportsystem",
                new List<string>()
                {
                    "FahreZu",
                    "StarteHauptprozess",
                    "Hauptprozess"
                },
                new List<String>()
                {
                    "_Name",
                    "_Position"
                }
            );

        private readonly Tuple<String, List<String>, List<String>> _extenionsFunctionList =
            new("Extenions",
                new List<string>()
                {
                },
                new List<String>()
                {
                }
            );
        #endregion 


        private Dictionary<string, List<string>>? _dictOfMethodsPerClass;
        private Dictionary<string, List<string>>? _dictOfAttributesPerClass;


        [OneTimeSetUp]
        public void AssembleDataStructures()
        {
            _dictOfMethodsPerClass = new()
            {
                { _lagerFunctionList.Item1, _lagerFunctionList.Item2 },
                { _fertigungsinselFunktionList.Item1, _fertigungsinselFunktionList.Item2 },
                { _eingangslagerFunctionList.Item1, _eingangslagerFunctionList.Item2 },
                { _ausgangslagerFunctionList.Item1, _ausgangslagerFunctionList.Item2 },
                { _teilFunctionList.Item1, _teilFunctionList.Item2 },
                { _transportfahrzeugFunctionList.Item1, _transportfahrzeugFunctionList.Item2 },
                //Buildins
                { _produktionseinrichtungFunctionList.Item1, _produktionseinrichtungFunctionList.Item2 },
                { _abteilungssteuerungFunctionList.Item1, _abteilungssteuerungFunctionList.Item2 },
                { _transportsystemFunctionList.Item1, _transportsystemFunctionList.Item2 },
                { _extenionsFunctionList.Item1, _extenionsFunctionList.Item2 }
            };

            _dictOfAttributesPerClass = new()
            {
                { _lagerFunctionList.Item1, _lagerFunctionList.Item3 },
                { _fertigungsinselFunktionList.Item1, _fertigungsinselFunktionList.Item3 },
                { _eingangslagerFunctionList.Item1, _eingangslagerFunctionList.Item3 },
                { _ausgangslagerFunctionList.Item1, _ausgangslagerFunctionList.Item3 },
                { _teilFunctionList.Item1, _teilFunctionList.Item3 },
                { _transportfahrzeugFunctionList.Item1, _transportfahrzeugFunctionList.Item3 },
                //Buildins
                { _produktionseinrichtungFunctionList.Item1, _produktionseinrichtungFunctionList.Item3 },
                { _abteilungssteuerungFunctionList.Item1, _abteilungssteuerungFunctionList.Item3 },
                { _transportsystemFunctionList.Item1, _transportsystemFunctionList.Item3 },
                { _extenionsFunctionList.Item1, _extenionsFunctionList.Item2 }
            };

        }

        [Test]
        public void Test_Structure_CheckIfAllClassesArePublic()
        {

            IEnumerable<string> t = TypesExistingInCode.Where(t => !t.IsPublic).Select(t => t.Name);
            if (t.Any())
            {
                Assert.Fail("Die Klassen " + String.Join("; ", t) + " sind nicht PUBLIC, das sollten Sie ändern, da es sonst zu Problemen in der Bewertung kommen wird.");
            }
            else
            {
                Console.WriteLine("Alle Ihre Klassen scheinen Public zu sein. O.K.");
                Assert.Pass("Alle Ihre Klassen scheinen Public zu sein. O.K.");
            };

        }

        [Test]
        public void Test_Structure_CheckIfAbstractClassesAreAbstract()
        {
            IEnumerable<string> t = TypesExistingInCode.Where(t => t.IsAbstract).Select(t => t.Name);

            var wrongAbstract = t.Except(_expectedAbstractTypes);
            var missingAbstract = _expectedAbstractTypes.Except(t);

            if (wrongAbstract.Any())
            {
                Assert.Fail("Die Klasse(n) (" + String.Join(", ", wrongAbstract) + ") sind ABSTRACT, das sollten Sie ändern, da es sonst zu Problemen in der Bewertung kommen wird.");
            }
            else if (missingAbstract.Any())
            {
                Assert.Fail("Die Klasse(n) (" + String.Join(", ", missingAbstract) + ") sind nicht ABSTRACT, das sollten Sie ändern, da es sonst zu Problemen in der Bewertung kommen wird.");
            }
            else
            {
                Console.WriteLine("Die richtigen Klassen scheinen Abstract zu sein. O.K.");
                Assert.Pass("Die richtigen Klassen scheinen Abstract zu sein. O.K.");
            }

        }

        [Test]
        public void Test_Structure_CheckIfAllClassesAreInExpectedNamespace()
        {
            List<Tuple<string, string>> t = TypesExistingInCode.Where(t => t.Namespace != "Beleg2022")
                .Select(t => new Tuple<string, string>(t.Name, t.Namespace?.ToString() ?? string.Empty)).ToList();
            if (t.Any())
            {
                Assert.Fail("Eine oder mehrere Klassen die Sie definiert haben, sind nicht im erwarteten Namespace \"Beleg2022\". " +
                    "Das kann zu Problemen in der weiteren Bearbeitung führen.\n " + String.Join(" ", t.Select(t => new string(String.Join(" --> ", t.Item1, t.Item2 + "\n")))));
            }
            else
            {
                Console.WriteLine("Alle Ihre Klassen sind im passenden Namespace");
                Assert.Pass("Alle Ihre Klassen sind im passenden Namespace");
            }
        }

        [Test]
        public void Test_Structure_CheckIfAllClassesArePresent_AllArePresent()
        {
            List<string> defNames = TypesExistingInCode.Select(t => t.Name).ToList();
            var expectedTypes = _expectedNonAbstractTypes.Concat(_expectedAbstractTypes);
            foreach (String s in expectedTypes)
            {
                if (!TypesExistingInCode.Any(t => t.Name == s))
                {
                    Console.WriteLine("Es wurden noch nicht alle erwarteten Klassen gefunden! Achten Sie auch auf Rechtschreibfehler");
                    Assert.Fail("Es wurden noch nicht alle erwarteten Klassen gefunden! Achten Sie auch auf Rechtschreibfehler");
                }
            }
            Assert.Pass("Alle Klassen scheinen wie erwartet vorhanden zu sein.");
        }

        [Test]
        public void Test_Structure_CheckIfMethodsArePresentForAvailableTypes_AllArePresentByName()
        {
            string msg = "";
            bool failed = false;
            foreach (Type t in TypesExistingInCode)
            {
                if (t.IsClass)
                {
                    if (_dictOfMethodsPerClass != null && _dictOfMethodsPerClass.ContainsKey(t.Name))
                    {
                        int x = _dictOfMethodsPerClass[t.Name].Count();
                        MethodInfo[] methods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        if ((_dictOfMethodsPerClass[t.Name].Any()) && !_dictOfMethodsPerClass[t.Name].All(n => methods.Any(m => n == m.Name)))
                        {
                            failed = true;
                            msg += "Der Klasse " + t.Name + " fehlen noch Funktionen oder es liegen Tippfehler vor. " +
                                "Sie sollten dies überprüfen (Schreibweise, Sichtbarkeit, etc.) \n";
                        };
                    }
                    else
                    {
                        msg += "Die von Ihnen definierte Klasse " + t.Name + " wurde nicht auf Methoden überprüft." +
                            "Das kann entweder bedeuten, dass Sie eine zusätzliche Klasse definiert haben oder das ein Schreibfehler" +
                            "vorliegt. In beiden Fällen sollten Sie Ihren Code überprüfen\n";
                        failed = true;
                    }
                }

            }
            if (!failed)
            {
                Assert.Pass("Alle von Ihnen implementierten Klassen weisen die geforderten " +
                "Funktionen auf. Ob evtl. noch Klassen fehlen, entnehmen Sie bitte den anderen Tests");
            }
            else
            {
                Assert.Fail("Es wurden noch nicht alle Funktionen für die von Ihnen implementierten Klassen  gefunden:\n" + msg);
            }
        }

        [Test]
        public void Test_Structure_CheckIfAttributesArePresentForAvailableTypes_AllArePresentByName()
        {
            string msg = "";
            bool failed = false;
            foreach (Type t in TypesExistingInCode)
            {
                if (t.IsClass)
                {
                    if (_dictOfAttributesPerClass != null && _dictOfAttributesPerClass.ContainsKey(t.Name))
                    {
                        FieldInfo[] fields = t.GetFields(BindingFlags.NonPublic |
                            BindingFlags.Public |
                            BindingFlags.Instance |
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Static);
                        if (_dictOfAttributesPerClass[t.Name].Any(a => t.IsProperty(a)))
                            msg +=
                                "\nSie verwenden die erwarteten Felder Attribute als Properties, diese Test behandeln\n" +
                                "das nicht! Sie werden eine große Anzahl an Tests fehlschlagen sehen. Nutzen sie Properties,\n" +
                                "Auto Properties und Backening Fields nur, wenn Sie genau wissen, was Sie tun! Da diese Tests\n" +
                                "vor allem die Implementierung von unerfahrenen Kommilitonen unterstützen sollen werden sie ab\n" +
                                "hier gar nicht mehr berücksichtigt. Buchen Sie sich einen Termin für die Belegeinsicht :)\n";

                        if (_dictOfAttributesPerClass[t.Name].Any() &&
                            !_dictOfAttributesPerClass[t.Name].All(n => fields.Any(m => n == m.Name || m.Name ==
                                $"<{n}>k__BackingField")))
                        {
                            failed = true;
                            msg += "Der Klasse " + t.Name + " fehlen noch Attribute oder es liegen Tippfehler vor. " +
                                "Sie sollten dies überprüfen (Schreibweise, Sichtbarkeit AUCH von ElternKlassen, etc.) \n";
                        };
                    }
                    else
                    {
                        msg += "Die von Ihnen definierte Klasse " + t.Name + " wurde nicht auf Attribute überprüft." +
                            "Das kann entweder bedeuten, dass Sie eine zusätzliche Klasse definiert haben oder das ein Schreibfehler" +
                            "vorliegt. In beiden Fällen sollten Sie Ihren Code überprüfen\n";
                        failed = true;
                    }
                }

            }
            if (!failed)
            {
                Console.WriteLine("Alle von Ihnen implementierten Klassen weisen die geforderten " +
                                  "Attribute auf. Ob evtl. noch Klassen fehlen, entnehmen Sie bitte den anderen Tests\n\n" + msg);
                Assert.Pass("Alle von Ihnen implementierten Klassen weisen die geforderten " +
                "Attribute auf. Ob evtl. noch Klassen fehlen, entnehmen Sie bitte den anderen Tests\n\n" + msg);
            }
            else
            {
                Assert.Fail("Es wurden noch nicht alle Attribute für die von Ihnen implementierten Klassen gefunden:\n" + msg);
            }
        }
    }
}
