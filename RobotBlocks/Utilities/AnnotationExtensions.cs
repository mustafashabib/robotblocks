using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Utilities
{
  internal static class AnnotationExtensions
  {
    internal static RobotBlocks.Core.IndexType ConvertToCoreIndexType(this RobotBlocks.Annotations.Data.Entity.Index.IndexType annotationsIndexType)
    {
      switch (annotationsIndexType)
      {
        case RobotBlocks.Annotations.Data.Entity.Index.IndexType.CLUSTERED:
          return RobotBlocks.Core.IndexType.CLUSTERED;
        case  RobotBlocks.Annotations.Data.Entity.Index.IndexType.NONCLUSTERED:
          return RobotBlocks.Core.IndexType.NONCLUSTERED;
        case  RobotBlocks.Annotations.Data.Entity.Index.IndexType.UNIQUE:
          return RobotBlocks.Core.IndexType.UNIQUE;
        default:
          throw new System.ArgumentException("Cannot convert annotations index type to core indextype.");
      }
    }
  }
}
