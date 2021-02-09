public class HardwareID
{
    public string LastID { get; private set; } = string.Empty;

    public string Generate()
    {
        var wmiData = new[]
        {
            GetProperties(WMI_CLASSES.MOTHERBOARD),
            GetProperties(WMI_CLASSES.CDROM),
            GetProperties(WMI_CLASSES.BIOS),
            GetProperties(WMI_CLASSES.GPU),
            GetProperties(WMI_CLASSES.CPU),
            GetProperties(WMI_CLASSES.HDD),
            GetProperties(WMI_CLASSES.ACCOUNT)
        };
        return LastID = GetHash(data: string.Join(string.Empty, wmiData));
    }

    private static string GetProperties(string[] wmiData)
    {
        var property = new StringBuilder();
        string query = CreateQuery(wmiData);
        using (var moSearcher = new ManagementObjectSearcher("root\\CIMV2", query))
        using (ManagementObjectCollection moCollection = moSearcher.Get())
        {
            foreach (ManagementBaseObject mbObject in moCollection)
                using (mbObject)
                {
                    for (int i = 1; i < wmiData.Length; i++)
                        property.Append(value: mbObject[propertyName: wmiData[i]]);
                }
        }

        return property.ToString();
    }

    private static string CreateQuery(string[] wmiData)
    {
        if (wmiData == null)
            throw new ArgumentNullException(paramName: nameof(wmiData));
        var query = new StringBuilder();
        string wmiClass = string.Empty;
        query.Append("SELECT ");
        for (int i = 0; i < wmiData.Length; i++)
            if (i == 0)
                wmiClass = wmiData[i];
            else
                query.Append(value: i < wmiData.Length - 1 ? $"{wmiData[i]}, " : $"{wmiData[i]} ");
        query.Append(value: $"FROM {wmiClass}");
        return query.ToString();
    }

    private static string GetHash(string data)
    {
        using (var sha = new SHA1CryptoServiceProvider())
        {
            var hash = sha.ComputeHash(buffer: Encoding.Default.GetBytes(data));
            return GetHexString(hash);
        }
    }

    private static string GetHexString(IList<byte> bt)
    {
        var sBuilder = new StringBuilder();
        for (int i = 0; i < bt.Count; i++)
        {
            byte b = bt[i];
            byte n = b;
            int n1 = n & 15;
            int n2 = (n >> 4) & 15;
            sBuilder.Append(value: n2 > 9
                ? ((char) (n2 - 10 + 'A')).ToString(CultureInfo.InvariantCulture)
                : n2.ToString(CultureInfo.InvariantCulture));
            sBuilder.Append(value: n1 > 9
                ? ((char) (n1 - 10 + 'A')).ToString(CultureInfo.InvariantCulture)
                : n1.ToString(CultureInfo.InvariantCulture));
            if (i + 1 != bt.Count && (i + 1) % 2 == 0)
                sBuilder.Append("-");
        }

        return sBuilder.ToString();
    }

    private static class WMI_CLASSES
    {
        public static readonly string[] MOTHERBOARD = {"Win32_BaseBoard", "Name", "Manufacturer", "Version"};
        public static readonly string[] GPU = {"Win32_VideoController", "Name", "DeviceID", "DriverVersion"};
        public static readonly string[] CDROM = {"Win32_CDROMDrive", "Name", "Manufacturer", "DeviceID"};
        public static readonly string[] CPU = {"Win32_Processor", "Name", "Manufacturer", "ProcessorId"};
        public static readonly string[] HDD = {"Win32_DiskDrive", "Name", "Manufacturer", "Model"};
        public static readonly string[] BIOS = {"Win32_BIOS", "Name", "Manufacturer", "SerialNumber"};
        public static readonly string[] ACCOUNT = {"Win32_UserAccount", "Name"};
    }
}
