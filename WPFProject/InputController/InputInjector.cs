using static WPFProject.InputController.Common;

namespace WPFProject.InputController
{
    public class InputInjector : IInputInjector
    {
        public const int LeftMouseButtonDown = 0x201;
        public const int LeftMouseButtonUp = 0x202;

        public bool InjectMouseClick(int xPosition, int yPosition)
        {
            // Set the cursor position to the desired coordinates
            if (!ImportMethods.SetCursorPos(xPosition, yPosition))
            {
                return false;
            }

            ImportMethods.mouse_event(LeftMouseButtonDown, 0, 0, 0, 0);
            ImportMethods.mouse_event(LeftMouseButtonUp, 0, 0, 0, 0);

            return true;
        }

        public bool InjectMouseClick(IntPtr windowHandle, int xPosition, int yPosition, bool shouldFocusWindow)
        {
            FocusWindowIfNeeded(windowHandle, shouldFocusWindow);

            int lParam = (yPosition << 16) | (xPosition & 0xFFFF);

            return ImportMethods.SetCursorPos(xPosition, yPosition) &&
                   ImportMethods.SendMessage(windowHandle, LeftMouseButtonDown, 0x1, lParam) != 0 &&
                   ImportMethods.SendMessage(windowHandle, LeftMouseButtonUp, 0x1, lParam) != 0;
        }

        public bool InjectTouchInput(IntPtr windowHandle, int xPosition, int yPosition, bool shouldFocusWindow)
        {
            FocusWindowIfNeeded(windowHandle, shouldFocusWindow);

            PointerTouchInfo touchContact = CreateTouchContact(xPosition, yPosition,
                PointerFlags.Down | PointerFlags.InRange | PointerFlags.InContact);

            return InjectTouchSequence(touchContact);
        }

        private bool InjectTouchSequence(PointerTouchInfo touchContact)
        {
            if (!ImportMethods.InjectTouchInput(1, new[] { touchContact })) return false;

            touchContact.pointerInfo.pointerFlags = PointerFlags.Up;
            return ImportMethods.InjectTouchInput(1, new[] { touchContact });
        }

        private PointerTouchInfo CreateTouchContact(int xPosition, int yPosition, PointerFlags pointerFlags)
        {
            return new PointerTouchInfo
            {
                pointerInfo = new PointerInfo
                {
                    pointerType = PointerInputType.PT_TOUCH,
                    pointerId = 0,
                    PixelLocation = new Point { X = xPosition, Y = yPosition },
                    pointerFlags = pointerFlags
                },
                touchFlags = TouchFlags.None,
                touchMask = TouchMask.None,
                ContactArea = new Rect
                {
                    Left = xPosition,
                    Top = yPosition,
                    Right = xPosition,
                    Bottom = yPosition
                },
                Orientation = 90,
                Pressure = 32000
            };
        }

        private void FocusWindowIfNeeded(IntPtr windowHandle, bool shouldFocusWindow)
        {
            if (!shouldFocusWindow) return;

            ImportMethods.ShowWindow(windowHandle, 9);
            ImportMethods.SetForegroundWindow(windowHandle);
            ImportMethods.BringWindowToTop(windowHandle);
        }
    }
}
