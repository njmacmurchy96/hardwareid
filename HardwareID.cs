using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace HardwareIDGenerator
{
    public class HardwareID
    {
        private static class WMI_CLASSES
        {
	    public static readonly string[] MOTHERBOARD = { "Win32_BaseBoard", "Name", "Manufacturer", "Version" };
	    public static readonly string[] GPU = { "Win32_VideoController", "Name", "DeviceID", "DriverVersion" };
            public static readonly string[] CDROM = { "Win32_CDROMDrive", "Name", "Manufacturer", "DeviceID" };
            public static readonly string[] CPU = { "Win32_Processor", "Name", "Manufacturer", "ProcessorId" };
	    public static readonly string[] HDD = { "Win32_DiskDrive", "Name", "Manufacturer", "Model" };
            public static readonly string[] BIOS = { "Win32_BIOS", "Name", "Manufacturer", "Version" };
        }

        public string LastID { get; private set; } = string.Empty;
        
        public HardwareID() { }
        ~HardwareID() { }
        
        public string Generate()
        {
            var wmiData = new string[]
            {
                GetProperties(WMI_CLASSES.MOTHERBOARD),
                GetProperties(WMI_CLASSES.CDROM),
                GetProperties(WMI_CLASSES.BIOS),
                GetProperties(WMI_CLASSES.GPU),
                GetProperties(WMI_CLASSES.CPU),
                GetProperties(WMI_CLASSES.HDD)
            };
            return LastID = GetHash(string.Join(string.Empty, wmiData));
        }

        private string GetProperties(string[] wmiData)
        {
            var property = new StringBuilder();
        	var query = GenerateQuery(wmiData);
            using (var moSearcher = new ManagementObjectSearcher("root\\CIMV2", query))
            using (var moCollection = moSearcher.Get())
                foreach (var mbObject in moCollection)
                    using (mbObject)
                        for (int i = 1; i < wmiData.Length; i++)
                            property.Append(mbObject[wmiData[i]]);
            return property.ToString();
        }
        
        private string GenerateQuery(string[] wmiData)
        {
        	var query = new StringBuilder();
        	var wmiClass = string.Empty;
        	query.Append("SELECT ");
        	for (int i = 0; i < wmiData.Length; i++)
        		if (i == 0)
        			wmiClass = wmiData[i];
				else
					query.Append( (i < wmiData.Length - 1) ? $"{wmiData[i]}, " : $"{wmiData[i]} " );
        	query.Append($"FROM {wmiClass}");
        	return query.ToString();
        }

        private string GetHash(string data)
        {
            using (var sha = new SHA1CryptoServiceProvider())
            {
                var hash = sha.ComputeHash(Encoding.Default.GetBytes(data));
                return GetHexString(hash);
            }
        }

        private string GetHexString(IList<byte> bt)
        {
            var sBuilder = new StringBuilder();
            for (var i = 0; i < bt.Count; i++)
            {
                var b = bt[i];
                var n = b;
                var n1 = n & 15;
                var n2 = (n >> 4) & 15;
                if (n2 > 9)
                    sBuilder.Append(((char)(n2 - 10 + 'A')).ToString(CultureInfo.InvariantCulture));
                else
                    sBuilder.Append(n2.ToString(CultureInfo.InvariantCulture));
                if (n1 > 9)
                    sBuilder.Append(((char)(n1 - 10 + 'A')).ToString(CultureInfo.InvariantCulture));
                else
                    sBuilder.Append(n1.ToString(CultureInfo.InvariantCulture));
                if ((i + 1) != bt.Count && (i + 1) % 2 == 0)
                    sBuilder.Append("-");
            }
            return sBuilder.ToString();
        }
    }
}
