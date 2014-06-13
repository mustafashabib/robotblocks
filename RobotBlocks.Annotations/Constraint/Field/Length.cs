using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Annotations.Constraint.Field
{

  [System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public class Length : RobotBlocks.Annotations.Core.FieldAttribute
  {
      public int Maximum { get; set; }
      public Length(int maximum) 
      {
          Maximum = maximum;
      }
  }
}
