using System;
using System.Collections.Generic;
using System.Collections;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RobotBlocks.Tests.Generator
{
  [TestClass]
  public class SQLServerGeneratorTest
  {
    [TestMethod]
    public void TestSQLServerGenerator()
    {
      RobotBlocks.Implementation.SQLServer.DatabaseGenerator g = new Implementation.SQLServer.DatabaseGenerator();
      List<string> finalSources = new List<string>()
      {
        TestResources.Account,
        TestResources.Employee,
        TestResources.EmployeeJobApplicantRole,
        TestResources.Entity,
        TestResources.History,
        TestResources.Job, 
        TestResources.JobApplicant,
        TestResources.JobApplicantMembership,
        TestResources.JobApplicantScreeningTest,
        TestResources.JobApplicantScreeningTestAnswer,
        TestResources.JobApplicantScreeningTestStatus,
        TestResources.JobApplication,
        TestResources.JobApplicationHistory,
        TestResources.JobApplicationStatus,
        TestResources.JobHistory,
        TestResources.Keyword,
        TestResources.Question,
        TestResources.QuestionKeyword,
        TestResources.ScreeningTest,
        TestResources.ScreeningTestQuestion,
        TestResources.User,
        TestResources.UserStatus
      };

      Assert.AreEqual(true, Core.Director<string>.Generate("test", 
        new RobotBlocks.Core.DatabaseGeneratorInput<string>( 
        new RobotBlocks.Implementation.SQLServer.DatabaseGenerator(), 
        new RobotBlocks.Implementation.SQLServer.DatabaseDiffer()),
        finalSources.ToArray(), Core.Director<string>.GenerateType.SOURCE).IsSuccessful);
    }
  }
}
