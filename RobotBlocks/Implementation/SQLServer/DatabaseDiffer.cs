using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using ErikEJ.SqlCeScripting;

namespace RobotBlocks.Implementation.SQLServer
{
  public class DatabaseDiffer : Interfaces.IDatabaseDiffer
  {
    private static Common.Logging.ILog LOG = Utilities.Logging.Logger.GetLogger<DatabaseDiffer>();

    private const string ROBOT_BLOCKS_TAG = "--===ROBOTBLOCKS===--";
    
    public string GetDifferences(string fromDatabase, string toDatabase)
    {
      string fromDatabaseSQL = GetSQL(fromDatabase);
      string toDatabaseSQL = GetSQL(toDatabase);


      try
      {
        System.IO.File.Delete("fromDB.sdf");
        System.IO.File.Delete("toDB.sdf");

        string fromConnectionString = string.Format("DataSource=fromDB.sdf;Persist Security Info=False;");
        SqlCeEngine fromEngine = new SqlCeEngine(fromConnectionString);
        fromEngine.CreateDatabase();


        using (System.Data.SqlServerCe.SqlCeConnection connection = new SqlCeConnection(fromConnectionString))
        {
          connection.Open();
          using (System.Data.SqlServerCe.SqlCeTransaction transaction = connection.BeginTransaction())
          {
            using (System.Data.SqlServerCe.SqlCeCommand command = new System.Data.SqlServerCe.SqlCeCommand(fromDatabaseSQL, connection, transaction))
            {
              command.ExecuteNonQuery();
              transaction.Commit();
            }
          }
        }


        string toConnectionString = string.Format("DataSource=toDB.sdf;Persist Security Info=False;");
        SqlCeEngine toEngine = new SqlCeEngine(toConnectionString);


        using (System.Data.SqlServerCe.SqlCeConnection connection = new SqlCeConnection(toConnectionString))
        {
          connection.Open();
          using (System.Data.SqlServerCe.SqlCeTransaction transaction = connection.BeginTransaction())
          {
            using (System.Data.SqlServerCe.SqlCeCommand command = new System.Data.SqlServerCe.SqlCeCommand(toDatabaseSQL, connection, transaction))
            {
              command.ExecuteNonQuery();
              transaction.Commit();
            }
          }
        }

        return CreateSqlDiffScript(fromConnectionString, toConnectionString);
      }
      catch (Exception ex)
      {
        LOG.Error("Could not generate diff script.", ex);
      }
      finally
      {
        System.IO.File.Delete("fromDB.sdf");
        System.IO.File.Delete("toDB.sdf");
      }
      return null;
    }

    private string GetSQL(string source)
    {
      string allText = System.IO.File.ReadAllText(source);
      int robotBlocksTagPosition = allText.IndexOf(ROBOT_BLOCKS_TAG);
      return allText.Substring(robotBlocksTagPosition + ROBOT_BLOCKS_TAG.Length);
    }

    private string CreateSqlDiffScript(string source, string target)
    {
      using (IRepository sourceRepository = new DB4Repository(source))
      {
        var diffGenerator = new Generator4(sourceRepository);
        using (IRepository targetRepository = new DB4Repository(target))
        {
          SqlCeDiff.CreateDiffScript(sourceRepository, targetRepository, diffGenerator, false);
          return diffGenerator.GeneratedScript;
        }
      }
    }
  }
}
