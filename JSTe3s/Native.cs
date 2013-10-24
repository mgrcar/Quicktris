using System;
using System.Runtime.CompilerServices;

namespace JSTe3s
{
    [IgnoreNamespace]
    [Imported]
    [ScriptName("JSTe3s_Native")]
    public enum Key
    {
        Key_None = 0,
        Key_Left = 1,
        Key_Right = 2,
        Key_Rotate = 3,
        Key_Drop = 4,
        Key_Down = 5,
        Key_Restart = 6,
        Key_Pause = 7,
        Key_ShowNext = 8,
        Key_SpeedUp = 9,
        Key_Other = 10
    }

    [IgnoreNamespace]
    [Imported]
    [ScriptName("JSTe3s_Native")]
    public static class Native
    {
        // Utils

        public static Date Utils_Subtract(Date date, int ms)
        {
            return null;
        }

        public static void Utils_ArrayClear(int[][] array)
        {
        }

        public static void Utils_ArrayCopy(int[][] src, int[][] dest)
        {
        }

        public static void Utils_ArrayClear2(int[][] array, int row)
        {
        }

        public static void Utils_ArrayCopy4(int[][] src, int srcRow, int[][] dest, int destRow)
        {
        }

        // Renderer

        public static void Renderer_Init()
        {
        }        

        public static void Renderer_RenderPlayfield()
        {
        }

        public static void Renderer_RenderRow(int row)
        {
        }

        public static void Renderer_RenderBlock()
        {
        }

        public static void Renderer_RenderGameOver()
        {
        }

        public static void Renderer_RenderPause()
        {
        }

        public static void Renderer_ClearPause()
        {
        }

        public static void Renderer_RenderGoodbye()
        {
        }

        public static void Renderer_RenderScore()
        {
        }

        public static void Renderer_RenderNextBlock()
        {
        }

        public static void Renderer_ClearNextBlock()
        {
        }

        public static void Renderer_RenderFullLines()
        {
        }

        public static void Renderer_RenderLevel()
        {
        }

        public static void Renderer_RenderStats()
        {
        }

        // Keyboard

        public static Key Keyboard_GetKey()
        {
            return Key.Key_Other;
        }
    }
}
