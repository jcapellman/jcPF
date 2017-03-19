using System;
using System.Threading;
using System.Threading.Tasks;

using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace jcPF.WPF.Managers
{
    public class PacketScanning
    {
        public event EventHandler<string> NewPacketEntry;

        public void OnNewPacketEntry(string e)
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

                                    OnNewPacketEntry(packet.Length.ToString());

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
