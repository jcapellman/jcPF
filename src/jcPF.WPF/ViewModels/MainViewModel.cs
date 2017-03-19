﻿using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using jcPF.WPF.Managers;
using jcPF.WPF.Objects;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace jcPF.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ConcurrentQueue<string> _packets;

        public ConcurrentQueue<string> Packets
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


            Packets = new ConcurrentQueue<string>();
        }

        public async Task<bool> RunScan()
        {
            var scanner = new PacketScanning();

            var cToken = new CancellationToken();

            scanner.NewPacketEntry += Scanner_NewPacketEntry;
            return await scanner.RunScan(cToken, SelectedDevice.PDevice);
        }

        private void Scanner_NewPacketEntry(object sender, string e)
        {
            Packets.Enqueue(e);

            Packets = new ConcurrentQueue<string>(Packets);
        }
    }
}