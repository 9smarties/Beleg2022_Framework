namespace Beleg2022_Framework.TA42Hauptprozess;

[TestFixture]
public class TesteHauptprzessEinfach
{
    private static readonly string SEPARATOR = Environment.NewLine + "\t";

    private Util.Konfiguration konfiguration = new();

    [SetUp]
    public void Setup()
    {
        Util.RunSilent(() => {
            konfiguration = new() {
                Eingangslager = Util.CreateProduktionseinrichtung<Eingangslager>("TestLagerE", "A0"
            , new() { Verarbeitungsschritt.INITIALISIEREN }),

                Ausgangslager = Util.CreateProduktionseinrichtung<Ausgangslager>("TestLagerA", "A2"
            , new() { Verarbeitungsschritt.EINLAGERN }),

                FertigungsinselA = Util.CreateProduktionseinrichtung<Fertigungsinsel>("FraesBohrer", "B1"
            , new() { Verarbeitungsschritt.FRAESEN, Verarbeitungsschritt.BOHREN }),
                FertigungsinselB = Util.CreateProduktionseinrichtung<Fertigungsinsel>("SchweissBohrer", "B2"
            , new() { Verarbeitungsschritt.SCHWEISSEN, Verarbeitungsschritt.BOHREN }),
                FertigungsinselC = Util.CreateProduktionseinrichtung<Fertigungsinsel>("Lackierer", "B3"
            , new() { Verarbeitungsschritt.LACKIEREN }),

                Transportfahrzeug = new("Willi", "A0")
            };
        });
    }

    [Test]
    public void Test_Finde_Eingangslager_Und_Ausgangslager_Allgemein()
    {
        Teil expected = new(id: "ID0001", new() { Verarbeitungsschritt.EINLAGERN });

        Util.RunSilent(() => {
            Util.ForceTeile(konfiguration.Eingangslager, new() { expected });
            Util.ForceStatus(konfiguration.Eingangslager, Status.ABHOLBEREIT);

            konfiguration.CallHauptprozess();
        });

        var actual = Util.ErmittleTeile(konfiguration.Ausgangslager);

        Assert.Contains(expected, actual.ToList(), "Nach der vollständigen Abarbeitung müssen sich alle Teile im Ausgangslager befinden.");
        Assert.AreEqual(1, actual.Count(), "Die Anzahl der Teile im Ausgangslager ist fehlerhaft.");

        konfiguration.CheckForEmptyElements();
        Console.WriteLine($"Ausgangslager:{SEPARATOR}{string.Join(SEPARATOR, actual)}");
    }

    [Test]
    public void Test_Finde_Fertigungsinsel_Allgemein()
    {
        Teil expected = new(id: "ID0001", new() { Verarbeitungsschritt.LACKIEREN, Verarbeitungsschritt.EINLAGERN });

        Util.RunSilent(() => {
            Util.ForceTeile(konfiguration.Eingangslager, new() { expected });
            Util.ForceStatus(konfiguration.Eingangslager, Status.ABHOLBEREIT);

            konfiguration.CallHauptprozess();
        });

        var actual = Util.ErmittleTeile(konfiguration.Ausgangslager);

        Assert.Contains(expected, actual.ToList(), "Nach der vollständigen Abarbeitung müssen sich alle Teile im Ausgangslager befinden.");
        Assert.AreEqual(1, actual.Count(), "Die Anzahl der Teile im Ausgangslager ist fehlerhaft.");

        konfiguration.CheckForEmptyElements();
        Console.WriteLine($"Ausgangslager:{SEPARATOR}{string.Join(SEPARATOR, actual)}");
    }
}
