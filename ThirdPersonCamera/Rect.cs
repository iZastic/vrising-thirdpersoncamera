using System.Runtime.InteropServices;

namespace ThirdPersonCamera
{
    [StructLayout(LayoutKind.Sequential)]
    struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
