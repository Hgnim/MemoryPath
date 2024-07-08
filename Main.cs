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
            DifficultyInput.PlaceholderText = "�Ѷ�: 1-" + BlockClass.diff_maxValue.ToString();
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
                case "��ʼ��Ϸ":
                    {
                        void error()
                        {
                            MessageBox.Show("�Ѷ���ֵ����ֻ����д1-" + BlockClass.diff_maxValue.ToString() + "����ֵ��", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        GameButton.Text = "ֹͣ��Ϸ";
                        GY.Visible=false;

                        BlockClass.Start(diff);

                        GameButton.Enabled = true;
                        BlockPanel.Enabled = true;
                        Watcher.Enabled = true;
                        Watcher.Visible = true;
                    }
                    break;
                case "ֹͣ��Ϸ":
                    GameButton.Enabled = false;
                    BlockPanel.Enabled = false;
                    Watcher.Enabled = false;
                    Watcher.Visible = false;

                    BlockClass.Clear();

                    GameButton.Text = "��ʼ��Ϸ";
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
                    MessageBox.Show("��Ϸʧ�ܡ�", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GameButton_Click(null!, null!);
                    break;
                case 1:
                    MessageBox.Show("��ϲ����Ϸͨ�ء�", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            static public int y = 10;//����
            static public int x = 10;//����
            static public List<PictureBox> block = [];//�ؼ��鶨��
            static int diff;//�Ѷ�ֵ
            public const int diff_maxValue = 5;//�Ѷ����ֵ
            static public int[] state;//���鵱ǰ״̬���У�0: δ����; 1: ����
            static public int[,] aim;//Ŀ��·���������У���01��ʾ�Ƿ����
            static public List<Point> aim_point;//����˳������Ŀ��·������ֵ
            public static int endingID = -1;//���ID���ж��Ƿ��ʤ
            public static bool stop = false;//���������ǿ�����˳�������ѭ�����
            public static void Start(int _diff)
            {
                stop = false;
                //�����ݵ������
                diff = _diff;
                state = new int[block.Count];
                Array.Clear(state, 0, state.Length);
                //aim = state;
                aim = new int[x, y];
                Array.Clear(aim, 0, aim.Length);
                aim_point = [];


                {
                    Random rand = new();

                    int blockNum;//�����Ѷ����ɷ�������
                    {
                        int min;
                        int max;
                        min = (int)((float)(x * y - x * 2 + 2) * (float)((float)(diff - 1) / diff_maxValue));
                        max = (int)((float)(x * y - x * 2 + 2) * (float)((float)diff / diff_maxValue));
                        if (min == 0) min = y;
                        blockNum = rand.Next(min, max);
                    }

                    int lastX = -1;//��һ�����һ��xֵ
                    int[] nowX = [-1, -1];//��ǰһ��x��ֵ��0: ��һ��xֵ��1: ��ǰ��xֵ


                    for (int yy = 0; yy < y; yy++)//����ÿ�н��м���
                    {
                        if (yy == 0)//��һ��ֻ������һ������
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
                            rand_value = rand.Next(-1, 1 + 1);//�������-1,0,1����ֵ���ֱ����������ƶ�����������

                            switch (rand_value)
                            {
                                case 0:
                                    if ((y - 1 - yy) * x <= (blockNum - aim_point.Count))//���ʣ�µĸ���С��Ԥ�ڵĸ�����ǿ�Ʋ�Ϊ0
                                    {
                                        do
                                        {
                                            rand_value = rand.Next(-1, 1 + 1);
                                        }
                                        while (rand_value == 0);
                                    }
                                    else if ((y - 1 - yy) * x <= (blockNum - aim_point.Count) * 1.25)//����2�����������������ж�һ��������1��-1�ļ��ʣ������Ӳ�Ϊ0�ļ���
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
                                    if (y - 1 - yy >= blockNum - aim_point.Count)//���Ԥ�ڸ���С�ڵ��ײ���С·���ĸ���������ǿ��Ϊ0
                                    {
                                        rand_value = 0;
                                    }
                                    break;
                            }

rego:;
                            switch (rand_value)
                            {
                                case -1:
                                    if (nowX[1] - 1 >= 0)//�ж��Ƿ�ײǽ
                                    {
                                        if (nowX[1] + rand_value != nowX[0])//�ж��Ƿ��߹��ø��ӣ��߹��򷴷�������
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
                                        if (nowX[1] + 1 != nowX[0])//������ѭ�����ж�ײǽ���Ƿ��ܷ�������
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
            static int triggerNum = 0;//����˳��
            static bool triggerOverLocker = false;//�������������ס�����¼��������ظ���������bug
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
                                if (triggerNum + 1 == aim_point.Count) //�Ӹ��жϱ�������
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
                            endingID = 0;//ʧ��
                        }

                    }
                }
            }
            public static int watchStateID = -1;//�ۿ���ǰ����·��������״̬ID��-1: ����; 0: ȡ���ۿ�; 1: �����ۿ�
            public static void Watch(int commandID)//ָ��ID��״̬IDͬ��
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
                        foreach (Point point in aim_point)//������Ŀ����ʾ��������ͼ
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
            MessageBox.Show("������: MemoryPath" +
"\r\n����: ����·��" +
"\r\n�汾:V" + version +
"\r\nCopyright (C) 2024 Hgnim, All rights reserved." +
"\r\nGithub: https://github.com/Hgnim/MemoryPath", "����");
        }
    }
}
