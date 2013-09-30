using System;
using System.Text;

namespace Quicktris
{
    class Program
    {
        public class Area
        {
            public int[,] mScreen
                = new int[10, 20];
            public int[,] mBuffer
                = new int[10, 20];

            public void Copy()
            { 
                // mArea -> mAreaCopy
                Array.Copy(mScreen, mBuffer, mScreen.Length);
            }

            public void Update(Block block, int color)
            {
                // mAreaCopy -> mArea
                Array.Copy(mBuffer, mScreen, mBuffer.Length);
                // block -> mArea
                string[] shape = block.mShape[block.mRot];
                for (int blockY = 0, areaY = block.mPosY; blockY < 4 && areaY < 20; blockY++, areaY++)
                {
                    for (int blockX = 0, areaX = block.mPosX; blockX < 4 && areaX < 10; blockX++, areaX++)
                    {
                        if (shape[blockY][blockX] == '1')
                        {
                            mScreen[areaX, areaY] = color;
                        }
                    }
                }
            }

            private char GetChar(byte b)
            {
                return Encoding.GetEncoding(437).GetChars(new byte[] { b })[0];
            }

            public void OutputToConsole(int left, int top)
            {
                Console.CursorTop = top;
                for (int row = 0; row < 20; row++)
                {
                    Console.CursorLeft = left;
                    for (int col = 0; col < 10; col++)
                    {
                        char ch = col % 2 == 0 ? ' ' : '.';
                        if (mScreen[col, row] != 0) { ch = GetChar(219); }
                        Console.Write(ch);
                    }
                    Console.WriteLine();
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
                for (int blockY = 0, areaY = posY; blockY < 4; blockY++, areaY++)
                {
                    for (int blockX = 0, areaX = posX; blockX < 4; blockX++, areaX++)
                    {
                        if (shape[blockY][blockX] == '1' && (areaX < 0 || areaY < 0 || areaX >= 10 || areaY >= 20 || Program.mArea.mBuffer[areaX, areaY] != 0))
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

        public static Area mArea
            = new Area();

        static void Main(string[] args)
        {
            Console.Clear();
            mArea.Update(mBlocks[0], 1);
            mArea.OutputToConsole(10, 10);
            while (true)
            {
                ConsoleKeyInfo ki = Console.ReadKey();
                switch (ki.Key)
                {
                    case ConsoleKey.LeftArrow: 
                        mBlocks[0].MoveLeft(); 
                        break;
                    case ConsoleKey.RightArrow: 
                        mBlocks[0].MoveRight(); 
                        break;
                    case ConsoleKey.DownArrow: 
                        mBlocks[0].MoveDown(); 
                        break;
                    case ConsoleKey.UpArrow:
                        mBlocks[0].Rotate();
                        break;
                }
                mArea.Update(mBlocks[0], 1);
                mArea.OutputToConsole(10, 10);
            }
        }
    }
}
