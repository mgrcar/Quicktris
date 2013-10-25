using System;

namespace JSTe3s
{
    public static class Playfield
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

        public static void ArrayClear(int[][] array)
        {
            for (int i = 0; i < 20; i++)
            {
                array[i] = new int[10];
            }
        }

        public static void ArrayClear2(int[][] array, int y)
        {
            array[y] = new int[10];
        }

        public static void ArrayCopy(int[][] src, int[][] dst)
        {
            for (int i = 0; i < 20; i++)
            {
                dst[i] = (int[])src[i].Clone();
            }
        }

        public static void ArrayCopy4(int[][] src, int srcY, int[][] dst, int dstY)
        {
            dst[dstY] = (int[])src[srcY].Clone();
        }

        public static void Clear()
        {
            ArrayClear(mGrid);
            ArrayClear(mGridBkgr);
        }

        public static void UpdateBkgr()
        {
            // mGrid -> mGridBkgr
            ArrayCopy(mGrid, mGridBkgr);
        }

        public static void UpdateBlock()
        {
            // mGridBkgr -> mGrid
            ArrayCopy(mGridBkgr, mGrid);
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
                    ArrayClear2(mGrid, y);
                    Native.Renderer_RenderRow(y);
                    render = true;
                    fullLines++;
                }
                else
                {
                    ArrayCopy4(mGrid, y, tmp, yTmp);
                    yTmp--;
                }
            }
            if (render)
            {
                ArrayCopy(tmp, mGrid);
                Native.Renderer_RenderPlayfield();
            }
            return fullLines;
        }
    }

    public class Block
    {
        #region Blocks
        public static Block[] mBlocks
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
            Native.Renderer_RenderStats();
            mNextBlock = (Block)mBlocks[Math.Round(Math.Random() * 6)].Clone();
            bool success = Check(mBlock.mPosX, mBlock.mPosY, mBlock.mRot);
            Playfield.UpdateBlock();
            Native.Renderer_RenderBlock();
            if (Program.mShowNext) { Native.Renderer_RenderNextBlock(); }
            return success;
        }

        public static bool Check(int posX, int posY, int rot)
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

        public static void Render()
        {
            Playfield.UpdateBlock();
            Native.Renderer_RenderBlock();
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
                Native.Renderer_RenderPlayfield();
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

    public enum Key
    {
        None = 0,
        Left = 1,
        Right = 2,
        Rotate = 3,
        Drop = 4,
        Down = 5,
        Restart = 6,
        Pause = 7,
        ShowNext = 8,
        SpeedUp = 9,
        Other = 10
    }

    public enum State
    {
        Play = 0,
        Pause = 1,
        GameOver = 2
    }

    public class Program
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

        public static State mState
            = State.Play;
        public static int mTimeLeft
            = 0;

        public static void Play()
        {
            if (mState == State.Play)
            {
                switch (Native.Keyboard_GetKey())
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
                        mTimeLeft = Date.Now - mTimer;
                        Native.Renderer_RenderPause();
                        mState = State.Pause;
                        return;
                    case Key.ShowNext:
                        mShowNext = !mShowNext;
                        if (mShowNext) { Native.Renderer_RenderNextBlock(); } else { Native.Renderer_ClearNextBlock(); }
                        break;
                    case Key.SpeedUp:
                        mLevel++;
                        if (mLevel > 9) { mLevel = 9; }
                        Native.Renderer_RenderLevel();
                        break;
                }
                if (Timer())
                {
                    if (!Block.MoveDown())
                    {
                        int points = (21 + 3 * (mLevel + 1)) - mSteps;
                        mScore += points;
                        if (mScore > 99999) { mScore = 99999; } // prevent overflow
                        Native.Renderer_RenderScore();
                        mSteps = 0;
                        mFullLines += Playfield.Collapse();
                        if (mFullLines > 99) { mFullLines = 99; } // prevent overflow
                        Native.Renderer_RenderFullLines();
                        mLevel = Math.Max(mLevel, Math.Floor((mFullLines - 1) / 10));
                        Native.Renderer_RenderLevel();
                        Playfield.UpdateBkgr();
                        if (!Block.NewBlock())
                        {
                            Native.Renderer_RenderGameOver();
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
                if (Native.Keyboard_GetKey() != Key.None)
                {
                    Native.Renderer_ClearPause();
                    mTimer = new Date(Date.Now.GetTime() - mTimeLeft);
                    mState = State.Play;
                }
            }
            else if (mState == State.GameOver)
            {
                Key key = Native.Keyboard_GetKey();
                if (key != Key.None)
                {
                    if (key == Key.Restart)
                    {
                        ResetStats();
                        Native.Renderer_Init();
                        Playfield.Clear();
                        Native.Renderer_RenderPlayfield();
                        Block.NewBlock();
                        mState = State.Play;
                    }
                }
            }
        }

        public static void Init()
        {
            Native.Renderer_Init();
            Native.Renderer_RenderPlayfield();
            Block.NewBlock();
            ResetTimer();        
        }

        //static void Main(string[] args)
        //{
        //    Init();
        //    while (true)
        //    {
        //        Play();
        //        Thread.Sleep(1);
        //    }
        //}
    }
}