using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Qurre.API.Addons;

[PublicAPI]
public static class BetterColors
{
    public static bool Enabled { get; internal set; } = true;

    public static string Reset(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Reset[0])}{text}{ParseCode(Codes.Reset[1])}";
    }

    public static string Bold(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Bold[0])}{text}{ParseCode(Codes.Bold[1])}";
    }

    public static string Dim(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Dim[0])}{text}{ParseCode(Codes.Dim[1])}";
    }

    public static string Italic(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Italic[0])}{text}{ParseCode(Codes.Italic[1])}";
    }

    public static string Underline(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Underline[0])}{text}{ParseCode(Codes.Underline[1])}";
    }

    public static string Inverse(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Inverse[0])}{text}{ParseCode(Codes.Inverse[1])}";
    }

    public static string Hidden(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Hidden[0])}{text}{ParseCode(Codes.Hidden[1])}";
    }

    public static string Strikethrough(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Strikethrough[0])}{text}{ParseCode(Codes.Strikethrough[1])}";
    }

    public static string Black(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Black[0])}{text}{ParseCode(Codes.Black[1])}";
    }

    public static string Red(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Red[0])}{text}{ParseCode(Codes.Red[1])}";
    }

    public static string Green(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Green[0])}{text}{ParseCode(Codes.Green[1])}";
    }

    public static string Yellow(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Yellow[0])}{text}{ParseCode(Codes.Yellow[1])}";
    }

    public static string Blue(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Blue[0])}{text}{ParseCode(Codes.Blue[1])}";
    }

    public static string Magenta(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Magenta[0])}{text}{ParseCode(Codes.Magenta[1])}";
    }

    public static string Cyan(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Cyan[0])}{text}{ParseCode(Codes.Cyan[1])}";
    }

    public static string White(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.White[0])}{text}{ParseCode(Codes.White[1])}";
    }

    public static string Grey(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.Grey[0])}{text}{ParseCode(Codes.Grey[1])}";
    }

    public static string BrightRed(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightRed[0])}{text}{ParseCode(Codes.BrightRed[1])}";
    }

    public static string BrightGreen(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightGreen[0])}{text}{ParseCode(Codes.BrightGreen[1])}";
    }

    public static string BrightYellow(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightYellow[0])}{text}{ParseCode(Codes.BrightYellow[1])}";
    }

    public static string BrightBlue(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightBlue[0])}{text}{ParseCode(Codes.BrightBlue[1])}";
    }

    public static string BrightMagenta(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightMagenta[0])}{text}{ParseCode(Codes.BrightMagenta[1])}";
    }

    public static string BrightCyan(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightCyan[0])}{text}{ParseCode(Codes.BrightCyan[1])}";
    }

    public static string BrightWhite(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightWhite[0])}{text}{ParseCode(Codes.BrightWhite[1])}";
    }

    public static string BgBlack(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBlack[0])}{text}{ParseCode(Codes.BgBlack[1])}";
    }

    public static string BgRed(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgRed[0])}{text}{ParseCode(Codes.BgRed[1])}";
    }

    public static string BgGreen(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgGreen[0])}{text}{ParseCode(Codes.BgGreen[1])}";
    }

    public static string BgYellow(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgYellow[0])}{text}{ParseCode(Codes.BgYellow[1])}";
    }

    public static string BgBlue(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBlue[0])}{text}{ParseCode(Codes.BgBlue[1])}";
    }

    public static string BgMagenta(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgMagenta[0])}{text}{ParseCode(Codes.BgMagenta[1])}";
    }

    public static string BgCyan(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgCyan[0])}{text}{ParseCode(Codes.BgCyan[1])}";
    }

    public static string BgWhite(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgWhite[0])}{text}{ParseCode(Codes.BgWhite[1])}";
    }

    public static string BgGrey(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgGrey[0])}{text}{ParseCode(Codes.BgGrey[1])}";
    }

    public static string BgBrightRed(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightRed[0])}{text}{ParseCode(Codes.BgBrightRed[1])}";
    }

    public static string BgBrightGreen(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightGreen[0])}{text}{ParseCode(Codes.BgBrightGreen[1])}";
    }

    public static string BgBrightYellow(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightYellow[0])}{text}{ParseCode(Codes.BgBrightYellow[1])}";
    }

    public static string BgBrightBlue(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightBlue[0])}{text}{ParseCode(Codes.BgBrightBlue[1])}";
    }

    public static string BgBrightMagenta(object text)
    {
        return !Enabled
            ? $"{text}"
            : $"{ParseCode(Codes.BgBrightMagenta[0])}{text}{ParseCode(Codes.BgBrightMagenta[1])}";
    }

    public static string BgBrightCyan(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightCyan[0])}{text}{ParseCode(Codes.BgBrightCyan[1])}";
    }

    public static string BgBrightWhite(object text)
    {
        return !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightWhite[0])}{text}{ParseCode(Codes.BgBrightWhite[1])}";
    }

    private static string ParseCode(int code)
    {
        return $"\u001b[{code}m";
    }


    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class Codes
    {
        public static int[] Reset { get; } = [0, 0];

        public static int[] Bold { get; } = [1, 22];
        public static int[] Dim { get; } = [2, 22];
        public static int[] Italic { get; } = [3, 23];
        public static int[] Underline { get; } = [4, 24];
        public static int[] Inverse { get; } = [7, 27];
        public static int[] Hidden { get; } = [8, 28];
        public static int[] Strikethrough { get; } = [9, 29];

        public static int[] Black { get; } = [30, 39];
        public static int[] Red { get; } = [31, 39];
        public static int[] Green { get; } = [32, 39];
        public static int[] Yellow { get; } = [33, 39];
        public static int[] Blue { get; } = [34, 39];
        public static int[] Magenta { get; } = [35, 39];
        public static int[] Cyan { get; } = [36, 39];
        public static int[] White { get; } = [37, 39];
        public static int[] Grey { get; } = [90, 39];

        public static int[] BrightRed { get; } = [91, 39];
        public static int[] BrightGreen { get; } = [92, 39];
        public static int[] BrightYellow { get; } = [93, 39];
        public static int[] BrightBlue { get; } = [94, 39];
        public static int[] BrightMagenta { get; } = [95, 39];
        public static int[] BrightCyan { get; } = [96, 39];
        public static int[] BrightWhite { get; } = [97, 39];

        public static int[] BgBlack { get; } = [40, 49];
        public static int[] BgRed { get; } = [41, 49];
        public static int[] BgGreen { get; } = [42, 49];
        public static int[] BgYellow { get; } = [43, 49];
        public static int[] BgBlue { get; } = [44, 49];
        public static int[] BgMagenta { get; } = [45, 49];
        public static int[] BgCyan { get; } = [46, 49];
        public static int[] BgWhite { get; } = [47, 49];
        public static int[] BgGrey { get; } = [100, 49];

        public static int[] BgBrightRed { get; } = [101, 49];
        public static int[] BgBrightGreen { get; } = [102, 49];
        public static int[] BgBrightYellow { get; } = [103, 49];
        public static int[] BgBrightBlue { get; } = [104, 49];
        public static int[] BgBrightMagenta { get; } = [105, 49];
        public static int[] BgBrightCyan { get; } = [106, 49];
        public static int[] BgBrightWhite { get; } = [107, 49];
    }
}