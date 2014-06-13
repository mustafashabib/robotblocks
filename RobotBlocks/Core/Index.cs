using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Core
{
  [Serializable]
  public enum IndexType
    {
        UNIQUE,
        CLUSTERED,
        NONCLUSTERED
    }

  [Serializable]
  public class Index
  {
      public IndexType IndexType;
      public IEnumerable<string> ColumnNames;
  }
}
