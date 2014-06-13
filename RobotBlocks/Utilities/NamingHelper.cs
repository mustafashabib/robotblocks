using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotBlocks.Utilities
{
    public static class NamingHelper
    {
        internal class NameMatchResult
        {
          internal bool IsSuccessful { get; set; }
          internal string Replacement { get; set; }
          internal NameMatchResult(bool isSuccessful, string replacement)
            {
                IsSuccessful = isSuccessful;
                Replacement = replacement;
            }
        }

        internal class NameMatchEvaluator
        {
          internal System.Text.RegularExpressions.Regex Expression { get; set; }
          internal System.Text.RegularExpressions.MatchEvaluator Evaluator { get; set; }

          internal NameMatchResult Convert(string input)
          {
                if (Expression.IsMatch(input))
                {
                    return new NameMatchResult(true, Expression.Replace(input, Evaluator));
                }
                else
                {
                    return new NameMatchResult(false, input);
                }
            }

        }

        

        #region namers

        private static readonly System.Collections.Generic.SortedList<int, NameMatchEvaluator> SINGULARIZER = 
         new SortedList<int,NameMatchEvaluator>(){
              { 0,
                new NameMatchEvaluator(){
                   Evaluator = delegate(System.Text.RegularExpressions.Match m) 
                        {
                          return string.Format("{0}s", m.Groups[1].Value);
                        },
                        Expression = new System.Text.RegularExpressions.Regex("^(.+)ses$")
                }
              },
            
            { 1, 
              new NameMatchEvaluator(){
                   Evaluator = delegate(System.Text.RegularExpressions.Match m) 
                        {
                          return string.Format("{0}y", m.Groups[1].Value);
                        },
                        Expression = new System.Text.RegularExpressions.Regex("^(.+)ies$")
                }
            },
            { 2, 
              new NameMatchEvaluator(){
                   Evaluator = delegate(System.Text.RegularExpressions.Match m) 
                        {
                          switch(m.Value){
                                case "people":
                                    return "person";
                                case "People":
                                    return "Person";
                                case "Deer":
                                    return "Deer";
                                case "deer":
                                    return "deer";
                                case "Geese":
                                    return "Goose";
                                case "geese":
                                    return "goose";
                                default:
                                    return m.Value;
                          }
                        },
                        Expression = new System.Text.RegularExpressions.Regex("^[pP]eople$|^[dD]eer$|^[gG]eese$")
                }
            },
            { 3, 
              new NameMatchEvaluator(){
                   Evaluator = delegate(System.Text.RegularExpressions.Match m) 
                        {
                          return string.Format("{0}", m.Groups[1].Value);
                        },
                        Expression = new System.Text.RegularExpressions.Regex("(.+)s")
                }
            }
         };
        private static readonly System.Collections.Generic.SortedList<int, NameMatchEvaluator> PLURALIZER =
         new SortedList<int, NameMatchEvaluator>(){
           { 0, 
              new NameMatchEvaluator(){
                   Evaluator = delegate(System.Text.RegularExpressions.Match m) 
                        {
                          return string.Format("{0}ses", m.Groups[1].Value);
                        },
                        Expression = new System.Text.RegularExpressions.Regex("^(.+)s$")
                }
            },
            { 1, 
              new NameMatchEvaluator(){
                   Evaluator = delegate(System.Text.RegularExpressions.Match m) 
                        {
                          return string.Format("{0}ies", m.Groups[1].Value);
                        },
                        Expression = new System.Text.RegularExpressions.Regex("^(.+)y$")
                }
            },
            { 2, 
              new NameMatchEvaluator(){
                   Evaluator = delegate(System.Text.RegularExpressions.Match m) 
                        {
                          switch (m.Value)
                          {
                            case "person":
                              return "people";
                            case "Person":
                              return "People";
                            case "deer":
                              return "deer";
                            case "Deer":
                              return "Deer";
                            case "Goose":
                              return "Geese";
                            case "goose":
                              return "geese";
                            default:
                              return m.Value;
                          }
                        },
                        Expression = new System.Text.RegularExpressions.Regex("^[pP]erson|^[dD]eer$|^[gG]oose$")
                }
            },
            { 3, 
              new NameMatchEvaluator(){
                   Evaluator =delegate(System.Text.RegularExpressions.Match m) 
                        {
                          return string.Format("{0}s", m.Groups[1].Value);
                        },
                        Expression = new System.Text.RegularExpressions.Regex("^(.+)$")
                }
            }
         };
        #endregion

        public static string ToSingular(this string input)
        {
            foreach(KeyValuePair<int, NameMatchEvaluator> nameMatchEvaluator in SINGULARIZER){
                NameMatchResult result = nameMatchEvaluator.Value.Convert(input);
                if(result.IsSuccessful)
                {
                    return result.Replacement;
                }
            }
            return input;
        }

        public static string ToPlural(this string input)
        {
            foreach (KeyValuePair<int, NameMatchEvaluator> nameMatchEvaluator in PLURALIZER)
            {
                NameMatchResult result = nameMatchEvaluator.Value.Convert(input);
                if (result.IsSuccessful)
                {
                    return result.Replacement;
                }
            }
            return input;
        }

    }
}
