using System;
using System.Threading;
using System.Threading.Tasks;

using jcPF.WPF.Objects;

using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace jcPF.WPF.Managers
{
    public class PacketScanning
    {
        public event EventHandler<PacketLogItem> NewPacketEntry;

        public void OnNewPacketEntry(PacketLogItem e)
        {
            NewPacketEntry?.Invoke(null, e);
        }

        public async Task<bool> RunScan(CancellationToken token, PacketDevice pd)
        {
            return await Task.Run(() =>
                {
                    using (var communicator = pd.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
                    {
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

                                    var packetItem = new PacketLogItem
                                    {
                                        Destination = packet.IpV4.Destination.ToString(),
                                        Size = packet.Length,
                                        Source = packet.IpV4.Source.ToString(),
                                        TimeStamp = DateTime.Now
                                    };

                                    OnNewPacketEntry(packetItem);

                                    break;
                            }

                            if (token.IsCancellationRequested)
                            {
                                return false;
                            }
                        } while (true);
                    }
                }, token);
        }
    }
}
