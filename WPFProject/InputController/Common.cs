using System.Runtime.InteropServices;

namespace WPFProject.InputController
{
    public class Common
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MonitorInfo
        {
            public int Size;
            public Rect Monitor;
            public Rect WorkArea;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointerTouchInfo
        {
            public PointerInfo pointerInfo;
            public TouchFlags touchFlags;
            public TouchMask touchMask;
            public Rect ContactArea;
            public Rect RawContactArea;
            public uint Orientation;
            public uint Pressure;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointerInfo
        {
            public PointerInputType pointerType;
            public uint pointerId;
            public uint frameId;
            public PointerFlags pointerFlags;
            public IntPtr sourceDevice;
            public IntPtr targetWindowHandle;
            public Point PixelLocation;
            public Point RawPixelLocation;
            public Point HiMetricLocation;
            public Point RawHiMetricLocation;
            public uint Timestamp;
            public uint historyCount;
            public int inputData;
            public uint KeyStates;
            public ulong PerformanceCount;
            public PointerButtonChangeType ButtonChangeType;
        }

        public enum PointerInputType
        {
            PT_TOUCH = 2
        }

        [Flags]
        public enum PointerFlags
        {
            InRange = 0x00000002,
            InContact = 0x00000004,
            Down = 0x00010000,
            Up = 0x00040000
        }

        public enum PointerButtonChangeType
        {
            None = 0
        }

        [Flags]
        public enum TouchFlags
        {
            None = 0x00000000
        }

        [Flags]
        public enum TouchMask
        {
            None = 0x00000000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }
    }
}
