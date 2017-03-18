using PcapDotNet.Core;

namespace jcPF.WPF.Objects
{
    public class DeviceListingItem
    {
        public string Name => PDevice.Description;

        public PacketDevice PDevice { get; set; }
    }
}