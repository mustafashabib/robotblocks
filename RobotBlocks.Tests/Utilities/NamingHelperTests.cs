using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotBlocks.Utilities;

namespace RobotBlocks.Tests.Utilities
{
  internal class NamingHelperTestCase
  {
    internal string Singular { get; set; }
    internal string Plural { get; set; }
    internal NamingHelperTestCase(string singular, string plural)
    {
      Singular = singular;
      Plural = plural;
    }
  }


  [TestClass]
  public class NamingHelperTests
  {
    private static readonly IEnumerable<NamingHelperTestCase> TEST_CASES = 
        new List<NamingHelperTestCase>(){
          new NamingHelperTestCase("cat", "cats"),
          new NamingHelperTestCase("dog", "dogs"),
          new NamingHelperTestCase("status", "statuses"),
          new NamingHelperTestCase("bee", "bees"),
          new NamingHelperTestCase("person", "people"),
          new NamingHelperTestCase("Person", "People"),
          new NamingHelperTestCase("Goose", "Geese"),
          new NamingHelperTestCase("Entity", "Entities"),
          new NamingHelperTestCase("fly", "flies"),
          new NamingHelperTestCase("deer", "deer")
        };

    [TestMethod]
    public void TestSingularize()
    {
      foreach(NamingHelperTestCase testCase in TEST_CASES)
      {
        Assert.AreEqual(testCase.Plural.ToSingular(), testCase.Singular);
      }
    }

    [TestMethod]
    public void TestPluralize()
    {
      foreach(NamingHelperTestCase testCase in TEST_CASES)
      {
        Assert.AreEqual(testCase.Singular.ToPlural(), testCase.Plural);
      }
    }
  }
}
