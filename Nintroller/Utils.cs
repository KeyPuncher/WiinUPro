namespace NintrollerLib
{
    internal static class Utils
    {
        internal static bool ReportContainsCoreButtons(InputReport reportType)
        {
            switch (reportType)
            {
                case InputReport.ReadMem:
                case InputReport.Acknowledge:
                case InputReport.BtnsOnly:
                case InputReport.BtnsAcc:
                case InputReport.BtnsExt:
                case InputReport.BtnsAccIR:
                case InputReport.BtnsExtB:
                case InputReport.BtnsAccExt:
                case InputReport.BtnsIRExt:
                case InputReport.BtnsAccIRExt:
                // 0x3E & 0x3F Also return button data but we don't use those
                    return true;

                default:
                    return false;
            }
        }

        internal static bool ReportContainsAccelerometer(InputReport reportType)
        {
            switch (reportType)
            {
                case InputReport.BtnsAcc:
                case InputReport.BtnsAccExt:
                case InputReport.BtnsAccIR:
                case InputReport.BtnsAccIRExt:
                    return true;

                default:
                    return false;
            }
        }

        internal static int GetExtensionOffset(InputReport reportType)
        {
            switch (reportType)
            {
                case InputReport.BtnsExt:
                case InputReport.BtnsExtB:
                    return 3;
                    
                case InputReport.BtnsAccExt:
                    return 6;
                    
                case InputReport.BtnsIRExt:
                    return 13;
                    
                case InputReport.BtnsAccIRExt:
                    return 16;
                    
                case InputReport.ExtOnly:
                    return 1;
                    
                // No other reports send extension bytes
                default:
                    return -1;
            }
        }
    }
}
