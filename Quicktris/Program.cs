using System;
using System.Text;
using System.Threading;

namespace Quicktris
{
    class Program
    {
        public class Playfield
        {
            public int[,] mGrid
                = new int[10, 20];
            public int[,] mGridBkgr
                = new int[10, 20];

            public void Update(Block block, int color)
            {
                // mGridBkgr -> mGrid
                Array.Copy(mGridBkgr, mGrid, mGridBkgr.Length);
                // block -> mGrid
                string[] shape = block.mShape[block.mRot];
                for (int blockY = 0, gridY = block.mPosY; blockY < 4 && gridY < 20; blockY++, gridY++)
                {
                    for (int blockX = 0, gridX = block.mPosX; blockX < 4 && gridX < 10; blockX++, gridX++)
                    {
                        if (shape[blockY][blockX] == '1')
                        {
                            mGrid[gridX, gridY] = color;
                        }
                    }
                }
            }
        }

        public class Block
        {
            public string[][] mShape;
            public int mRot
                = 0;
            public int mPosX
                = 0;
            public int mPosY
                = 0;

            public Block(string[][] shape)
            {
                mShape = shape;
            }

            public bool Check(int posX, int posY, int rot)
            {
                string[] shape = mShape[rot];
                for (int blockY = 0, gridY = posY; blockY < 4; blockY++, gridY++)
                {
                    for (int blockX = 0, gridX = posX; blockX < 4; blockX++, gridX++)
                    {
                        if (shape[blockY][blockX] == '1' && (gridX < 0 || gridY < 0 || gridX >= 10 || gridY >= 20 || mPlayfield.mGridBkgr[gridX, gridY] != 0))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public bool MoveLeft()
            {
                if (Check(mPosX - 1, mPosY, mRot)) { mPosX--; return true; }
                return false;
            }

            public bool MoveRight()
            {
                if (Check(mPosX + 1, mPosY, mRot)) { mPosX++; return true; }
                return false;
            }

            public bool MoveDown()
            {
                if (Check(mPosX, mPosY + 1, mRot)) { mPosY++; return true; }
                return false;
            }

            public bool Rotate()
            {
                int rot = (mRot + 1) % 4;
                if (Check(mPosX, mPosY, rot)) { mRot = rot; return true; }
                return false;
            }
        }

        public class Renderer
        {
            public static char GetChar(byte b)
            {
                return Encoding.GetEncoding(437).GetChars(new byte[] { b })[0];
            }

            public static void Init()
            {
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
                    Console.WriteLine("*          *");
                }
                Console.CursorLeft = 14;
                Console.WriteLine("************");
            }

            public static void RenderPlayfield()
            {
                Console.CursorTop = 1;
                for (int row = 0; row < 20; row++)
                {
                    Console.CursorLeft = 15;
                    for (int col = 0; col < 10; col++)
                    {
                        char ch = col % 2 == 0 ? ' ' : '.';
                        if (mPlayfield.mGrid[col, row] != 0) { ch = GetChar(219); }
                        Console.Write(ch);
                    }
                    Console.WriteLine();
                }            
            }
        }

        #region Blocks
        static Block[] mBlocks
            = new Block[] 
            { 
                new Block
                (
                    new string[][] 
                    { 
                        "0000,1110,1000,0000".Split(','),
                        "0100,0100,0110,0000".Split(','),
                        "0010,1110,0000,0000".Split(','),
                        "1100,0100,0100,0000".Split(',')
                    }
                ),
                new Block
                (
                    new string[][] 
                    { 
                        "0000,1110,1000,0000".Split(','),
                        "0100,0100,0110,0000".Split(','),
                        "0010,1110,0000,0000".Split(','),
                        "1100,0100,0100,0000".Split(',')
                    }
                ),
                new Block
                (
                    new string[][] 
                    { 
                        "0000,1110,1000,0000".Split(','),
                        "0100,0100,0110,0000".Split(','),
                        "0010,1110,0000,0000".Split(','),
                        "1100,0100,0100,0000".Split(',')
                    }
                ),
                new Block
                (
                    new string[][] 
                    { 
                        "0000,1110,1000,0000".Split(','),
                        "0100,0100,0110,0000".Split(','),
                        "0010,1110,0000,0000".Split(','),
                        "1100,0100,0100,0000".Split(',')
                    }
                ),
                new Block
                (
                    new string[][] 
                    { 
                        "0000,1110,1000,0000".Split(','),
                        "0100,0100,0110,0000".Split(','),
                        "0010,1110,0000,0000".Split(','),
                        "1100,0100,0100,0000".Split(',')
                    }
                ),
                new Block
                (
                    new string[][] 
                    { 
                        "0000,1110,1000,0000".Split(','),
                        "0100,0100,0110,0000".Split(','),
                        "0010,1110,0000,0000".Split(','),
                        "1100,0100,0100,0000".Split(',')
                    }
                ),
                new Block
                (
                    new string[][] 
                    { 
                        "0000,1110,1000,0000".Split(','),
                        "0100,0100,0110,0000".Split(','),
                        "0010,1110,0000,0000".Split(','),
                        "1100,0100,0100,0000".Split(',')
                    }
                )
            };
        #endregion

        public static Playfield mPlayfield
            = new Playfield();

        static void Main(string[] args)
        {
            Renderer.Init();
            Block block = mBlocks[0];
            mPlayfield.Update(block, 1);
            Renderer.RenderPlayfield();
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            block.MoveLeft();
                            break;
                        case ConsoleKey.RightArrow:
                            block.MoveRight();
                            break;
                        case ConsoleKey.DownArrow:
                            block.MoveDown();
                            break;
                        case ConsoleKey.UpArrow:
                            block.Rotate();
                            break;
                    }
                    mPlayfield.Update(block, 1);
                    Renderer.RenderPlayfield();
                }
                Thread.Sleep(1);
            }
        }
    }
}
