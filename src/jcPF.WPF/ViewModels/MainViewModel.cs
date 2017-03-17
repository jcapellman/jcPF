using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
            
            using (PacketCommunicator communicator =
                pd.Open(65536, // portion of the packet to capture
                    // 65536 guarantees that the whole packet will be captured on all the link layers
                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                    1000)) // read timeout
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
