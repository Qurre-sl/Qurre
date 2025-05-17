using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Qurre.API.Addons;

[PublicAPI]
public static class BetterColors
{
    public static bool IsEnabled { get; internal set; } = true;

    public static string Reset(object text) => Apply(text, Style.Reset);
    public static string Bold(object text) => Apply(text, Style.Bold);
    public static string Dim(object text) => Apply(text, Style.Dim);
    public static string Italic(object text) => Apply(text, Style.Italic);
    public static string Underline(object text) => Apply(text, Style.Underline);
    public static string Inverse(object text) => Apply(text, Style.Inverse);
    public static string Hidden(object text) => Apply(text, Style.Hidden);
    public static string Strikethrough(object text) => Apply(text, Style.Strikethrough);

    public static string Black(object text) => Apply(text, Style.Black);
    public static string Red(object text) => Apply(text, Style.Red);
    public static string Green(object text) => Apply(text, Style.Green);
    public static string Yellow(object text) => Apply(text, Style.Yellow);
    public static string Blue(object text) => Apply(text, Style.Blue);
    public static string Magenta(object text) => Apply(text, Style.Magenta);
    public static string Cyan(object text) => Apply(text, Style.Cyan);
    public static string White(object text) => Apply(text, Style.White);
    public static string Grey(object text) => Apply(text, Style.Grey);

    public static string BrightRed(object text) => Apply(text, Style.BrightRed);
    public static string BrightGreen(object text) => Apply(text, Style.BrightGreen);
    public static string BrightYellow(object text) => Apply(text, Style.BrightYellow);
    public static string BrightBlue(object text) => Apply(text, Style.BrightBlue);
    public static string BrightMagenta(object text) => Apply(text, Style.BrightMagenta);
    public static string BrightCyan(object text) => Apply(text, Style.BrightCyan);
    public static string BrightWhite(object text) => Apply(text, Style.BrightWhite);

    public static string BgBlack(object text) => Apply(text, Style.BgBlack);
    public static string BgRed(object text) => Apply(text, Style.BgRed);
    public static string BgGreen(object text) => Apply(text, Style.BgGreen);
    public static string BgYellow(object text) => Apply(text, Style.BgYellow);
    public static string BgBlue(object text) => Apply(text, Style.BgBlue);
    public static string BgMagenta(object text) => Apply(text, Style.BgMagenta);
    public static string BgCyan(object text) => Apply(text, Style.BgCyan);
    public static string BgWhite(object text) => Apply(text, Style.BgWhite);
    public static string BgGrey(object text) => Apply(text, Style.BgGrey);

    public static string BgBrightRed(object text) => Apply(text, Style.BgBrightRed);
    public static string BgBrightGreen(object text) => Apply(text, Style.BgBrightGreen);
    public static string BgBrightYellow(object text) => Apply(text, Style.BgBrightYellow);
    public static string BgBrightBlue(object text) => Apply(text, Style.BgBrightBlue);
    public static string BgBrightMagenta(object text) => Apply(text, Style.BgBrightMagenta);
    public static string BgBrightCyan(object text) => Apply(text, Style.BgBrightCyan);
    public static string BgBrightWhite(object text) => Apply(text, Style.BgBrightWhite);

    public static string Apply(object text, Style style)
    {
        return !IsEnabled ? text.ToString() : string.Concat(ParseCode(style.Begin), text, ParseCode(style.End));
    }

    private static string ParseCode(int code)
    {
        // ReSharper disable once CanSimplifyStringEscapeSequence
        return $"\u001b[{code}m";
    }

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public readonly struct Style(int begin, int end)
    {
        public int Begin { get; } = begin;
        public int End { get; } = end;

        public static readonly Style Reset = new(0, 0);

        public static readonly Style Bold = new(1, 22);
        public static readonly Style Dim = new(2, 22);
        public static readonly Style Italic = new(3, 23);
        public static readonly Style Underline = new(4, 24);
        public static readonly Style Inverse = new(7, 27);
        public static readonly Style Hidden = new(8, 28);
        public static readonly Style Strikethrough = new(9, 29);

        public static readonly Style Black = new(30, 39);
        public static readonly Style Red = new(31, 39);
        public static readonly Style Green = new(32, 39);
        public static readonly Style Yellow = new(33, 39);
        public static readonly Style Blue = new(34, 39);
        public static readonly Style Magenta = new(35, 39);
        public static readonly Style Cyan = new(36, 39);
        public static readonly Style White = new(37, 39);
        public static readonly Style Grey = new(90, 39);

        public static readonly Style BrightRed = new(91, 39);
        public static readonly Style BrightGreen = new(92, 39);
        public static readonly Style BrightYellow = new(93, 39);
        public static readonly Style BrightBlue = new(94, 39);
        public static readonly Style BrightMagenta = new(95, 39);
        public static readonly Style BrightCyan = new(96, 39);
        public static readonly Style BrightWhite = new(97, 39);

        public static readonly Style BgBlack = new(40, 49);
        public static readonly Style BgRed = new(41, 49);
        public static readonly Style BgGreen = new(42, 49);
        public static readonly Style BgYellow = new(43, 49);
        public static readonly Style BgBlue = new(44, 49);
        public static readonly Style BgMagenta = new(45, 49);
        public static readonly Style BgCyan = new(46, 49);
        public static readonly Style BgWhite = new(47, 49);
        public static readonly Style BgGrey = new(100, 49);

        public static readonly Style BgBrightRed = new(101, 49);
        public static readonly Style BgBrightGreen = new(102, 49);
        public static readonly Style BgBrightYellow = new(103, 49);
        public static readonly Style BgBrightBlue = new(104, 49);
        public static readonly Style BgBrightMagenta = new(105, 49);
        public static readonly Style BgBrightCyan = new(106, 49);
        public static readonly Style BgBrightWhite = new(107, 49);
    }
}