using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
   
    public partial class MainWindow : Window
    {
        String PathSet = @"F:\TDDOWNLOAD\WpfApplication2\WpfApplication2\bin\Debug\image1.jpg";//设定图片的临时存储位置
        int MaxX=10, MaxY=10,GroupNum;//节点范围
        int NodeNumber;//节点的数量
        int MaxCountTime;
        double[,] Node = new double[1000,3];//0，1存储节点实际的xy数值，2存储节点归属点
        double[,] Group = new double[20, 1000];//存储当前的组
        int[] GroupMaster = new int[20];//指示现在的主宰节点
        double[,] TwoPointWeight = new double[1000, 3];//存储两个节点和节点之间的距离 0,a 1,b 2,w
        int step = -1;
        int ImageDrawRatio = 3;//绘制比例，解决过小的问题
        double OveredLimitNum = 0.01;//设置失真临界值
        double OldDistortion = 0, NewDistortion;//记录旧的/新的失真
        int Round = 0;//记录回合
        int CompleteFlag = 0;//等于1的时候说明已经收敛了
        int InitialNodeNumber;
        Window1 ImageWindow = new Window1();//用于显示节点在图片中图形的窗体

        


        void Init()//初始化，对设定的变量进行赋值
        {
            MaxY = Convert.ToInt32( tbY.Text);
            MaxX = Convert.ToInt32( tbX.Text);
            GroupNum = Convert.ToInt32(tbGroup.Text);
            NodeNumber = Convert.ToInt32(tbNumber.Text);
            InitialNodeNumber = Convert.ToInt32(tbNumber.Text);
            tbOUTPUT.FontSize = 12;//输出字体的大小
            OveredLimitNum =Convert.ToSingle( tbDistortion.Text);//设置失真临界值
        }
        
        public MainWindow()
        {
            InitializeComponent();
            
        }
        void FirstCount()
        {
           
        }
        private double TwoNodeWeight(double x1,double y1,double x2,double y2)//求两点之间的距离
        {
            double Weight  =0;
            Weight = System.Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            return Weight;
        }
        private void DrawAndShow()
        {
            //g.drawrectangle(pens.red, 0, 0, maxx * 20, maxy * 20);
            //bmp.setpixel(30, 30, system.drawing.color.blue);
            //g.drawellipse(pens.black, 30, 30, 10, 10);
            //graphics temp = graphics.fromimage(bmp);
            //bmp.save("image1.jpg");
            //bitmapimage image = new bitmapimage(new uri(pathset, urikind.relativeorabsolute));
            ////------上面这部分在不同电脑中需要修改
            //imagewindow.image.source = image;
            //imagewindow.show();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (step == -1)
            {
                Init();
                RunningBar.Value = 17.7;
                step++;
            }
            if (step == 0)
            {
                ProductRandomNode();
                RunningBar.Value = 37.7;
                tbOUTPUT.Select(tbOUTPUT.Text.Length, 0);//跳到最下面一行
                Keyboard.Focus(tbOUTPUT);
                step++;
                return;
            }
            if(step == 1)
            {
                CutGroup();
                RunningBar.Value = 47.7;
                tbOUTPUT.Select(tbOUTPUT.Text.Length, 0);//跳到最下面一行
                Keyboard.Focus(tbOUTPUT);
                step++;
                return;
            }
            if (step == 2)
            {
                NewVirtualNode();
                RunningBar.Value = 77.7;
                tbOUTPUT.Select(tbOUTPUT.Text.Length, 0);//跳到最下面一行
                Keyboard.Focus(tbOUTPUT);
                step++;
                return;
            }
            if (step == 3)
            {

            
                CountDistortion();
                if (CompleteFlag == 1)
                {
                    RunningBar.Value = 100;
                    DrawNodeInImage();
                }
                tbOUTPUT.Select(tbOUTPUT.Text.Length, 0);//跳到最下面一行
                step = 1;//从第1步开始重复
                Round++;
                return;
            }
            


        }

        private void DrawNodeInImage()
        {
            int CircleWidth = 10;//绘制圆的大小
            int DrawOffset = 10;//绘制偏移，防止圆圈跑到外边

            Bitmap Bmp = new Bitmap(580, 360);//实例化位图图片类
            Graphics g = Graphics.FromImage(Bmp);//实例化图形绘制类
            for (int j = 0; j < GroupNum; j++)
            {


       

        Random ran = new Random();
                Thread.Sleep(15);
                int R = ran.Next(255);
                Thread.Sleep(15);
                int G = ran.Next(255);
                Thread.Sleep(15);
                int B = ran.Next(255);
                for (int i = 0; i < NodeNumber ; i++)
                {

                    if (Node[i, 2] == j&& Node[i, 0] >=0 && Node[i, 1]>=0)
                    {
                        System.Drawing.Color color = System.Drawing.Color.FromArgb(R, G, B);
                        System.Drawing.Pen myPen = new System.Drawing.Pen(color);
                        System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(color);
                        System.Drawing.SolidBrush WhiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                        g.FillEllipse(myBrush, Convert.ToInt32(Node[i, 0]) * ImageDrawRatio - CircleWidth / 2 + DrawOffset,//在图像上面绘制代表节点的实心圆圈
                            Convert.ToInt32(Node[i, 1]) * ImageDrawRatio - CircleWidth / 2 + DrawOffset, CircleWidth, CircleWidth);
                        if(i > InitialNodeNumber)//大于初始节点则说明是后面生成的虚拟节点
                            g.FillEllipse(WhiteBrush, Convert.ToInt32(Node[i, 0]) * ImageDrawRatio - CircleWidth / 2 + DrawOffset+2,//在图像上面绘制代表虚拟节点的实心黑圆圈
                            Convert.ToInt32(Node[i, 1]) * ImageDrawRatio - CircleWidth / 2 + DrawOffset+2, CircleWidth-4, CircleWidth-4);

                        foreach (int k in GroupMaster)
                        {
                            if (i >= NodeNumber - GroupNum)
                                g.DrawEllipse(myPen, Convert.ToInt32(Node[i, 0]) * ImageDrawRatio - (CircleWidth+4) / 2 + DrawOffset,//在图像上面绘制代表主宰节点的圆圈
                                   Convert.ToInt32(Node[i, 1]) * ImageDrawRatio - (CircleWidth+4) / 2 + DrawOffset, CircleWidth + 4, CircleWidth + 4);
                        }
                    }
                }
            }
            //try
            //{//显示目前绘制的图形
                
                Bmp.SetResolution(72, 72);
                Bmp.Save("imageR"+Round+".jpg");
                BitmapImage image = new BitmapImage(new Uri(@"F:\TDDOWNLOAD\WpfApplication2\WpfApplication2\bin\Debug\imageR"+Round+".jpg", UriKind.RelativeOrAbsolute));
                ImageWindow.image.Source = image;
                ImageWindow.Show();
            //}
            //catch
            //{
            //    MessageBox.Show("写入文件错误", "ERROR");
            //}

        }
        private void btClearOutput_Click(object sender, RoutedEventArgs e)
        {
            tbOUTPUT.Text = "";
        }
        private void CountDistortion()
        {
            double num = 0;
            double Enum = 0;
            int HowManyNodeInTheGroup = 0;
            for (int j = 0; j < GroupNum; j++)
            {
                for (int i = 0; i < Convert.ToInt32(tbNumber.Text); i++)
                {
                    if (Node[i, 2] == j)
                    num += TwoNodeWeight(Node[i, 0], Node[i, 1], Node[GroupMaster[j], 0], Node[GroupMaster[j], 1]);

                }
            }
            //Enum = num / NodeNumber;
            Enum = num / Convert.ToInt32(tbNumber.Text);//除以最先设置的节点数量
            tbOUTPUT.Text += "\r当前失真率为：" + Enum;
            double dis =( Enum - OldDistortion) / Enum;
            if (Round > 0) tbOUTPUT.Text += "\r当前失真分值率：" + Math.Abs(dis);
            OldDistortion = Enum;

            if (Math.Abs(dis) < OveredLimitNum)//对失真是否已经达到临界值进行判别
            {
                tbOUTPUT.Text += "\r\r------------已经达到设定的失真临界值以下------------" + dis;
                CompleteFlag = 1;//已经完成
            }

        }
        private void CutGroup()
        {
            double num, Enum;
            double MinWeight = MaxX * MaxY;
            double MinWeightNode = -1;
            ///-------------------判断节点和节点距离
            ///
            if(Round > 0)
            {
                tbOUTPUT.Text = "";
                tbOUTPUT.Text = "=============现在开始第 " + Round + " 轮训练==============";
                tbOUTPUT.Text += "\r主宰节点为:";
                for (int j = 0; j < GroupNum; j++) tbOUTPUT.Text += Node[GroupMaster[j], 0] + "," + Node[GroupMaster[j], 1] + "  ";
            }
            for (int i = 0; i < NodeNumber; i++)//对节点进行分类
            {
                MinWeight = MaxX * MaxY;//虚拟出一个较大的数值
                for (int j = 0; j < GroupNum; j++)
                {
                    num = TwoNodeWeight(Node[i, 0], Node[i, 1], Node[GroupMaster[j], 0], Node[GroupMaster[j], 1]);//该节点和主宰节点j的比较
                   
                    if (num < MinWeight)
                    {
                        MinWeight = num;
                        MinWeightNode = j;
                        Node[i, 2] = j;//设置节点归属于该节点
                    }
                }

            }
            tbOUTPUT.Text += "\r当前节点分类：";
            for (int j = 0; j < GroupNum; j++)
            {
                tbOUTPUT.Text += "\r第" + j + "组：";
                for(int i = 0;i<InitialNodeNumber;i++)
                {
                    if (Node[i, 2] == j)
                    { tbOUTPUT.Text += Node[i, 0] + "," + Node[i, 1] + "   "; }
                }
                tbOUTPUT.Text += " ";
            }
        }

        private void btTest_Click(object sender, RoutedEventArgs e)
        {
            DrawAndShow();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            tbOUTPUT.FontSize += 1;
        }

        private void button_Click(object sender, RoutedEventArgs e)//数据重置
        {
            Init();
            step = -1;
            tbOUTPUT.Text = "";
          //  ImageWindow.Close();
            Round = 0;
        }
        private void NewVirtualNode()
        {
            int TheGroupNodeAmount = 0;//用以统计目前正在计算的组的节点有多少
            double x = 0, y = 0;
            double newNodeX, newNodeY;
            for (int i=0;i<GroupNum;i++)
            {
                TheGroupNodeAmount = 0;
                tbOUTPUT.Text += "\r第" + i + "组的虚拟节点坐标为：";
                for (int j = 0; j <Convert.ToInt32( tbNumber.Text); j++)//遍历一开始随机产生的节点，不包含后面产生的虚拟节点
                {
                   
                    if (Node[j, 2] == i )//计算所有属于该组的坐标之总和,并不算入虚拟节点的数值
                    {
                        x += Node[j, 0];
                        y += Node[j, 1];
                        TheGroupNodeAmount++;//统计属于这一组的节点数量
                     }    
                 }
                newNodeX  = x / TheGroupNodeAmount;//算出新的虚拟节点的XY坐标
                newNodeY = y / TheGroupNodeAmount;
                Node[NodeNumber , 0] = newNodeX;//记录虚拟节点的XY坐标
                Node[NodeNumber , 1] = newNodeY;
                Node[NodeNumber , 2] = i;//节点的归属组

                GroupMaster[i] = NodeNumber;//新增加的节点是虚拟节点，记录在GroupMaster数组中
                NodeNumber++;//目前已经记录的节点数量加1
                TheGroupNodeAmount = 0;
                x = 0;//清零，预备下一次的计算
                y = 0;
                tbOUTPUT.Text += " " + Node[NodeNumber - 1, 0] + "," + Node[NodeNumber - 1, 1]+ " ";//显示该节点
            }
            
        }
            
        private void ProductRandomNode()
        {
            Random ran = new Random();
            int[] node = new int[100];
            tbOUTPUT.Text += "产生节点为:";
            for (int j=0;j< NodeNumber;j++)//随机产生节点
            {
                Node[j, 0] = ran.Next(0,MaxX);
                Thread.Sleep(10);
                Node[j, 1] = ran.Next(0,MaxY);
                Thread.Sleep(10);
                RunningBar.Value += 20 / NodeNumber;//随着产生节点进度条增加
            }
            for (int i = 0; i < NodeNumber; i++)//遍历所有已经生成的节点
                tbOUTPUT.Text += Node[i, 0] + "," + Node[i, 1] + "  ";

            tbOUTPUT.Text += "\r";//下一行

            tbOUTPUT.Text += "\r初次选定的节点为:";
            for (;;)
            {
                int k = 0;
                for (int i = 0; i < GroupNum; i++)//随机选择组数内的节点作为第一次的主宰节点
                {
                    node[i] = ran.Next(NodeNumber);//产生的随机数为节点数目以内
                    Thread.Sleep(20);
                    GroupMaster[i] = node[i];//存入全局变量中准备下次调用
                    for (int j = 0; j <= i; j++)//防止重复，遍历已经产生的节点进行比较
                    {
                        if (node[i] == node[j]) k++;//如果相同则计数器k++
                        //不为1，有重复，重新执行该函数
                    }
                    Node[node[i], 2] = i;
                }
                if( k == GroupNum) break;
            }
            for (int i = 0; i < GroupNum; i++)
            {
                tbOUTPUT.Text += Node[node[i], 0] + "," + Node[node[i], 1] + "  ";
            }
            tbOUTPUT.Text += "\r ========现在可进行节点的分组======";
        }
       
    }
}
