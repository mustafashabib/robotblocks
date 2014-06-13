using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Core.Builder
{
  internal static class TableBuilder
  {
    internal static Table Create(string name)
    {
      return new Table(name);
    }

    internal static Table AddColumn(this Table t, Column column)
    {
      t.Columns.Add(column);
      return t;
    }

    internal static Table AddIndex(this Table t, Index index)
    {
      t.Indices.Add(index);
      return t;
    }

    internal static Table AddRelationship(this Table t, Relationship relationship)
    {
      t.Relationships.Add(relationship);
      return t;
    }

   
  }
}
