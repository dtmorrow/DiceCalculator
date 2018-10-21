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
            Die[] diceArray = StringToDiceArray(diceString);

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
            Console.WriteLine("Dice Rolled: {0}", DiceArrayToString(diceArray));
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
                    Console.WriteLine("Chance of rolling sum of {0} or higher: {1}%", desiredSums[i], CalculateChanceForSum(sumsValues, combinations, desiredSums[i]).ToString("F2"));
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

        // Parse string of space separated dice to array of individual dice
        private static Die[] StringToDiceArray(string diceString)
        {
            List<Die> diceArray = new List<Die>();

            // Split diceString into separate dice amounts, e.g. { 2d20, 1d6, 1d4 }
            string[] diceSplit = diceString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < diceSplit.Length; i++)
            {
                // Convert dice amount + type into individual dice, e.g. 2d20 = { 1d20, 1d20 }, 5d4 = { 1d4, 1d4, 1d4, 1d4, 1d4 }
                string[] pair = diceSplit[i].Split('d');
                int amount = 0;
                int type = 0;

                // If incorrect format, or either part can't be parsed, show error and exit
                if (pair.Length != 2 || !int.TryParse(pair[0], out amount) || !int.TryParse(pair[1], out type))
                {
                    Console.WriteLine("ERROR: Could not parse value '{0}'", diceSplit[i]);
                    Environment.Exit(1);
                }

                for (int j = 0; j < amount; j++)
                {
                    diceArray.Add(new Die() { NumberOfSides = type, CurrentShowingSide = 1 });
                }
            }

            return diceArray.ToArray();
        }

        // Convert array of dice into a string of space separated dice amounts + types
        // Will also simplify dice if original dice string was unsimplified, e.g. 2d6 1d6 = 3d6
        private static string DiceArrayToString(Die[] diceArray)
        {
            string diceString = string.Empty;
            Dictionary<int, int> dice = new Dictionary<int, int>(); // Key: Type of die, Value: Amount of that type

            for (int i = 0; i < diceArray.Length; i++)
            {
                // If dice type is already in dictionary, increment amount; otherwise, add dice type to dictionary with amount of 1
                if (dice.ContainsKey(diceArray[i].NumberOfSides))
                {
                    dice[diceArray[i].NumberOfSides]++;
                }
                else
                {
                    dice.Add(diceArray[i].NumberOfSides, 1);
                }
            }

            foreach (int die in dice.Keys)
            {
                diceString += dice[die] + "d" + die + " ";
            }

            return diceString.Trim();
        }

        // Calculate chance for a specific sum
        private static double CalculateChanceForSum(Dictionary<int, int> sums, double combinations, int value)
        {
            double chance = 0;

            foreach (int sum in sums.Keys)
            {
                if (sum >= value)
                {
                    chance += (sums[sum] / combinations) * 100;
                }
            }

            return chance;
        }

        // Print sums into table with percentages
        private static void PrintSums(Dictionary<int, int> sums, int combinations)
        {
            double combos = combinations;
            int[] keys = sums.Keys.ToArray();
            int[] values = sums.Values.ToArray();

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
