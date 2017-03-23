using System.Threading.Tasks;

using jcPF.WPF.DAL.Tables;
using jcPF.WPF.Objects;

namespace jcPF.WPF.DAL
{
    public class PacketTable : BaseDAL<Packets>
    {
        public async Task<bool> WritePacketAsync(PacketLogItem packetItem)
        {
            return await WriteAsync(new Packets
            {
                Size = packetItem.Size,
                DestinationPort = packetItem.DestinationPort,
                DestinationIP = packetItem.Destination,
                SourceIP = packetItem.Source,
                SourtPort = packetItem.SourcePort,
                Timestamp = packetItem.TimeStamp
            });
        }
    }
}