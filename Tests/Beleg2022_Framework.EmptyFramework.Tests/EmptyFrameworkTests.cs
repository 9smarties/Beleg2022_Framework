using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace Beleg2022.Tests
{
    [TestFixture]
    public class EmptyFrameworkTests
    {
        /// <summary>
        /// Diese Methode ist nur ein Wrapper und selbst kein Test
        /// </summary>
        /// <param name="funcToRun"></param>
        /// <param name="ex"></param>
        private static void SafeExec(Action funcToRun, out Exception? ex)
        {
            ex = null;
            try
            {
                funcToRun.Invoke();
            }
            catch (Exception? e)
            {
                ex = e;
            }
        }

        /// <summary>
        /// Dieser Test sollte niemals fehlschlagen, es sei denn, Sie haben Programm umbenannt, bzw.
        /// strukturell sehr verändert
        /// </summary>
        [MaxTime(1000)]
        [Test]
        public void Test_Program_NewProgram__ObjectCreated()
        {
            try
            {
                Program prog = new Program();
            }
            catch (Exception)
            {
                Assert.Fail("Wenn dieser Test fehlschlägt, haben Sie an Ihrem Projekt etwas " +
                            "ganz grundsätzliches kaputt gemacht. Schauen Sie nach, ob die Klasse Program " +
                            "Syntaxfehler enthält. Haben Sie die Klasse Program evtl. umbenannt? Das kann " +
                            "diesen Fehler verursachen");
            }

            Assert.Pass();
        }

        /// <summary>
        /// Dieser Test Versucht die Main-Methode zu starten. Er wird bestanden, wenn das 
        /// Programm nach 10 Sekunden noch läuft ohne bis dahin eine Exception geworfen zu haben,
        /// oder wenn es innerhalb dieser Zeitspanne terminiert. Das Ihr Programm noch läuft ist
        /// an dieser Stelle O.K. (läuft wirklich noch oder Sie nutzen einen ReadKey um das Programm
        /// irgendwo anzuhalten. Wenn eine Exception geworfen wurde (die NICHT SuccessException heißt)
        /// sollten Sie sich diese genau anschauen. 
        /// </summary>
        [MaxTime(15000)]
        // Wenn der Test, aus welchen Gründen auch immer nach 15sekunden immer
        // noch nicht durch ist wird er auch fehlschlagen
        [Test]
        public void Test_Program_Main_InvokeMain_NoException()
        {
            Exception? exception = null;
            try
            {
                //Thread t = new Thread(() => Beleg2022.Program.Main(null));
                Thread t = new Thread(() => SafeExec(() => Beleg2022.Program.Main(null), out exception));
                t.Start();
                Thread.Sleep(1000);
                if (t.IsAlive)
                {
                    Thread.Sleep(9000);
                    t.Interrupt();
                }
                else
                    t.Join();

                if (exception is IOException or NullReferenceException or ArgumentNullException
                    or FileNotFoundException)
                    throw exception;

                Assert.Pass("Ihr Programm wurde erfolgreich beendet oder unterbrochen.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("\n Ihr Code hat eine unbehandelte FileNotFound Exception" + ex);
                Assert.Fail("Ihr Code hat eine unbehandelte FileNotFound Exception");
            }
            catch (IOException ex)
            {
                Console.WriteLine("\nIhr Code hat eine unbehandelte IO Exception" + ex);
                Assert.Fail("Ihr Code hat eine unbehandelte IO Exception");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("\nIhr Code hat eine unbehandelte NullReference Exception" + ex);
                Assert.Fail("Ihr Code hat eine unbehandelte NullReference Exception");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("\nIhr Code hat eine unbehandelte ArgumentNull Exception" + ex);
                Assert.Fail("Ihr Code hat eine unbehandelte ArgumentNull Exception");
            }
            catch (SuccessException)
            {
                Console.WriteLine("\nAuch wenn das komisch klingt, diese Exception sagt, das alles ok ist\n");
                Assert.Pass("Auch wenn das komisch klingt, diese Exception sagt, das alles ok ist");
            }
            catch (Exception ex)
            {
                Assert.Fail("\nHier ist etwas schief gelaufen. Schauen Sie sich die Exception genau an!\n" +
                            ex.ToString());
            }
        }

        /// <summary>
        /// Dieser Test prüft, ob die Abteilungssteuerung ganz grundsätzlich noch funktioniert.
        /// Wenn dieser Test fehlschlägt, haben Sie etwas unbeabsichtigtes getan. 
        /// </summary>
        [MaxTime(10000)]
        [Test]
        public void Test_Abteilungssteuerung_NewAbteilungssteuerung_RegularFile_ObjectCreated()
        {
            Abteilungssteuerung? prog = null;
            Exception? exception = null;
            bool wasRunningForMoreThanABlink = false;
            try
            {
                Thread t = new Thread(() =>
                    SafeExec(() => prog = new Abteilungssteuerung(@"../../../Konfiguration.csv"), out exception));
                t.Start();
                Thread.Sleep(1000);
                if (t.IsAlive)
                {
                    Thread.Sleep(4000);
                    t.Interrupt();
                    wasRunningForMoreThanABlink = true;
                }
                else
                    t.Join();
                if (exception is NotImplementedException)
                    throw exception;
                
            }
            catch (NotImplementedException? ex)
            {
                Console.WriteLine("Ihre Implementierung wirft z.Z. eine NotImplementedException, das " +
                                  "kann von Ihnen beabsichtigt sein. In dem Fall, verschwindet diese Meldung, sobald die" +
                                  "entsprechende Methode implementieren. " + ex.ToString());
                exception = ex;
            }
            finally
            {
                if (prog is null && !wasRunningForMoreThanABlink)
                {
                    if (exception == null)
                        Assert.Fail("Wenn dieser Test fehlschlägt, haben Sie an Ihrem Projekt etwas " +
                                    "ganz grundsätzliches kaputt gemacht. Schauen Sie nach, ob die Klasse Abteilungssteuerung " +
                                    "Syntaxfehler enthält");
                    else
                        Assert.Fail("Dieser Test schlägt Momentan fehl da Sie irgendwo eine NotImplementedException " +
                                    "werfen. Wenn das von Ihnen beabsichtigt is, kann das vorübergehend OK sein, dann wird " +
                                    "dieser Test wieder bestanden werden, sobald Sie die NotImplementedException entfernen.\n" +
                                    exception.ToString());
                }
                else
                    Assert.Pass();
            }
        }
     

        [MaxTime(1000)]
        [Test]
        public void Test_Abteilungssteuerung_NewAbteilungssteuerung_FileDoesNotExist_Exit()
        {
            Abteilungssteuerung? prog = null;
            Exception? exception = null;
            bool softFail = false;
            string msg = "";
            bool wasRunningForMoreThanABlink = false;
            try
            {
                //prog = new Abteilungssteuerung(@"../../../foobar.csv");
                Thread t = new Thread(() =>
                    SafeExec(() => prog = new Abteilungssteuerung(@"../../../foobar.csv"), out exception));
                Console.SetOut(new StreamWriter(System.IO.Stream.Null));
                t.Start();
                Thread.Sleep(2000);
                if (t.IsAlive)
                {
                    Thread.Sleep(3000);
                    t.Interrupt();
                    wasRunningForMoreThanABlink = true;
                    msg = "\n\nObwohl ihr Code mit einem nicht vorhandenen File \n" +
                          "aufgerufen wurde und wahrscheinlich, die richtige Exception \n" +
                          "wirft, läuft er noch. Das muss an dieser Stelle noch kein \n" +
                          "Problem sein, kann aber darauf hindeuten, dass die Abbruchbedingung\n" +
                          "in einer Schleife nicht richtig ist.";
                }
                else
                    t.Join();

                Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
                if (exception is NullReferenceException or NotImplementedException or FileNotFoundException)
                    throw exception;
            }
            catch (NotImplementedException ex)
            {
                Console.WriteLine("Ihre Implementierung wirft z.Z. eine NotImplementedException, das " +
                                  "kann von Ihnen beabsichtigt sein. In dem Fall, verschwindet diese Meldung, sobald die" +
                                  "entsprechende Methode implementieren. ");
                exception = ex;
            }
            catch (NullReferenceException ex)
            {
                msg = "\n\nIhr Code erzeugt gerade eine NullReferenceException bei diesem \n" +
                      "Test. Wenn sie die Not Implemented Exception im Hauptprozess bereits \n" +
                      "entfernt haben und dort implementieren bzw. implementiert haben, ist \n" +
                      "dieses Verhalten zu erwarten, der Test wird dennoch bestanden. Sollte \n" +
                      "diese Exception an einer anderen Stelle geworfen werden müssen Sie sich \n" + "darum kümmern.";
                exception = ex;
                if (ex.StackTrace != null && ex.StackTrace.Contains("Transportfahrzeug.Hauptprozess")) 
                    softFail = true;
            }
            catch (FileNotFoundException ex)
            {
                if (ex.StackTrace != null && !ex.StackTrace.Contains("Eingangslager")) exception = null;
            }
            finally
            {
                if (prog is null)
                {
                    if (wasRunningForMoreThanABlink) 
                        Assert.Pass(msg);

                    if (exception == null)
                        Assert.Fail("Dieser Test sollte nie Fehlschlagen, es sei denn, Sie haben den/einen" +
                                    "Try-Catch-Block entfernt.");
                    else if (softFail)
                    {
                        Console.WriteLine(msg + " \n\n" + exception.ToString());
                        Assert.Pass(msg);
                    }
                    else
                        Assert.Fail("Dieser Test schlägt Momentan fehl da Sie irgendwo eine Exception " +
                                    "werfen. Wenn das von Ihnen beabsichtigt is, kann das vorübergehend OK sein, dann wird " +
                                    "dieser Test wieder bestanden werden, sobald Sie die Exception entfernen.\n" +
                                    exception.ToString());
                }
                else
                    Assert.Pass("Alles Ok hier");
            }
        }
    }
}
