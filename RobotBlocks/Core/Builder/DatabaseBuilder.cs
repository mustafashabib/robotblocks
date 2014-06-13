using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Core.Builder
{
  internal static class DatabaseBuilder
  {
    internal static Database Create(string name, string userName = null, string passWord = null)
    {
      return new Database(name, userName, passWord);
    }

    internal static Database AddTable(this Database database, Table table)
    {
      database.Tables.Add(table);
      return database;
    }

    internal static Database AddTables(this Database database, IList<Table> tables)
    {
      database.Tables = tables;
      return database;
    }
  }
}
