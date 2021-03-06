﻿using System;
using System.Text;
using System.Threading;

namespace Quicktris
{
    class Program
    {
        static class Playfield
        {
            public static int[,] mGrid
                = new int[20, 10];
            public static int[,] mGridBkgr
                = new int[20, 10];

            public static void Clear()
            {
                Array.Clear(mGrid, 0, mGrid.Length);
                Array.Clear(mGridBkgr, 0, mGridBkgr.Length);
            }

            public static void UpdateBkgr()
            {
                // mGrid -> mGridBkgr
                Array.Copy(mGrid, mGridBkgr, mGrid.Length);
            }

            public static void UpdateBlock()
            {
                // mGridBkgr -> mGrid
                Array.Copy(mGridBkgr, mGrid, mGridBkgr.Length);
                // mBlock -> mGrid
                string[] shape = Block.mBlock.mShape[Block.mBlock.mRot];
                for (int blockY = 0, gridY = Block.mBlock.mPosY; blockY < 4 && gridY < 20; blockY++, gridY++)
                {
                    for (int blockX = 0, gridX = Block.mBlock.mPosX; blockX < 4 && gridX < 10; blockX++, gridX++)
                    {
                        if (shape[blockY][blockX] == '1')
                        {
                            mGrid[gridY, gridX] = Block.mBlock.mType;
                        }
                    }
                }
            }

            public static int Collapse()
            {
                int[,] tmp = new int[20, 10];
                int yTmp = 19;
                bool render = false;
                int fullLines = 0;
                for (int y = 19; y >= 0; y--)
                {
                    bool fullLine = true;
                    for (int x = 0; x < 10; x++)
                    {
                        if (mGrid[y, x] == 0) { fullLine = false; break; }
                    }
                    if (fullLine)
                    {
                        Array.Clear(mGrid, y * 10, 10);
                        Renderer.RenderRow(y);
                        render = true;
                        fullLines++;
                    }
                    else
                    { 
                        Array.Copy(mGrid, y * 10, tmp, yTmp * 10, 10);
                        yTmp--;                        
                    }
                }
                if (render)
                {
                    Array.Copy(tmp, mGrid, tmp.Length);
                    Renderer.RenderPlayfield();
                }
                return fullLines;
            }
        }

        class Block : ICloneable
        {
            #region Blocks
            private static Block[] mBlocks
                = new Block[] 
                { 
                    new Block
                    (
                        new string[][] 
                        { 
                            "0000,0111,0100,0000".Split(','),
                            "0010,0010,0011,0000".Split(','),
                            "0001,0111,0000,0000".Split(','),
                            "0110,0010,0010,0000".Split(',')
                        }, 1
                    ),
                    new Block
                    (
                        new string[][] 
                        { 
                            "0000,1111,0000,0000".Split(','),
                            "0010,0010,0010,0010".Split(','),
                            "0000,1111,0000,0000".Split(','),
                            "0010,0010,0010,0010".Split(',')
                        }, 2
                    ),
                    new Block
                    (
                        new string[][] 
                        { 
                            "0000,0111,0010,0000".Split(','),
                            "0010,0011,0010,0000".Split(','),
                            "0010,0111,0000,0000".Split(','),
                            "0010,0110,0010,0000".Split(',')
                        }, 3
                    ),
                    new Block
                    (
                        new string[][] 
                        { 
                            "0000,0011,0110,0000".Split(','),
                            "0010,0011,0001,0000".Split(','),
                            "0000,0011,0110,0000".Split(','),
                            "0010,0011,0001,0000".Split(',')
                        }, 4
                    ),
                    new Block
                    (
                        new string[][] 
                        { 
                            "0000,0110,0011,0000".Split(','),
                            "0001,0011,0010,0000".Split(','),
                            "0000,0110,0011,0000".Split(','),
                            "0001,0011,0010,0000".Split(',')
                        }, 5
                    ),
                    new Block
                    (
                        new string[][] 
                        { 
                            "0000,0110,0110,0000".Split(','),
                            "0000,0110,0110,0000".Split(','),
                            "0000,0110,0110,0000".Split(','),
                            "0000,0110,0110,0000".Split(',')
                        }, 6
                    ),
                    new Block
                    (
                        new string[][] 
                        { 
                            "0000,0111,0001,0000".Split(','),
                            "0011,0010,0010,0000".Split(','),
                            "0100,0111,0000,0000".Split(','),
                            "0010,0010,0110,0000".Split(',')
                        }, 7
                    )
                };
            #endregion

            private static Random mRandom
                = new Random();
            public static Block mBlock 
                = null;
            public static Block mNextBlock
                = null;

