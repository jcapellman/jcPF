using System;

namespace jcPF.WPF.Objects
{
    public class PacketLogItem
    {
        public string Source { get; set; }

        public string Destination { get; set; }

        public int Size { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}