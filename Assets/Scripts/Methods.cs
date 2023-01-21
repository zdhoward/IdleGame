using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BreakInfinity;
using System;

public static class Methods
{
    public static string Notate(this BigDouble number, int normalDigits = 2)
    {
        if (number < 1e6 && number > -1e6)
        {
            return number.ToDouble().ToString($"N{normalDigits}");
        }

        switch (GameController.Instance.data.NotationType)
        {
            case NotationType.Standard:
                string numberString = (number / BigDouble.Pow(10, 3 * BigDouble.Floor(number.Exponent / 3))).ToString($"F{normalDigits}");
                string abbreviationString = number.Exponent < 303 ? StandardNotation[(int)((number.Exponent - number.Exponent % 3) / 3)] : GetAbbreviation(number.Exponent);
                return numberString + abbreviationString;
            case NotationType.Scientific:
                return number.Mantissa.ToString($"F{normalDigits}") + "e" + number.Exponent.ToString("N0");
            case NotationType.Engineering:
                return (number / BigDouble.Pow(10, 3 * BigDouble.Floor(number.Exponent / 3))).ToString($"F{normalDigits}") + "e" + (number.Exponent - number.Exponent % 3).ToString("N0");
            case NotationType.Logarithmic:
                return "e" + BigDouble.Log10(BigDouble.Abs(number)).ToString($"F{normalDigits}");
            case NotationType.Letter:
                return LetterNotation(number);
        }

        return "";
    }

    public static void UpgradeCheck<T>(List<T> list, int length) where T : new()
    {
        try
        {
            if (list.Count == 0)
                list = new T[length].ToList();

            while (list.Count < length)
                list.Add(new T());
        }
        catch
        {
            list = new T[length].ToList();
        }
    }

    public static float Fill(BigDouble left, BigDouble right)
    {
        BigDouble fillAmount = left / right;
        if (fillAmount > 1)
            fillAmount = 1;
        if (fillAmount < 0.01)
            fillAmount = 0;

        return (float)fillAmount.ToDouble();
    }

    public static BigDouble GetCost(BigDouble baseCost, float costMult, BigDouble level)
    {
        return baseCost * BigDouble.Pow(costMult, level);
    }

    public static void BuyMax(ref BigDouble currency, ref List<BigDouble> level, int index, BigDouble baseCost, float costMult)
    {
        BigDouble n = BigDouble.Floor(BigDouble.Log(currency * (costMult - 1) / GetCost(baseCost, costMult, level[index]) + 1, costMult));
        BigDouble cost = GetCost(baseCost, costMult, level[index]) * ((BigDouble.Pow(costMult, n) - 1) / (costMult - 1));

        if (currency < cost)
            return;

        currency -= cost;
        level[index] += n;
    }

    // Notation Helpers
    #region NotationHelpers
    static readonly List<string> StandardNotation = new List<string>
    {
        "", "K", "M", "B", "T", "Qa", "Qt", "Sx", "Sp", "Oc", "No", "Dc", "UDc", "DDc", "TDc", "QaDc", "QtDc", "SxDc",
        "SpDc", "ODc", "NDc", "Vg", "UVg", "DVg", "TVg", "QaVg", "QtVg", "SxVg", "SpVg", "OVg", "NVg", "Tg", "UTg",
        "DTg", "TTg", "QaTg", "QtTg", "SxTg", "SpTg", "OTg", "NTg", "Qd", "UQd", "DQd", "TQd", "QaQd", "QtQd", "SxQd",
        "SpQd", "OQd", "NQd", "Qi", "UQi", "DQi", "TQi", "QaQi", "QtQi", "SxQi", "SpQi", "OQi", "NQi", "Se", "USe", "DSe",
        "TSe", "QaSe", "QtSe", "SxSe", "SpSe", "OSe", "NSe", "St", "USt", "DSt", "TSt", "QaSt", "QtSt", "SxSt", "SpSt",
        "OSt", "NSt", "Og", "UOg", "DOg", "TOg", "QaOg", "QtOg", "SxOg", "SpOg", "OOg", "NOg", "Nn", "UNn", "DNn", "TNn",
        "QaNn", "QtNn", "SxNn", "SpNn", "ONn", "NNn", "Ce",
    };


