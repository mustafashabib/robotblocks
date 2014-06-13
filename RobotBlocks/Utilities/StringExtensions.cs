using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Utilities
{
  public static class StringExtensions
  {
    public static string ToSafeString(this string s)
    {
      return System.Text.RegularExpressions.Regex.Replace(s, "\\W",string.Empty); 
    }

    private static System.Text.RegularExpressions.Regex ForeignKeyExpression =
      new System.Text.RegularExpressions.Regex("^related(.+)id$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    public static bool IsNamedAsARelationship(this string s)
    {
      return ForeignKeyExpression.IsMatch(s);
    }
    public static string GetReferencedEntityName(this string s)
    {
      return ForeignKeyExpression.Match(s).Groups[1].Value;
    }
  }
}
