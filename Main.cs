using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MemoryPath
{
    public partial class Main : Form
    {
        const string version = "1.0.0.20240708";
        public Main()
        {
            InitializeComponent();
        }


        private void Main_Load(object sender, EventArgs e)
        {
            DifficultyInput.PlaceholderText = "难度: 1-" + BlockClass.diff_maxValue.ToString();
        }
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            BlockClass.stop = true;
        }
        private void Main_Shown(object sender, EventArgs e)
        {
            CreateBlockView();
            this.Enabled = true;
        }
        private void GameButton_Click(object sender, EventArgs e)
        {
            switch (GameButton.Text)
            {
                case "开始游戏":
                    {
                        void error()
                        {
                            MessageBox.Show("难度数值错误，只能填写1-" + BlockClass.diff_maxValue.ToString() + "的数值。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        int diff;
                        try
                        {
                            if (DifficultyInput.Text != "")
                                diff = int.Parse(DifficultyInput.Text);
                            else
                                diff = 1;
                        }
                        catch { error(); return; }
                        if (diff > BlockClass.diff_maxValue || diff < 1)
                        {
                            error(); return;
                        }

                        DifficultyInput.Enabled = false;
                        GameButton.Enabled = false;
                        GameButton.Text = "停止游戏";
                        GY.Visible=false;

                        BlockClass.Start(diff);

                        GameButton.Enabled = true;
                        BlockPanel.Enabled = true;
                        Watcher.Enabled = true;
                        Watcher.Visible = true;
                    }
                    break;
                case "停止游戏":
                    GameButton.Enabled = false;
                    BlockPanel.Enabled = false;
                    Watcher.Enabled = false;
                    Watcher.Visible = false;

                    BlockClass.Clear();

                    GameButton.Text = "开始游戏";
                    GameButton.Enabled = true;
                    DifficultyInput.Enabled = true;
                    GY.Visible=true;
                    break;
            }

        }
        #region DifficultyInput
        private void DifficultyInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GameButton.Focus();
            }
        }
        private void DifficultyInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(13))
            {
                e.Handled = true;
            }
        }
        #endregion
        private void Watcher_MouseDown(object sender, MouseEventArgs e)
        {
            BlockPanel.Enabled = false;
            GameButton.Enabled = false;
            Watcher.ForeColor = Color.DarkGreen;
            BlockClass.Watch(1);
        }
        private void Watcher_MouseUp(object sender, MouseEventArgs e)
        {
            Watcher.ForeColor = Color.Black;
            BlockClass.Watch(0);
            BlockPanel.Enabled = true;
            GameButton.Enabled = true;
        }

        private void Block_MouseMove(object? sender, MouseEventArgs e)
        {
            if (Watcher.Enabled == true)
            {
                Watcher.Enabled = false; Watcher.Visible = false;
            }
            Debug.WriteLine(((PictureBox)sender!).Name);
            BlockClass.Trigger(int.Parse(((PictureBox)sender).Name.Substring(6)));
            switch (BlockClass.endingID)
            {
                case 0:
                    MessageBox.Show("游戏失败。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GameButton_Click(null!, null!);
                    break;
                case 1:
                    MessageBox.Show("恭喜！游戏通关。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GameButton_Click(null!, null!);
                    break;
            }
        }

        void CreateBlockView()
        {
            for (int y = 0; y < BlockClass.y; y++)
            {
                for (int x = 0; x < BlockClass.x; x++)
                {
                    BlockClass.block.Add(new());
                    int id = BlockClass.block.Count - 1;
                    BlockClass.block[id].Name = "block-" + id.ToString();
                    BlockClass.block[id].Location = new Point(2 + (2 + 20) * x, 2 + (2 + 20) * y);
                    BlockClass.block[id].Size = new Size(20, 20);
                    BlockClass.block[id].BackColor = Color.LightGray;
                    BlockClass.block[id].MouseMove += Block_MouseMove;

                    this.BlockPanel.Controls.Add(BlockClass.block[id]);
                }
            }
        }



        static class BlockClass
        {
            static public int y = 10;//行数
            static public int x = 10;//列数
            static public List<PictureBox> block = [];//控件组定义
            static int diff;//难度值
            public const int diff_maxValue = 5;//难度最高值
            static public int[] state;//方块当前状态阵列，0: 未激活; 1: 激活
            static public int[,] aim;//目标路径坐标阵列，用01表示是否包含
            static public List<Point> aim_point;//根据顺序排列目标路径坐标值
            public static int endingID = -1;//结局ID，判断是否获胜
            public static bool stop = false;//特殊情况下强制性退出残留的循环语句
            public static void Start(int _diff)
            {
                stop = false;
                //将数据导入变量
                diff = _diff;
                state = new int[block.Count];
                Array.Clear(state, 0, state.Length);
                //aim = state;
                aim = new int[x, y];
                Array.Clear(aim, 0, aim.Length);
                aim_point = [];


                {
                    Random rand = new();

                    int blockNum;//根据难度生成方块数量
                    {
                        int min;
                        int max;
                        min = (int)((float)(x * y - x * 2 + 2) * (float)((float)(diff - 1) / diff_maxValue));
                        max = (int)((float)(x * y - x * 2 + 2) * (float)((float)diff / diff_maxValue));
                        if (min == 0) min = y;
                        blockNum = rand.Next(min, max);
                    }

                    int lastX = -1;//上一行最后一个x值
                    int[] nowX = [-1, -1];//当前一行x的值，0: 上一个x值，1: 当前的x值


                    for (int yy = 0; yy < y; yy++)//根据每行进行计算
                    {
                        if (yy == 0)//第一行只允许有一个方格
                        {
                            int rand_x = rand.Next(0, x);
                            aim[rand_x, yy] = 1;
                            aim_point.Add(new(rand_x, yy));
                            lastX = rand_x;
                            continue;
                        }

                        nowX[1] = lastX;
                        nowX[0] = nowX[1];
                        aim[nowX[1], yy] = 1;
                        aim_point.Add(new(nowX[1], yy));
                        if (yy == y - 1)
                            continue;
                        while (true)
                        {
                            int rand_value;// = -2;
                            rand_value = rand.Next(-1, 1 + 1);//随机生成-1,0,1三个值，分别是往左右移动或者往下走

                            switch (rand_value)
                            {
                                case 0:
                                    if ((y - 1 - yy) * x <= (blockNum - aim_point.Count))//如果剩下的格子小于预期的格子则强制不为0
                                    {
                                        do
                                        {
                                            rand_value = rand.Next(-1, 1 + 1);
                                        }
                                        while (rand_value == 0);
                                    }
                                    else if ((y - 1 - yy) * x <= (blockNum - aim_point.Count) * 1.25)//乘以2，如果满足条件则多判断一次且增加1和-1的几率，以增加不为0的几率
                                    {
                                        int run_rand()
                                        {
                                            int temp;
                                            temp = rand.Next(-1, 1 + 1 + 2);
                                            switch (temp)
                                            {
                                                case -1:
                                                case 2:
                                                    return -1;
                                                case 1:
                                                case 3:
                                                    return 1;
                                                case 0:
                                                    return 0;
                                                default:
                                                    throw new Exception();
                                            }
                                        }
                                        rand_value = run_rand();
                                    }
                                    break;
                                case -1:
                                case 1:
                                    if (y - 1 - yy >= blockNum - aim_point.Count)//如果预期格子小于到底部最小路径的格子数，则强制为0
                                    {
                                        rand_value = 0;
                                    }
                                    break;
                            }

rego:;
                            switch (rand_value)
                            {
                                case -1:
                                    if (nowX[1] - 1 >= 0)//判断是否撞墙
                                    {
                                        if (nowX[1] + rand_value != nowX[0])//判断是否走过该格子，走过则反方向重试
                                        {
                                            nowX[0] = nowX[1];
                                            nowX[1] += rand_value;
                                            aim[nowX[1], yy] = 1;
                                            aim_point.Add(new(nowX[1], yy));
                                        }
                                        else
                                        {
                                            rand_value = 1; goto rego;
                                        }
                                    }
                                    else
                                    {
                                        if (nowX[1] + 1 != nowX[0])//避免死循环，判断撞墙后是否能反方向走
                                        {
                                            rand_value = 1;
                                            goto rego;
                                        }
                                        else
                                        {
                                            rand_value = 0; goto rego;
                                        }
                                    }
                                    break;
                                case 0:
                                    //aim[nowX[1], yy] = 1;
                                    //aim_point.Add(new(nowX[1], yy));
                                    lastX = nowX[1];
                                    goto reFor;
                                case 1:
                                    if (nowX[1] + 1 < x)
                                    {
                                        if (nowX[1] + rand_value != nowX[0])
                                        {
                                            nowX[0] = nowX[1];
                                            nowX[1] += rand_value;
                                            aim[nowX[1], yy] = 1;
                                            aim_point.Add(new(nowX[1], yy));
                                        }
                                        else
                                        {
                                            rand_value = -1; goto rego;
                                        }
                                    }
                                    else
                                    {
                                        if (nowX[1] - 1 != nowX[0])
                                        {
                                            rand_value = -1;
                                            goto rego;
                                        }
                                        else
                                        {
                                            rand_value = 0; goto rego;
                                        }
                                    }
                                    break;
                            }
                        }
reFor:;
                    }
                }

                Watch(1);
                Watch(0);
            }
            static int triggerNum = 0;//触发顺序
            static bool triggerOverLocker = false;//在特殊情况下锁住触发事件，避免重复触发产生bug
            public static void Trigger(int id)
            {
                if (!triggerOverLocker)
                {
                    if (block[id].BackColor == Color.LightGray && endingID == -1 && watchStateID == -1)
                    {
                        if (triggerNum + 1 == aim_point.Count)
                        {
                            triggerOverLocker = true;
                            foreach (Point point in aim_point)
                            {
                                if (triggerNum + 1 == aim_point.Count) //加个判断避免滞留
                                    block[point.Y * 10 + point.X].BackColor = Color.LightGreen;
                                else return;
                                Application.DoEvents();
                                Thread.Sleep(60);
                                if (stop) return;
                            }
                            endingID = 1;
                            return;
                        }
                        Point blockPoint = new(int.Parse(id.ToString().PadLeft(2, char.Parse("0")).Substring(1, 1)),
                                               int.Parse(id.ToString().PadLeft(2, char.Parse("0")).Substring(0, 1)));
                        if (blockPoint == aim_point[aim_point.Count - 1 - triggerNum])
                        {
                            block[id].BackColor = Color.Green;
                            triggerNum++;
                        }
                        else
                        {
                            block[id].BackColor = Color.Red;
                            endingID = 0;//失败
                        }

                    }
                }
            }
            public static int watchStateID = -1;//观看当前方块路径方法的状态ID，-1: 空闲; 0: 取消观看; 1: 启动观看
            public static void Watch(int commandID)//指令ID与状态ID同理
            {
                if (!(triggerNum == 0))
                {
                    commandID = 0;
                }
                switch (commandID)
                {
                    case 0:
                        watchStateID = 0;
                        foreach (Point point in aim_point)
                        {
                            if (watchStateID != 0)
                                return;
                            block[point.Y * 10 + point.X].BackColor = Color.LightGray;
                            Application.DoEvents();
                            Thread.Sleep(10);
                            if (stop) return;
                        }
                        watchStateID = -1;
                        break;
                    case 1:
                        watchStateID = 1;
                        foreach (Point point in aim_point)//将所有目标显示至方块视图
                        {
                            if (watchStateID != 1)
                                return;
                            block[point.Y * 10 + point.X].BackColor = Color.DarkGreen;
                            Application.DoEvents();
                            Thread.Sleep(60);
                            if (stop) return;
                        }
                        break;
                }
            }
            public static void Clear()
            {
                stop = true;
                state = [];
                aim = new int[0, 0];
                aim_point = [];
                diff = -1;
                triggerNum = 0;
                triggerOverLocker = false;
                endingID = -1;


                for (int i = 0; i < block.Count; i++)
                {
                    block[i].BackColor = Color.LightGray;
                }
            }
        }

        private void GY_Click(object sender, EventArgs e)
        {
            MessageBox.Show("程序名: MemoryPath" +
"\r\n别名: 记忆路径" +
"\r\n版本:V" + version +
"\r\nCopyright (C) 2024 Hgnim, All rights reserved." +
"\r\nGithub: https://github.com/Hgnim/MemoryPath", "关于");
        }
    }
}
