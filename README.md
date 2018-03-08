# Hardware ID Generator
Use WMI to generate a unique identifier for a user based on Motherboard, CDROM, BIOS, HDD, GPU, and CPU. Optimized HardwareID generator for licensing or locking user to one PC.

#### Windows Manage Instruments Classes (WMI) 
- [Win32_BaseBoard](https://msdn.microsoft.com/en-us/library/aa394072(v=vs.85).aspx)
- [Win32_DiskDrive](https://msdn.microsoft.com/en-us/library/aa394132(v=vs.85).aspx)
- [Win32_BIOS](https://msdn.microsoft.com/en-us/library/aa394077(v=vs.85).aspx)
- [Win32_VideoController](https://msdn.microsoft.com/en-us/library/aa394512(v=vs.85).aspx)
- [Win32_CDROMDrive](https://msdn.microsoft.com/en-us/library/aa394081(v=vs.85).aspx)

#### Usage

General usage:
```cs
HardwareID HWID = new HardwareID();
Console.WriteLine(HWID.Generate());
```

Every call to *Generate* sets the LastID property.
```cs
HWID.Generate();
Console.WriteLine(HWID.LastID);
```

The internal *GetProperties* function uses a string array named _wmiData_.
```cs  
//                           Motherboard      --> Parameters
// string[] MOTHERBOARD = { "Win32_BaseBoard", "Name", "Manufacturer", "Version" };
GetProperties(WMI_CLASSES.MOTHERBOARD);

private static string GetProcessorProperties()
{
    return GetProperties(WMI_CLASSES.CPU);
}
```  

The information gathered is then hashed with SHA1 leaving the output of:
```cs
//Output: A7EA-FB91-9995-84B7-E843-CCB4-4E81-51B9-2B3B-6587
```
**License**  

GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007
