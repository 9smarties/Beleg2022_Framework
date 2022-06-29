namespace Beleg2022_Framework.TA42Hauptprozess;

[TestFixture]
public class TesteHauptprozess
{
    private static readonly string SEPARATOR = Environment.NewLine + "\t";

    private Util.Konfiguration konfiguration = new();

    [SetUp]
    public void Setup()
    {
        Util.RunSilent(() => {
            konfiguration = new() {
                Eingangslager = Util.CreateProduktionseinrichtung<Eingangslager>("Eingangslager_A", "A1"
            , new() { Verarbeitungsschritt.INITIALISIEREN }),

                Ausgangslager = Util.CreateProduktionseinrichtung<Ausgangslager>("Ausgangslager_B", "D1"
            , new() { Verarbeitungsschritt.EINLAGERN }),

                FertigungsinselA = Util.CreateProduktionseinrichtung<Fertigungsinsel>("Fertigungsinsel_01", "A3"
            , new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.BOHREN }),
                FertigungsinselB = Util.CreateProduktionseinrichtung<Fertigungsinsel>("Fertigungsinsel_02", "B3"
            , new() { Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.BOHREN }),
                FertigungsinselC = Util.CreateProduktionseinrichtung<Fertigungsinsel>("Fertigungsinsel_03", "C3"
            , new() { Verarbeitungsschritt.LACKIEREN }),

                Transportfahrzeug = new("Transportfahrzeug_X1", "A1")
            };
        });
    }

    [Test]
    public void Test_Verarbeite_Teil_Einfach_TF1()
    {
        Teil expected = new(id: "t1", new() { Verarbeitungsschritt.BOHREN, Verarbeitungsschritt.EINLAGERN });

        Util.ForceTeile(konfiguration.Eingangslager, new() { expected });
        Util.ForceStatus(konfiguration.Eingangslager, Status.INTERAKTIONSBEREIT);

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 10.0);

        var actual = Util.ErmittleTeile(konfiguration.Ausgangslager);

        Assert.Contains(expected, actual.ToList(), "Nach der vollständigen Abarbeitung müssen sich alle Teile im Ausgangslager befinden.");
        Assert.AreEqual(1, actual.Count(), "Die Anzahl der Teile im Ausgangslager ist fehlerhaft.");

        konfiguration.CheckForEmptyElements();
        Console.WriteLine($"Ausgangslager:{SEPARATOR}{string.Join(SEPARATOR, actual)}");
    }

    [Test]
    public void Test_Eingangslager_Leer_TF2()
    {
        Util.ForceTeile(konfiguration.Eingangslager, new() { });
        Util.ForceStatus(konfiguration.Eingangslager, Status.EMPFANGSBEREIT);

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 0.0);

        Assert.IsEmpty(Util.ErmittleTeile(konfiguration.Ausgangslager)
            , "Wenn das Eingangslager leer ist, dann werden keine Teile Verarbeitet und das Ausgangslager bleibt ebenfalls leer.");

        konfiguration.CheckForEmptyElements();
    }

    [Test]
    public void Test_Verarbeite_Teil_Weiterverarbeiten_TF3()
    {
        Teil expected = new(id: "t2", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });

        Util.ForceTeile(konfiguration.Eingangslager, new() { });
        Util.ForceStatus(konfiguration.Eingangslager, Status.EMPFANGSBEREIT);

        Util.ForceTeil(konfiguration.FertigungsinselA, expected);
        Util.ForceStatus(konfiguration.FertigungsinselA, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselA, DateTime.Now);

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 10.0);

        var actual = Util.ErmittleTeile(konfiguration.Ausgangslager);

        Assert.Contains(expected, actual.ToList(), "Nach der vollständigen Abarbeitung müssen sich alle Teile im Ausgangslager befinden.");
        Assert.AreEqual(1, actual.Count(), "Die Anzahl der Teile im Ausgangslager ist fehlerhaft.");

        konfiguration.CheckForEmptyElements();
        Console.WriteLine($"Ausgangslager:{SEPARATOR}{string.Join(SEPARATOR, actual)}");
    }

    [Test]
    public void Test_Verarbeite_Teil_NULL_TF4()
    {
        Util.ForceTeile(konfiguration.Eingangslager, new() { null! });
        Util.ForceStatus(konfiguration.Eingangslager, Status.INTERAKTIONSBEREIT);

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 0.0);

        Assert.IsEmpty(Util.ErmittleTeile(konfiguration.Ausgangslager)
            , "Wenn das Eingangslager leer ist, dann werden keine Teile Verarbeitet und das Ausgangslager bleibt ebenfalls leer.");

        konfiguration.CheckForEmptyElements();
    }

    [Test]
    public void Test_Verarbeite_Teil_Wenn_Fertigungsinsel_Wieder_Frei_TF5()
    {
        Teil t4 = new(id: "t4", new() { Verarbeitungsschritt.BOHREN, Verarbeitungsschritt.EINLAGERN });
        Teil t5 = new(id: "t5", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN });
        Teil t6 = new(id: "t6", new() { Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });
        Teil t7 = new(id: "t7", new() { Verarbeitungsschritt.LACKIEREN, Verarbeitungsschritt.EINLAGERN });

        Util.ForceTeile(konfiguration.Eingangslager, new() { t4 });
        Util.ForceStatus(konfiguration.Eingangslager, Status.INTERAKTIONSBEREIT);

        var currentTime = DateTime.Now;

        Util.ForceTeil(konfiguration.FertigungsinselA, t5);
        Util.ForceStatus(konfiguration.FertigungsinselA, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselA, currentTime.AddSeconds(1));

        Util.ForceTeil(konfiguration.FertigungsinselB, t6);
        Util.ForceStatus(konfiguration.FertigungsinselB, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselB, currentTime.AddSeconds(1));

        Util.ForceTeil(konfiguration.FertigungsinselC, t7);
        Util.ForceStatus(konfiguration.FertigungsinselC, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselC, currentTime.AddSeconds(1));

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 11.0);

        var actual = Util.ErmittleTeile(konfiguration.Ausgangslager);

        var first = actual.Take(3);
        var last = actual.Last();

        Assert.AreEqual(4, actual.Count(), "Die Anzahl der Teile im Ausgangslager ist fehlerhaft.");
        Assert.Contains(t5, first.ToList(), "t5 muss eines der ersten drei Teile im Ausgangslager sein.");
        Assert.Contains(t6, first.ToList(), "t6 muss eines der ersten drei Teile im Ausgangslager sein.");
        Assert.Contains(t7, first.ToList(), "t7 muss eines der ersten drei Teile im Ausgangslager sein.");
        Assert.AreEqual(t4, last, "t4 muss das letzte Teile im Ausgangslager sein.");

        konfiguration.CheckForEmptyElements();
        Console.WriteLine($"Ausgangslager:{SEPARATOR}{string.Join(SEPARATOR, actual)}");
    }

    [Test]
    public void Test_Verarbeite_Teile_Mit_Eingangslager_Als_Parkplatz_TF6_A()
    {
        Teil t9 = new(id: "t9", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN });
        Teil t10 = new(id: "t10", new() { Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });
        Teil t11 = new(id: "t11", new() { Verarbeitungsschritt.LACKIEREN, Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });

        Util.ForceTeile(konfiguration.Eingangslager, new() { });
        Util.ForceStatus(konfiguration.Eingangslager, Status.EMPFANGSBEREIT);

        var currentTime = DateTime.Now;

        Util.ForceTeil(konfiguration.FertigungsinselA, t9);
        Util.ForceStatus(konfiguration.FertigungsinselA, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselA, currentTime.AddSeconds(1));

        Util.ForceTeil(konfiguration.FertigungsinselB, t10);
        Util.ForceStatus(konfiguration.FertigungsinselB, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselB, currentTime.AddSeconds(4));

        Util.ForceTeil(konfiguration.FertigungsinselC, t11);
        Util.ForceStatus(konfiguration.FertigungsinselC, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselC, currentTime.AddSeconds(2));

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 14.0);

        var actual = Util.ErmittleTeile(konfiguration.Ausgangslager);

        Assert.AreEqual(3, actual.Count(), "Die Anzahl der Teile im Ausgangslager ist fehlerhaft.");
        Assert.AreEqual(t9, actual.ElementAt(0), "t9 muss zuerst fertig sein.");
        Assert.AreEqual(t10, actual.ElementAt(1), "t10 muss als zweites fertig sein.");
        Assert.AreEqual(t11, actual.ElementAt(2), "t11 muss als letztes fertig sein.");

        konfiguration.CheckForEmptyElements();
        Console.WriteLine($"Ausgangslager:{SEPARATOR}{string.Join(SEPARATOR, actual)}");
    }

    [Test]
    public void Test_Verarbeite_Teile_Mit_Eingangslager_Als_Parkplatz_TF6_B()
    {
        Teil t9 = new(id: "t9", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN });
        Teil t10 = new(id: "t10", new() { Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.LACKIEREN, Verarbeitungsschritt.EINLAGERN });
        Teil t11 = new(id: "t11", new() { Verarbeitungsschritt.LACKIEREN, Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });

        Util.ForceTeile(konfiguration.Eingangslager, new() { });
        Util.ForceStatus(konfiguration.Eingangslager, Status.EMPFANGSBEREIT);

        var currentTime = DateTime.Now;

        Util.ForceTeil(konfiguration.FertigungsinselA, t9);
        Util.ForceStatus(konfiguration.FertigungsinselA, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselA, currentTime.AddSeconds(1));

        Util.ForceTeil(konfiguration.FertigungsinselB, t10);
        Util.ForceStatus(konfiguration.FertigungsinselB, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselB, currentTime.AddSeconds(4));

        Util.ForceTeil(konfiguration.FertigungsinselC, t11);
        Util.ForceStatus(konfiguration.FertigungsinselC, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselC, currentTime.AddSeconds(2));

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 14.0);

        var actual = Util.ErmittleTeile(konfiguration.Ausgangslager);

        Assert.AreEqual(3, actual.Count(), "Die Anzahl der Teile im Ausgangslager ist fehlerhaft.");
        Assert.AreEqual(t9, actual.ElementAt(0), "t9 muss zuerst fertig sein.");

        var lasts = actual.TakeLast(2).ToList();

        Assert.Contains(t10, lasts, "t10 muss eines der letzten beiden Teile sein.");
        Assert.Contains(t11, lasts, "t11 muss eines der letzten beiden Teile sein.");

        konfiguration.CheckForEmptyElements();
        Console.WriteLine($"Ausgangslager:{SEPARATOR}{string.Join(SEPARATOR, actual)}");
    }

    [Test]
    public void Test_Verarbeite_Teile_Mit_Eingangslager_In_Reihenfolge_TF7()
    {
        Teil t12 = new(id: "t12", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN });
        Teil t13 = new(id: "t13", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN });
        Teil t14 = new(id: "t14", new() { Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });
        Teil t15 = new(id: "t15", new() { Verarbeitungsschritt.LACKIEREN, Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });

        Util.ForceTeile(konfiguration.Eingangslager, new() { t12 });
        Util.ForceStatus(konfiguration.Eingangslager, Status.INTERAKTIONSBEREIT);

        var currentTime = DateTime.Now;

        Util.ForceTeil(konfiguration.FertigungsinselA, t13);
        Util.ForceStatus(konfiguration.FertigungsinselA, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselA, currentTime.AddSeconds(1));

        Util.ForceTeil(konfiguration.FertigungsinselB, t14);
        Util.ForceStatus(konfiguration.FertigungsinselB, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselB, currentTime.AddSeconds(4));

        Util.ForceTeil(konfiguration.FertigungsinselC, t15);
        Util.ForceStatus(konfiguration.FertigungsinselC, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselC, currentTime.AddSeconds(2));

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 14.0);

        var actual = Util.ErmittleTeile(konfiguration.Ausgangslager);

        Assert.AreEqual(4, actual.Count(), "Die Anzahl der Teile im Ausgangslager ist fehlerhaft.");
        Assert.AreEqual(t13, actual.ElementAt(0), "t13 muss zuerst fertig sein.");
        Assert.AreEqual(t14, actual.ElementAt(1), "t14 muss als zweites fertig sein.");
        Assert.AreEqual(t12, actual.ElementAt(2), "t12 muss als drittes fertig sein.");
        Assert.AreEqual(t15, actual.ElementAt(3), "t15 muss als letztes fertig sein.");

        konfiguration.CheckForEmptyElements();
        Console.WriteLine($"Ausgangslager:{SEPARATOR}{string.Join(SEPARATOR, actual)}");
    }

    [Test]
    public void Test_Volles_Ausgangslager_TF8()
    {
        Teil t16 = new(id: "t16", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN });

        Util.ForceTeile(konfiguration.Eingangslager, new() { });
        Util.ForceStatus(konfiguration.Eingangslager, Status.EMPFANGSBEREIT);

        List<Teil> teile = new();
        for(int i = 0; i < 20; i++)
            teile.Add(new(id: $"t2_{i}", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN }));
        Util.ForceTeile(konfiguration.Ausgangslager, teile);
        Util.ForceStatus(konfiguration.Ausgangslager, Status.ABHOLBEREIT);

        Util.ForceTeil(konfiguration.FertigungsinselA, t16);
        Util.ForceStatus(konfiguration.FertigungsinselA, Status.BELEGT);

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 0.0);
        Assert.NotNull(Util.ErmittleTeil(konfiguration.Transportfahrzeug), "Bei einem Abbruch bleibt das Teil beim Transportfahrzeug.");
    }

    [Test]
    public void Test_Volles_Eingangslager_TF9()
    {
        Teil t17 = new(id: "t17", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN });
        Teil t18 = new(id: "t18", new() { Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });
        Teil t19 = new(id: "t19", new() { Verarbeitungsschritt.LACKIEREN, Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });

        List<Teil> teile = new();
        for(int i = 0; i < 20; i++)
            teile.Add(new(id: $"t2_{i}", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN }));
        Util.ForceTeile(konfiguration.Eingangslager, teile);
        Util.ForceStatus(konfiguration.Eingangslager, Status.ABHOLBEREIT);

        var currentTime = DateTime.Now;

        Util.ForceTeil(konfiguration.FertigungsinselA, t17);
        Util.ForceStatus(konfiguration.FertigungsinselA, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselA, currentTime.AddSeconds(20));

        Util.ForceTeil(konfiguration.FertigungsinselB, t18);
        Util.ForceStatus(konfiguration.FertigungsinselB, Status.BELEGT);
        Util.ForceBelegtBis(konfiguration.FertigungsinselB, currentTime.AddSeconds(20));

        Util.ForceTeil(konfiguration.FertigungsinselC, t19);
        Util.ForceStatus(konfiguration.FertigungsinselC, Status.BELEGT);

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 0.0);
        Assert.NotNull(Util.ErmittleTeil(konfiguration.Transportfahrzeug), "Bei einem Abbruch bleibt das Teil beim Transportfahrzeug.");
    }

    [Test]
    public void Test_Ein_Normaler_Drucfhlauf_Mit_Drei_TEilen_TF10()
    {
        Teil t20 = new(id: "t20", new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.EINLAGERN });
        Teil t21 = new(id: "t21", new() { Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.EINLAGERN });
        Teil t22 = new(id: "t22", new() { Verarbeitungsschritt.LACKIEREN, Verarbeitungsschritt.EINLAGERN });

        Util.ForceTeile(konfiguration.Eingangslager, new() { t20, t21, t22 });
        Util.ForceStatus(konfiguration.Eingangslager, Status.INTERAKTIONSBEREIT);

        Util.AssertRun(() => konfiguration.CallHauptprozess(), 10.0);

        var actual = Util.ErmittleTeile(konfiguration.Ausgangslager).ToList();

        Assert.Contains(t20, actual, "t20 muss im Ausgangslager sein.");
        Assert.Contains(t21, actual, "t21 muss im Ausgangslager sein.");
        Assert.Contains(t22, actual, "t22 muss im Ausgangslager sein.");

        konfiguration.CheckForEmptyElements();
        Console.WriteLine($"Ausgangslager:{SEPARATOR}{string.Join(SEPARATOR, actual)}");
    }
}
