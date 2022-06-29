using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Diagnostics;
using System.Threading;

namespace Beleg2022
{
    /// <summary>
    /// Klasse der Abteilungssteuerung
    /// Diese Klasse wird vom Framework vorgegeben.
    /// Hier bitte nichts verändern
    /// </summary>
    public class Abteilungssteuerung
    {
        /// <summary>
        /// Datenstruktur für das Transportfahrzeug
        /// </summary>
        private readonly Transportsystem? _Transportsystem = null;

        /// <summary>
        /// Diese Datenstruktur soll alle Produktionseinrichtungen enthalten, nachdem diese initialisiert wurden!
        /// Die entsprechende Datei (Konfiguration.csv) besteht aus zwei Spalten. Die erste/linke Spalte enthaelt dabei
        /// den Namen der Produktionseinrichtung. Die zweite/rechte Spalte der Datei 
        /// enthaelt die ID der jeweiligen Produktionseinrichtung, diese wird beim Erzeugen benoetigt.  
        /// </summary>
        private readonly List<Produktionseinrichtung> _Produktionseinrichtungen = new List<Produktionseinrichtung>();

        /// <summary>
        /// Default Konstruktor, für den Fall, das ohne entsprechenden Pfad aufgerufen wird. 
        ///
        /// Achten Sie darauf, bei der Pfadangabe, keine absoluten
        /// (c:User/IhrName/) Pfade zu verwenden, vermeiden Sie auch
        /// Bachslashes ( \ ) da diese ausschließlich auf Windows
        /// funktionieren, Ihre Abgabe aber nicht auf Windows erfolgt!
        /// </summary>
        public Abteilungssteuerung() : this(@"../../../Konfiguration.csv")
            {}
        /// <summary>
        /// Konstruktor der Abteilungssteuerung. Dieser wird aufgerufen, wenn Abteilungssteuerungen
        /// mit entsprechendem Parameter aufgerufen wird
        /// </summary>
        /// <param name="path"> Wenn Sie den Konstruktor mit einem absoluten Pfad aufrufen wollen, ist dies der richtige Konstruktor für Sie</param>
        public Abteilungssteuerung(String path)
        {
            // Produktionseinrichtungen nach Konfigurationsdatei erzeugen
            InitialisiereAbteilung(path);
            // Erzeugen eines Transferfahrzeuges (Unterklasse von Transportsystem)
            _Transportsystem = (Transportsystem)Beleg2022._Internal.Anlegen(typeof(Transportsystem), "Transportfahrzeug", "Transportfahrzeug_X1", "A1");
            // Simulationsfunktion starten
            try
            {
                _Transportsystem?.StarteHauptprozess(_Produktionseinrichtungen);
            }catch(NotImplementedException)
            {
                Console.WriteLine("Ihr Hauptprozess ist noch nicht implementiert (wird eine NotImplementedException), " +
                    "diese Meldung verschwindet, sobald Sie den Hauptprozess implementieren (bzw. das " +
                    "throw new NotImplementedException() entfernen. ");
            }
        }

        /// <summary>
        /// Diese Methode ist nur ein einfacher Wrapper und befindet sich an dieser Stelle nur, damit Sie das EKD nicht für fehlerhaft halten. 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        internal object Anlegen(Type parent, string type, string name, string position) 
        {
            return Beleg2022._Internal.Anlegen(parent, type, name, position);
        }

        /// <summary>
        /// Funktion zum Einlesen der Produktionseinrichtungen
        /// Die Produktionseinrichtungen werden erzeugt und in der Liste der Produktionseinrichtungen abgelegt
        /// </summary>
        /// <param name="pfad">Pfad der Konfigurationsdatei</param>
        public void InitialisiereAbteilung(string pfad)
        {
            _Internal.Ausgabe("Abteilung-Konfiguration wird eingelesen");
            // Objekt zum Einlesen von Dateien wird angelegt

            StreamReader? reader = null;
            try
            {
                reader = new StreamReader(pfad);
                // Schleife bis zum Ende der Datei
                while (!reader.EndOfStream)
                {
                    // einzelne Zeile einlesen 
                    String line = reader.ReadLine() ?? string.Empty;
                    // Teile der Zeile werden getrennt (Semikolon ist Trennzeichen)
                    String[] values = line.Split(';');
                    // die erste Spalte gibt die klasse an
                    String objektKlasse = values[0];
                    // die zweite Spalte gibt den Namen an
                    String objektName = values[1];
                    // die dritte Spalte gibt die Position an
                    String objektPos = values[2];
                    // die vierte Spalte gibt die Verarbeitungsfähigkeiten an - (Komma ist hier das Trennzeichen)
                    // Teile des Fähigkeiten-Strings werden getrennt (Komma ist Trennzeichen)
                    String[] faehigkeitenArray = values[3].Split(',');
                    // Liste von Verarbeitungsfähigkeiten wird angelegt
                    List<Verarbeitungsschritt> faehigkeitenListe = new List<Verarbeitungsschritt>();
                    // alle getrennten Fähigkeiten bzw. Prozessschritte werden einzeln bearbeitet
                    foreach (string prozessschritt in faehigkeitenArray)
                    {
                        //Die gefundenen Verarbeitungsschritte werden der FähigkeitenListe hinzugefügt, wenn Sie vom Typ Verarbeitungsschritt sind
                        faehigkeitenListe.Add((Verarbeitungsschritt)Enum.Parse(typeof(Verarbeitungsschritt), prozessschritt));
                    }

                    // Das Objekt der speziellen Produktionseinrichtungen wird angelegt mit Name und Position
                    Object abteilungsobjekt = _Internal.Anlegen(typeof(Produktionseinrichtung), objektKlasse, objektName, objektPos);
                    if (abteilungsobjekt != null)
                    {
                        // die Produktionseinrichtungen werden der Liste der Produktionseinrichtungen hinzugefügt
                        _Produktionseinrichtungen.Add((Produktionseinrichtung)abteilungsobjekt);
                        // die Verarbeitungsfähigkeiten werden der Produktionseinrichtung hinzugefügt
                        _Produktionseinrichtungen.ElementAt(_Produktionseinrichtungen.Count - 1).SetFaehigkeiten(faehigkeitenListe);
                        _Internal.Ausgabe(((Produktionseinrichtung)abteilungsobjekt).GetName() + " wird hinzugefügt");

                    }
                }
                reader.Close();
            }
            catch (FileNotFoundException ioex)
            {
                Console.WriteLine(ioex.Message);
                Console.WriteLine("Überprüfen Sie, die relativen Pfade! Im Vorgabeprojekt sollten diese Korrekt sein," +
                    "wenn das bei Ihnen nicht so ist, melden Sie sich bei einem Tutor oder im Forum! ");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}

