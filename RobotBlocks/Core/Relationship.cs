using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Core
{
    /// <summary>
    /// houses all foreign key relationships for each table
    /// </summary>
    /// 
  [Serializable]
  public class Relationship
  {
    public string FromTable { get; set; }
    public string FromColumnNamed { get; set; }
    public string ToColumnNamed { get; set; }
    public string ToTable { get; set; }
    public bool IsRequired { get; set; }
  }
}
