using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Core
{
  [Serializable]
  public class Database
  {
    public IList<Table> Tables;
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    private static Common.Logging.ILog LOG = Utilities.Logging.Logger.GetLogger<Database>();
    public Database(string name, string username, string password)
    {
      Tables = new List<Table>();
      Name = name;
      UserName = username;
      Password = password;
    }

    internal static Database FromFile(string path)
    {
      using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
      {
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        try
        {
         return (Database)formatter.Deserialize(fs);
        }
        catch(System.Runtime.Serialization.SerializationException serializationException)
        {
          LOG.WarnFormat("Cannot deserialize database {0}", serializationException, path);
        }
      }
      return null;
    }
   
    internal bool Save()
    {
      //back up tables as current version of schema
      System.IO.FileStream fs = new System.IO.FileStream(
        string.Format("{0}_schema_{1}.rbs",
        this.Name, DateTime.Now.ToFileTimeUtc()),
        System.IO.FileMode.Create);
      System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      try
      {
        formatter.Serialize(fs, this);
      }
      catch (System.Runtime.Serialization.SerializationException serializationException)
      {
        LOG.WarnFormat("Cannot deserialize database {0}", serializationException, fs.Name);
        return false;
      }
      finally
      {
        fs.Close();
      }
      return true;
    }
  }
}
