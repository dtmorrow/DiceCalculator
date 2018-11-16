using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceCalculator
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: DiceCalculator dice [modifiers] [-s:sum | -q]");
                Console.WriteLine("Arguments:\n" +
                                  "  dice      -- Space separated dice amounts and types (e.g. 2d20 1d6 1d4).\n" +
                                  "  modifiers -- Optional. Space separated modifiers. '+' or '-' must be included\n" +
                                  "               directly before value (e.g. +5 -2 +1).\n" +
                                  "  -s:sum    -- Optional. Calculate chance of rolling sum or higher. Multiple -s\n" +
                                  "               arguments can be provided. Cannot be used with Quick mode.\n" +
                                  "  -q        -- Optional. Quick mode. Doesn't generate table; only calculates\n" +
                                  "               total combinations, average sum, standard deviation, and range\n" +
                                  "               within one standard deviation.\n"
                                  );
                Console.WriteLine("Arguments can be provided in any order.");
                Console.WriteLine("Example: DiceCalculator 2d4 +2 1d6 +1 -c:10 -c:5");
                Environment.Exit(0);
            }

            // Parse arguments
            string diceString;
            List<int> desiredSums;
            bool quickMode;
            int totalModifier;
            ParseArguments(args, out diceString, out desiredSums, out quickMode, out totalModifier);
            
            if (desiredSums.Count > 0 && quickMode)
            {
                Console.WriteLine("ERROR: Cannot use Quick mode and provide sum chances at the same time.");
                Environment.Exit(1);
            }

            // Create dice array
            Die[] diceArray = DiceMath.StringToDiceArray(diceString);

            if (diceArray.Length == 0)
            {
                Console.WriteLine("ERROR: Could not parse any dice from arguments. Exiting...");
                Environment.Exit(1);
            }
            diceArray = diceArray.OrderBy(d => d.NumberOfSides).ToArray();

            // Calculate combinations, average, and standard deviation
            int combinations = DiceMath.CalculateNumberOfCombinations(diceArray);
            double average = DiceMath.CalculateAverage(diceArray, totalModifier);
            double standardDeviation = DiceMath.CalculateStandardDeviation(diceArray);
            
            // Print results
            Console.WriteLine("Dice Rolled: {0}", DiceMath.DiceArrayToString(diceArray));
            Console.WriteLine("Modifier: {0}", totalModifier);
            Console.WriteLine("Combinations: {0}", combinations);
            Console.WriteLine("Average Sum: {0}", average);
            Console.WriteLine("Standard Deviation: {0}", standardDeviation);
            Console.WriteLine("Range within 1 Standard Deviation: {0} - {1}\n", (average - standardDeviation), (average + standardDeviation));

            if (!quickMode)
            {
                Dictionary<int, int> sumsValues = DiceMath.CalculateSums(diceArray, totalModifier);
                PrintSums(sumsValues, combinations);

                // Print chances of desired sums
                Console.WriteLine();
                for (int i = 0; i < desiredSums.Count; i++)
                {
                    Console.WriteLine("Chance of rolling sum of {0} or higher: {1}%", desiredSums[i], DiceMath.CalculateChanceForSum(sumsValues, combinations, desiredSums[i]).ToString("F2"));
                }
            }
        }

        // Parse arguments by determining argument type, removing arguments as they are processed until there are none left
        private static void ParseArguments(string[] args, out string diceString, out List<int> desiredSums, out bool quickMode, out int totalModifier)
        {
            List<string> arguments = new List<string>(args);
            
            diceString = string.Empty;
            desiredSums = new List<int>();
            quickMode = false;
            totalModifier = 0;

            while (arguments.Count > 0)
            {
                if (arguments[0].StartsWith("-s:"))
                {
                    int value;
                    if (int.TryParse(arguments[0].Substring(3), out value))
                    {
                        desiredSums.Add(value);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Could not parse desired sum '{0}'", arguments[0].Substring(3));
                        Environment.Exit(1);
                    }
                }
                else if (arguments[0] == "-q")
                {
                    quickMode = true;
                }
                else if (arguments[0].StartsWith("-"))
                {
                    int value;
                    if (int.TryParse(arguments[0], out value))
                    {
                        totalModifier += value;
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Could not parse value '{0}'", arguments[0]);
                        Environment.Exit(1);
                    }
                }
                else if (arguments[0].StartsWith("+"))
                {
                    int value;
                    if (int.TryParse(arguments[0].Substring(1), out value))
                    {
                        totalModifier += value;
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Could not parse value '{0}'", arguments[0]);
                        Environment.Exit(1);
                    }
                }
                else
                {
                    diceString += arguments[0] + " ";
                }

                arguments.RemoveAt(0);
            }

            diceString = diceString.Trim();
            desiredSums.Sort();
        }

        // Print sums into table with percentages
        private static void PrintSums(Dictionary<int, int> sums, int combinations)
        {
            double combos = combinations; // Convert combinations to double so we don't have to cast every time

            int longestSumLength = 3;
            int longestCountLength = 5;

            // Find longest string length for sum and count
            foreach (int sum in sums.Keys)
            {
                if (sum.ToString().Length > longestSumLength)
                {
                    longestSumLength = sum.ToString().Length;
                }
                if (sums[sum].ToString().Length > longestCountLength)
                {
                    longestCountLength = sums[sum].ToString().Length;
                }
            }

            Console.WriteLine("| " + "Sum".PadLeft(longestSumLength) + " | " + "Count".PadLeft(longestCountLength) + " |  Percent |");
            Console.WriteLine("+-" + "".PadLeft(longestSumLength, '-') + "-+-" + "".PadLeft(longestCountLength, '-') + "-+----------+");

            foreach (int sum in sums.Keys)
            {
                double percent = (sums[sum] / combos) * 100;
                Console.Write("| " + sum.ToString().PadLeft(longestSumLength) + " | " + sums[sum].ToString().PadLeft(longestCountLength) + " |  " + percent.ToString("F2").PadLeft(6) + "% |\n");
            }
        }
    }
}
