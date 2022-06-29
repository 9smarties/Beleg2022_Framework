using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;


namespace Beleg2022.Tests
{
    // Testhalber wird hier Eingangslager getestet und nicht Lager, da abstract
    // was aber natürlich voraussetzt, das die Tests davon bestanden sind.
    // wenn das initialisiere im Eingangslager nicht implementiert ist knallt es
    [TestFixture]
    public class GetTeilTestTA34b : TestsTA34Base
    {
        [SetUp]
        public void Setup()
        {
            MyLager = new Eingangslager("someName", "SomePosition");
            MyLagerKapa = (int)(typeof(Lager)
                .GetField("_Kapazitaet", BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetValue(null) ?? throw new InvalidOperationException("Neither Kapazitaet nor its Filed info can be null at this point."));
            MyLagerBestandFi = MyLager.GetType()
                .GetField("_Bestand", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [Test]
        public void Test_GetTeil_InvokeInStatusABHOLBEREIToINTERAKTIONSBEREIT_TeilThatWasOnPosZero()
        {
            //Arrange
            if(BestandFake != null)
            {
                FillLager(BestandFake.Count, ref MyLager, BestandFake);
                //Teil part_on_pos_zero = ((List<Teil>)myLager_Bestand_fi.GetValue(myLager)).FirstOrDefault();
                Teil? partOnPosZero = BestandFake.FirstOrDefault();
                //Act
                if(MyLager is null)
                    throw new InvalidOperationException("The Lager should not be null at this point.");
                Teil? result = MyLager.GetTeil();
                //Assert
                if(result != null && result.Equals(partOnPosZero))
                {
                    Console.WriteLine("GetTeil auf einem nicht leeren Lager liefert das erwartete Teil.");
                    Assert.Pass("GetTeil auf einem nicht leeren Lager liefert das erwartete Teil.");
                }
                else
                {
                    Console.WriteLine("GetTeil auf einem nicht leeren Lager liefert nicht das erwartete Teil.");
                    Assert.Fail("GetTeil auf einem nicht leeren Lager liefert nicht das erwartete Teil.");
                }
            }

            throw new InvalidOperationException("There is only one way this could have happened: The Test Project is damaged.");
        }

        [Test]
        public void Test_GetTeil_InvokeInStatusEMPFANGSBEREIT_null()
        {
            //Arrange
            FillLager(0, ref MyLager, null);
            if(MyLager is null)
                throw new InvalidOperationException("The Lager should not be null at this point.");
            //Act
            Teil? result = MyLager.GetTeil();
            //Assert
            if(result is null)
            {
                Console.WriteLine("GetTeil auf einem Empfangsbereiten Lager liefert wie erwartet ein null-Teil.");
                Assert.Pass("GetTeil auf einem Empfangsbereiten Lager liefert wie erwartet ein null-Teil.");
            }
            else
            {
                Console.WriteLine("GetTeil auf einem Empfangsbereiten Lager liefert nicht wie erwartet ein null-Teil.");
                Assert.Fail("GetTeil auf einem Empfangsbereiten Lager liefert nicht wie erwartet ein null-Teil.");
            }
        }

        [Test]
        public void Test_GetTeil_InvokeInStatusABHOLBEREIToINTERAKTIONSBEREIT_BestandOneLess()
        {
            //Arrange
            if(BestandFake != null)
            {
                FillLager(BestandFake.Count, ref MyLager, BestandFake);
                //Act
                if(MyLager is null)
                    throw new InvalidOperationException("The Lager should not be null at this point.");
                Teil? result = MyLager.GetTeil();
                int countAfter = (MyLagerBestandFi?.GetValue(MyLager) as IEnumerable<Teil> ??
                                  throw new InvalidOperationException(
                                      "Neither Bestand nor its Field info should be null at this point. Its is likely, that the test-project is damaged"))
                    .Count();
                //Assert
                if(BestandFake.Count() - countAfter == 1)
                {
                    Console.WriteLine("Teil geben hat wie erwartet den Bestand um 1 reduziert geliefert");
                    Assert.Pass("Teil geben hat wie erwartet den Bestand um 1 reduziert geliefert");
                }
                else
                {
                    Console.WriteLine("Teil geben hat wie nicht erwartet den Bestand um 1 reduziert geliefert");
                    Assert.Fail("Teil geben hat wie nicht erwartet den Bestand um 1 reduziert geliefert");
                }
            }
            throw new InvalidOperationException("There is only one way this could have happened: The Test Project is damaged.");

        }

        // damit die fehlschlägt muss man schon arg komisches Zeug machen ... etw new() Teile einfügen
        [Test]
        public void Test_GetTeil_InvokeInStatusEMPFANGSBEREIT_CountStillZero()
        {
            //Arrange
            FillLager(1, ref MyLager, BestandFake); //Wenn die Liste nie wirklich einen Wert enthält, bleibt sie in der reflection null
            //Act
            if(MyLager is null)
                throw new InvalidOperationException("The Lager should not be null at this point.");
            MyLager.GetTeil();
            MyLager.GetTeil(); // Das ist der eigentlich Schritt in dem der Bestand dann leer ist. 
            var locBestand = MyLagerBestandFi?.GetValue(MyLager) as IEnumerable<Teil> ?? throw new InvalidOperationException(
                "Neither Bestand nor its Field info should be null at this point. Its is likely, that the test-project is damaged");
            int count = locBestand.Count();
            //Assert
            Assert.AreEqual(0, count, "GetTeil in einem leeren Lager sorgt dafür, das danach das Lager nicht mehr leer ist. Das darf nicht passieren.");
        }
        [Test]
        public void Test_GetTeil_InvokeWithOnOneTeilInBestand_StatusChangesToEMPFANGSBEREIT()
        {
            //Arrange
            FillLager(1, ref MyLager, null);
            //Act
            if(MyLager is null)
                throw new InvalidOperationException("The Lager should not be null at this point.");
            MyLager.GetTeil();
            Status statusAfter = (Status)(MyLager
                .GetType()
                .GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(MyLager) ?? throw new InvalidOperationException(
                "Neither AktuellerStatus nor its Field info should be null at this point. Its is likely, that the test-project is damaged"));
            //Assert
            if(statusAfter == Status.EMPFANGSBEREIT)
            {
                Console.WriteLine("Beim Übergang zum leer-Zustand wird der Status wie erwartet geändert.");
                Assert.Pass("Beim Übergang zum leer-Zustand wird der Status wie erwartet geändert.");
            }
            else
            {
                Console.WriteLine("Beim Übergang zum leer-Zustand wird der Status nicht wie erwartet geändert.");
                Assert.Fail("Beim Übergang zum leer-Zustand wird der Status nicht wie erwartet geändert.");
            }
        }

        [Test]
        public void Test_GetTeil_InvokeWithOnFullBestand_StatusChangesToINTERAKTIONSBEREIT()
        {
            //Arrange
            FillLager(20, ref MyLager, null);
            //Act
            if(MyLager is null)
                throw new InvalidOperationException("The Lager should not be null at this point.");
            _ = MyLager.GetTeil();
            Status statusAfter = (Status)(MyLager
                .GetType()
                .GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(MyLager) ??
                                          throw new InvalidOperationException(
                                              "Neither AktuellerStatus nor its Field info should be null at this point. Its is likely, that the test-project is damaged"));
            Status statusAfterSimple = MyLager.ErmittleStatus();
            //Assert
            if(statusAfter is Status.INTERAKTIONSBEREIT || statusAfterSimple is Status.INTERAKTIONSBEREIT)
            {
                Console.WriteLine("Beim Übergang zum fast-voll-Zustand wird der Status wie erwartet geändert.");
                Assert.Pass("Beim Übergang zum fast-voll-Zustand wird der Status wie erwartet geändert.");
            }
            else
            {
                Console.WriteLine("Beim Übergang zum fast-voll-Zustand wird der Status nicht wie erwartet geändert.");
                Assert.Fail("Beim Übergang zum fast-voll-Zustand wird der Status nicht wie erwartet geändert.");
            }
        }
    }
}
