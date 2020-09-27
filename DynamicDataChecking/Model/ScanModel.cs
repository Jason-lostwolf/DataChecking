using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataChecking
{
    public class ScanModel
    {
        public string DeviceId { get; set; }
        public string ModelNumber { get; set; }
        public string HardwareVersion { get; set; }
        public string QRCodeFormat { get; set; }
        public string EncryptedQRCode { get; set; }
        public string ScanTime { get; set; }
        public string Barcode { get; set; }
    }
}
