using System;
using System.Threading;

namespace Quicktris
{
    static class Playfield
    {
        public static int[][] mGrid
            = new int[20][];
        public static int[][] mGridBkgr
            = new int[20][];

        static Playfield()
        {
            for (int i = 0; i < 20; i++)
            {
                mGrid[i] = new int[10];
                mGridBkgr[i] = new int[10];
            }
        }

        private static void Array_Clear(int[][] array)
        {
            // will do this in JS
        }

        private static void Array_Copy(int[][] src, int[][] dest)
        {
            // will do this in JS
        }

        private static void Array_Clear2(int[][] array, int row)
        {
            // will do this in JS
        }

        private static void Array_Copy4(int[][] src, int srcRow, int[][] dest, int destRow)
        {
            // will do this in JS
        }

        public static void Clear()
        {
            Array_Clear(mGrid);
            Array_Clear(mGridBkgr);
        }

        public static void UpdateBkgr()
        {
            // mGrid -> mGridBkgr
            Array_Copy(mGrid, mGridBkgr);
        }

        public static void UpdateBlock()
        {
            // mGridBkgr -> mGrid
            Array_Copy(mGridBkgr, mGrid);
            // mBlock -> mGrid
            string[] shape = Block.mBlock.mShape[Block.mBlock.mRot];
            for (int blockY = 0, gridY = Block.mBlock.mPosY; blockY < 4 && gridY < 20; blockY++, gridY++)
            {
                for (int blockX = 0, gridX = Block.mBlock.mPosX; blockX < 4 && gridX < 10; blockX++, gridX++)
                {
                    if (shape[blockY].CharAt(blockX) == '1')
                    {
                        mGrid[gridY][gridX] = Block.mBlock.mType;
                    }
                }
            }
        }

        public static int Collapse()
        {
            int[][] tmp = new int[20][];
            for (int i = 0; i < 20; i++) { tmp[i] = new int[10]; }
            int yTmp = 19;
            bool render = false;
            int fullLines = 0;
            for (int y = 19; y >= 0; y--)
            {
                bool fullLine = true;
                for (int x = 0; x < 10; x++)
                {
                    if (mGrid[y][x] == 0) { fullLine = false; break; }
                }
                if (fullLine)
                {
                    Array_Clear2(mGrid, y);
                    Renderer.RenderRow(y);
                    render = true;
                    fullLines++;
                }
                else
                {
                    Array_Copy4(mGrid, y, tmp, yTmp);
                    yTmp--;
                }
            }
            if (render)
            {
                Array_Copy(tmp, mGrid);
                Renderer.RenderPlayfield();
            }
            return fullLines;
        }
    }

    class Block
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
            if (mNextBlock == null) { mNextBlock = (Block)mBlocks[Math.Round(Math.Random() * 6)].Clone(); }
            mBlock = mNextBlock;
            Program.mStats[mBlock.mType - 1]++;
            if (Program.mStats[mBlock.mType - 1] > 1428) { Program.mStats[mBlock.mType - 1] = 1428; } // prevent overflow (also of the overall sum)
            Renderer.RenderStats();
            mNextBlock = (Block)mBlocks[Math.Round(Math.Random() * 6)].Clone();
            bool success = Check(mBlock.mPosX, mBlock.mPosY, mBlock.mRot);
            Playfield.UpdateBlock();
            Renderer.RenderBlock();
            if (Program.mShowNext) { Renderer.RenderNextBlock(); }
            return success;
        }

        private static bool Check(int posX, int posY, int rot)
        {
            string[] shape = mBlock.mShape[rot];
            for (int blockY = 0, gridY = posY; blockY < 4; blockY++, gridY++)
            {
                for (int blockX = 0, gridX = posX; blockX < 4; blockX++, gridX++)
                {
                    if (shape[blockY].CharAt(blockX) == '1' && (gridX < 0 || gridY < 0 || gridX >= 10 || gridY >= 20 || Playfield.mGridBkgr[gridY][gridX] != 0))
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
        public static void Init()
        {
            // ...
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
            // ...
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
            // ...
        }

        public static void RenderPause()
        {
            // ...
        }

        public static void ClearPause()
        {
            // ...
        }

        public static void RenderGoodbye()
        {
            // ...
        }

        public static void RenderScore()
        {
            // ...
        }

        public static void RenderNextBlock()
        {
            // ...
        }

        public static void ClearNextBlock()
        {
            // ...
        }

        public static void RenderFullLines()
        {
            // ...
        }

        public static void RenderLevel()
        {
            // ...
        }

        public static void RenderStats()
        {
            // ...
        }
    }

    public enum Key
    {
        None = 1,
        Left = 2,
        Right = 3,
        Rotate = 4,
        Drop = 5,
        Down = 6,
        Restart = 7,
        Pause = 8,
        ShowNext = 9,
        SpeedUp = 10,
        Other = 11
    }

    public static class Keyboard
    {
        public static Key GetKey()
        {
            // ...
            return Key.Other;
        }
    }

    class Program
    {
        public static int mLevel
            = 0;
        public static int mSteps
            = 0;
        public static int mScore
            = 0;
        public static int mFullLines
            = 0;
        public static bool mShowNext
            = false;
        public static int[] mStats
            = new int[7];

        public static Date mTimer;

        public static void ResetTimer()
        {
            mTimer = Date.Now;
        }

        public static bool Timer()
        {
            return Date.Now - mTimer >= (10 - mLevel) * 50;
        }

        public static void ResetStats()
        {
            mScore = 0;
            mLevel = 0;
            mFullLines = 0;
            mStats = new int[7];
        }

        public static Date Subtract(Date date, int ms)
        {
            // will do this in JS
            return null;
        }

        // TODO: this needs to be rewritten with events in JavaScript
        public static void Main(string[] args)
        {
            Renderer.Init();
            Renderer.RenderPlayfield();
            Block.NewBlock();
            ResetTimer();
            while (true)
            {
                switch (Keyboard.GetKey())
                {
                    case Key.Left:
                        Block.MoveLeft();
                        break;
                    case Key.Right:
                        Block.MoveRight();
                        break;
                    case Key.Rotate:
                        Block.Rotate();
                        break;
                    case Key.Drop:
                        Block.Drop();
                        break;
                    case Key.Pause:
                        int ts = Date.Now - mTimer;
                        Renderer.RenderPause();
                        while (Keyboard.GetKey() != Key.Restart) { /*Thread.Sleep(1);*/ }
                        Renderer.ClearPause();
                        mTimer = Subtract(Date.Now, ts);
                        break;
                    case Key.ShowNext:
                        mShowNext = !mShowNext;
                        if (mShowNext) { Renderer.RenderNextBlock(); } else { Renderer.ClearNextBlock(); }
                        break;
                    case Key.SpeedUp:
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
                        mLevel = Math.Max(mLevel, Math.Floor((mFullLines - 1) / 10));
                        Renderer.RenderLevel();
                        Playfield.UpdateBkgr();
                        if (!Block.NewBlock())
                        {
                            Renderer.RenderGameOver();
                            Key key;
                            while ((key = Keyboard.GetKey()) == Key.None) { /*Thread.Sleep(1);*/ }
                            if (key == Key.Restart)
                            {
                                Renderer.Init();
                                Playfield.Clear();
                                Renderer.RenderPlayfield();
                                Block.NewBlock();
                                ResetStats();
                            }
                            else
                            {
                                Renderer.RenderGoodbye();
                                return;
                            }
                        }
                    }
                    else { mSteps++; }
                    ResetTimer();
                }
                /*Thread.Sleep(1);*/
            }
        }
    }
}
