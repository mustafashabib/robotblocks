using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotBlocks.Core.Builder;
using RobotBlocks.Utilities;

namespace RobotBlocks.Core
{
    public class Director<T>
    {
      private static Common.Logging.ILog LOG = Utilities.Logging.Logger.GetLogger<Director<T>>();
      
      public enum GenerateType{
        PATH,
        FILES,
        SOURCE
      }
      private static Core.DatabaseGeneratorResult GenerateFromSource(string databaseName, RobotBlocks.Core.DatabaseGeneratorInput<T> generatorInput, string[] sources)
      {
        RobotBlocks.Interfaces.IDatabaseGenerator dbGenerator = generatorInput.Generator;
        RobotBlocks.Interfaces.IDatabaseDiffer dbDiffer = generatorInput.Differ;

        /**
        * for every class in the domain entity path
        * copy to temp memory and compile
        *
        * for every compiled class in the domain library
        * parse each property of each class, create a columndefinition
        * create a tabledefinition for each class
        * 
        * for every compiled class in the domain library
        * parse each relationship of each class - relationships must be named Related{XXXX}Id
        * forevery relationship of every class
        *  create foreign key
        *  create cascade rules
        *
        * forevery compiled class in the domain library
        *  forevery index attribute on the class
        *    create indexdefinition
        **/

        /* force load the robot blocks annotations library */
        RobotBlocks.Annotations.Core.EntityAttribute ea = new Annotations.Core.EntityAttribute();

        Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
        System.CodeDom.Compiler.CompilerParameters compilerParameters = new System.CodeDom.Compiler.CompilerParameters(new string[]{
            "System.dll", getAnnotationsAssemblyPath(), "System.Core.dll",
            "System.Data.dll", "System.Data.DataSetExtensions.dll", 
            "System.Xml.dll", "System.Xml.Linq.dll"});

        compilerParameters.GenerateInMemory = true;
        compilerParameters.GenerateExecutable = false;

        System.CodeDom.Compiler.CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParameters, sources);

        IEnumerable<Type> models = results.CompiledAssembly.ExportedTypes;
        IList<Table> tables = new List<Table>();

        /* 
         * brute force the generation - takes multiple passes - easier to get the sql script defined
         * in the right order this way (so tables exist before we create FK constraints, cascade rules,
         * and indexes on them
         * */

        //pass 1: generate all tables
        foreach (Type currentModel in models.Where(m=> m.IsClass))
        {
          //get ignored fields if any
          string[] ignoredFieldNames = new string[1] { string.Empty };
          if(currentModel.IsDefined(typeof(RobotBlocks.Annotations.Data.Entity.Ignore), false))
          {
            object[] entityIgnoreAttributes = currentModel.GetCustomAttributes(typeof(RobotBlocks.Annotations.Data.Entity.Ignore), false);
            foreach (RobotBlocks.Annotations.Data.Entity.Ignore ignoreAttribute in entityIgnoreAttributes)
            {
              if (ignoreAttribute != null)
              {
                ignoredFieldNames = ignoredFieldNames.Concat(ignoreAttribute.FieldNames).ToArray();
                break;
              }
            }
          }

          //get indexes
          IList<Index> indices = new List<Index>();

          if (currentModel.IsDefined(typeof(RobotBlocks.Annotations.Data.Entity.Index), false))
          {
            object[] entityIndexAttributes = currentModel.GetCustomAttributes(typeof(RobotBlocks.Annotations.Data.Entity.Index), false);
            foreach (RobotBlocks.Annotations.Data.Entity.Index indexAttribute in entityIndexAttributes)
            {
              if (indexAttribute != null)
              {
                Index index = new Index()
                {
                  ColumnNames = indexAttribute.FieldNames,
                  IndexType = indexAttribute.IndexClustering.ConvertToCoreIndexType()                
                };
              }
            }
          }

          //create table
          Table table = Builder.TableBuilder.Create(currentModel.Name.ToPlural());
          System.Reflection.PropertyInfo[] publicProperties = currentModel.GetProperties();
          foreach (System.Reflection.PropertyInfo propertyInfo in publicProperties)
          {
            if(propertyInfo.IsDefined(typeof(RobotBlocks.Annotations.Data.Field.Ignore), false) ||
              ignoredFieldNames.Contains(propertyInfo.Name)
              )
            {
              LOG.InfoFormat("Skipping property: {0}", propertyInfo.Name);
              continue;
            }
            Column column = new Column(propertyInfo);
            if (dbGenerator.IsMappableType(propertyInfo.PropertyType.UnderlyingSystemType))
            {
              table.AddColumn(column);
            }
          }
          if(table.Columns.Where(c=> c.IsPrimaryKey).Any())
          {
            tables.Add(table);
          }
          else
          {

            LOG.Warn(m => m("Skipping Table: {0}", table.Name));
            continue;
          }

        }

