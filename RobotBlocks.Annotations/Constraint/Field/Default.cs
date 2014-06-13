using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Annotations.Constraint.Field
{
    public class Default : Core.FieldAttribute
    {
        public string Value {get;set;}
        public Default(string value) {
            Value = value;
        }
    }
}