            public string[][] mShape;
            public int mType;
            public int mRot
                = 0;
            public int mPosX
                = 3;
            public int mPosY
                = -1;

            public Block(string[][] shape, int type)
            {
                mShape = shape;
                mType = type;
            }

            public static bool NewBlock()
            {
                if (mNextBlock == null) { mNextBlock = (Block)mBlocks[mRandom.Next(7)].Clone(); }
                mBlock = mNextBlock;
                mStats[mBlock.mType - 1]++;
                if (mStats[mBlock.mType - 1] > 1428) { mStats[mBlock.mType - 1] = 1428; } // prevent overflow (also of the overall sum)
                Renderer.RenderStats();
                mNextBlock = (Block)mBlocks[mRandom.Next(7)].Clone();
                bool success = Check(mBlock.mPosX, mBlock.mPosY, mBlock.mRot);
                Playfield.UpdateBlock();
                Renderer.RenderBlock();
                if (mShowNext) { Renderer.RenderNextBlock(); }
                return success;
            }

            private static bool Check(int posX, int posY, int rot)
            {
                string[] shape = mBlock.mShape[rot];
                for (int blockY = 0, gridY = posY; blockY < 4; blockY++, gridY++)
                {
                    for (int blockX = 0, gridX = posX; blockX < 4; blockX++, gridX++)
                    {
                        if (shape[blockY][blockX] == '1' && (gridX < 0 || gridY < 0 || gridX >= 10 || gridY >= 20 || Playfield.mGridBkgr[gridY, gridX] != 0))
                        {
                            return false;
                        }
                    }
                }                
                return true;
            }

            private static void Render()
            {
                Playfield.UpdateBlock();
                Renderer.RenderBlock();
            }

            public static bool MoveLeft()
            {
                if (Check(mBlock.mPosX - 1, mBlock.mPosY, mBlock.mRot)) 
                { 
                    mBlock.mPosX--; 
                    Render(); 
                    return true; 
                }
                return false;
            }

            public static bool MoveRight()
            {
                if (Check(mBlock.mPosX + 1, mBlock.mPosY, mBlock.mRot)) 
                { 
                    mBlock.mPosX++; 
                    Render(); 
                    return true; 
                }
                return false;
            }

            public static bool MoveDown()
            {
                if (Check(mBlock.mPosX, mBlock.mPosY + 1, mBlock.mRot)) 
                { 
                    mBlock.mPosY++; 
                    Render(); 
                    return true; 
                }
                return false;
            }

            public static bool Rotate()
            {
                int rot = (mBlock.mRot + 1) % 4;
                if (Check(mBlock.mPosX, mBlock.mPosY, rot)) 
                {  
                    mBlock.mRot = rot; 
                    Render(); 
                    return true; 
                }
                return false;
            }

            public static bool Drop()
            {
                bool success = Check(mBlock.mPosX, mBlock.mPosY + 1, mBlock.mRot);
                while (Check(mBlock.mPosX, mBlock.mPosY + 1, mBlock.mRot)) { mBlock.mPosY++; }
                if (success)
                {
                    Playfield.UpdateBlock();
                    Renderer.RenderPlayfield();
                }
                return success;
            }

            public object Clone()
            {
                Block block = new Block(mShape, mType);
                block.mPosX = mPosX;
                block.mPosY = mPosY;
                block.mRot = mRot;
                return block;
            }
        }
        
        static class Renderer
        {
            private static ConsoleColor[] mConsoleColors
                = new ConsoleColor[]
                {
                    (ConsoleColor)0,
                    ConsoleColor.Magenta,
                    ConsoleColor.Red,
                    ConsoleColor.Yellow,
                    ConsoleColor.Green,
                    ConsoleColor.Cyan,
                    ConsoleColor.Blue,
                    ConsoleColor.Gray
                };

            private static char GetChar(byte b)
            {
                return Encoding.GetEncoding(437).GetChars(new byte[] { b })[0];
            }

