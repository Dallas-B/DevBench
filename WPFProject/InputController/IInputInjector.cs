namespace WPFProject.InputController
{
    public interface IInputInjector
    {
        public bool InjectMouseClick(IntPtr windowHandle, int xPosition, int yPosition, bool shouldFocusWindow);

        public bool InjectMouseClick(int xPosition, int yPosition);

        public bool InjectTouchInput(IntPtr windowHandle, int xPosition, int yPosition, bool shouldFocusWindow);
    }
}
