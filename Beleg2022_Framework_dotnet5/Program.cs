using System;

namespace Beleg2022
{
    /// <summary>
    /// Dies ist der Einstiegspunkt für das Progamm.
    /// Bitte ändern Sie hier nichts.
    /// Durch Schließen des Fensters beenden Sie das Programm.
    /// </summary>
    /// ///
    public class Program
    {
        public static void Main(string?[]? args)
        {
         
            // Achten Sie darauf, bei der Pfadangabe, keine absoluten
            // (c:User/IhrName/) Pfade zu verwenden, vermeiden Sie auch
            // Bachslashes ( \ ) da diese ausschließlich auf Windows
            // funktionieren, Ihre Abgabe aber nicht auf Windows erfolgt!
            
            Beleg2022.Abteilungssteuerung verwaltung = new Abteilungssteuerung(@"../../../Konfiguration.csv");            
            

#if DEBUG
            Console.WriteLine("\nProgram exits. Push any key to continue ...");
            Console.ReadKey();
#endif
        }
    }
}
