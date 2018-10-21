using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceCalculator
{
    public static class DiceMath
    {
        // The formulas for the average and standard deviation of a single die were taken from this helpful article: http://www.d8uv.org/dice-to-distribution/

        /// <summary>
        /// Calculates the average sum for an array of dice.
        /// </summary>
        /// <param name="dice">An array of Die objects to calculate the average sum of.</param>
        /// <param name="modifier">The amount to add or subtract from a sum.</param>
        /// <returns>The average sum of an array of dice.</returns>
        public static double CalculateAverage(Die[] dice, int modifier)
        {
            // Average of Single Die = (NumberOfSides + 1) / 2
            // Sum together for total average
            return dice.Sum(die => die.NumberOfSides + 1) / 2.0 + modifier;
        }

        /// <summary>
        /// Calculates the standard deviation for an array of dice.
        /// </summary>
        /// <param name="dice">An array of Die objects to calculate the standard deviation for.</param>
        /// <returns>The standard deviation of an array of dice.</returns>
        public static double CalculateStandardDeviation(Die[] dice)
        {
            // Standard Deviation of Single Die = sqrt(NumberOfSides * NumberOfSides - 1) / (2 * sqrt(3))
            // Can simplify this to StdDev = sqrt((NumberOfSides * NumberOfSides - 1) / 12)
            // Sum together for total standard deviation
            return Math.Sqrt(dice.Sum(die => die.NumberOfSides * die.NumberOfSides - 1) / 12.0);
        }

        /// <summary>
        /// Calculates the number of combinations that an array of dice can produce.
        /// </summary>
        /// <param name="dice">An array of Die objects to calculate the number of combinations for.</param>
        /// <returns>The number of combinations that an array of dice can produce.</returns>
        public static int CalculateNumberOfCombinations(Die[] dice)
        {
            // Find number of combinations by multiplying all dice number of sides together
            int combinations = 1;
            Array.ForEach(dice, die => combinations *= die.NumberOfSides);
            return combinations;
        }

        /// <summary>
        /// Calculate all possible sums and their occurrences for an array of dice.
        /// </summary>
        /// <param name="dice">An array of Die objects to calculate the sums for.</param>
        /// <param name="modifier">The amount to add or subtract from a sum.</param>
        /// <returns>Dictionary that uses the sum as the Key and the count of how many times that sum has been rolled for the Value.</returns>
        public static Dictionary<int, int> CalculateSums(Die[] dice, int modifier)
        {
            // Dictionary that uses the sum as the key, and how many times that sum has been rolled for the value
            Dictionary<int, int> sums = new Dictionary<int, int>();

            // While we have more combinations to sum, add together current top sides of dice, and change next dice sides to next side (increment side). Then, add together final sides (without incrementing afterwards)
            while (NotFinished(dice))
            {
                Calculate(dice, sums);
                IncrementSides(dice);
            }
            Calculate(dice, sums);

            if (modifier != 0)
            {
                Dictionary<int, int> modifiedSums = new Dictionary<int, int>(sums.Count);
                foreach (var pair in sums)
                {
                    modifiedSums.Add(pair.Key + modifier, pair.Value);
                }
                return modifiedSums;
            }

            return sums;
        }

        // Calculate sum for currently showing sides and add sum to total
        private static void Calculate(Die[] dice, Dictionary<int, int> sums)
        {
            // Add sum of dice sides to running total
            int sum = dice.Sum(die => die.CurrentShowingSide);

            // If sum already exists, increment sum count; otherwise add sum to dictionary with count of 1
            if (sums.ContainsKey(sum))
            {
                sums[sum]++;
            }
            else
            {
                sums.Add(sum, 1);
            }
        }

        // Check all dice to see if they have reached their maximum side
        // If any die has not reached its maximum side, returns true
        // Otherwise, all dice are currently on their maximum sides, so returns false
        private static bool NotFinished(Die[] dice)
        {
            for (int i = 0; i < dice.Length; i++)
            {
                if (dice[i].NumberOfSides > dice[i].CurrentShowingSide)
                {
                    return true;
                }
            }

            return false;
        }

        // Increments sides of each die that need to be incremented.
        // When a die has reached its maximum value and is incremented, its value goes back to 1 and the next die in line gets incremented.
        // This can cascade across multiple dice.
        // Essentially, the current sides can be thought of a number, with each digit being an individual die.
        // Instead of the entire number being the same base, each digit is of the base of that die's type.
        // Additionally, instead of the digits going from 0 to (base - 1), they go from 1 to base.
        // Doing it this way will go through all possible combinations of each side until the full "number" has reached its maximum value, then it will roll back around to all 1s.
        private static void IncrementSides(Die[] dice)
        {
            dice[dice.Length - 1].CurrentShowingSide++; // Increment last die side

            for (int i = dice.Length - 1; i > 0; i--)
            {
                if (dice[i].CurrentShowingSide >= dice[i].NumberOfSides + 1) // If side is above its max value, set side to 1 and increment next die's side
                {
                    dice[i].CurrentShowingSide = 1;

                    if (i - 1 >= 0)
                    {
                        dice[i - 1].CurrentShowingSide++;
                    }
                }
            }
        }
    }
}
