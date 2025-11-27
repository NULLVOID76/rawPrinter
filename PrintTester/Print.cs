using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Printing;

namespace PrintTester
{
    public static class Print
    {
        private static readonly string LogFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PrintLog.txt");

        /// <summary>
        /// Prints raw string data to a printer using Win32 API.
        /// Logs all errors automatically.
        /// </summary>
        public static bool Printx(string data, string printerName)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                LogError("Data is empty");
                throw new ArgumentException("Print data cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(printerName))
            {
                LogError("Printer name is empty");
                throw new ArgumentException("Printer name cannot be empty");
            }

            IntPtr hPrinter = IntPtr.Zero;

            DOCINFO docInfo = new DOCINFO
            {
                pDocName = "Raw Print Job",
                pDataType = "RAW"
            };

            try
            {
                if (OpenPrinter(printerName, ref hPrinter, 0) == 0)
                {
                    LogError($"OpenPrinter failed for: {printerName}");
                    throw new Exception($"Unable to open printer: {printerName}");
                }

                if (StartDocPrinter(hPrinter, 1, ref docInfo) == 0)
                {
                    LogError($"StartDocPrinter failed for: {printerName}");
                    throw new Exception("Unable to start document");
                }

                if (StartPagePrinter(hPrinter) == 0)
                {
                    LogError($"StartPagePrinter failed for: {printerName}");
                    throw new Exception("Unable to start page");
                }

                int bytesWritten = 0;
                int result = WritePrinter(hPrinter, data, data.Length, ref bytesWritten);

                if (result == 0)
                {
                    LogError($"WritePrinter failed. Bytes written: {bytesWritten}");
                    throw new Exception("Error writing to printer");
                }

                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);

                LogInfo($"Print success on printer: {printerName}, Bytes: {bytesWritten}");
                return true;
            }
            catch (Exception ex)
            {
                LogError("Exception: " + ex.Message);
                LogError("StackTrace: " + ex.StackTrace);
                return false;
            }
            finally
            {
                if (hPrinter != IntPtr.Zero)
                    ClosePrinter(hPrinter);
            }
        }

        // ---------- Win32 API ----------

        [DllImport("winspool.drv", CharSet = CharSet.Unicode)]
        private static extern int OpenPrinter(string pPrinterName, ref IntPtr phPrinter, int pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode)]
        private static extern int StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFO pDocInfo);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode)]
        private static extern int StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Ansi)]
        private static extern int WritePrinter(IntPtr hPrinter, string data, int buf, ref int pcWritten);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode)]
        private static extern int EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode)]
        private static extern int EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode)]
        private static extern int ClosePrinter(IntPtr hPrinter);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DOCINFO
        {
            [MarshalAs(UnmanagedType.LPWStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPWStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)] public string pDataType;
        }


        private static void LogError(string message)
        {
            WriteLog("ERROR", message);
        }

        private static void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        private static void WriteLog(string level, string message)
        {
            try
            {
                string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
                File.AppendAllText(LogFilePath, log + Environment.NewLine);
            }
            catch
            {
                // Avoid crashes if logging fails
            }
        }
    }
}
