using NUnit.Framework;
using System;
using System.Reflection;

namespace Beleg2022.Tests
{
    public class BearbeiteTeilTestTa41A
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_BearbeiteTeil_Invoke_SetsBelegtBisToNowPlus10Sec()
        {
            // Arrange
            Fertigungsinsel myFertigungsinsel = new("testFertigungsinsel", "XYZ");
            FieldInfo? myFertigungsinselBelegtBisFi = myFertigungsinsel.GetType().GetField("_BelegtBis", BindingFlags.NonPublic | BindingFlags.Instance);
            DateTime now = DateTime.Now;
            // Act
            myFertigungsinsel.GetType()
                .GetMethod("BearbeiteTeil", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(myFertigungsinsel, null);
            //myFertigungsinsel.BearbeiteTeil();
            DateTime myFertigungsinselBelegtBis = (DateTime) (myFertigungsinselBelegtBisFi?.GetValue(myFertigungsinsel) ??
                                                              throw new InvalidOperationException(
                                                                  "Neither BelegtBis nor its Field info should be null at this point"));
            //Assert
            TimeSpan difftime = myFertigungsinselBelegtBis - now;
            if (difftime.Seconds is >= 10 and < 20)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}