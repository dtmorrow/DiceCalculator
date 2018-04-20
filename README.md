# DiceCalculator
This application will accept dice amounts and names (e.g. 2d20, 1d6, 1d4) and output a table of percent chances for all possible sums of those dice rolls.

You can also provide several values to match or beat and it will output the percent chance of those sums occurring (e.g. "Chance of rolling sum of 6 or higher: 75.00%").

## Requirements
Microsoft .NET Framework 4.5.2 or higher.

## Usage
Arguments: space separated dice amounts and names (ex: "2d20 1d6 1d4")

Optional: -c:n, where n equals desired value. Specify multiple -c:n's to find multiple desired values.

### Example
```
>DiceCalculator.exe 2d4 1d6 -c:10 -c:5
Dice Rolled: 2d4 1d6
Combinations: 96
Average Sum: 8.5

| Sum | Count |  Percent |
+-----+-------+----------+
|   3 |     1 |    1.04% |
|   4 |     3 |    3.13% |
|   5 |     6 |    6.25% |
|   6 |    10 |   10.42% |
|   7 |    13 |   13.54% |
|   8 |    15 |   15.63% |
|   9 |    15 |   15.63% |
|  10 |    13 |   13.54% |
|  11 |    10 |   10.42% |
|  12 |     6 |    6.25% |
|  13 |     3 |    3.13% |
|  14 |     1 |    1.04% |

Chance of rolling sum of 5 or higher: 95.83%
Chance of rolling sum of 10 or higher: 34.38%
```

## Building
Solution requires Visual Studio 2015.
