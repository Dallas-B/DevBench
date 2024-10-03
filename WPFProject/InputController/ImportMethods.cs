using System.Runtime.InteropServices;
using System.Text;
using static WPFProject.InputController.Common;

namespace WPFProject.InputController
{
    public class ImportMethods
    {
        public delegate bool EnumWindowsProc(IntPtr windowHandle, IntPtr lParam);

        public delegate bool MonitorEnumProc(IntPtr monitorHandle, IntPtr hdcMonitor, ref Rect monitorRect, IntPtr dwData);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr windowHandle);

        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr parentWindowHandle, EnumWindowsProc enumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr clipRect, MonitorEnumProc enumProc, IntPtr dwData);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr monitorHandle, ref MonitorInfo monitorInfo);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr windowHandle, out Rect windowRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InjectTouchInput(uint contactCount, [MarshalAs(UnmanagedType.LPArray), In] PointerTouchInfo[] contacts);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int xPosition, int yPosition);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr windowHandle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr windowHandle, int commandShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr windowHandle, StringBuilder windowTitle, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowTitle);
    }
}
