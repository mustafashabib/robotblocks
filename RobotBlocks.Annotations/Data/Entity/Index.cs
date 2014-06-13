using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Annotations.Data.Entity
{
  public class Index : RobotBlocks.Annotations.Core.EntityAttribute
  {
    public enum IndexType
    {
      CLUSTERED,
      NONCLUSTERED,
      UNIQUE
    }

    public IEnumerable<string> FieldNames { get; set; }
    public IndexType IndexClustering { get; set; }
    public Index(IEnumerable<string> fieldNames, IndexType indexClustering)
    {
      this.FieldNames = fieldNames;
      this.IndexClustering = indexClustering;
    }
  }
}
