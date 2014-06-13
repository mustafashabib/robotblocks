using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace RobotBlocks.Utilities.Logging
{
  internal static class Logger
  {
    internal static Common.Logging.ILog GetLogger<T>()
    {
      // create properties
      NameValueCollection properties = new NameValueCollection();
      properties["configType"] = "FILE";
      string assemblyPath =
        System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(RobotBlocks.Utilities.Logging.Logger)).Location);
      properties["configFile"] = System.IO.Path.Combine(assemblyPath, "RobotBlocks.log4net.config");

      // set Adapter
      Common.Logging.ILoggerFactoryAdapter adapter = new Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter(properties);
      return Common.Logging.LogManager.GetLogger<T>();

    
    }
  }
}
