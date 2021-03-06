# C#/VB.NET Hardware ID Generator (HWID / HardwareID)
An API that makes efficient use of the [Windows Management Instrument Win32 Providers](https://msdn.microsoft.com/en-us/library/aa394388(v=vs.85).aspx) to generate a unique identifier (Hardware ID) based upon the properties of the Motherboard, CDROM, BIOS, HDD, GPU, and CPU. This APIs main purpose is to allow software to remain locked to one PC (or multiple) depending upon its utilization.

#### Windows Manage Instruments based on the Win32 Provider Classes (WMI) 
- [Win32_BaseBoard](https://msdn.microsoft.com/en-us/library/aa394072(v=vs.85).aspx)
- [Win32_DiskDrive](https://msdn.microsoft.com/en-us/library/aa394132(v=vs.85).aspx)
- [Win32_BIOS](https://msdn.microsoft.com/en-us/library/aa394077(v=vs.85).aspx)
- [Win32_VideoController](https://msdn.microsoft.com/en-us/library/aa394512(v=vs.85).aspx)
- [Win32_CDROMDrive](https://msdn.microsoft.com/en-us/library/aa394081(v=vs.85).aspx)

#### Usage

C# Syntax Usage:
```cs
//Initialize instance of HardwareID
HardwareID HWID = new HardwareID();

//Generate Unique ID based upon hardware of current PC.
Console.WriteLine(HWID.Generate());

//Every time *Generate* is called it sets the *LastID* property.
Console.WriteLine(HWID.LastID)

//The internal *GetProperties* function uses a string array that follows the format { WMI_CLASS, Properties... }. 
//                           Motherboard      --> Parameters
// string[] MOTHERBOARD = { "Win32_BaseBoard", "Name", "Manufacturer", "Version" };
GetProperties(WMI_CLASSES.MOTHERBOARD);

private static string GetProcessorProperties()
{
    return GetProperties(WMI_CLASSES.CPU);
}
```

VB.NET Syntax Usage:
```vb
'Initialize instance of HardwareID
Dim HWID As New HardwareID()

'Generate Unique ID based upon hardware of current PC
Console.WriteLine(HWID.Generate())

'Every time Generate is called LastID is set
Console.WriteLine(HWID.LastID)

'The internal *GetProperties* function uses a string array that follows the format { WMI_CLASS, Properties... }. 
'                                              Motherboard      --> Parameters
' Dim MOTHERBOARD As String() = New String() { "Win32_BaseBoard", "Name", "Manufacturer", "Version" }
Private Shared Function GetProcessorProperties()
    Return GetProperties(WMI_CLASSES.CPU)
End Function
```

Every property gathered from the WMI class is hashed together with SHA1 and then turned into a *Hex* string.
```cs
//Output: A7EA-FB91-9995-84B7-E843-CCB4-4E81-51B9-2B3B-6587
```  
**Updates**
- Increased speed and efficiency.
- Found reliable properties of each piece of hardware. 

**TO-DO**
- Further Optimize WMI Access & Query Generation
- Check if Hardware Exists, if not then attempt other options (Runtime Error Prevention) 

**Issues**
- Assumes CDROMDrive exists.  

**License**  
GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007
