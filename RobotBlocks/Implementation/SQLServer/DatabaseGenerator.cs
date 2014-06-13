using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace RobotBlocks.Implementation.SQLServer
{
  public class DatabaseGenerator : RobotBlocks.Interfaces.IDatabaseGenerator
  {
    private static Common.Logging.ILog LOG = Utilities.Logging.Logger.GetLogger<DatabaseGenerator>();
    
    private const string ROBOT_BLOCKS_TAG = "--===ROBOTBLOCKS===--";
    private readonly System.Collections.Generic.Dictionary<Type, string> CLR_TYPES = new Dictionary<Type, string>()
      {
          { typeof(System.String), "NVARCHAR" },
            
          /*required*/
          { typeof(System.Int16), "SMALLINT NOT NULL" },
          { typeof(System.Int32), "INT NOT NULL" },
          { typeof(System.Int64), "BIGINT NOT NULL" },
          { typeof(System.Boolean), "BIT NOT NULL" },
          { typeof(System.Single), "DECIMAL NOT NULL" },
          { typeof(System.Double), "DECIMAL NOT NULL" },
          { typeof(System.Decimal), "DECIMAL(10,2) NOT NULL"},
          { typeof(System.DateTime), "DATETIME NOT NULL"},
          /*nullable*/
          { typeof(System.Nullable<System.Int16>), "SMALLINT NULL" },
          { typeof(System.Nullable<System.Int32>), "INT NULL" },
          { typeof(System.Nullable<System.Int64>), "BIGINT NULL" },
          { typeof(System.Nullable<System.Boolean>), "BIT NULL" },
          { typeof(System.Nullable<System.Single>), "DECIMAL NULL" },
          { typeof(System.Nullable<System.Double>), "DECIMAL NULL" },
          { typeof(System.Nullable<System.Decimal>), "DECIMAL(10,2) NULL"},
          { typeof(System.Nullable<System.DateTime>), "DATETIME NULL"}
      };

    public bool IsMappableType(Type fieldType)
    {
      return CLR_TYPES.ContainsKey(fieldType);
    }
    public string GetColumnDefinition(Core.Column column)
    {

      string columnDefinition = string.Format("{0} {1}", column.Name, CLR_TYPES[column.UnderlyingSystemType]);

      //get length rules if incoming type is a string
      if (column.UnderlyingSystemType == typeof(string) || column.UnderlyingSystemType == typeof(String))
      {
        if (column.Length.HasValue)
        {
          columnDefinition = string.Format("{0}({1})", columnDefinition, column.Length.Value);
        }
        else
        {
          //assume maxlength
          columnDefinition = string.Format("{0}(MAX)", columnDefinition);
        }
      }

      //get nullability first if it wasn't already derived
      if (!columnDefinition.ToLower().Contains(" null"))
      {
        if (column.IsRequired)
        {
          columnDefinition = string.Format("{0} NOT NULL", columnDefinition);
        }
        else
        {
          columnDefinition = string.Format("{0} NULL", columnDefinition);
        }
      }

      if (column.IsPrimaryKey)
      {
        columnDefinition = string.Format("{0} IDENTITY(1,1) PRIMARY KEY", columnDefinition);
      }

      if (column.IsUnique)
      {
        columnDefinition = string.Format("{0} UNIQUE", columnDefinition);
      }


      if (column.DefaultValue != null)
      {
        columnDefinition =
            string.Format("{0} DEFAULT {1}", columnDefinition, column.DefaultValue);
      }


      return columnDefinition;
    }

    public Core.DatabaseGeneratorResult Generate(Core.Database database)
    {
      
      System.Text.StringBuilder finalSQL = new StringBuilder();

      database.UserName = String.IsNullOrWhiteSpace(database.UserName) ? string.Format("{0}_user", database.Name) : database.UserName;
      database.Password = String.IsNullOrWhiteSpace(database.Password) ? GeneratePassword() : database.Password;

      LOG.InfoFormat("Generating database sql creation script.");

      finalSQL.AppendFormat(@"IF EXISTS (SELECT * FROM sys.databases WHERE name = '{0}')
BEGIN
    DROP DATABASE WHERE name = '{0}'
END

CREATE DATABASE [{0}];
", database.Name);

      //log: generating database user/login and assign roles
      finalSQL.AppendFormat(@"CREATE LOGIN [{0}] WITH PASSWORD='{1}', 
DEFAULT_DATABASE=[{2}], CHECK_POLICY=OFF

CREATE USER [{0}] FOR LOGIN [{0}]
EXEC sp_addrolemember N'db_datareader', N'{2}'
EXEC sp_addrolemember N'db_datawriter', N'{2}'


", database.UserName, database.Password, database.Name);

      finalSQL.AppendLine(ROBOT_BLOCKS_TAG);
      LOG.Info("Generating database.");

      foreach (Core.Table t in database.Tables)
      {
        LOG.Info(m=> m("Generating Table: {0}", t.Name));

        System.Text.StringBuilder columnDefinitions = new StringBuilder();
        bool first = true;
        foreach (Core.Column c in t.Columns)
        {
          string columnSQL = GetColumnDefinition(c);
          if (!first)
          {
            columnDefinitions.AppendFormat(@",
{0}", columnSQL);
          }
          else
          {
            first = false;
            columnDefinitions.AppendFormat(@"{0}", columnSQL);
          }
        }

        finalSQL.AppendLine();
        finalSQL.AppendFormat("-- creating table {0}", t.Name);
        finalSQL.AppendLine();
        finalSQL.AppendFormat(@"CREATE TABLE {0}
(
{1}
);
                               ", t.Name, columnDefinitions);

        //generate indices on this table
        finalSQL.AppendLine();

        if (t.Indices.Any())
        {
          finalSQL.AppendLine(string.Format("-- indexes for {0}", t.Name));
        }
        foreach (Core.Index index in t.Indices)
        {
          finalSQL.AppendFormat(@"CREATE {0} INDEX IX_{1}_{2}
ON {1} ({3});
",
            GetIndexTypeAsString(index),
            t.Name,
            String.Join("_", index.ColumnNames),
            String.Join(",", index.ColumnNames));
        }


      }

      //generate foreign keys
      foreach (Core.Table t in database.Tables)
      {
        //generate cascading rules on record deletion on table
        finalSQL.AppendLine();
        if (t.Relationships.Any())
        {
          finalSQL.AppendLine(string.Format("-- fk constraints for {0}", t.Name));
        }
        foreach (Core.Relationship relationship in t.Relationships)
        {
          string onDeleteRule = string.Empty;
          if (relationship.IsRequired)
          {
            onDeleteRule = "ON DELETE CASCADE";
          }
          else
          {
            onDeleteRule = "ON DELETE SET NULL";
          }

          finalSQL.AppendFormat(@"ALTER TABLE {0}
ADD CONSTRAINT fk_{0}_{1}
FOREIGN KEY ({2})
REFERENCES {1}({3}) {4}
",
                      relationship.FromTable,
                      relationship.ToTable,
                      relationship.FromColumnNamed,
                      relationship.ToColumnNamed,
                      onDeleteRule);

        }
      }

      return new Core.DatabaseGeneratorResult()
      {
        IsSuccessful = true,
        Output = finalSQL.ToString()
      };
    }

    private string GetIndexTypeAsString(Core.Index index)
    {
      return index.IndexType == Core.IndexType.UNIQUE ?
         "UNIQUE" :
           (index.IndexType == Core.IndexType.CLUSTERED ?
           "CLUSTERED" : "NONCLUSTERED");
    }

    private string GeneratePassword(ushort passwordLength = 12)
    {
      const string randomChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
      ushort MAX_LENGTH = (ushort)randomChars.Length;
      char[] randomElements = randomChars.ToCharArray();
      byte[] randomBytes = new byte[4];
      System.Security.Cryptography.RandomNumberGenerator random = System.Security.Cryptography.RandomNumberGenerator.Create();
      random.GetBytes(randomBytes);
      int randomSeed = BitConverter.ToInt32(randomBytes, 0);
      Random realRandom = new Random(randomSeed);
      StringBuilder finalPassword = new StringBuilder();
      for (short i = 0; i < passwordLength; i++)
      {
        finalPassword.Append(randomElements[realRandom.Next(0, MAX_LENGTH)]);
      }
      return finalPassword.ToString();
    }
  }
}
