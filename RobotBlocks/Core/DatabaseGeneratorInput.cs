using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Core
{
  public class DatabaseGeneratorInput<DifferInputOutputType>
  {
    public Interfaces.IDatabaseGenerator Generator { get; private set;}
    public Interfaces.IDatabaseDiffer Differ { get; private set; }
    public DatabaseGeneratorInput(Interfaces.IDatabaseGenerator generator, Interfaces.IDatabaseDiffer differ)
    {
      Generator = generator;
      Differ = differ;
    }
  }
}
