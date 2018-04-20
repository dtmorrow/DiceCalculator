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
                Console.WriteLine("Arguments: space separated dice amounts and names (ex: \"2d20 1d6 1d4\")\n" + 
                                  "Optional: -c:n, where n equals desired value. Specify multiple -c:n's to find multiple desired values.");
                Environment.Exit(0);
            }

            List<string> arguments = new List<string>(args);

            // Parse arguments by adding dice to string and desired values to list, removing arguments as they are processed until there are none left
            string diceString = string.Empty;
            List<int> desiredValues = new List<int>();
            while (arguments.Count > 0)
            {
                if (arguments[0].StartsWith("-c:"))
                {
                    int value;
                    if (int.TryParse(arguments[0].Substring(3), out value))
                    {
                        desiredValues.Add(value);
                    }
                    else
                    {
                        Console.WriteLine("Could not parse desired value \"" + arguments[0] + "\". Skipping...");
                    }
                    arguments.RemoveAt(0);
                }
                else
                {
                    diceString += arguments[0] + " ";
                    arguments.RemoveAt(0);
                }
            }
            diceString = diceString.Trim();
            desiredValues.Sort();

            // Create dice array
            Die[] diceArray = StringToDiceArray(diceString);

            if (diceArray.Length == 0)
            {
                Console.WriteLine("ERROR: Could not parse any dice from arguments. Exiting...");
                Environment.Exit(0);
            }
            diceArray = diceArray.OrderBy(d => d.NumberOfSides).ToArray();

            // Calculate sums, combinations, and average
            double average;
            int combinations;
            Dictionary<int, int> sums;
            DiceMath.RunCalculations(diceArray, out sums, out combinations, out average);

            // Print results
            Console.WriteLine("Dice Rolled: " + DiceArrayToString(diceArray));
            Console.WriteLine("Combinations: " + combinations);
            Console.WriteLine("Average Sum: " + average + "\n");
            PrintSums(sums, combinations);

            // Print chances of desired values
            Console.WriteLine();
            for (int i = 0; i < desiredValues.Count; i++)
            {
                Console.WriteLine("Chance of rolling sum of " + desiredValues[i] + " or higher: " + CalculateChance(sums, combinations, desiredValues[i]).ToString("F2") + "%");
            }
        }

        // Parse string of space separated dice to array of individual dice
        private static Die[] StringToDiceArray(string diceString)
        {
            List<Die> diceArray = new List<Die>();

            string[] diceSplit = diceString.Split(' '); // Each element of diceSplit will be a separate dice amounts, e.g. { 2d20, 1d6, 1d4 }

            for (int i = 0; i < diceSplit.Length; i++)
            {
                // Convert dice amount + name into individual dice, e.g. 2d20 = { 1d20, 1d20 }, 5d4 = { 1d4, 1d4, 1d4, 1d4, 1d4 }
                string[] pair = diceSplit[i].Split('d');
                int amount = 0;
                int type = 0;

                if (pair.Length < 2 || !int.TryParse(pair[0], out amount) || !int.TryParse(pair[1], out type)) // If incorrect format, or either part can't be parsed, skip
                {
                    Console.WriteLine("Could not parse desired value \"" + diceSplit[i] + "\". Skipping...");
                    continue;
                }

                for (int j = 0; j < amount; j++)
                {
                    diceArray.Add(new Die() { NumberOfSides = type, CurrentShowingSide = 1 });
                }
            }

            return diceArray.ToArray();
        }

        // Convert array of dice into a string of space separated dice amounts + names
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

            foreach (int die in dice.Keys.ToArray())
            {
                diceString += dice[die] + "d" + die + " ";
            }

            return diceString.Trim();
        }

        // Calculate chance for a specific sum
        private static double CalculateChance(Dictionary<int, int> sums, int combinations, int value)
        {
            double combos = combinations;
            double chance = 0;

            foreach (int sum in sums.Keys.ToArray())
            {
                if (sum >= value)
                {
                    chance += (sums[sum] / combos) * 100;
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
            foreach (int sum in sums.Keys.ToArray())
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

            foreach (int sum in sums.Keys.ToArray())
            {
                double percent = (sums[sum] / combos) * 100;
                Console.Write("| " + sum.ToString().PadLeft(longestSumLength) + " | " + sums[sum].ToString().PadLeft(longestCountLength) + " |  " + percent.ToString("F2").PadLeft(6) + "% |\n");
            }
        }
    }
}
