using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Beleg2022.Tests
{
    //[TestFixture]
    public class TestsTA34Base
    {

        protected readonly List<Teil>? BestandFake = new()
        {
            new Teil("ID01", new List<Verarbeitungsschritt>()
                    {
                        Verarbeitungsschritt.BOHREN,
                        Verarbeitungsschritt.FRAESEN,
                        Verarbeitungsschritt.SCHWEISSEN,
                        Verarbeitungsschritt.LACKIEREN,
                        Verarbeitungsschritt.EINLAGERN
                    }
                ),
            new Teil("ID02", new List<Verarbeitungsschritt>()
                    {
                        Verarbeitungsschritt.FRAESEN,
                        Verarbeitungsschritt.SCHWEISSEN,
                        Verarbeitungsschritt.LACKIEREN,
                        Verarbeitungsschritt.EINLAGERN
                    }
                ),
            new Teil("ID03", new List<Verarbeitungsschritt>()
                    {
                        Verarbeitungsschritt.SCHWEISSEN,
                        Verarbeitungsschritt.BOHREN,
                        Verarbeitungsschritt.LACKIEREN,
                        Verarbeitungsschritt.EINLAGERN
                    }
                ),
            new Teil("ID04", new List<Verarbeitungsschritt>()
                    {
                        Verarbeitungsschritt.LACKIEREN,
                        Verarbeitungsschritt.BOHREN,
                        Verarbeitungsschritt.BOHREN,
                        Verarbeitungsschritt.EINLAGERN
                    }
                ),
            new Teil("ID05", new List<Verarbeitungsschritt>()
                    {
                        Verarbeitungsschritt.FRAESEN,
                        Verarbeitungsschritt.SCHWEISSEN,
                        Verarbeitungsschritt.LACKIEREN,
                        Verarbeitungsschritt.EINLAGERN
                    }
                ),
            new Teil("ID06", new List<Verarbeitungsschritt>()
                    {
                        Verarbeitungsschritt.FRAESEN,
                        Verarbeitungsschritt.FRAESEN,
                        Verarbeitungsschritt.SCHWEISSEN,
                        Verarbeitungsschritt.EINLAGERN
                    }
                ),
            new Teil("ID07", new List<Verarbeitungsschritt>()
                    {
                        Verarbeitungsschritt.BOHREN,
                        Verarbeitungsschritt.EINLAGERN
                    }
                ),
            new Teil("ID08", new List<Verarbeitungsschritt>()
                    {
                        Verarbeitungsschritt.FRAESEN,
                        Verarbeitungsschritt.LACKIEREN,
                        Verarbeitungsschritt.EINLAGERN
                    }
                ),
            new Teil("ID09", new List<Verarbeitungsschritt>()
                    {
                        Verarbeitungsschritt.LACKIEREN,
                        Verarbeitungsschritt.EINLAGERN
                    }
                )
        };

        protected Eingangslager? MyLager;
        protected int? MyLagerKapa;
        protected FieldInfo? MyLagerBestandFi;

        protected List<Verarbeitungsschritt> ValidSteps = new List<Verarbeitungsschritt>
        {
            Verarbeitungsschritt.BOHREN,
            Verarbeitungsschritt.FRAESEN,
            Verarbeitungsschritt.SCHWEISSEN,
            Verarbeitungsschritt.LACKIEREN
        };

        /// <summary>
        /// Eine einfache Hilfsfunktion, die das Lager mit der angegebenen
        /// Anzahl an Teilen bestückt, wird ein Lager generiert, das mehr Elemente enthalten soll
        /// als die Liste bietet, wird das letzte Element der Liste wiederholt eingefügt. 
        /// 
        /// !Diese Methode ist nicht sicher, da sie die Kapazität ignoriert!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numbertofill"></param>
        /// <param name="lager"></param>
        /// <param name="teils"></param>
        /// <param name="overFullStatus">default UNDEFINED</param>
        protected void FillLager<T>(int? numbertofill, ref T lager, List<Teil>? teils, Status overFullStatus = Status.UNDEFINIERT) where T : Lager?
        {
            if(lager == null)
                return;
            int lagerKapa = (int)(lager.GetType()
                .GetField("_Kapazitaet", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                ?.GetValue(null) ?? 0);
            FieldInfo? lagerBestandFi = lager.GetType()
                .GetField("_Bestand", BindingFlags.Instance | BindingFlags.NonPublic) ??
                                        throw new InvalidOperationException(
                                            " If this reflections returns null at this point, something is going wrong.");
            ;
            FieldInfo? lagerStatusFi = lager.GetType()
                .GetField("_AktuellerStatus", BindingFlags.Instance | BindingFlags.NonPublic) ??
                                       throw new InvalidOperationException(
                                           " If this reflections returns null at this point, something is going wrong.");

            IEnumerable<Teil> tmpLst = GenerateLagerBestand(numbertofill, teils, lagerBestandFi.FieldType);
            lagerBestandFi.SetValue(lager, tmpLst);
            if(tmpLst.Count() < lagerKapa)
            {
                lagerStatusFi.SetValue(lager, Status.INTERAKTIONSBEREIT);
            }
            else if(tmpLst.Count() == lagerKapa)
            {
                lagerStatusFi.SetValue(lager, Status.ABHOLBEREIT);
            }
            else
            {
                lagerStatusFi.SetValue(lager, overFullStatus);
            }
        }

        /// <summary>
        /// Eine einfache Hilfsfunktion, die das Lager mit der Angegebenen
        /// Anzahl an Teilen bestückt, wird ein Lager generiert, das mehr Elemente enthalten soll
        /// als die Liste bietet, wird das letzte Element der Liste wiederholt eingefügt. 
        /// 
        /// !Diese Methode ist nicht sicher, da sie die Kapazität ignoriert!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numbertofill"></param>
        /// <param name="lager"></param>
        /// <param name="overFullStatus">default UNDEFINED, wird ignoriert wenn numbertofill <= Lager._Kapazitaet</param>
        protected void FillLager<T>(int? numbertofill, ref T lager, Status overFullStatus = Status.UNDEFINIERT) where T : Lager
        {
            this.FillLager(numbertofill, ref lager, null, overFullStatus);
        }

        protected IEnumerable<Teil> GenerateLagerBestand(int? numbertofill, List<Teil>? teils, Type type)
        {
            List<Teil> buffer = teils is null ? new() : new(teils);

            if(teils == null)
            {
                //Random Teile
                for(int i = 0; i < numbertofill; i++)
                {
                    buffer.Add(GetRandomTeil());
                }
            }
            else
            {
                if(teils.Count() < numbertofill)
                {
                    // Rest auffüllen
                    int dif = numbertofill - teils.Count() ?? 0;
                    for(int i = 0; i < dif; i++)
                    {
                        buffer.Add(GetRandomTeil());
                    }
                }
                else
                {
                    buffer = teils.GetRange(0, numbertofill ?? 0);
                }
            }

            return Activator.CreateInstance(type, new object[] { buffer }) as IEnumerable<Teil>
            ?? (Activator.CreateInstance(type) as IEnumerable<Teil>)!;
        }

        protected Teil GetRandomTeil()
        {
            List<Verarbeitungsschritt> l = new();
            Random r = new();
            int numSteps = r.Next(10);
            for(int i = 0; i < numSteps; i++)
            {
                l.Add(ValidSteps[r.Next(ValidSteps.Count())]);
            }
            l.Add(Verarbeitungsschritt.EINLAGERN);
            Teil t = new Teil("Teil_" + DateTime.Now.ToString("hh:mm:ss.ffffff"), l);
            return t;
        }
    }
}