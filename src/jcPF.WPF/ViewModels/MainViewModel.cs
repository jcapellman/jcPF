using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using jcPF.WPF.Managers;
using jcPF.WPF.Objects;

using PcapDotNet.Core;

namespace jcPF.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Visibility _ScanButtonVisibility;

        public Visibility ScanButtonVisibility
        {
            get { return _ScanButtonVisibility; }
            set { _ScanButtonVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _CancelScanButtonVisibility;

        public Visibility CancelScanButtonVisibility
        {
            get { return _CancelScanButtonVisibility; }
            set { _CancelScanButtonVisibility = value; OnPropertyChanged(); }
        }

        private ConcurrentQueue<PacketLogItem> _packets;

        public ConcurrentQueue<PacketLogItem> Packets
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
            set {
                _selectedDevice = value;
                OnPropertyChanged();
                EnableScanButton = value != null;
            }
        }

        private bool _enableScanButton;

        public bool EnableScanButton
        {
            get { return _enableScanButton; }
            set {
                _enableScanButton = value;
                OnPropertyChanged();
                
                if (value)
                {
                    CancelScanButtonVisibility = Visibility.Hidden;
                    ScanButtonVisibility = Visibility.Visible;
                }
            }
        }

        private CancellationTokenSource _cToken;

        public void LoadData()
        {
            ScanButtonVisibility = Visibility.Visible;
            CancelScanButtonVisibility = Visibility.Hidden;
            EnableScanButton = false;

            var devices = LivePacketDevice.AllLocalMachine;

            Devices = new ObservableCollection<DeviceListingItem>(devices.Select(a => new DeviceListingItem
            {
                PDevice = a
            }).ToList());
            
            Packets = new ConcurrentQueue<PacketLogItem>();
        }

        public async Task<bool> RunScan()
        {
            ScanButtonVisibility = Visibility.Hidden;
            CancelScanButtonVisibility = Visibility.Visible;

            var scanner = new PacketScanning();

            _cToken = new CancellationTokenSource();
            
            scanner.NewPacketEntry += Scanner_NewPacketEntry;
            return await scanner.RunScan(_cToken, SelectedDevice.PDevice);
        }

        private void Scanner_NewPacketEntry(object sender, PacketLogItem e)
        {
            Packets.Enqueue(e);

            Packets = new ConcurrentQueue<PacketLogItem>(Packets);
        }

        public void CancelScan()
        {
            _cToken.Cancel();

            EnableScanButton = true;
        }
    }
}