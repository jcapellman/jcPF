using System;

using SQLite;

namespace jcPF.WPF.DAL.Tables
{
    public class Packets
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string SourceIP { get; set; }

        public int SourtPort { get; set; }

        public string DestinationIP { get; set; }

        public int DestinationPort { get; set; }

        public int Size { get; set; }

        public DateTime Timestamp { get; set; }
    }
}