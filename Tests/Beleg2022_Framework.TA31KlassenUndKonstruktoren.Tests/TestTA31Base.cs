using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Beleg2022.Tests
{
    [TestFixture]
    public class TestTa31Base
    {
        protected IEnumerable<Type> TypesExistingInCode = null!;

        [OneTimeSetUp]
        public void GetClassesFromAssembly()
        {

            IEnumerable<Type> tx = typeof(Abteilungssteuerung).Assembly.GetTypes().Where(t => (t.IsClass || t.IsEnum) && t.Name != "Program" && !t.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), true));
            tx = tx.Concat(
                typeof(Produktionseinrichtung).Assembly.GetTypes().
                    Where(t =>
                            (t.IsClass || t.IsEnum) &&
                            t.Name != "_Internal" &&
                            !t.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), true)
                    )
                );
            this.TypesExistingInCode = tx;

        }
    }
}