using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Beleg2022.Tests
{
    [TestFixture]
    public class TestsTa41Base
    {
       public Fertigungsinsel? MyFertigungsinsel;
       public FieldInfo? MyFertigungsinselBelegtBisFi;
       public FieldInfo? MyFertigungsinselAktuellesTeilFi;
       public FieldInfo? MyFertigungsinselAktuellerStatusFi;
       public FieldInfo? MyFertigungsinselFaehigkeitenFi;
       public Teil? TeilNextIsValid = new Teil
            (
                "validesTeil",
                new List<Verarbeitungsschritt>()
                    {
                    Verarbeitungsschritt.BOHREN,
                    Verarbeitungsschritt.FRAESEN,
                    Verarbeitungsschritt.EINLAGERN
                    }
            );
        public Teil? TeilNextIsNotValid = new Teil
            (
                "validesTeil",
                new List<Verarbeitungsschritt>()
                    {
                    Verarbeitungsschritt.LACKIEREN,
                    Verarbeitungsschritt.FRAESEN,
                    Verarbeitungsschritt.EINLAGERN
                    }
            );
        public Teil? TeilNoneValid = new Teil
            (
                "validesTeil",
                new List<Verarbeitungsschritt>()
                    {
                    Verarbeitungsschritt.LACKIEREN,
                    Verarbeitungsschritt.SCHWEISSEN,
                    Verarbeitungsschritt.EINLAGERN
                    }
            );
        // keine Einlagern
        public Teil TeilNextIsValidEmptyAfter = new Teil
            (
                "validesTeil",
                new List<Verarbeitungsschritt>()
                    {
                    Verarbeitungsschritt.BOHREN
                    }
            );

        [SetUp]
        public void Setup()
        {
            MyFertigungsinsel = new("testFertigungsinsel", "XYZ");
            MyFertigungsinsel.SetFaehigkeiten(new List<Verarbeitungsschritt>() { Verarbeitungsschritt.BOHREN, Verarbeitungsschritt.FRAESEN });
            MyFertigungsinselBelegtBisFi = MyFertigungsinsel.GetType().GetField("_BelegtBis", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("If that exception ever happens a multitude of tests failed before");
            MyFertigungsinselAktuellesTeilFi = MyFertigungsinsel.GetType().GetField("_AktuellesTeil", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("If that exception ever happens a multitude of tests failed before");
            MyFertigungsinselAktuellerStatusFi = MyFertigungsinsel.GetType().GetField("_AktuellerStatus", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("If that exception ever happens a multitude of tests failed before"); 
            MyFertigungsinselFaehigkeitenFi = MyFertigungsinsel.GetType().GetField("_Faehigkeiten", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("If that exception ever happens a multitude of tests failed before");


        }

        public void ForceBelegtBis(ref Fertigungsinsel? insel, DateTime dt)
            => insel?.GetType().GetField("_BelegtBis", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(insel, dt);
        public void ForceStatus(ref Fertigungsinsel? insel, Status s)
            => insel?.GetType().GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(insel, s);
        public void ForceTeil(ref Fertigungsinsel? insel, Teil? t)
            => insel?.GetType().GetField("_AktuellesTeil", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(insel, t);
    }
}