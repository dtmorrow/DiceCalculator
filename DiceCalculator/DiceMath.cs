using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceCalculator
{
    class DiceMath
    {
        /// <summary>
        /// Calculate all possible sums of dice, how many combinations there are, and the average sum
        /// </summary>
        /// <param name="diceArray">Array of dice</param>
        /// <param name="sums">Dictionary with Key: Sum of roll and Value: Amount of times that sum occurs</param>
        /// <param name="combinations">How many combinations can be rolled with these dice</param>
        /// <param name="average">Average sum of all dice rolls</param>
        public static void RunCalculations(Die[] diceArray, out Dictionary<int, int> sums, out int combinations, out double average)
        {
            // Find number of combinations by multiplying all dice types together
            combinations = diceArray[0].NumberOfSides;
            for (int i = 1; i < diceArray.Length; i++)
            {
                combinations *= diceArray[i].NumberOfSides;
            }

            // Running total that will be used to calculate the average at the end
            double total = 0;

            // Dictionary that uses the sum as the key, and how many times that sum has been rolled for the value
            sums = new Dictionary<int, int>();

            // While we have more combinations to sum, add together current top sides of dice, and change next dice sides to next side (increment side). Then, add together final sides (without incrementing afterwards)
            while (NotFinished(diceArray))
            {
                Calculate(diceArray, ref total, ref sums);
                IncrementSides(ref diceArray);
            }
            Calculate(diceArray, ref total, ref sums);

            // Average = Total Sum / Number of Combinations
            average = total / combinations;
        }

        // Calculate sum for currently showing sides and add sum to total
        private static void Calculate(Die[] diceArray, ref double total, ref Dictionary<int, int> sums)
        {
            // Add sum of dice sides to running total
            int sum = 0;
            for (int i = 0; i < diceArray.Length; i++)
            {
                sum += diceArray[i].CurrentShowingSide;
            }
            total += sum;

            // If sum already exists, increment sum amount; otherwise add sum to dictionary with amount of 1
            if (sums.ContainsKey(sum))
            {
                sums[sum]++;
            }
            else
            {
                sums.Add(sum, 1);
            }
        }

        // Check each side to see if it has reached its max
        // If any side of any dice is not at its max, returns true
        // Otherwise, all dice are currently on their maximum sides, so returns false
        private static bool NotFinished(Die[] diceArray)
        {
            for (int i = 0; i < diceArray.Length; i++)
            {
                if (diceArray[i].NumberOfSides > diceArray[i].CurrentShowingSide)
                {
                    return true;
                }
            }

            return false;
        }

        // Increments sides of each dice that need to be incremented.
        // When a die has reached its maximum value and is incremented, its value goes back to 1 and the next die in line gets incremented.
        // This can cascade across multiple dice.
        // Essentially, the current sides can be thought of a number, with each digit being an individual die.
        // Instead of the entire number being the same base, each digit is of the base of that die's type.
        // Additionally, instead of the digits going from 0 to (base - 1), they go from 1 to base.
        // Doing it this way will go through all possible combinations of each side until the "number" has reached its maximum value, then it will roll back around to all 1's.
        private static void IncrementSides(ref Die[] diceArray)
        {
            diceArray[diceArray.Length - 1].CurrentShowingSide++; // Increment last dice side

            for (int i = diceArray.Length - 1; i > 0; i--)
            {
                if (diceArray[i].CurrentShowingSide >= diceArray[i].NumberOfSides + 1) // If side is above its max value, set side to 1 and increment next dice's side
                {
                    diceArray[i].CurrentShowingSide = 1;

                    if (i - 1 >= 0)
                    {
                        diceArray[i - 1].CurrentShowingSide++;
                    }
                }
            }
        }
    }
}
