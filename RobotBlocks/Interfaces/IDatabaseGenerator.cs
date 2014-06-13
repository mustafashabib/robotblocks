using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Interfaces
{
    public interface IDatabaseGenerator
    {
      RobotBlocks.Core.DatabaseGeneratorResult Generate(Core.Database dataBase); 
      
      bool IsMappableType(Type fieldType);

      string GetColumnDefinition(Core.Column column);
        

    }
}
