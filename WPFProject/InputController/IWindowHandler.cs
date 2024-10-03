using static WPFProject.InputController.Common;

namespace WPFProject.InputController
{
    public interface IWindowHandler
    {
        public Rect? GetScreenBounds(int? monitorIndex = null);

        public Rect? GetWindowBounds(IntPtr windowHandle);

        public IntPtr GetChildWindowHandle(IntPtr parentWindowHandle, string childWindowTitle);

        public IntPtr GetWindowHandle(string windowClassName, string windowTitle);
    }
}