            public static void Init()
            {
                Console.CursorVisible = false;
                Console.SetWindowSize(40, 25);
                Console.BufferWidth = 40;
                Console.BufferHeight = 25;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
                Console.WriteLine("Your level: 0");
                Console.WriteLine("Full lines: 0");
                Console.WriteLine("                             STATISTICS");
                Console.Write(" Score:    ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("0");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("                            {0}{0}{0}    -   0", GetChar(219));
                Console.WriteLine("                            {0}", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("   H E L P                    ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("{0}{0}{0}{0} -   0", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
                Console.Write("F1:Pause                    ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("{0}{0}{0}    -   0", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" 7:Left                      ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0}", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" 9:Right                        ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("{0}{0} -   0", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" 8:Rotate                      ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0}{0}", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" 1:Draw next                ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("{0}{0}     -   0", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" 6:Speed up                  ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("{0}{0}", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" 4:Drop                         ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("{0}{0} -   0", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("  SPACE:Drop                    ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("{0}{0}", GetChar(219));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("                            {0}{0}{0}    -   0", GetChar(219));
                Console.WriteLine("                              {0}", GetChar(219));
                Console.Write("   Next:                    ------------");
                Console.Write("                            Sum    :   0");
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("              Play TETRIS !");
                Console.CursorTop = 1;
                Console.ForegroundColor = ConsoleColor.Blue;
                for (int i = 0; i < 20; i++)
                {
                    Console.CursorLeft = 14;
                    Console.WriteLine("* . . . . .*");
                }
                Console.CursorLeft = 14;
                Console.WriteLine("************");
            }

            public static void RenderPlayfield()
            {
                for (int row = 0; row < 20; row++)
                {
                    RenderRow(row);
                }            
            }

            public static void RenderRow(int row)
            {                
                Console.SetCursorPosition(15, row + 1);
                for (int col = 0; col < 10; col++)
                {
                    char ch = col % 2 == 0 ? ' ' : '.';
                    int type = Playfield.mGrid[row, col];
                    if (type != 0)
                    {
                        ch = GetChar(219);
                        Console.ForegroundColor = mConsoleColors[type];
                    }
                    Console.Write(ch);
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
            }

            public static void RenderBlock()
            {
                for (int row = Block.mBlock.mPosY - 1; row < Block.mBlock.mPosY + 4; row++)
                {
                    if (row >= 0 && row < 20)
                    {
                        RenderRow(row);
                    }
                }
            }

            public static void RenderGameOver()
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(14, 10);
                Console.WriteLine("            ");
                Console.CursorLeft = 14;
                Console.WriteLine(" GAME  OVER ");
                Console.CursorLeft = 14;
                Console.WriteLine("            ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(1, 16);
                Console.WriteLine("R: RESTART ");
            }

            public static void RenderPause()
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(14, 10);
                Console.WriteLine("            ");
                Console.CursorLeft = 14;
                Console.WriteLine("   PAUSED   ");
                Console.CursorLeft = 14;
                Console.WriteLine("            ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(1, 16);
                Console.WriteLine("R: RESUME  ");            
            }

            public static void ClearPause()
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.CursorTop = 10;
                for (int i = 9; i < 12; i++)
                {
                    Console.CursorLeft = 14;
                    Console.Write("*");
                    RenderRow(i);
                    Console.WriteLine("*");
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(1, 16);
                Console.WriteLine(" SPACE:Drop");            
            }

            public static void RenderGoodbye()
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("Your score: {0}", mScore);
                Console.WriteLine();
            }

            public static void RenderScore()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(12 - mScore.ToString().Length, 4);
                Console.WriteLine(mScore);
            }

            public static void RenderNextBlock()
            {
                Console.CursorTop = 20;
                string[] shape = Block.mNextBlock.mShape[0];
                Console.ForegroundColor = mConsoleColors[Block.mNextBlock.mType];
                for (int blockY = 0; blockY < 4; blockY++)
                {
                    Console.CursorLeft = 3;
                    for (int blockX = 0; blockX < 4; blockX++)
                    {
                        Console.Write(shape[blockY][blockX] == '1' ? GetChar(219) : ' ');
                    }
                    Console.WriteLine();
                }
            }

            public static void ClearNextBlock()
            {
                Console.CursorTop = 20;
                for (int i = 0; i < 4; i++)
                {
                    Console.CursorLeft = 3;
                    Console.WriteLine("    ");
                }
            }

            public static void RenderFullLines()
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(13 - mFullLines.ToString().Length, 2);
                Console.Write(mFullLines);
            }

            public static void RenderLevel()
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(13 - mLevel.ToString().Length, 1);
                Console.Write(mLevel);
            }

            public static void RenderStats()
            {
                int all = 0;
                for (int i = 0; i < 7; i++)
                {
                    Console.CursorTop = i * 2 + 5;
                    Console.CursorLeft = 40 - mStats[i].ToString().Length;
                    Console.ForegroundColor = mConsoleColors[i + 1];
                    Console.Write(mStats[i]);
                    all += mStats[i];
                }
                Console.SetCursorPosition(40 - all.ToString().Length, 20);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(all);
            }
        }

        static class Keyboard
        {
            public enum Key
            {
                None,
                Left,
                Right,
                Rotate,
                Drop,
                Restart,
                Pause,
                ShowNext,
                SpeedUp,
                Other
            }

            public static Key GetKey()
            {
                if (!Console.KeyAvailable) { return Key.None; }
                ConsoleKeyInfo key = Console.ReadKey(true);
                while (Console.KeyAvailable) { Console.ReadKey(true); }
                switch (key.Key)
                { 
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        return Key.Left;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D9:
                    case ConsoleKey.NumPad9:
                        return Key.Right;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.D8:
                    case ConsoleKey.NumPad8:
                        return Key.Rotate;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.DownArrow:
                        return Key.Drop;
                    case ConsoleKey.R:
                        return Key.Restart;
                    case ConsoleKey.F1:
                        return Key.Pause;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        return Key.ShowNext;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        return Key.SpeedUp;
                }
                return Key.Other;
            }
        }

        static int mLevel
            = 0;
        static int mSteps
            = 0;
        static int mScore
            = 0;
        static int mFullLines
            = 0;
        static bool mShowNext
            = false;
        static int[] mStats
            = new int[7];

        static DateTime mTimer;

        static void ResetTimer()
        { 
            mTimer = DateTime.Now;
        }

        static bool Timer()
        {
            return (DateTime.Now - mTimer).TotalMilliseconds >= (10 - mLevel) * 50;
        }

        static void ResetStats()
        {
            mScore = 0;
            mLevel = 0;
            mFullLines = 0;
            mStats = new int[7];
        }

        enum State
        { 
            Play,
            Pause,
            GameOver
        }

        static State mState
            = State.Play;
        static TimeSpan mTimeLeft
            = TimeSpan.Zero;

        static void Play()
        {
            if (mState == State.Play)
            {
                switch (Keyboard.GetKey())
                {
                    case Keyboard.Key.Left:
                        Block.MoveLeft();
                        break;
                    case Keyboard.Key.Right:
                        Block.MoveRight();
                        break;
                    case Keyboard.Key.Rotate:
                        Block.Rotate();
                        break;
                    case Keyboard.Key.Drop:
                        Block.Drop();
                        break;
                    case Keyboard.Key.Pause:
                        mTimeLeft = DateTime.Now - mTimer;
                        Renderer.RenderPause();
                        mState = State.Pause;
                        return;
                    case Keyboard.Key.ShowNext:
                        mShowNext = !mShowNext;
                        if (mShowNext) { Renderer.RenderNextBlock(); } else { Renderer.ClearNextBlock(); }
                        break;
                    case Keyboard.Key.SpeedUp:
                        mLevel++;
                        if (mLevel > 9) { mLevel = 9; }
                        Renderer.RenderLevel();
                        break;
                }
                if (Timer())
                {
                    if (!Block.MoveDown())
                    {
                        int points = (21 + 3 * (mLevel + 1)) - mSteps;
                        mScore += points;
                        if (mScore > 99999) { mScore = 99999; } // prevent overflow
                        Renderer.RenderScore();
                        mSteps = 0;
                        mFullLines += Playfield.Collapse();
                        if (mFullLines > 99) { mFullLines = 99; } // prevent overflow
                        Renderer.RenderFullLines();
                        mLevel = Math.Max(mLevel, (mFullLines - 1) / 10);
                        Renderer.RenderLevel();
                        Playfield.UpdateBkgr();
                        if (!Block.NewBlock())
                        {
                            Renderer.RenderGameOver();
                            mState = State.GameOver;
                            return;
                        }
                    }
                    else { mSteps++; }
                    ResetTimer();
                }
            }
            else if (mState == State.Pause)
            {
                Keyboard.Key key = Keyboard.GetKey();
                if (key == Keyboard.Key.Restart || key == Keyboard.Key.Pause)
                {
                    Renderer.ClearPause();
                    mTimer = DateTime.Now - mTimeLeft;
                    mState = State.Play;
                }
            }
            else if (mState == State.GameOver)
            {
                Keyboard.Key key = Keyboard.GetKey();
                if (key != Keyboard.Key.None)
                {
                    if (key == Keyboard.Key.Restart)
                    {
                        ResetStats();
                        Renderer.Init();
                        Playfield.Clear();
                        Renderer.RenderPlayfield();
                        Block.NewBlock();
                        ResetTimer();
                        mState = State.Play;
                    }
                    else
                    {
                        Renderer.RenderGoodbye();
                        Environment.Exit(0);
                    }
                }
            }
        }

        static void Init()
        {
            Renderer.Init();
            Renderer.RenderPlayfield();
            Block.NewBlock();
            ResetTimer();
        }

        static void Main(string[] args)
        {
            Init();
            while (true)
            {
                Play();
                Thread.Sleep(1);
            }
        }
    }
}