    static readonly List<List<string>> StandardPrefixes = new List<List<string>>
    {
        new List<string> {"", "U", "D", "T", "Qa", "Qt", "Sx", "Sp", "O", "N"},
        new List<string> {"", "Dc", "Vg", "Tg", "Qd", "Qi", "Se", "St", "Og", "Nn"},
        new List<string> {"", "Ce", "Dn", "Tc", "Qe", "Qu", "Sc", "Si", "Oe", "Ne"}
    };

    static readonly List<string> StandardPrefixes2 = new List<string>
    {
        "", "MI-", "MC-", "NA-", "PC-", "FM-", "Sx", "Sp", "O", "N"
    };

    public static string GetAbbreviation(BigDouble exp)
    {
        // Example: e300
        exp = BigDouble.Floor(exp / 3) - 1; // Floor(300 / 3) - 1 = 99
        var index2 = 0;
        var prefix = new List<string> { StandardPrefixes[0][(int)exp.ToDouble() % 10] }; // {"N"}

        // Start with 99
        // exp then becomes Floor(99/10) = 9
        // Add StandardPrefixes[1][9] = "Nn" to prefix. So prefix = {"N", "Nn"}
        // Since exp is 9, less than 10. We are done with the while loop. index2 is 1.
        while (exp >= 10)
        {
            exp = BigDouble.Floor(exp / 10);
            prefix.Add(StandardPrefixes[++index2 % 3][(int)exp.ToDouble() % 10]);
            // ++index2 basically adds 1 to index2 before accessing it.
        }

        // index2 = Floor(1 / 3) = 0
        index2 = (int)BigDouble.Floor(index2 / 3).ToDouble();
        // prefix.Count = 2. 
        // 3 - (2 % 3) = 1, so this while loop adds "" 1 time. prefix.Count is now 3.
        // Regardless of what occurs above, prefix.Count will always end up being 3.
        while (prefix.Count % 3 != 0) prefix.Add("");


        var ret = "";
        // index2 is 0.
        // First run: We add: prefix[0 * 3] + prefix[0 * 3 + 1] + prefix[0 * 3 + 2] + StandardPrefixes2[0] to ret,
        // which is: prefix[0] + prefix[1] + prefix[2] + StandardPrefixes2[0]
        // which is: "N" + "Nn" + "" + "" = "NNn"
        // We then subtract index2 by one by: StandardPrefixes2[index2--]
        // Since index2 < 0, the while loop stops.
        while (index2 >= 0) ret += prefix[index2 * 3] + prefix[index2 * 3 + 1] + prefix[index2 * 3 + 2] + StandardPrefixes2[index2--];

        // This occurs only with bigger numbers, most StandardPrefixes2 have a "-" at the end.
        // If ret has a "-" at the end, it will crop it by 1 character.
        if (ret.EndsWith("-")) ret = ret.Slice(0, ret.Length - 1);
        // After that, it will just replace a bumch of strings with new strings:
        return ret.Replace("UM", "M").Replace("UNA", "NA").Replace("UPC", "PC").Replace("UFM", "FM");
    }

    private static string Slice(this string source, int start, int end)
    {
        // If end < 0 (-2 for example), and the source.Length is 5, end will then be 3.
        // Len (length) will be 3 if start was 0. (3 - 0)
        // Returns a part of the string start from index 0 and ending at index 2. (0, 1, 2 = length of 3).
        if (end < 0) end = source.Length + end;
        var len = end - start;
        return source.Substring(start, len);
    }

    private static readonly int charA = Convert.ToInt32('a');

    static string LetterNotation(BigDouble number)
    {
        if (number < 1d)
        {
            return "0";
        }

        int n = (int)BigDouble.Log(number, 1000);
        BigDouble m = number / BigDouble.Pow(1000, n);
        string unit = "";

        if (n < 5)
        {
            unit = StandardNotation[n];
        }
        else
        {
            int unitInt = n - 5;
            int secondUnit = unitInt % 26;
            int firstUnit = unitInt / 26;
            unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
        }

        // Math.Floor(m * 100) / 100) fixes rounding errors
        return ((BigDouble.Floor(m * 100) / 100).ToString() + unit);
    }
    #endregion
}
