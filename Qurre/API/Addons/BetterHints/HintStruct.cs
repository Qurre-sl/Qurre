namespace Qurre.API.Addons.BetterHints
{
    public class HintStruct
    {
        public HintStruct(int pos, int vf, string msg, int dur, bool @static)
        {
            Position = pos;
            Voffset = vf;
            Message = msg;
            Duration = dur;
            Static = @static;
        }

        public int Position { get; }
        public int Voffset { get; }
        public string Message { get; set; }
        public int Duration { get; }
        public bool Static { get; }
    }
}