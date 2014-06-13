using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Annotations.Data.Entity
{
  /// <summary>
  /// This attribute decorates a class and lets you list the property names you would like to ignore
  /// when generating the database model. If you have a lot of them, this is easier to use than
  /// individual RobotBlocks.Annotations.Data.Field.Ignore attributes on each field.
  /// </summary>
  public class Ignore : RobotBlocks.Annotations.Core.EntityAttribute
  {
    public string[] FieldNames { get; set; }
    public Ignore(params string[] fieldNames)
    {
      FieldNames = fieldNames;
    }
  }
}
