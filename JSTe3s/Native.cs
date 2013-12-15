using System;
using System.Runtime.CompilerServices;

namespace JSTe3s
{
    [GlobalMethods]
    [Imported]
    public static class Native
    {
        // Renderer

        public static void Renderer_Init()
        {
        }        

        public static void Renderer_RenderPlayfield(int delay)
        {
        }

        public static void Renderer_RenderRow(int row, int delay)
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
            return Key.Other;
        }
    }
}
