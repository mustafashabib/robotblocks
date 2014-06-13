using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Annotations.Data.Field
{
  /// <summary>
  /// decorate a property in your entity with this attribute 
  /// to prevent it from creating a column in a database table
  /// </summary>
  public class Ignore : RobotBlocks.Annotations.Core.FieldAttribute
  {
  }
}
