using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Interfaces
{
  public interface IDatabaseDiffer
  {
    string GetDifferences(string fromDatabase, string toDatabase);
  }
}
