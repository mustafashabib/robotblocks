using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Core
{
  [Serializable]
  public class Table
  {
    internal Table(string name)
    {
      Name = name;
      Columns = new List<Column>();
      Relationships = new List<Relationship>();
      Indices = new List<Index>();
    }
    public string Name { get; set; }
    public IList<Column> Columns;
    public IList<Relationship> Relationships { get; set; }
    public IList<Index> Indices { get; set; }
  }
}
