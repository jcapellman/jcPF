using System;
using System.Threading;
using System.Threading.Tasks;
using jcPF.WPF.DAL;
using jcPF.WPF.Objects;

using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace jcPF.WPF.Managers
{
    public class PacketScanning
    {
        public event EventHandler<PacketLogItem> NewPacketEntry;

        public void OnNewPacketEntry(PacketLogItem e)
        {
            NewPacketEntry?.Invoke(null, e);
        }
        
        public async Task<bool> RunScan(CancellationTokenSource token, PacketDevice pd)
        {
            return await Task.Run(async () =>
            {
                using (var communicator = pd.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    using (var filter = communicator.CreateFilter("ip and udp"))
                    {                 
                        communicator.SetFilter(filter);
                    }

                    var packetTable = new PacketTable();

                    do
                    {
                        Packet packet;

                        var result = communicator.ReceivePacket(out packet);

                        switch (result)
                        {
                            case PacketCommunicatorReceiveResult.Ok:
                                if (packet.IpV4 == null)
                                {
                                    continue;
                                }

                                var ip = packet.Ethernet.IpV4;
                                var udp = ip.Udp;

                                var packetItem = new PacketLogItem
                                {
                                    Destination = ip.Destination.ToString(),
                                    DestinationPort = udp.DestinationPort,
                                    Size = packet.Length,
                                    Source = ip.Source.ToString(),
                                    SourcePort = udp.SourcePort,
                                    TimeStamp = DateTime.Now
                                };

                                OnNewPacketEntry(packetItem);

                                await packetTable.WritePacketAsync(packetItem);

                                break;
                        }
                    } while (true);

                    return true;
                }
            }, token.Token);
        }
    }
}
