using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Core
{
    [Serializable]
    public class Column
    {
      private static bool CanBeNull(Type type)
      {
        return !type.IsValueType || (Nullable.GetUnderlyingType(type) != null);
      }

      public Column(string name, Type underlyingSystemType, int? length, bool isRequired, bool isPrimaryKey, bool isUnique, string defaultValue)
      {
        Name = name;
        UnderlyingSystemType = underlyingSystemType;
        Length = length;
        IsRequired = isRequired;
        IsPrimaryKey = isPrimaryKey;
        IsUnique = isUnique;
        DefaultValue = defaultValue;
      }

      public Column(System.Reflection.PropertyInfo propertyInfo)
      {
        
        bool isPrimaryKey = propertyInfo.Name.ToLower() == "id";

        bool isRequired = isPrimaryKey ||
          propertyInfo.IsDefined(typeof(RobotBlocks.Annotations.Constraint.Field.Required), false) ||
          !Column.CanBeNull(propertyInfo.PropertyType);
        int? maximumLength = null;
        object[] lengthAttributes = propertyInfo.GetCustomAttributes(typeof(RobotBlocks.Annotations.Constraint.Field.Length), false);
        foreach (RobotBlocks.Annotations.Constraint.Field.Length lengthConstraint in lengthAttributes)
        {
          if (lengthConstraint != null)
          {
            maximumLength = lengthConstraint.Maximum;
            break;
          }
        }

        bool isUnique = propertyInfo.IsDefined(typeof(RobotBlocks.Annotations.Constraint.Field.Unique), false);

        string defaultValue = null;
        object[] defaultAttributes = propertyInfo.GetCustomAttributes(typeof(RobotBlocks.Annotations.Constraint.Field.Default), false);
        foreach (RobotBlocks.Annotations.Constraint.Field.Default defaultConstraint in defaultAttributes)
        {
          if (defaultConstraint != null)
          {
            defaultValue = defaultConstraint.Value;
            break;
          }
        }
        Name = propertyInfo.Name;
        UnderlyingSystemType = propertyInfo.PropertyType.UnderlyingSystemType;
        Length = maximumLength;
        IsRequired = isRequired;
        IsPrimaryKey = isPrimaryKey;
        IsUnique = isUnique;
        DefaultValue = defaultValue;
      }

      public string Name { get; private set; }
      public Type UnderlyingSystemType { get; private set; }
      public int? Length { get; private set; }
      public bool IsRequired { get; private set; }
      public bool IsPrimaryKey { get; private set; }
      public bool IsUnique { get; private set; }
      public string DefaultValue { get; private set; }
    }
}