        //pass 2: generate all table relationships
        foreach (Table table in tables)
        {

          //for every column whose name is RelatedXXXId (case insensitive) we assume
          //it is a foreign key into the table with the pluralized name of XXX
          foreach (Column c in table.Columns.Where(c => c.Name.IsNamedAsARelationship()))
          {
           
            //also add cascading rules on the parent table - 
            //if this current child table has an FK which is non nullable
            //then that means the parent table is required. when it gets
            //deleted, then this record should be deleted as well.
            //if this current child table has an FK which is nullable
            //then we need to update this FK column to null 
            Table parentTable = (from t in tables where t.Name == c.Name.GetReferencedEntityName().ToPlural() select t).FirstOrDefault();
            if (parentTable == null)
            {
              //log warning and continue -- skip this whole fk thing for 
              //this table since it references a non existent table
              //as far as we are concerned
              LOG.WarnFormat("Skipping foreign key constraints for {0}. Cannot find foreign table {1}", 
                table.Name,
                c.Name.GetReferencedEntityName().ToPlural());
              continue;
            }

            Relationship relationship = new Relationship()
            {
              FromTable = table.Name,
              ToTable = c.Name.GetReferencedEntityName().ToPlural(),
              IsRequired = c.IsRequired,
              FromColumnNamed = c.Name,
              ToColumnNamed = (from parentTableColumn 
                                 in parentTable.Columns 
                               where parentTableColumn.IsPrimaryKey
                               select parentTableColumn.Name).FirstOrDefault()
            };

            //also add an index on this foreign key since 
            //we assume lookups and joins
            Index fkIndex = new Index()
            {
              IndexType = IndexType.NONCLUSTERED,
              ColumnNames = new List<string>(){ 
                 c.Name
               }
            };

            table.AddRelationship(relationship).AddIndex(fkIndex);
          }
        }
        Database database = DatabaseBuilder.Create(databaseName).AddTables(tables);
        
        DatabaseGeneratorResult finalGeneratorResult = dbGenerator.Generate(database);
        if (finalGeneratorResult.IsSuccessful)
        {
          //get most recent schema and create database from it
          string oldDatabaseSchema = getMostRecentSchemaPath(database.Name);
          string newDatabaseSchema = backup(database.Name, finalGeneratorResult.Output);
          string differences = dbDiffer.GetDifferences(oldDatabaseSchema, newDatabaseSchema);
        }

        return finalGeneratorResult;
      }

      private static string backup(string databaseName, string outputContent)
      {
        string backupName = string.Format("{0}_schema_{1}",
            databaseName,
            DateTime.Now.ToFileTimeUtc());
        System.IO.File.WriteAllText(
          backupName,
          outputContent);
        return backupName;
      }
      private static string getMostRecentSchemaPath(string databaseName)
      {
        string executingPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(executingPath);
        string mostRecentSchemaPath = (from fileInfos in
                                     directoryInfo.EnumerateFileSystemInfos(
                                     string.Format("{0}_schema_*",
                                     databaseName),
                                     System.IO.SearchOption.AllDirectories)
                                   orderby fileInfos.CreationTimeUtc descending
                                   select fileInfos.FullName).FirstOrDefault();
        return mostRecentSchemaPath;
      }

      private static Core.DatabaseGeneratorResult GenerateFromFiles(string databaseName, RobotBlocks.Core.DatabaseGeneratorInput<T> generatorInput, string[] domainFilePaths)
      {
        string[] domainFileSources = new string[domainFilePaths.Length];
        int currentCount = 0;
        foreach (string currentPath in domainFilePaths)
        {
          domainFileSources[currentCount++] = System.IO.File.ReadAllText(currentPath);
        }
        
        return Generate(databaseName, generatorInput, domainFileSources);
      }

      private static string getAnnotationsAssemblyPath()
      {
        System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
        string path = System.IO.Path.GetDirectoryName(a.Location);
        string relativeReference = "RobotBlocks.Annotations";
        // if the file exists in this Path - prepend the path 
        string fullReference = System.IO.Path.Combine(path, relativeReference);
        if (System.IO.File.Exists(fullReference))
        {
          return fullReference;
        }
        else
        {
          // Strip off any trailing ".dll" if present.
          fullReference = relativeReference;
          foreach (System.Reflection.Assembly currAssembly in
          AppDomain.CurrentDomain.GetAssemblies())
          {
            if (string.Compare(currAssembly.GetName().Name, fullReference,
            true) == 0)
            {
              // Found it, return the location as the full reference.
              return currAssembly.Location;
            }
          }
        }

        throw new DllNotFoundException("Cannot find the RobotBlocks.Annotations assembly.");
      }

      private static Core.DatabaseGeneratorResult GenerateFromPath(string databaseName, RobotBlocks.Core.DatabaseGeneratorInput<T> generatorInput, string[] pathsToDomainModel)
      {
        IList<string> finalClassFiles = new List<string>();
        foreach(string pathToDomainModel in pathsToDomainModel){
          finalClassFiles.Concat(System.IO.Directory.EnumerateFiles(pathToDomainModel, "*.cs", System.IO.SearchOption.AllDirectories));
        }
        return GenerateFromFiles(databaseName,  generatorInput, finalClassFiles.ToArray());
      }

      public static Core.DatabaseGeneratorResult Generate(string databaseName, RobotBlocks.Core.DatabaseGeneratorInput<T> generatorInput, string[] arguments, GenerateType generateType = GenerateType.PATH)
      {
        databaseName = databaseName.ToSafeString();
        switch (generateType)
        {
          case GenerateType.FILES:
            return GenerateFromFiles(databaseName, generatorInput, arguments);
          case GenerateType.SOURCE:
            return GenerateFromSource(databaseName, generatorInput, arguments);
          case GenerateType.PATH:
          default:
            return GenerateFromPath(databaseName, generatorInput, arguments);
          
            
        }
      }

    }
}
