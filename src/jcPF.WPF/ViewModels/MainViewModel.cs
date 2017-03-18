using System.Collections.ObjectModel;
using System.Linq;
using jcPF.WPF.Objects;
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

        private ObservableCollection<DeviceListingItem> _devices;

        public ObservableCollection<DeviceListingItem> Devices
        {
            get { return _devices; }
            set { _devices = value; OnPropertyChanged(); }
        }

        private DeviceListingItem _selectedDevice;

        public DeviceListingItem SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; OnPropertyChanged(); }
        }

        public void LoadData()
        {
            var devices = LivePacketDevice.AllLocalMachine;

            Devices = new ObservableCollection<DeviceListingItem>(devices.Select(a => new DeviceListingItem
            {
                PDevice = a
            }).ToList());            
        }

        public void RunScan()
        {
            Packets = new ObservableCollection<string>();

            var pd = SelectedDevice;

            using (var communicator = pd.PDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
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