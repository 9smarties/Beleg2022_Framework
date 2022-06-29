using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Beleg2022.Tests
{
    // Testhalber wird hier Eingangslager getestet und nicht Lager, da abstract
    [TestFixture]
    public class TestsEmpfangeTeilTestTa33A : TestsTA33Base
    {
        [SetUp]
        public void Setup()
        {
            MyLager = new Eingangslager("someName", "SomePosition");
            MyLagerKapa = (int)(typeof(Lager)
                .GetField("_Kapazitaet", BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetValue(null) ?? 0);
        }

        [Test]
        public void Test_Empfang_InvokeInStatusEMPFANGSBEREIToINTERAKTINSBEREIT_TRUE()
        {
            //Arrange
            FillLager(MyLagerKapa - 1, ref MyLager, null);
            //Act
            bool result = MyLager != null && MyLager.EmpfangeTeil(GetRandomTeil());
            //Assert
            if(result)
            {
                Console.WriteLine("Teil Empfangen hat wie erwartet True geliefert");
                Assert.Pass("Teil Empfangen hat wie erwartet True geliefert");
            }
            else
            {
                Console.WriteLine("Teil Empfangen hat nicht wie erwartet True geliefert");
                Assert.Fail("Teil Empfangen hat nicht wie erwartet True geliefert");
            }
        }

        [Test]
        public void Test_Empfang_InvokeInStatusABHOLBEREIT_FALSE()
        {
            //Arrange
            FillLager(MyLagerKapa, ref MyLager, null);
            //Act
            bool result = MyLager != null && MyLager.EmpfangeTeil(GetRandomTeil());
            //Assert
            if(!result)
            {
                Console.WriteLine("Teil Empfangen hat wie erwartet False geliefert");
                Assert.Pass("Teil Empfangen hat wie erwartet False geliefert");
            }
            else
            {
                Console.WriteLine("Teil Empfangen hat nicht wie erwartet False geliefert");
                Assert.Fail("Teil Empfangen hat nicht wie erwartet False geliefert");
            }
        }
        [Test]
        public void Test_Empfang_InvokeInStatusEMPFANGSBEREIToINTERAKTINSBEREIT_NewTeilInBestand()
        {
            //Arrange

            FillLager(2, ref MyLager, null);
            //Act
            if(MyLager is null)
                throw new InvalidOperationException("Lager should not have been null at this point");
            Teil myTeil = GetRandomTeil();
            MyLager.EmpfangeTeil(myTeil);
            FieldInfo? myLagerFiBestand = MyLager.GetType().GetField("_Bestand", BindingFlags.Instance | BindingFlags.NonPublic);
            IEnumerable<Teil> myLagerBestand = myLagerFiBestand?.GetValue(MyLager) as IEnumerable<Teil> ??
                                         throw new InvalidOperationException(
                                             "This must not happen. If Bestand or its corresponding field info is null at this point something went wrong");
            bool partIsIn = myLagerBestand.Last().Equals(myTeil);
            //Assert
            if((myLagerBestand.Count()) == (3) && partIsIn)
            {
                Console.WriteLine("Teil Empfangen hat wie erwartet das richtige Teil empfangen und eingelagert");
                Assert.Pass("Teil Empfangen hat wie erwartet das richtige Teil empfangen und eingelagert");
            }
            else
            {
                Console.WriteLine("Teil Empfangen hat nicht wie erwartet das richtige Teil empfangen oder eingelagert. " +
                    "Nutzen Sie .add() zum Einfügen?");
                Assert.Fail("Teil Empfangen hat nicht wie erwartet das richtige Teil empfangen oder eingelagert." +
                    "Nutzen Sie .add() zum Einfügen?");
            }
        }

        [Test]
        public void Test_Empfang_InvokeInStatusABHOLBEREIT_BestandUnchanged()
        {
            //Arrange

            FillLager(MyLagerKapa, ref MyLager, null);
            //Act
            if(MyLager is null)
                throw new InvalidOperationException("Lager should not have been null at this point");
            Teil myTeil = GetRandomTeil();
            MyLager.EmpfangeTeil(myTeil);
            FieldInfo? myLagerFiBestand = MyLager.GetType().GetField("_Bestand", BindingFlags.Instance | BindingFlags.NonPublic);
            IEnumerable<Teil> myLagerBestand = (myLagerFiBestand?.GetValue(MyLager) as IEnumerable<Teil>) ??
                                        throw new InvalidOperationException(
                                            "This must not happen. If Bestand or its corresponding field info is null at this point something went wrong");
            ;
            bool partIsIn = myLagerBestand.Contains(myTeil);
            //Assert
            if((myLagerBestand.Count()) == MyLagerKapa && !partIsIn)
            {
                Console.WriteLine("Teil Empfangen hat wie erwartet das Teil abgewiesen und nicht eingelagert");
                Assert.Pass("Teil Empfangen hat wie erwartet das Teil abgewiesen und nicht eingelagert");
            }
            else
            {
                Console.WriteLine("Obwohl der Bestand voll war, befindet sich das Testteil nun im Bestand " +
                    "oder die Kapazitaet wurde überschritten, dass sollte nicht passieren");
                Assert.Fail("Obwohl der Bestand voll war, befindet sich das Testteil nun im Bestand " +
                    "oder die Kapazitaet wurde überschritten, dass sollte nicht passieren");
            }
        }


        [Test]
        public void Test_Empfang_InvokeWithKapazitaetMinusOne_StatusChangesToAbholbereit()
        {
            //Arrange
            FillLager(MyLagerKapa - 1, ref MyLager, null);
            //Act
            if(MyLager is null)
                throw new InvalidOperationException("Lager should not have been null at this point");
            Teil myTeil = GetRandomTeil();
            bool eingelagert = MyLager.EmpfangeTeil(myTeil);
            Status myLagerStatus = (Status)(MyLager
                .GetType()
                .GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(MyLager) ?? throw new InvalidOperationException(
                "Neither the Field info for _AktuellerStatus nor its value should be null at this point. "));
            //Assert
            if(myLagerStatus == Status.ABHOLBEREIT)
            {
                Console.WriteLine("Das Lager hat beim Übergang zu Voll, korrekt" +
                    "in den Zustand Status.Abholbereit gewechselt");
                Assert.Pass("Das Lager hat beim Übergang zu Voll, korrekt" +
                    "in den Zustand Status.Abholbereit gewechselt");
            }
            else
            {
                Console.WriteLine("Das Lager hat beim Übergang zu Voll, nicht korrekt" +
                    "in den Zustand Status.Abholbereit gewechselt");
                Assert.Fail("Das Lager hat beim Übergang zu Voll, nicht korrekt" +
                    "in den Zustand Status.Abholbereit gewechselt");
            }
        }

        [Test]
        public void Test_Empfang_InvokeWithEmpty_StatusChangesToINTERAKTIONSBEREIT()
        {
            //Arrange
            FillLager(0, ref MyLager, null);
            //Act
            if(MyLager is null)
                throw new InvalidOperationException("Lager should not have been null at this point");
            Teil myTeil = GetRandomTeil();
            bool eingelagert = MyLager.EmpfangeTeil(myTeil);
            Status myLagerStatus = (Status)((MyLager
                .GetType()
                .GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(MyLager)) ?? throw new InvalidOperationException(
                "Neither the Field info for _AktuellerStatus nor its value should be null at this point. "));
            //Assert
            if(myLagerStatus == Status.INTERAKTIONSBEREIT)
            {
                Console.WriteLine("Das Lager hat beim Übergang zu Voll, korrekt" +
                    "in den Zustand Status.Abholbereit gewechselt");
                Assert.Pass("Das Lager hat beim Übergang zu Voll, korrekt" +
                    "in den Zustand Status.Abholbereit gewechselt");
            }
            else
            {
                Console.WriteLine("Das Lager hat beim Übergang zu Voll, nicht korrekt" +
                    "in den Zustand Status.Abholbereit gewechselt");
                Assert.Fail("Das Lager hat beim Übergang zu Voll, nicht korrekt" +
                    "in den Zustand Status.Abholbereit gewechselt");
            }
        }

        [Test]
        public void Test_Kapazitaet__KapazitaetEqToEKD()
        {
            int num = (int)(typeof(Lager).GetField("_Kapazitaet", BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetValue(null) ?? 0);
            if(num == 20)
                Assert.Pass();
            else
                Assert.Fail("Die in der Aufgabenstellung / EKD vorgegebene Kapazitaet wird nicht eingehalten, " +
               "das hat Auswirkungen auf alle Tests dieser Gruppe");

        }
    }
}
