using System.Runtime.InteropServices;
using System.Text;
using static WPFProject.InputController.Common;

namespace WPFProject.InputController
{
    public class WindowHandler : IWindowHandler
    {
        public const uint PrimaryMonitorFlag = 0x00000001;

        public Rect? GetScreenBounds(int? monitorIndex = null)
        {
            Rect? monitorBounds = null;
            int currentMonitorIndex = 0;
            bool monitorFound = false;

            ImportMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr monitorHandle, IntPtr _, ref Rect _, IntPtr _) =>
            {
                MonitorInfo monitorInfo = new MonitorInfo
                {
                    Size = Marshal.SizeOf<MonitorInfo>()
                };

                if (monitorIndex == null)
                {
                    if (ImportMethods.GetMonitorInfo(monitorHandle, ref monitorInfo) &&
                        (monitorInfo.Flags & PrimaryMonitorFlag) != 0)
                    {
                        monitorBounds = monitorInfo.Monitor;
                        monitorFound = true;
                        return false;
                    }
                }
                else if (currentMonitorIndex == monitorIndex)
                {
                    if (ImportMethods.GetMonitorInfo(monitorHandle, ref monitorInfo))
                    {
                        monitorBounds = monitorInfo.Monitor;
                        monitorFound = true;
                        return false;
                    }
                }
                currentMonitorIndex++;
                return true;
            }, IntPtr.Zero);

            return monitorFound ? monitorBounds : null;
        }

        public Rect? GetWindowBounds(IntPtr windowHandle)
        {
            if (ImportMethods.GetWindowRect(windowHandle, out Rect windowBounds))
            {
                return windowBounds;
            }
            return null;
        }

        public IntPtr GetChildWindowHandle(IntPtr parentWindowHandle, string childWindowTitle)
        {
            IntPtr childWindowHandle = IntPtr.Zero;

            ImportMethods.EnumChildWindows(
                parentWindowHandle,
                (IntPtr childHandle, IntPtr _) =>
                {
                    StringBuilder windowTitleBuilder = new StringBuilder(256);
                    if (ImportMethods.GetWindowText(childHandle, windowTitleBuilder, 256) > 0)
                    {
                        string windowTitle = windowTitleBuilder.ToString();
                        if (windowTitle == childWindowTitle)
                        {
                            childWindowHandle = childHandle;
                            return false;
                        }
                    }
                    return true;
                },
                IntPtr.Zero
            );

            return childWindowHandle;
        }

        public IntPtr GetWindowHandle(string windowClassName, string windowTitle)
        {
            return ImportMethods.FindWindow(windowClassName, windowTitle);
        }
    }
}
