using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;


namespace Beleg2022.Tests
{
    [TestFixture]
    public class RezeptHandlinTestTA32
    {
        Teil? _myTeil;

        [SetUp]
        public void Setup()
        {
            _myTeil = new("TestTeil", new List<Verarbeitungsschritt>() { Verarbeitungsschritt.BOHREN, Verarbeitungsschritt.EINLAGERN });
        }

        [Test]
        public void Test_GetNaechsterSchritt_InvokeOnmyTeil_VerarbeitungsschrittBohren()
        {
            //Arrange
            //Act
            if(_myTeil is null)
                throw new InvalidOperationException("If myTeil is empty at this position, you likely messed with the Setup.");
            Verarbeitungsschritt nextStep = _myTeil.GetNaechstenSchritt();
            //Assert
            if(nextStep == Verarbeitungsschritt.BOHREN)
            {
                Console.WriteLine("Ihre Implementierung von Teil.GetNaechstenSchritt scheint den erwarteten Schritt zu liefern.");
                Assert.Pass("Ihre Implementierung von Teil.GetNaechstenSchritt scheint den erwarteten Schritt zu liefern.");
            }
            else
            {
                Assert.Fail("Ihre Implementierung von Teil.GetNaechstenSchritt liefert nicht den erwarteten Schritt zurück." +
                            "Denken Sie daran, in diesem Belegt werden Listen immer von Links nach Rechts gelesen/geschrieben. Sie sollten also das" +
                            "Element zurückgeben, dass ganz Links steht");
            }

        }
        [Test]
        public void Test_EntferneFertigenSchritt_InvokeOnmyTeil_CountOneLess()
        {
            //Arrange
            if(_myTeil is null)
                throw new InvalidOperationException("If myTeil is empty at this position, you likely messed with the Setup.");
            FieldInfo? refRezept_fi = _myTeil.GetType().GetField("_Rezept", BindingFlags.Instance | BindingFlags.NonPublic);
            refRezept_fi ??= _myTeil.GetType().GetField("<_Rezept>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            ;
            var refRezept = refRezept_fi
                ?.GetValue(_myTeil) as IEnumerable<Verarbeitungsschritt> ??
                                                   throw new InvalidOperationException(
                                                       "Neither Rezept nor its Field info should be null at this point");
            int oldCount = refRezept.Count();
            //Act
            _myTeil.EntferneFertigenSchritt();

            //Assert
            if(oldCount - refRezept.Count() == 1)
            {
                Console.WriteLine("\nIhre Implementierung von Teil.EntferneFertigenSchritt scheint zu funktionieren.");
                Assert.Pass("\nIhre Implementierung von Teil.EntferneFertigenSchritt scheint zu funktionieren.");
            }
            else
            {
                Assert.Fail("Ihre Implementierung von Teil.EntferneFertigenSchritt verhält sich anders als erwartet. " +
                    "Ihre Datenstruktur _Rezept sollte nach dem Aufruf geschrumpft sein");
            }

        }

        /// Dieser Test ist schlägt immer fehl, wenn der darüber fehl schlägt, darüber hinaus prüft er 
        /// implizit ab, ob die Datenstruktur verwendet wird oder das Rezept einfach statisch im Code steht
        [Test]
        public void Test_EntferneFertigenSchritt_InvokeOnmyTeil_NextStepisUNDEFINIERT()
        {
            //Arrange
            if(_myTeil is null)
                throw new InvalidOperationException("If myTeil is empty at this position, you likely messed with the Setup.");
            IEnumerable<Verarbeitungsschritt> refRezept =
                _myTeil.GetType().GetField("_Rezept", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(_myTeil)
                    as IEnumerable<Verarbeitungsschritt> ??
                throw new InvalidOperationException("Neither Rezept nor its Field info should be null at this point");

            //Act
            _myTeil.EntferneFertigenSchritt();

            //Assert
            if(refRezept.Contains(Verarbeitungsschritt.EINLAGERN))
            {
                Console.WriteLine("\nIhre Implementierung von Teil.EntferneFertigenSchritt scheint zu funktionieren.");
                Assert.Pass("\nIhre Implementierung von Teil.EntferneFertigenSchritt scheint zu funktionieren.");
            }
            else
            {
                Assert.Fail("Ihre Implementierung von Teil.EntferneFertigenSchritt verhält sich anders als erwartet. " +
                    "Offenbar entfernen Sie ein anderes als das erwartete Element. ");
            }

        }
    }
}