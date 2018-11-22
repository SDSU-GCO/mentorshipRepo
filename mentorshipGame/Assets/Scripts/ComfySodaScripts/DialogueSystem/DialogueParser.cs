using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using cs.CustomExtensions;

namespace cs
{
    public static class Parser
    {
        public struct ActionCharacter
        {
            public char? character;
            public string action;
        }

        public static List<ActionCharacter> parseLine(string lineToParse)
        {
            //handle the escaped Characters
            List<ActionCharacter> sentence = new List<ActionCharacter>();

            for (int i = 0; i < lineToParse.Length; i++)
            {
                ActionCharacter tempActionCharacter;
                tempActionCharacter.action = "";
                tempActionCharacter.character = null;
                
                if (lineToParse[i] == '\\')
                {
                    if (lineToParse[i + 1] == '\\')
                    {
                        tempActionCharacter.character = '\\';
                        i++;
                    }
                    else if (lineToParse.containsSubstringAtPosition("DYNAMIC_ACTION: ", i + 1))
                    {
                        tempActionCharacter.action = "DYNAMIC_ACTION: ";
                        i += "DYNAMIC_ACTION: ".Length;

                        i++;
                        while (lineToParse[i] != '\\' && lineToParse[i + 1] != 'n')
                        {
                            if (lineToParse[i] == '\\' && lineToParse[i + 1] == '\\')
                            {
                                tempActionCharacter.action += lineToParse[i];
                                i++;
                            }
                            i++;
                        }
                        i++;
                    }
                    else if (lineToParse.containsSubstringAtPosition("Player Name", i + 1))
                    {
                        replaceSubStringAtPosition(ref lineToParse, "Player Name", "Timmy", i + 1);
                    }
                }
                else
                {
                    tempActionCharacter.character = lineToParse[i];
                    sentence.Add(tempActionCharacter);
                }
            }
            return sentence;
        }

        public static void replaceSubStringAtPosition(ref string completeString, string substringToReplace, string replacementString, int position)
        {
            string temp = "";
            for (int i = 0; i < position; i++)
            {
                temp += completeString[i];
            }
            temp += replacementString;

            position += substringToReplace.Length;
            for (int i = position; i < completeString.Length; i++)
            {
                temp += completeString[i];
            }
        }
    }


    namespace CustomExtensions
    {
        //Extension methods must be defined in a static class
        public static class StringExtension
        {
            // This is the extension method.
            // The first parameter takes the "this" modifier
            // and specifies the type for which the method is defined.
            public static string TrimAndReduce(this string str)
            {
                return ConvertWhitespacesToSingleSpaces(str).Trim();
            }

            public static string ConvertWhitespacesToSingleSpaces(this string value)
            {
                return Regex.Replace(value, @"\s+", " ");
            }

            public static bool containsSubstringAtPosition(this string value, string substring, int position)
            {
                string tempString = "";
                for (int i = position; i < value.Length && i < substring.Length; i++)
                {
                    tempString += value[i];
                }

                bool returnValue;
                if (tempString == substring)
                {
                    returnValue = true;
                }
                else
                {
                    returnValue = false;
                }

                return returnValue;
            }
        }
    }
}

