namespace Qurre.API.Addons
{
    static public class BetterColors
    {
        static public bool Enabled { get; internal set; } = true;

        static public string Reset(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Reset[0])}{text}{ParseCode(Codes.Reset[1])}";

        static public string Bold(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Bold[0])}{text}{ParseCode(Codes.Bold[1])}";
        static public string Dim(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Dim[0])}{text}{ParseCode(Codes.Dim[1])}";
        static public string Italic(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Italic[0])}{text}{ParseCode(Codes.Italic[1])}";
        static public string Underline(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Underline[0])}{text}{ParseCode(Codes.Underline[1])}";
        static public string Inverse(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Inverse[0])}{text}{ParseCode(Codes.Inverse[1])}";
        static public string Hidden(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Hidden[0])}{text}{ParseCode(Codes.Hidden[1])}";
        static public string Strikethrough(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Strikethrough[0])}{text}{ParseCode(Codes.Strikethrough[1])}";

        static public string Black(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Black[0])}{text}{ParseCode(Codes.Black[1])}";
        static public string Red(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Red[0])}{text}{ParseCode(Codes.Red[1])}";
        static public string Green(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Green[0])}{text}{ParseCode(Codes.Green[1])}";
        static public string Yellow(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Yellow[0])}{text}{ParseCode(Codes.Yellow[1])}";
        static public string Blue(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Blue[0])}{text}{ParseCode(Codes.Blue[1])}";
        static public string Magenta(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Magenta[0])}{text}{ParseCode(Codes.Magenta[1])}";
        static public string Cyan(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Cyan[0])}{text}{ParseCode(Codes.Cyan[1])}";
        static public string White(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.White[0])}{text}{ParseCode(Codes.White[1])}";
        static public string Grey(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.Grey[0])}{text}{ParseCode(Codes.Grey[1])}";

        static public string BrightRed(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightRed[0])}{text}{ParseCode(Codes.BrightRed[1])}";
        static public string BrightGreen(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightGreen[0])}{text}{ParseCode(Codes.BrightGreen[1])}";
        static public string BrightYellow(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightYellow[0])}{text}{ParseCode(Codes.BrightYellow[1])}";
        static public string BrightBlue(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightBlue[0])}{text}{ParseCode(Codes.BrightBlue[1])}";
        static public string BrightMagenta(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightMagenta[0])}{text}{ParseCode(Codes.BrightMagenta[1])}";
        static public string BrightCyan(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightCyan[0])}{text}{ParseCode(Codes.BrightCyan[1])}";
        static public string BrightWhite(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BrightWhite[0])}{text}{ParseCode(Codes.BrightWhite[1])}";

        static public string BgBlack(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBlack[0])}{text}{ParseCode(Codes.BgBlack[1])}";
        static public string BgRed(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgRed[0])}{text}{ParseCode(Codes.BgRed[1])}";
        static public string BgGreen(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgGreen[0])}{text}{ParseCode(Codes.BgGreen[1])}";
        static public string BgYellow(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgYellow[0])}{text}{ParseCode(Codes.BgYellow[1])}";
        static public string BgBlue(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBlue[0])}{text}{ParseCode(Codes.BgBlue[1])}";
        static public string BgMagenta(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgMagenta[0])}{text}{ParseCode(Codes.BgMagenta[1])}";
        static public string BgCyan(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgCyan[0])}{text}{ParseCode(Codes.BgCyan[1])}";
        static public string BgWhite(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgWhite[0])}{text}{ParseCode(Codes.BgWhite[1])}";
        static public string BgGrey(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgGrey[0])}{text}{ParseCode(Codes.BgGrey[1])}";

        static public string BgBrightRed(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightRed[0])}{text}{ParseCode(Codes.BgBrightRed[1])}";
        static public string BgBrightGreen(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightGreen[0])}{text}{ParseCode(Codes.BgBrightGreen[1])}";
        static public string BgBrightYellow(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightYellow[0])}{text}{ParseCode(Codes.BgBrightYellow[1])}";
        static public string BgBrightBlue(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightBlue[0])}{text}{ParseCode(Codes.BgBrightBlue[1])}";
        static public string BgBrightMagenta(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightMagenta[0])}{text}{ParseCode(Codes.BgBrightMagenta[1])}";
        static public string BgBrightCyan(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightCyan[0])}{text}{ParseCode(Codes.BgBrightCyan[1])}";
        static public string BgBrightWhite(object text) => !Enabled ? $"{text}" : $"{ParseCode(Codes.BgBrightWhite[0])}{text}{ParseCode(Codes.BgBrightWhite[1])}";

        static string ParseCode(int code) => $"\u001b[{code}m";


        static public class Codes
        {
            static public int[] Reset { get; } = new int[] { 0, 0 };

            static public int[] Bold { get; } = new int[] { 1, 22 };
            static public int[] Dim { get; } = new int[] { 2, 22 };
            static public int[] Italic { get; } = new int[] { 3, 23 };
            static public int[] Underline { get; } = new int[] { 4, 24 };
            static public int[] Inverse { get; } = new int[] { 7, 27 };
            static public int[] Hidden { get; } = new int[] { 8, 28 };
            static public int[] Strikethrough { get; } = new int[] { 9, 29 };

            static public int[] Black { get; } = new int[] { 30, 39 };
            static public int[] Red { get; } = new int[] { 31, 39 };
            static public int[] Green { get; } = new int[] { 32, 39 };
            static public int[] Yellow { get; } = new int[] { 33, 39 };
            static public int[] Blue { get; } = new int[] { 34, 39 };
            static public int[] Magenta { get; } = new int[] { 35, 39 };
            static public int[] Cyan { get; } = new int[] { 36, 39 };
            static public int[] White { get; } = new int[] { 37, 39 };
            static public int[] Grey { get; } = new int[] { 90, 39 };

            static public int[] BrightRed { get; } = new int[] { 91, 39 };
            static public int[] BrightGreen { get; } = new int[] { 92, 39 };
            static public int[] BrightYellow { get; } = new int[] { 93, 39 };
            static public int[] BrightBlue { get; } = new int[] { 94, 39 };
            static public int[] BrightMagenta { get; } = new int[] { 95, 39 };
            static public int[] BrightCyan { get; } = new int[] { 96, 39 };
            static public int[] BrightWhite { get; } = new int[] { 97, 39 };

            static public int[] BgBlack { get; } = new int[] { 40, 49 };
            static public int[] BgRed { get; } = new int[] { 41, 49 };
            static public int[] BgGreen { get; } = new int[] { 42, 49 };
            static public int[] BgYellow { get; } = new int[] { 43, 49 };
            static public int[] BgBlue { get; } = new int[] { 44, 49 };
            static public int[] BgMagenta { get; } = new int[] { 45, 49 };
            static public int[] BgCyan { get; } = new int[] { 46, 49 };
            static public int[] BgWhite { get; } = new int[] { 47, 49 };
            static public int[] BgGrey { get; } = new int[] { 100, 49 };

            static public int[] BgBrightRed { get; } = new int[] { 101, 49 };
            static public int[] BgBrightGreen { get; } = new int[] { 102, 49 };
            static public int[] BgBrightYellow { get; } = new int[] { 103, 49 };
            static public int[] BgBrightBlue { get; } = new int[] { 104, 49 };
            static public int[] BgBrightMagenta { get; } = new int[] { 105, 49 };
            static public int[] BgBrightCyan { get; } = new int[] { 106, 49 };
            static public int[] BgBrightWhite { get; } = new int[] { 107, 49 };
        }
    }
}