# DiceCalculator
This application will accept dice amounts and types (e.g. 2d20, 1d6, 1d4) and modifiers (e.g. +5 -2 +1) and show statistics about the possible rolls, including total number of combinations, average sum, standard deviation, and range within one standard deviation.

It can also output a table of percent chances for all possible sums of those dice rolls.

You can provide several values to match or beat and it will output the percent chance of those sums occurring (e.g. "Chance of rolling sum of 6 or higher: 75.00%").

## Requirements
Microsoft .NET Framework 4.5.2 or higher.

## Usage
```
Usage: DiceCalculator dice [modifiers] [-s:sum | -q]
Arguments:
  dice      -- Space separated dice amounts and types (e.g. 2d20 1d6 1d4).
  modifiers -- Optional. Space separated modifiers. '+' or '-' must be included directly before value (e.g. +5 -2 +1).
  -s:sum    -- Optional. Calculate chance of rolling sum or higher. Multiple -s arguments can be provided. Cannot be used with Quick mode.
  -q        -- Optional. Quick mode. Doesn't generate table; only calculates total combinations, average sum, standard deviation, and range within one standard deviation.

Arguments can be provided in any order.
```

### Example
```
>DiceCalculator 2d4 +2 1d6 -1 -s:10 -s:5
Dice Rolled: 2d4 1d6
Modifier: 1
Combinations: 96
Average Sum: 9.5
Standard Deviation: 2.32737334062816
Range within 1 Standard Deviation: 7.17262665937184 - 11.8273733406282

| Sum | Count |  Percent |
+-----+-------+----------+
|   4 |     1 |    1.04% |
|   5 |     3 |    3.13% |
|   6 |     6 |    6.25% |
|   7 |    10 |   10.42% |
|   8 |    13 |   13.54% |
|   9 |    15 |   15.63% |
|  10 |    15 |   15.63% |
|  11 |    13 |   13.54% |
|  12 |    10 |   10.42% |
|  13 |     6 |    6.25% |
|  14 |     3 |    3.13% |
|  15 |     1 |    1.04% |

Chance of rolling sum of 5 or higher: 98.96%
Chance of rolling sum of 10 or higher: 50.00%
```

## Building
Solution requires Visual Studio 2015 or higher.
