using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace NintrollerLib
{
    public sealed class DevicePropertyKey
    {
        internal struct DEVPROPKEY
        {
            public Guid fmtid;
            public uint pid;
        };

        internal readonly DEVPROPKEY _propKey;

        public DevicePropertyKey(Guid propertyCategory, uint propertyId)
        {
            this._propKey = new DEVPROPKEY() { fmtid = propertyCategory, pid = propertyId };
        }
    }

    public static class DevicePropertyKeys
    {
        public static readonly DevicePropertyKey DEVPKEY_Device_DeviceDesc = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 2);
        public static readonly DevicePropertyKey DEVPKEY_Device_HardwareIds = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 3);     // DEVPROP_TYPE_STRING_LIST

        public static readonly DevicePropertyKey DEVPKEY_Device_CompatibleIds = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 4);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_Service = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 6);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_Class = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 9);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_ClassGuid = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 10);    // DEVPROP_TYPE_GUID
        public static readonly DevicePropertyKey DEVPKEY_Device_Driver = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 11);    // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_ConfigFlags = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 12);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_Manufacturer = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 13);    // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_FriendlyName = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 14);    // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_LocationInfo = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 15);    // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_PDOName = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 16);    // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_Capabilities = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 17);    // DEVPROP_TYPE_UNINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_UINumber = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 18);    // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_UpperFilters = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 19);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_LowerFilters = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 20);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_BusTypeGuid = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 21);    // DEVPROP_TYPE_GUID
        public static readonly DevicePropertyKey DEVPKEY_Device_LegacyBusType = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 22);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_BusNumber = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 23);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_EnumeratorName = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 24);    // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_Security = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 25);    // DEVPROP_TYPE_SECURITY_DESCRIPTOR
        public static readonly DevicePropertyKey DEVPKEY_Device_SecuritySDS = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 26);    // DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_DevType = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 27);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_Exclusive = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 28);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_Device_Characteristics = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 29);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_Address = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 30);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_UINumberDescFormat = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 31);    // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_PowerData = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 32);    // DEVPROP_TYPE_BINARY
        public static readonly DevicePropertyKey DEVPKEY_Device_RemovalPolicy = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 33);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_RemovalPolicyDefault = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 34);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_RemovalPolicyOverride = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 35);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_InstallState = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 36);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_LocationPaths = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 37);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_BaseContainerId = new DevicePropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 38);    // DEVPROP_TYPE_GUID

        public static readonly DevicePropertyKey DEVPKEY_Device_DevNodeStatus = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 2);     // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_ProblemCode = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 3);     // DEVPROP_TYPE_UINT32

        public static readonly DevicePropertyKey DEVPKEY_Device_EjectionRelations = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 4);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_RemovalRelations = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 5);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_PowerRelations = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 6);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_BusRelations = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 7);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_Parent = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 8);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_Children = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 9);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_Siblings = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 10);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_TransportRelations = new DevicePropertyKey(new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), 11);    // DEVPROP_TYPE_STRING_LIST

        public static readonly DevicePropertyKey DEVPKEY_Device_Reported = new DevicePropertyKey(new Guid(0x80497100, 0x8c73, 0x48b9, 0xaa, 0xd9, 0xce, 0x38, 0x7e, 0x19, 0xc5, 0x6e), 2);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_Device_Legacy = new DevicePropertyKey(new Guid(0x80497100, 0x8c73, 0x48b9, 0xaa, 0xd9, 0xce, 0x38, 0x7e, 0x19, 0xc5, 0x6e), 3);     // DEVPROP_TYPE_BOOLEAN

        public static readonly DevicePropertyKey DEVPKEY_Device_InstanceId = new DevicePropertyKey(new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57), 256);   // DEVPROP_TYPE_STRING

        public static readonly DevicePropertyKey DEVPKEY_Device_ContainerId = new DevicePropertyKey(new Guid(0x8c7ed206, 0x3f8a, 0x4827, 0xb3, 0xab, 0xae, 0x9e, 0x1f, 0xae, 0xfc, 0x6c), 2);     // DEVPROP_TYPE_GUID

        public static readonly DevicePropertyKey DEVPKEY_Device_ModelId = new DevicePropertyKey(new Guid(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b), 2);     // DEVPROP_TYPE_GUID
        public static readonly DevicePropertyKey DEVPKEY_Device_FriendlyNameAttributes = new DevicePropertyKey(new Guid(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b), 3);     // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_ManufacturerAttributes = new DevicePropertyKey(new Guid(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b), 4);     // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_PresenceNotForDevice = new DevicePropertyKey(new Guid(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b), 5);     // DEVPROP_TYPE_BOOLEAN

        public static readonly DevicePropertyKey DEVPKEY_Numa_Proximity_Domain = new DevicePropertyKey(new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2), 1);     // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_DHP_Rebalance_Policy = new DevicePropertyKey(new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2), 2);     // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_Numa_Node = new DevicePropertyKey(new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2), 3);     // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_BusReportedDeviceDesc = new DevicePropertyKey(new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2), 4);     // DEVPROP_TYPE_STRING

        public static readonly DevicePropertyKey DEVPKEY_Device_SessionId = new DevicePropertyKey(new Guid(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29), 6);     // DEVPROP_TYPE_UINT32

        public static readonly DevicePropertyKey DEVPKEY_Device_InstallDate = new DevicePropertyKey(new Guid(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29), 100);   // DEVPROP_TYPE_FILETIME
        public static readonly DevicePropertyKey DEVPKEY_Device_FirstInstallDate = new DevicePropertyKey(new Guid(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29), 101);   // DEVPROP_TYPE_FILETIME

        public static readonly DevicePropertyKey DEVPKEY_Device_DriverDate = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 2);      // DEVPROP_TYPE_FILETIME
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverVersion = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 3);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverDesc = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 4);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverInfPath = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 5);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverInfSection = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 6);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverInfSectionExt = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 7);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_MatchingDeviceId = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 8);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverProvider = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 9);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverPropPageProvider = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 10);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverCoInstallers = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 11);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_Device_ResourcePickerTags = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 12);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_ResourcePickerExceptions = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 13);   // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverRank = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 14);     // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_Device_DriverLogoLevel = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 15);     // DEVPROP_TYPE_UINT32

        public static readonly DevicePropertyKey DEVPKEY_Device_NoConnectSound = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 17);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_Device_GenericDriverInstalled = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 18);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_Device_AdditionalSoftwareRequested = new DevicePropertyKey(new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), 19); //DEVPROP_TYPE_BOOLEAN

        public static readonly DevicePropertyKey DEVPKEY_Device_SafeRemovalRequired = new DevicePropertyKey(new Guid(0xafd97640, 0x86a3, 0x4210, 0xb6, 0x7c, 0x28, 0x9c, 0x41, 0xaa, 0xbe, 0x55), 2);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_Device_SafeRemovalRequiredOverride = new DevicePropertyKey(new Guid(0xafd97640, 0x86a3, 0x4210, 0xb6, 0x7c, 0x28, 0x9c, 0x41, 0xaa, 0xbe, 0x55), 3); // DEVPROP_TYPE_BOOLEAN

        public static readonly DevicePropertyKey DEVPKEY_DrvPkg_Model = new DevicePropertyKey(new Guid(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32), 2);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DrvPkg_VendorWebSite = new DevicePropertyKey(new Guid(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32), 3);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DrvPkg_DetailedDescription = new DevicePropertyKey(new Guid(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32), 4);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DrvPkg_DocumentationLink = new DevicePropertyKey(new Guid(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32), 5);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DrvPkg_Icon = new DevicePropertyKey(new Guid(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32), 6);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_DrvPkg_BrandingIcon = new DevicePropertyKey(new Guid(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32), 7);     // DEVPROP_TYPE_STRING_LIST

        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_UpperFilters = new DevicePropertyKey(new Guid(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b), 19);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_LowerFilters = new DevicePropertyKey(new Guid(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b), 20);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_Security = new DevicePropertyKey(new Guid(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b), 25);    // DEVPROP_TYPE_SECURITY_DESCRIPTOR
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_SecuritySDS = new DevicePropertyKey(new Guid(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b), 26);    // DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_DevType = new DevicePropertyKey(new Guid(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b), 27);    // DEVPROP_TYPE_UINT32
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_Exclusive = new DevicePropertyKey(new Guid(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b), 28);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_Characteristics = new DevicePropertyKey(new Guid(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b), 29);    // DEVPROP_TYPE_UINT32

        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_Name = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 2);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_ClassName = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 3);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_Icon = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 4);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_ClassInstaller = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 5);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_PropPageProvider = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 6);      // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_NoInstallClass = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 7);      // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_NoDisplayClass = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 8);      // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_SilentInstall = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 9);      // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_NoUseClass = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 10);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_DefaultService = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 11);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_IconPath = new DevicePropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 12);     // DEVPROP_TYPE_STRING_LIST

        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_DHPRebalanceOptOut = new DevicePropertyKey(new Guid(0xd14d3ef3, 0x66cf, 0x4ba2, 0x9d, 0x38, 0x0d, 0xdb, 0x37, 0xab, 0x47, 0x01), 2);    // DEVPROP_TYPE_BOOLEAN

        public static readonly DevicePropertyKey DEVPKEY_DeviceClass_ClassCoInstallers = new DevicePropertyKey(new Guid(0x713d1703, 0xa2e2, 0x49f5, 0x92, 0x14, 0x56, 0x47, 0x2e, 0xf3, 0xda, 0x5c), 2);     // DEVPROP_TYPE_STRING_LIST

        public static readonly DevicePropertyKey DEVPKEY_DeviceInterface_FriendlyName = new DevicePropertyKey(new Guid(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22), 2);     // DEVPROP_TYPE_STRING
        public static readonly DevicePropertyKey DEVPKEY_DeviceInterface_Enabled = new DevicePropertyKey(new Guid(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22), 3);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceInterface_ClassGuid = new DevicePropertyKey(new Guid(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22), 4);     // DEVPROP_TYPE_GUID

        public static readonly DevicePropertyKey DEVPKEY_DeviceInterfaceClass_DefaultInterface = new DevicePropertyKey(new Guid(0x14c83a99, 0x0b3f, 0x44b7, 0xbe, 0x4c, 0xa1, 0x78, 0xd3, 0x99, 0x05, 0x64), 2); // DEVPROP_TYPE_STRING

        public static readonly DevicePropertyKey DEVPKEY_DeviceDisplay_IsShowInDisconnectedState = new DevicePropertyKey(new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57), 0x44); // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceDisplay_IsNotInterestingForDisplay = new DevicePropertyKey(new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57), 0x4a); // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceDisplay_Category = new DevicePropertyKey(new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57), 0x5a); // DEVPROP_TYPE_STRING_LIST
        public static readonly DevicePropertyKey DEVPKEY_DeviceDisplay_UnpairUninstall = new DevicePropertyKey(new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57), 0x62); // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceDisplay_RequiresUninstallElevation = new DevicePropertyKey(new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57), 0x63); // DEVPROP_TYPE_BOOLEAN
        public static readonly DevicePropertyKey DEVPKEY_DeviceDisplay_AlwaysShowDeviceAsConnected = new DevicePropertyKey(new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57), 0x65); // DEVPROP_TYPE_BOOLEAN
    }

    public interface IDeviceProperty
    {
        DevicePropertyKey Key { get; }

        object Value { get; }
    }

    public class DeviceProperty : IDeviceProperty
    {
        readonly object _value;

        public DeviceProperty(DevicePropertyKey key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            this.Key = key;
            this._value = value;
        }

        public DevicePropertyKey Key { get; private set; }

        public virtual object Value { get { return this._value; } }
    }

    internal static class DevicePropertyHandlers
    {

        public static uint ParseUInt32(IntPtr pBuffer, uint bufferLength)
        {
            return (uint)Marshal.ReadInt32(pBuffer);
        }

        public static string ParseString(IntPtr pBuffer, uint bufferLength)
        {
            return Marshal.PtrToStringAuto(pBuffer);
        }

        public static IEnumerable<string> ParseStringList(IntPtr pBuffer, uint propertyBufferSize)
        {
            int stringLength = ((int)propertyBufferSize) / Marshal.SizeOf(typeof(short));

            string s = Marshal.PtrToStringAuto(pBuffer, stringLength);

            return s.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries).ToList().AsReadOnly();
        }

        public static Guid ParseGuid(IntPtr pBuffer, uint bufferLength)
        {
            return (Guid)Marshal.PtrToStructure(pBuffer, typeof(Guid));
        }

        internal static bool ParseBoolean(IntPtr p, uint bufferLength)
        {
            if (bufferLength == 1)
            {
                int value = (int)Marshal.ReadByte(p);
                return (value != 0 ? true : false);
            }

            return false;
        }
    }
}