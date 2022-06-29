using System.Threading.Tasks;

namespace Beleg2022_Framework.TA42Hauptprozess;

internal static class Util
{
    public static T CreateProduktionseinrichtung<T>(string name, string position
        , List<Verarbeitungsschritt> faehigkeiten)
        where T : Produktionseinrichtung
    {
        T @out = (T)Activator.CreateInstance(typeof(T), new object[] { name, position })!;
        @out.SetFaehigkeiten(faehigkeiten);

        return @out!;
    }

    public static void RunSilent(Action action)
    {
        var buffer = Console.Out;
        Console.SetOut(new StreamWriter(Stream.Null));

        action?.Invoke();

        Console.SetOut(buffer);
    }

    public static void AssertRun(Action action, double expectedTimeSeconds, int maxTimeSeconds = 30, double toleranceSeconds = 0.5)
    {
        var buffer = Console.Out;
        Console.SetOut(new StreamWriter(Stream.Null));

        StopWatch stopWatch = new(toleranceSeconds);
        if(!Task.Run(action).Wait(maxTimeSeconds * 1000))
            Assert.Fail($"Die Funktion brauchte wesentlich mehr Zeit als erwartet und wurde abgebrochen. Prüfen Sie Ihre Abbruchbedingung.");
        stopWatch.Stop(expectedTimeSeconds);

        Console.SetOut(buffer);
    }

    public static Status ErmittleStatus(Produktionseinrichtung? produktionseinrichtung)
        => Enum.Parse<Status>(
                produktionseinrichtung?.GetType()
                .GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(produktionseinrichtung)?.ToString()
                ?? "UNDEFINIERT"
            );

    public static IEnumerable<Teil> ErmittleTeile(Lager? lager)
        => lager?.GetType()
        .GetField("_Bestand", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(lager) as IEnumerable<Teil>
        ?? new List<Teil>();

    public static Teil? ErmittleTeil(Fertigungsinsel? fertigungsinsel)
        => fertigungsinsel?.GetType()
        .GetField("_AktuellesTeil", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(fertigungsinsel) as Teil;

    public static Teil? ErmittleTeil(Transportfahrzeug? transportfahrzeug)
        => transportfahrzeug?.GetType()
        .GetField("_AktuellesTeil", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(transportfahrzeug) as Teil;

    public static void ForceTeile(Lager? lager, List<Teil> bestand)
    {
        var _bestand = lager?.GetType()
        .GetField("_Bestand", BindingFlags.Instance | BindingFlags.NonPublic);

        _bestand?.SetValue(lager, Activator.CreateInstance(_bestand.FieldType, new object[] { bestand }) as IEnumerable<Teil>);
    }

    public static void ForceStatus(Produktionseinrichtung? produktionseinrichtung, Status status)
        => produktionseinrichtung?.GetType()
        .GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(produktionseinrichtung, status);

    public static void ForceBelegtBis(Fertigungsinsel? fertigungsinsel, DateTime time)
        => fertigungsinsel?.GetType()
        .GetField("_BelegtBis", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(fertigungsinsel, time);

    public static void ForceTeil(Fertigungsinsel? fertigungsinsel, Teil? teil)
        => fertigungsinsel?.GetType()
        .GetField("_AktuellesTeil", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(fertigungsinsel, teil);

    public record Konfiguration
    {
        public Eingangslager? Eingangslager;
        public Ausgangslager? Ausgangslager;

        public Fertigungsinsel? FertigungsinselA;
        public Fertigungsinsel? FertigungsinselB;
        public Fertigungsinsel? FertigungsinselC;

        public Transportfahrzeug? Transportfahrzeug;

        public void CallHauptprozess()
            => Transportfahrzeug?.StarteHauptprozess(Produktionseinrichtungen());

        public List<Produktionseinrichtung> Produktionseinrichtungen()
            => new()
            {
                Eingangslager ?? new Default() as Produktionseinrichtung,
                Ausgangslager ?? new Default() as Produktionseinrichtung,
                FertigungsinselA ?? new Default() as Produktionseinrichtung,
                FertigungsinselB ?? new Default() as Produktionseinrichtung,
                FertigungsinselC ?? new Default() as Produktionseinrichtung
            };

        public void CheckForEmptyElements()
        {
            Assert.IsEmpty(ErmittleTeile(Eingangslager)
                , "Nach der vollständigen Verarbeitung darf sich kein Teil mehr im Eingagnslager befinden.");
            Assert.IsNull(ErmittleTeil(FertigungsinselA)
                , "Nach der vollständigen Verarbeitung darf sich kein Teil mehr in einer Fertigungsinsel befinden.");
            Assert.IsNull(ErmittleTeil(FertigungsinselB)
                , "Nach der vollständigen Verarbeitung darf sich kein Teil mehr in einer Fertigungsinsel befinden.");
            Assert.IsNull(ErmittleTeil(FertigungsinselC)
                , "Nach der vollständigen Verarbeitung darf sich kein Teil mehr in einer Fertigungsinsel befinden.");
            Assert.IsNull(ErmittleTeil(Transportfahrzeug)
                , "Nach der vollständigen Verarbeitung darf sich kein Teil mehr beim Transportfahrzeug befinden.");
        }

        private class Default : Produktionseinrichtung
        {
            public Default() : base("DEFAULT", "0")
            {
            }

            public override Status ErmittleStatus()
                => Status.UNDEFINIERT;
        }
    }

    private class StopWatch
    {
        private double _tolerance;

        private DateTime _timer = DateTime.Now;

        /// <summary>
        /// A StopWatch starts instantly - with a possible tollerance - (in seconds) to calculate the expected time.
        /// </summary>
        /// <param name="toleranceSeconds"></param>
        public StopWatch(double toleranceSeconds)
            => _tolerance = toleranceSeconds * 1000;

        /// <summary>
        /// Stopts the watch and checks for validity, with the given tolerance.
        /// If the expected time in seconds is not given, this method calls an Assertion. 
        /// </summary>
        /// <param name="expectedTimeSeconds"></param>
        public void Stop(double expectedTimeSeconds)
            => Assert.LessOrEqual(
                Math.Abs((DateTime.Now - _timer).TotalMilliseconds - (expectedTimeSeconds * 1000))
                , _tolerance
                , $"Der Vorgang darf nicht wesentlich länger oder kürzer dauern als {expectedTimeSeconds} Sekunden.");
    }
}
