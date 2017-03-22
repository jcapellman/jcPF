using System;
using System.Threading;
using System.Threading.Tasks;

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

        private static void PacketHandler(Packet packet)
        {
            IpV4Datagram ip = packet.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;
            
            Console.WriteLine(ip.Source + ":" + udp.SourcePort + " -> " + ip.Destination + ":" + udp.DestinationPort);
        }

        public async Task<bool> RunScan(CancellationTokenSource token, PacketDevice pd)
        {
            return await Task.Run(() =>
            {
                using (var communicator = pd.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    using (var filter = communicator.CreateFilter("ip and udp"))
                    {                 
                        communicator.SetFilter(filter);
                    }
                    
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

                                break;
                        }
                    } while (true);

                    return true;
                }
            }, token.Token);
        }
    }
}
