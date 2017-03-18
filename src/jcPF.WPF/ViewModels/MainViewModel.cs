using System.Collections.ObjectModel;

using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace jcPF.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<string> _packets;

        public ObservableCollection<string> Packets
        {
            get { return _packets; }
            set { _packets = value; OnPropertyChanged(); }
        }
        
        public void LoadData()
        {
            var allDevices = LivePacketDevice.AllLocalMachine;
            Packets = new ObservableCollection<string>();

            PacketDevice pd = allDevices[0];
            
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

                            Packets.Add(packet.IpV4.Destination.ToString());
                            break;
                    }
                } while (true);
            }
        }
    }
}