using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using NADemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//完成后publish即可安装
namespace final_01
{
    public partial class Form1 : Form
    {
        private GMapOverlay markersOverlay1 = new GMapOverlay("markers"); //放置marker的图层
        private GMapOverlay markersOverlay2 = new GMapOverlay("markers"); //放置marker的图层
        private GMapOverlay markersOverlay3 = new GMapOverlay("markers"); //放置marker的图层

        private GMapOverlay markersOverlay = new GMapOverlay("markers"); //放置marker的图层
        private GMapOverlay markersOverlay_before = new GMapOverlay("markers"); //放置marker的图层
        private GMapOverlay markersOverlay_after = new GMapOverlay("markers"); //放置marker的图层
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//禁用此异常socket
            //-------------------Gmap_1初始化-------------------//
            try
            {
                System.Net.IPHostEntry e = System.Net.Dns.GetHostEntry("ditu.google.cn");
            }
            catch
            {
                mapControl1.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection avaible, going to CacheOnly mode.", "GMap.NET Demo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            mapControl1.CacheLocation = Environment.CurrentDirectory + "\\GMapCache\\"; //缓存位置
            mapControl1.MapProvider = GMapProviders.GoogleChinaMap; //google china 地图
            mapControl1.MinZoom = 2;  //最小比例
            mapControl1.MaxZoom = 24; //最大比例
            mapControl1.Zoom = 16;     //当前比例
            mapControl1.ShowCenter = false; //不显示中心十字点
            mapControl1.DragButton = System.Windows.Forms.MouseButtons.Left; //左键拖拽地图
            mapControl1.Position = new PointLatLng(31.7508319669, 119.9192261696); //地图中心位置：江理工图书馆

            mapControl1.Overlays.Add(markersOverlay);
            mapControl1.Overlays.Add(markersOverlay1);
            mapControl1.Overlays.Add(markersOverlay3);
            mapControl1.Overlays.Add(markersOverlay2);

            mapControl1.MouseClick += new MouseEventHandler(mapControl_MouseClick);            
            
            //------------调用打点函数---------------//
            SetLableOnMap(All_Points.XiPoints, mapControl1);
            SetLableOnMap(All_Points.BeiPoints, mapControl1);
            SetLableOnMap(All_Points.NanPoints, mapControl1);
            SetLableOnMap(All_Points.LirPoints, mapControl1);
            //------------调用绘制矩形方法------------//
            DrawPolygons(119.9182605743, 31.7512516335, 119.9200415611, 31.7502663263);
            //-------------调用显示标签方法-----------//
            PointShark(12, 14, "warning!!! i'm full");
            //------------初始化图表----------------//
            setChart1();
            setChart2();
            setChart3();
            setChart4();
            //--------------退出线程----------------//
            this.FormClosing += (o, e) =>       //退出关闭线程
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    System.Environment.Exit(0);
                    //e.Cancel = MessageBox.Show("sure?", this.Text, MessageBoxButtons.OKCancel) != DialogResult.OK;
                }
            };
        }
        //窗体加载初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            this.txtPltIP.Text = "218.4.33.72";
            this.txtAppid.Text = "7G5WdQBaqPRBaLXRj8uDfIXwzKMa";
            this.txtAppPwd.Text = "cXJzkOY7v_Qdw1LWDYlsxBUS__Ua";
            this.txtCertPwd.Text = "IoM@1234";
            this.txtCertFile.Text = "iot3rd.p12";
        }
        //----------------------------MapControl方法集---------------------------//
        //***********************************************************************//
        //***********************************************************************//
        //-----------------------------------------------------------------------//
        /*
         * 功能: 调用MapControl实现坐标打点
         * 调用: 参见具体方法
         * 作者: later
         * 日期: 2018/07/27
         * 修改: ---------
         */
        //方法一: ------------右击标绿点方法----------//
        void mapControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                PointLatLng point = mapControl1.FromLocalToLatLng(e.X, e.Y);
                GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.green);
                markersOverlay.Markers.Add(marker);
            }
        }
        //方法二: ------------数组打点方法--------加上去!!!!gMapOverlay.Markers.Add(marker); ----//
        //将字符串组LatLngInfo的点打标记，经纬度中用','隔开, 把点的颜色也写成形参
        public void SetLableOnMap(string[] LatLngInfo, MapControl mapControl)
        {
            //创建图层  
            GMapOverlay gMapOverlay = new GMapOverlay();
            //给每个坐标打点
            for (int i = 0; i < LatLngInfo.Length; i++)
            {
                string[] LatLng = LatLngInfo[i].Split(',');
                //在坐标点上绘制一绿色点并向图层中添加标签 
                gMapOverlay.Markers.Add(new GMarkerGoogle(new PointLatLng(double.Parse(LatLng[1]), double.Parse(LatLng[0])), GMarkerGoogleType.gray_small));
                //方便之后寻找到是第几个GMapMarker   
                gMapOverlay.Markers[i].Tag = i;
                gMapOverlay.Markers[i].Tag = "xxxx";
                gMapOverlay.Id = "markroad";
            }
            //向控件中添加图层  
            mapControl.Overlays.Add(gMapOverlay);
        }
        //方法三: --------------绘制矩形形方法---------------------//
        public void DrawPolygons(double Lat_top, double Lng_left, double Lat_bottom, double Lng_right)
        {
            //在地图上画矩阵，知道矩阵上下左右（Lat_top、Lat_bottom、Lng_left、Lng_right）的4个点
            GMapOverlay polyOverlay = new GMapOverlay("polygons");
            List<PointLatLng> points = new List<PointLatLng>();
            //注意添加点的顺序
            points.Add(new PointLatLng(Lat_top, Lng_left));   //这里是矩形, 可以随便加点构成其它形状, 逆时针连线
            points.Add(new PointLatLng(Lat_bottom, Lng_left));
            points.Add(new PointLatLng(Lat_bottom, Lng_right));
            points.Add(new PointLatLng(Lat_top, Lng_right));
            GMapPolygon polygon = new GMapPolygon(points, "mypolygon");
            //颜色
            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red, 1);
            polyOverlay.Polygons.Add(polygon);
            mapControl1.Overlays.Add(polyOverlay);
        }
        //方法四: --------------加一个闪烁点&  文字标签---------------//
        /*
         * 备注: 只有文字标签, 不闪烁了
         */
        public void PointShark(double Lat, double Lng, string label)
        {
            GMapOverlay gMapOverlay = new GMapOverlay();
            PointLatLng point = new PointLatLng(Lat, Lng);
            gMapOverlay.Markers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red));
            GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.green);
            marker.ToolTipText = label;       //这是标签
            marker.Tag = 1;
            marker.ToolTipMode = MarkerTooltipMode.Always;
            gMapOverlay.Markers.Add(marker);   //这一句话也很关键!!!!, 不然不显示标签
            mapControl1.Overlays.Add(gMapOverlay);//添加图层, 只有添加才会显示我们需要的点
        }
        //方法五: ------------------去除图层-------------------------//
        public void clearMark(GMapOverlay gMapOverlay)
        {
            gMapOverlay.Markers.Clear();
        }
        //----------------------------工人设备管理方法集---------------------------//
        //***********************************************************************//
        //***********************************************************************//
        //-----------------------------------------------------------------------//
        /*
         * 功能: MySql增删查改工人及设备信息
         * 调用: 参见具体方法
         * 作者: later
         * 日期: 2018/07/28
         * 修改: ---------
         */
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        //---------换数据源时情况box-----------------//
        private void TextBoxNull()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }
        //---------数据输入label更换为数据库中的字段值-----------//
        private void labelshow()
        {
            label1.Text = dataGridView1.Columns[0].HeaderText;
            label2.Text = dataGridView1.Columns[1].HeaderText;
            label3.Text = dataGridView1.Columns[2].HeaderText;
            label4.Text = dataGridView1.Columns[3].HeaderText;
            label5.Text = dataGridView1.Columns[4].HeaderText;
            label6.Text = dataGridView1.Columns[5].HeaderText;
            //try
            //{
            //    label4.Text = dataGridView1.Columns[3].HeaderText;
            //}
            //catch (Exception)
            //{

            //    label4.Text = "None";
            //}
        }

        private void 工人管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBoxNull();
            ds = SqlDesigner.ExecuteDataSet("select * from tb_worker");
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
            labelshow();
        }

        private void 设备管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBoxNull();
            ds = SqlDesigner.ExecuteDataSet("select * from tb_device");
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
            labelshow();
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBoxNull();
            ds = SqlDesigner.ExecuteDataSet("select * from help");
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string index = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            if (label1.Text == "deviceId")
            {
                ds = SqlDesigner.ExecuteDataSet("select *from tb_device where deviceId='" + index + "'");
                dt = ds.Tables[0];
                DataRow row = dt.Rows[0];
                textBox1.Text = row["deviceId"].ToString();
                textBox2.Text = row["位置"].ToString();
                textBox3.Text = row["负责人"].ToString();
                textBox4.Text = row["部署日期"].ToString();
                textBox5.Text = row["报警器"].ToString();
                textBox6.Text = row["分贝检测"].ToString();
            }
            if (label1.Text == "workerId")
            {
                ds = SqlDesigner.ExecuteDataSet("select *from tb_worker where workerId='" + index + "'");
                dt = ds.Tables[0];
                DataRow row = dt.Rows[0];
                textBox1.Text = row["workerId"].ToString();
                textBox2.Text = row["姓名"].ToString();
                textBox3.Text = row["性别"].ToString();
                textBox4.Text = row["年龄"].ToString();
                textBox5.Text = row["负责区域"].ToString();
                textBox6.Text = row["工资"].ToString();                
            }
            //if (label1.Text == "fid")
            //{
            //    ds = SqlDesigner.ExecuteDataSet("select *from dtfunction where fid='" + index + "'");
            //    dt = ds.Tables[0];
            //    DataRow row = dt.Rows[0];
            //    textBox1.Text = row["fid"].ToString();
            //    textBox2.Text = row["fname"].ToString();
            //    textBox3.Text = row["flag"].ToString();
            //    textBox4.Text = row["uflag"].ToString();
            //}
        }
        //---------添加记录-------------------------------//
        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (label1.Text == "deviceId")
            {
                i = SqlDesigner.ExecuteNoQuery("insert into tb_device(deviceId,位置,负责人,部署日期,报警器,分贝检测)values('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" + textBox5.Text + "','" + textBox6.Text + "')");
            }
            else if (label1.Text == "workerId")
            {
                i = SqlDesigner.ExecuteNoQuery("insert into tb_worker(workerId,姓名,性别,年龄,负责区域,工资)values('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" + textBox5.Text + "','" + textBox6.Text + "')");
            }            
            if (i > 0)
            {
                MessageBox.Show("添加成功");
            }
            else
            {
                MessageBox.Show("添加失败");
            }
        }
        //--------------删除记录----------------------------//
        private void button2_Click(object sender, EventArgs e)
        {
            int i = 0;
            string currentIndex = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            if (label1.Text == "deviceId")
            {
                i = SqlDesigner.ExecuteNoQuery("delete from tb_device where deviceId='" + currentIndex + "'");
            }
            else if (label1.Text == "workerId")
            {
                i = SqlDesigner.ExecuteNoQuery("delete from tb_worker where workeerId='" + currentIndex + "'");
            }
            if (i > 0)
            {
                MessageBox.Show("删除成功");
            }
            else
            {
                MessageBox.Show("删除失败");
            }
        }
        //--------------修改记录--------------------------//
        private void button3_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (label1.Text == "deviceId")
            {
                i = SqlDesigner.ExecuteNoQuery("update tb_device set 位置='" + textBox2.Text + "',负责人='" + textBox3.Text + "',部署日期='" + textBox4.Text + "',报警器='" + textBox5.Text + "',分贝检测='" + textBox6.Text + "'where devcieId='" + textBox1.Text + "'");
            }
            if (label1.Text == "workerId")
            {
                i = SqlDesigner.ExecuteNoQuery("update tb_worker set 姓名='" + textBox2.Text + "',性别='" + textBox3.Text + "',年龄='" + textBox4.Text + "',负责区域='" + textBox5.Text + "',工资='" + textBox6.Text + "'where workerId='" + textBox1.Text + "'");
            }
            if (i > 0)
            {
                MessageBox.Show("Succeed！");
            }
            else
            {
                MessageBox.Show("Failed！");
            }
        }
        //----------------------------工人app通信方法集---------------------------//
        //**************************万不得已使用**********************************//
        //***********************************************************************//
        //-----------------------------------------------------------------------//
        /*
         * 功能: 和Android端建立socket通信
         * 调用: 参见具体方法
         * 作者: later
         * 日期: 2018/08/01
         * 修改: ---------
         */
        private bool bConnected = false;
        private Thread tAcceptMsg = null;
        private IPEndPoint IPP = null;
        private Socket socket = null;
        private Socket clientSocket = null;
        private NetworkStream nStream = null;
        private TextReader tReader = null;
        private TextWriter wReader = null;
        //显示信息
        public void AcceptMessage()
        {
            //接受客户机的连接请求
            clientSocket = socket.Accept();

            if (clientSocket != null)
            {
                bConnected = true;
            }

            nStream = new NetworkStream(clientSocket);
            tReader = new StreamReader(nStream);
            wReader = new StreamWriter(nStream);

            string sTemp; //临时存储读取的字符串
            while (bConnected)
            {
                try
                {
                    //连续从当前流中读取字符串直至结束
                    sTemp = tReader.ReadLine();
                    if (sTemp.Length != 0)
                    {
                        //richTextBox2_KeyPress()和AcceptMessage()
                        //都将向richTextBox1写字符，可能访问有冲突，
                        //所以，需要多线程互斥
                        lock (this)
                        {
                            textBox7.AppendText(comboBox1.Text + "：" + sTemp + "\n");
                            //textBox7.Text = comboBox1.Text+"：" + sTemp + "\n" + textBox7.Text;
                            //textBox7.AppendText("\n");
                        }
                    }
                }
                catch
                {
                    tAcceptMsg.Abort();
                    MessageBox.Show("消息发送失败");
                }
            }
            //禁止当前Socket上的发送与接收
            clientSocket.Shutdown(SocketShutdown.Both);
            //关闭Socket，并释放所有关联的资源
            clientSocket.Close();
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        //--------------启动监听65535端口--------------------//
        private void button4_Click(object sender, EventArgs e)
        {
            IPP = new IPEndPoint(IPAddress.Any, 65535);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(IPP);//关联（绑定）节点
            socket.Listen(0);//0表示连接数量不限

            //创建侦听线程
            tAcceptMsg = new Thread(new ThreadStart(this.AcceptMessage));
            tAcceptMsg.Start();
            button4.Enabled = false;
        }
        //--------------------发送消息--------------------//
        private void button5_Click(object sender, EventArgs e)
        {
            if (bConnected)
            {
                try
                {
                    //richTextBox2_KeyPress()和AcceptMessage()
                    //都将向richTextBox1写字符，可能访问有冲突，
                    //所以，需要多线程互斥
                    lock (this)
                    {
                        textBox7.AppendText("管理员->"+comboBox1.Text + "：" + textBox8.Text+"\n");
                        //textBox7.Text = "管理员：" + textBox8.Text + textBox7.Text;
                        //textBox7.AppendText("\n");
                        //客户机聊天信息写入网络流，以便服务器接收
                        wReader.WriteLine(textBox8.Text);
                        //清理当前缓冲区数据，使所有缓冲数据写入基础设备
                        wReader.Flush();
                        //发送成功后，清空输入框并聚集之
                        textBox8.Text = "";
                        textBox8.Focus();
                    }
                }
                catch
                {
                    MessageBox.Show("发送失败1");
                }
            }
            else
            {
                MessageBox.Show("发送失败2");
            }
        }
        //-------------关闭socket线程--------------------------//
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                socket.Close();
                tAcceptMsg.Abort();
            }
            catch
            { }
            button4.Enabled = true;
        }

        //----------------------------数据获取及展示方法集-----------------------//
        //***********************************************************************//
        //***********************************************************************//
        //-----------------------------------------------------------------------//
        /*
         * 功能: API获取oceanconnect数据及展示
         *       SDK封装了一部分,可查阅
         * 调用: 参见具体方法
         * 作者: later
         * 日期: 2018/08/01-2018/08/03
         * 修改: ---------
         */
        NASDK currsdk = null;
        //-----------------检查是否已获取token----------------------//
        private bool check()
        {
            if (currsdk == null)
            {
                MessageBox.Show("请先获取token");
                return false;
            }
            return true;
        }
        //----------------获取token并以textbox方法保存---------------------//
        public void getToken()
        {
            this.txtToken.Text = "......";
            currsdk = new NASDK(this.txtPltIP.Text, Convert.ToInt32(this.txtPort.Text), this.txtAppid.Text, this.txtAppPwd.Text, txtCertFile.Text, txtCertPwd.Text);
            TokenResult token = currsdk.getToken();
            if (token == null)
            {
                MessageBox.Show("获取失败，请看日志");

            }
            else
            {
                panel3.Visible = false;
                MessageBox.Show("配置成功!");
                this.txtToken.Text = token.accessToken;
                startThread_drawmap();
            }
        }
        //------------------------获取数据方法-------------------------//
        /*
         * 原函数: 选择性获取历史数据
         * 参数: 第几页, 每页记录数, 设备Id, 服务Id
         * 返回值: 待确定.../已确定
         * PS: pageno=0, pagesize为所需记录数. serviceId=null???
         */
        public HistoryDataResult getHistoryData(int PageNo, int PageSize, string DeviceId, string ServiceId)
        {
            if (!check()) { return null; }//null
            string startTime = dtpHDQueryStart.Value.ToString("yyyyMMddTHHmmssZ");
            string endTime = dtpHDQueryEnd.Value.ToString("yyyyMMddTHHmmssZ");
            int pageno; int pagesize;
            try
            {
                pageno = Convert.ToInt16(PageNo);
                pagesize = Convert.ToInt16(PageSize);
            }

            catch (Exception ex)
            {

            }
            HistoryDataResult result = currsdk.queryHistoryData(txtToken.Text, DeviceId, ServiceId, PageNo, PageSize, startTime, endTime);
            if (result == null)
            {

                MessageBox.Show("获取失败，请看日志");
                return result; //result

            }
            return result;
            
        }
        //-----------------------解析数据方法, 暂时写在这, 不采取调用方式---------------//
        /*
         * 返回值: 数据数组5*5
         */
        public int[][] transData(HistoryDataResult result)
        {
            //------------------------解析数据PS: 写成方法-----------------------------//
            List<DeviceDataHistoryDTOsItem> str = result.deviceDataHistoryDTOs;
            string result3 = "";
            //str.count 为记录条数, 等于参数PageSzie
            //str[i]为该条记录的第i个值
            int[] Ins_01 = new int[5];       //存储第一条记录的五个值, 不用这种方法
            int[][] Five_data = new int[5][];//我们要二维数组的形式
            string[] TimeStamp = new string[5];//时间戳数组
            Five_data[0] = new int[5];
            Five_data[1] = new int[5];
            Five_data[2] = new int[5];
            Five_data[3] = new int[5];
            Five_data[4] = new int[5];
            for (int i = 0; i < str.Count; i++)//i是记录条数, j是该条记录下的第j个值
            {
                int j = 0;
                foreach (KeyValuePair<string, string> item in str[i].data)
                {
                    result3 = item.Key + ":" + item.Value + "\r\n";
                    Five_data[i][j++] = Convert.ToInt16(item.Value);
                    TimeStamp[i] = str[i].timestamp;//时间戳, 判断是否是最后一次的数据
                    //MessageBox.Show(result3);
                }
            }
            //------------------------下拦截线-----------------------------// 
            return Five_data;
        }
        //--------------------这是click方法,已修改-----------------------//
        /*
        public void getCert()
        {
            OpenFileDialog ofg = new OpenFileDialog();
            if (ofg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            txtCertFile.Text = ofg.FileName;
        }
        */
        //-----------------参数配置方法---------------------------------------//
        private void 参数配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
        }
        //-----------------选择证书------------------------------------------//
        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofg = new OpenFileDialog();
            if (ofg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            txtCertFile.Text = ofg.FileName;
        }
        //------------------参数配置完成----------------------------//
        /*
         * 备忘: 这里还要开启一个线程, 用来检测数据变化, 地图更新
         * 说明: 备忘已完成
         */
        private void button8_Click(object sender, EventArgs e)
        {
            getToken();
            panel3.Visible = false;
        }
        //------------------以下放置四个区域总数据-------------------//
        /*
         * 解释: 
         * 横坐标: 垃圾桶序号
         * 纵坐标: 当前时间下倾倒次数
         * 说明: 假数据, 可以开始是查询计算显示一次就好, 但是平台上为创建足够多应用
         * 备忘: 不用蓝色图例
         */
        public void setChart1()
        {
            //绑定数据
            List<string> xData = new List<string>() {"Lir_01","Lir_02","Lir_03" };//x
            List<int> yData = new List<int>() { 3,5,7 };          //y
            chart1.Series[0]["PieLabelStyle"] = "Outside";//将文字移到外侧
            chart1.Series[0]["PieLineColor"] = "Black";//绘制黑色的连线。
            chart1.Series[0].Points.DataBindXY(xData, yData);
            this.chart1.Titles.Add("图书馆");
            //this.chart1.Legends.Clear();
        }
        public void setChart2()
        {
            //绑定数据
            List<string> xData = new List<string>() { "Bei_01","Bei_02","Bei_03","Bei_04","Bei_05" };//x
            List<int> yData = new List<int>() { 1,3,2,1,1 };          //y
            chart2.Series[0]["PieLabelStyle"] = "Outside";//将文字移到外侧
            chart2.Series[0]["PieLineColor"] = "Black";//绘制黑色的连线。
            chart2.Series[0].Points.DataBindXY(xData, yData);
            this.chart2.Titles.Add("北苑");
        }
        public void setChart3()
        {
            //绑定数据
            List<string> xData = new List<string>() { "Xi_01","Xi_02","Xi_03","Xi_04","Xi_05","Xi_06","Xi_07" };//x
            List<int> yData = new List<int>() { 1, 1, 0, 2, 1, 4, 3 };          //y
            chart3.Series[0]["PieLabelStyle"] = "Outside";//将文字移到外侧
            chart3.Series[0]["PieLineColor"] = "Black";//绘制黑色的连线。
            chart3.Series[0].Points.DataBindXY(xData, yData);
            this.chart3.Titles.Add("西苑");
        }
        public void setChart4()
        {
            //绑定数据
            List<string> xData = new List<string>() {"Nan_01","Nan_02","Nan_03","Nan_04","Nan_05"};//x
            List<int> yData = new List<int>() {3,1,0,2,1 };          //y
            chart4.Series[0]["PieLabelStyle"] = "Outside";//将文字移到外侧
            chart4.Series[0]["PieLineColor"] = "Black";//绘制黑色的连线。
            chart4.Series[0].Points.DataBindXY(xData, yData);
            this.chart4.Titles.Add("南苑");
        }
        //------------------选择指定垃圾桶查看其随时间变化的变化------------------------//
        /*
         * 问题: 数据不对, 多次点击上一个图表还在
         * 问题已解决: 配置时注意选择时间
         * 待解决: 变为折线图, 写入具体deviceId, 其它区域/其它区域不做
         */
        private void button9_Click(object sender, EventArgs e)
        {
            string deviceId = null;
            //翻译成指定设备,继续添加
            if (this.comboBox3.Text.Equals("device_Lir_01"))
                deviceId = All_Devices.Lir_deviceId1;
            if (this.comboBox3.Text.Equals("device_Lir_02"))
                deviceId = All_Devices.Lir_deviceId2;
            if (this.comboBox3.Text.Equals("device_Lir_03"))
                deviceId = All_Devices.Lir_deviceId3;

            HistoryDataResult result =getHistoryData(0, 5, deviceId, null);//获取某一设备的前5条数据
            //------------------------解析数据PS: 写成方法-----------------------------//
            List<DeviceDataHistoryDTOsItem> str = result.deviceDataHistoryDTOs;
            string result3 = "";
            //str.count 为记录条数, 等于参数PageSzie
            //str[i]为该条记录的第i个值
            int[] Ins_01 = new int[5];       //存储第一条记录的五个值, 不用这种方法
            int[][] Five_data = new int[5][];//我们要二维数组的形式
            Five_data[0] = new int[5];
            Five_data[1] = new int[5];
            Five_data[2] = new int[5];
            Five_data[3] = new int[5];
            Five_data[4] = new int[5];
            string[] TimeStamp = new string[5];//时间戳数组
            for (int i = 0; i < str.Count; i++)//i是记录条数, j是该条记录下的第j个值
            {
                int j = 0;
                foreach (KeyValuePair<string, string> item in str[i].data)
                {
                    result3 = item.Key + ":" + item.Value + "\r\n";
                    Five_data[i][j++] = Convert.ToInt16(item.Value);
                    TimeStamp[i] = str[i].timestamp;//时间戳, 判断是否是最后一次的数据, 这里还要解析为时间
                    //MessageBox.Show(result3);// 为什么这里的不弹窗?
                    //MessageBox.Show(item.Value);
                }
            }
            //MessageBox.Show("result3: "+result3);
            //------------------------下拦截线-----------------------------// 
            //绑定数据
            List<string> xData = new List<string>() { TimeStamp[0], TimeStamp[1].Substring(9), TimeStamp[2], TimeStamp[3], TimeStamp[4] };//x
            List<int> yData = new List<int>() { Five_data[0][0], Five_data[1][0], Five_data[2][0], Five_data[3][0], Five_data[4][0] };          //y
            chart5.Series[0]["PieLabelStyle"] = "Outside";//将文字移到外侧
            chart5.Series[0]["PieLineColor"] = "Black";//绘制黑色的连线。
            chart5.Series[0].Points.DataBindXY(xData, yData);
            //this.chart5.Titles.Add(this.comboBox3.Text);
        }
        //-------------------查询指定垃圾桶的分贝变化折线图----------------------//
        /*
         * 问题: 数据是反的, 该倒过来 6 5 4 3 2 1 0
         * 说明: 问题已解决
         * 备注: 七条数据是不是少了?
         */
        private void button10_Click(object sender, EventArgs e)
        {
            string deviceId = null;
            //翻译成指定设备,继续添加
            if (this.comboBox5.Text.Equals("device_Lir_01"))
                deviceId = All_Devices.Lir_deviceId1;
            if (this.comboBox5.Text.Equals("device_Lir_02"))
                deviceId = All_Devices.Lir_deviceId2;
            if (this.comboBox5.Text.Equals("device_Lir_03"))
                deviceId = All_Devices.Lir_deviceId3;

            HistoryDataResult result = getHistoryData(0, 7, deviceId, null);//获取某一设备的前7条数据
            //------------------------解析数据PS: 写成方法-----------------------------//
            List<DeviceDataHistoryDTOsItem> str = result.deviceDataHistoryDTOs;
            string result3 = "";
            //str.count 为记录条数, 等于参数PageSzie
            //str[i]为该条记录的第i个值
            int[][] Five_data = new int[7][];//我们要二维数组的形式
            Five_data[0] = new int[5];
            Five_data[1] = new int[5];
            Five_data[2] = new int[5];
            Five_data[3] = new int[5];
            Five_data[4] = new int[5];
            Five_data[5] = new int[5];
            Five_data[6] = new int[5];
            string[] TimeStamp = new string[7];//时间戳数组
            for (int i = 0; i < str.Count; i++)//i是记录条数, j是该条记录下的第j个值
            {
                int j = 0;
                foreach (KeyValuePair<string, string> item in str[i].data)
                {
                    result3 = item.Key + ":" + item.Value + "\r\n";
                    Five_data[i][j++] = Convert.ToInt16(item.Value);
                    TimeStamp[i] = str[i].timestamp;//时间戳, 判断是否是最后一次的数据, 这里还要解析为时间
                }
            }
            //绑定数据
            List<string> xData = new List<string>() { TimeStamp[6].Substring(9), TimeStamp[5].Substring(9), TimeStamp[4].Substring(9), TimeStamp[3].Substring(9), TimeStamp[2].Substring(9), TimeStamp[1].Substring(9), TimeStamp[0].Substring(9) };//x
            List<int> yData = new List<int>() { Five_data[6][2], Five_data[5][2], Five_data[4][2], Five_data[3][2], Five_data[2][2],Five_data[1][2],Five_data[0][2] };          //y
            chart6.Series[0]["PieLabelStyle"] = "Outside";//将文字移到外侧
            chart6.Series[0]["PieLineColor"] = "Black";//绘制黑色的连线。
            chart6.Series[0].Points.DataBindXY(xData, yData);
            //this.chart5.Titles.Add(this.comboBox3.Text);
        }
        //------------------------报警计数和-------------------------------//
        /*
         * 备注: 数据处理交给单片机, 我这里不做
         *       把数据获取与解析搬过来
         * 说明: 已完成
         */
        private void button11_Click(object sender, EventArgs e)
        {
            HistoryDataResult result1 = getHistoryData(0, 1, All_Devices.Lir_deviceId1, null);//获取某一设备的前1条数据
            HistoryDataResult result2 = getHistoryData(0, 1, All_Devices.Lir_deviceId2, null);
            HistoryDataResult result3 = getHistoryData(0, 1, All_Devices.Lir_deviceId3, null);
            
            int[][] alert1=transData(result1);
            int[][] alert2 = transData(result2);
            int[][] alert3 = transData(result3);
            int Lir_All = alert1[0][1]+alert2[0][1]+alert3[0][1];
            
            this.label23.Text = Lir_All.ToString()+"次";
            this.label24.Text = "2次";
            this.label25.Text = "4次";
            this.label26.Text = "7次";
        }
        //------------------------------数据预测方法集---------------------------//
        //***********************************************************************//
        //***********************************************************************//
        //-----------------------------------------------------------------------//
        /*
         * 功能: 粒子群算法
         * 调用: 参见具体方法
         * 作者: later
         * 日期: 2018/08/03
         * 修改: ---------
         */
        //-------------------Gmap_before初始化-------------------//
        /*
         * 备注: 图书馆区域, 放大比例是17
         * 待修改: 选择的参数作为形参, 判断形参, 选择不同的地区初始化/已修改
         */
        public void Init_before()
        {
            clearMark(markersOverlay_before);            
            try
            {
                System.Net.IPHostEntry e = System.Net.Dns.GetHostEntry("ditu.google.cn");
            }
            catch
            {
                mapControl2.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection avaible, going to CacheOnly mode.", "GMap.NET Demo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            mapControl2.CacheLocation = Environment.CurrentDirectory + "\\GMapCache\\"; //缓存位置
            mapControl2.MapProvider = GMapProviders.GoogleChinaMap; //google china 地图
            mapControl2.MinZoom = 2;  //最小比例
            mapControl2.MaxZoom = 24; //最大比例
            mapControl2.Zoom = 16;     //当前比例
            mapControl2.ShowCenter = false; //不显示中心十字点
            mapControl2.DragButton = System.Windows.Forms.MouseButtons.Left; //左键拖拽地图
            if (comboBox6.Text.Equals("图书馆"))
            {
                mapControl2.Position = new PointLatLng(31.7508319669, 119.9192261696); //地图中心位置：江理工图书馆
                mapControl2.Overlays.Add(markersOverlay_before);   //MapControl添加图层
                //SetLableOnMap_before_green_pushpin(LirPoints[1], markersOverlay_before,null);//图层再添加marker
                SetLableOnMap_fore(All_Points.LirPoints, markersOverlay_before);
            }

            if (comboBox6.Text.Equals("北苑"))
            {
                clearMark(markersOverlay_before);
                mapControl2.Position = new PointLatLng(31.7533279076, 119.9200723302);
                mapControl2.Overlays.Add(markersOverlay_before);
                SetLableOnMap_fore(All_Points.BeiPoints, markersOverlay_before);
            }

            if (comboBox6.Text.Equals("西苑"))
            {
                clearMark(markersOverlay_before);
                mapControl2.Position = new PointLatLng(31.7507630367, 119.9136728194);
                mapControl2.Overlays.Add(markersOverlay_before);
                SetLableOnMap_fore(All_Points.XiPoints, markersOverlay_before);
            }
            if (comboBox6.Text.Equals("南苑"))
            {
                clearMark(markersOverlay_before);
                mapControl2.Position = new PointLatLng(31.7480957415, 119.9205551278);
                mapControl2.Overlays.Add(markersOverlay_before);
                SetLableOnMap_fore(All_Points.NanPoints, markersOverlay_before);
            }

            //beiyuan: 31.7533279076,119.9200723302
            //xiyuan: 31.7507630367,119.9136728194
            //nanyuan: 31.7480957415,119.9205551278

            //mapControl2.Overlays.Add(markersOverlay_before);

            mapControl2.MouseClick += new MouseEventHandler(mapControl_MouseClick);
        }
        //-------------------Gmap_after初始化-------------------//
        /*
         * 问题: 第一次点击只有中心点, 出现绿色点, 在次点击会出现预设点, 但是绿点小时
         * 解决: 试试把清除图层函数去掉
         */
        public void Init_after()
        {
            clearMark(markersOverlay_after);
            try
            {
                System.Net.IPHostEntry e = System.Net.Dns.GetHostEntry("ditu.google.cn");
            }
            catch
            {
                mapControl3.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection avaible, going to CacheOnly mode.", "GMap.NET Demo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            mapControl3.CacheLocation = Environment.CurrentDirectory + "\\GMapCache\\"; //缓存位置
            mapControl3.MapProvider = GMapProviders.GoogleChinaMap; //google china 地图
            mapControl3.MinZoom = 2;  //最小比例
            mapControl3.MaxZoom = 24; //最大比例
            mapControl3.Zoom = 16;     //当前比例
            mapControl3.ShowCenter = false; //不显示中心十字点
            mapControl3.DragButton = System.Windows.Forms.MouseButtons.Left; //左键拖拽地图
            if (comboBox6.Text.Equals("图书馆"))
            {
                clearMark(markersOverlay_after);
                mapControl3.Position = new PointLatLng(31.7508319669, 119.9192261696); //地图中心位置：江理工图书馆
                mapControl3.Overlays.Add(markersOverlay_after);
                SetLableOnMap_fore(All_Points.LirPoints, markersOverlay_after);
            }

            if (comboBox6.Text.Equals("北苑"))
            {
                clearMark(markersOverlay_after);
                mapControl3.Position = new PointLatLng(31.7533279076, 119.9200723302);
                mapControl3.Overlays.Add(markersOverlay_after);
                SetLableOnMap_fore(All_Points.BeiPoints, markersOverlay_after);
            }

            if (comboBox6.Text.Equals("西苑"))
            {
                clearMark(markersOverlay_after);
                mapControl3.Position = new PointLatLng(31.7507630367, 119.9136728194);
                mapControl3.Overlays.Add(markersOverlay_after);
                SetLableOnMap_fore(All_Points.XiPoints, markersOverlay_after);
            }
            if (comboBox6.Text.Equals("南苑"))
            {
                clearMark(markersOverlay_after);
                mapControl3.Position = new PointLatLng(31.7480957415, 119.9205551278);
                mapControl3.Overlays.Add(markersOverlay_after);
                SetLableOnMap_fore(All_Points.NanPoints, markersOverlay_after);
            }

            //未成功
            string[] LatLng1 = All_Points.LirPoints[0].Split(',');
            Convert.ToDouble(LatLng1[0]);
            Convert.ToDouble(LatLng1[1]);
            string[] LatLng2 = All_Points.LirPoints[1].Split(',');
            Convert.ToDouble(LatLng2[0]);
            Convert.ToDouble(LatLng2[1]);
            double lat0 = (Convert.ToDouble(LatLng1[0]) + Convert.ToDouble(LatLng2[0]) / 2);
            double lat1 = (Convert.ToDouble(LatLng1[1]) + Convert.ToDouble(LatLng2[1]) / 2);
            markersOverlay_after.Markers.Add(new GMarkerGoogle(new PointLatLng(lat1, lat0), GMarkerGoogleType.green));
            markersOverlay_after.Markers.Add(new GMarkerGoogle(new PointLatLng(lat0, lat1), GMarkerGoogleType.green));

            mapControl3.Overlays.Add(markersOverlay_after);

            mapControl3.MouseClick += new MouseEventHandler(mapControl_MouseClick);
        }
        //-------------------预测分布方法-----------------//
        /*
         * 备注: 算法内容见调用方法
         */
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            Init_before();
        }
        private void button12_Click(object sender, EventArgs e)
        {
            //Init_before();
            Init_after();
            fore_DrawPoints();
        }
        //--------------------预测算法-------------------//
        /*
         * 备注: 这里涉及到获取数据,根据数据打点,调用SetLableOnMap_before_green_pushpin方法
         * 思路: 获取最新十条数据, 加加减减, 打之前确定好的固定点
         *       同时生成预测报告
         * 问题: 还没写完
         */
        public void fore_DrawPoints()
        {
            //int Xi = 12;
            //int Bei = 12;
            //int Nan = 12;
            double center_Lng = 31.7508099361;
            double center_Lat= 119.9191711079;
            if (comboBox6.Text.Equals("图书馆"))
            { 
                HistoryDataResult result1 = getHistoryData(0, 5, All_Devices.Lir_deviceId1, null);//获取某一设备的前1条数据
                HistoryDataResult result2 = getHistoryData(0, 5, All_Devices.Lir_deviceId2, null);
                HistoryDataResult result3 = getHistoryData(0, 5, All_Devices.Lir_deviceId3, null);

                int[][] data1 = transData(result1);
                int[][] data2 = transData(result2);
                int[][] data3 = transData(result3);
                int data_1 = 0;
                int data_2 = 0;
                int data_3 = 0;

                for (int i = 0; i < 5; i++)
                {
                    data_1 += data1[i][0];
                    data_2 += data2[i][0];
                    data_3 += data3[i][0];
                }
                //max,min               
                int sum = data_1 + data_2 + data_3;
                int max_num=max(data_1,data_2,data_3);
                int min_num=min(data_1, data_2, data_3); 
                int temp=0;
                if (data_1 >= (sum / 3.0))
                {
                    temp++;
                }
                if (data_2 >= (sum / 3.0))
                {
                    temp++;
                }
                if (data_3 >= (sum / 3.0))
                {
                    temp++;
                }
                if (temp == 3)
                {
                    //PointsKeep();
                    //do nothing 
                    this.label28.Text = "目前图书馆区有3个垃圾桶,\n经过粒子群算法预测:\n此区域共需放置0个垃圾用\n相比目前增加了0几个,\n分布具体位置是----------空-----------\n.....";
                }
                if (temp == 2)
                {
                    //PointsAdd();
                    int n =max_num-sum/3;   //垃圾桶个数 
                    double[] Lat_new = new double[n];
                    double[] Lng_new = new double[n];

                    //      1/(n+1)*距离
                    int max_temp = 0;
                    if (data_1 == max_num)
                        max_temp = 0;
                    if (data_2 == max_num)
                        max_temp = 1;
                    if (data_3 == max_num)
                        max_temp = 2;

                    int min_temp2 = 0;
                    if (data_1 == min_num)
                        min_temp2 = 0;
                    if (data_2 == min_num)
                        min_temp2 = 1;
                    if (data_3 == min_num)
                        min_temp2 = 2;
                    int second_temp = 3 - max_temp - max_temp;
                    string location = null;
                    for (int i = 0; i < n; n++)
                    {
                        if (i % 2 == 0)
                        {
                            Lat_new[i] = (center_Lat + All_Points.LirPoints_Lat[max_temp]) / (1.0 / (n + 1));
                            Lng_new[i] = (center_Lng + All_Points.LirPoints_Lng[max_temp]) / (1.0 / (n + 1));
                        }
                        else
                        {                            
                                Lat_new[i] = (center_Lat + All_Points.LirPoints_Lat[second_temp]) / (1.0 / (n + 1));
                                Lng_new[i] = (center_Lng + All_Points.LirPoints_Lng[second_temp]) / (1.0 / (n + 1));                                                    
                        }
                        SetLableOnMap_before_green_pushpin(Lat_new[i].ToString()+","+Lng_new[i].ToString(),markersOverlay_after,"新增第"+n.ToString()+"个垃圾桶");
                        location += n.ToString()+". "+Lat_new[i].ToString() +","+ Lng_new[i].ToString()+"\n";
                    }
                    this.label28.Text = "目前图书馆区有3个垃圾桶,\n经过粒子群算法预测:\n此区域共需放置"+(n+3).ToString()+ "个垃圾用\n相比目前增加了" + n.ToString() + "几个,\n分布具体位置是n"+location;

                }
                if (temp == 1)
                {
                    int min_temp = 0;
                    if (data_1 == min_num)
                        min_temp = 0;
                    if (data_2 == min_num)
                        min_temp = 1;
                    if (data_3 == min_num)
                        min_temp = 2;
                    //this.PointsDec();   //去掉最小值点
                    SetLableOnMap_before_red(All_Points.LirPoints[min_temp],markersOverlay_after,"去掉的垃圾桶");
                    this.label28.Text = "目前图书馆区有3个垃圾桶,\n经过粒子群算法预测:\n此区域共需放置2个垃圾用\n相比目前减少了1几个,\n分布具体位置是: \n"+(min_temp+1).ToString()+"号垃圾桶:"+All_Points.LirPoints[min_temp];

                }

            }
            //其它区域固定死
            if (comboBox6.Text.Equals("西苑"))//-3
            {
                //SetLableOnMap_before_red();
                SetLableOnMap_before_red(All_Points.XiPoints[1], markersOverlay_after, "去掉的垃圾桶1");
                SetLableOnMap_before_red(All_Points.XiPoints[5], markersOverlay_after, "去掉的垃圾桶2");
                SetLableOnMap_before_red(All_Points.XiPoints[6], markersOverlay_after, "去掉的垃圾桶3");
                string location = "去掉的垃圾桶1:\n" + All_Points.XiPoints[1].ToString() + "\n"+"去掉的垃圾桶2:\n" + All_Points.XiPoints[5].ToString() + "\n"+"去掉的垃圾桶3:\n" + All_Points.XiPoints[6].ToString() + "\n";
                this.label28.Text = "目前西苑区有7个垃圾桶,\n经过粒子群算法预测:\n此区域共需4个垃圾用\n相比目前减少了3几个,\n分布具体位置是: \n" + location;

            }
            if (comboBox6.Text.Equals("北苑"))//+1
            {
                SetLableOnMap_before_green_pushpin("119.9206516873,31.7532458010", markersOverlay_after, "新增第1个垃圾桶");
                this.label28.Text = "目前北苑区有5个垃圾桶,\n经过粒子群算法预测:\n此区域共需6个垃圾用\n相比目前增加了1几个,\n分布具体位置是: \n" + "1 :" + "31.7532458010,119.9206516873";

            }
            if (comboBox6.Text.Equals("南苑"))
            {
                //do nothing
                this.label28.Text = "目前南苑区有5个垃圾桶,\n经过粒子群算法预测:\n此区域共需放置0个垃圾用\n相比目前增加了0几个,\n分布具体位置是-----------空----------\n.....";
            }


        }
        public int max(int a, int b, int c)
        {
            int max = 0;
            if (a > max)
                max = a;
            else if (b > max)
                max = b;
            else max = c;
            return max;

        }
        public int min(int a, int b, int c)
        {
            int min = 0;
            if (a < min)
                min = a;
            else if (b < min)
                min = b;
            else min = c;
            return min;
        }
        //-------------------打单个点--绿色----------//
        //将字符串组LatLngInfo的点打标记，经纬度中用','隔开, 把点的颜色也写成形参
        /*
         * 备注: 这样打点不会消失了
         *     : 肯定可以用
         * 参数: LatLng: 经纬度字符串
         *       markersOverlay : markersOverlay_before or markersOverlay_after
         *       label: null是不显示
         * 
         */
        public void SetLableOnMap_before_green_pushpin(string LatLngInfo, GMapOverlay markers_Overlay, string label)
        {
            string[] LatLng = null;
            LatLng = LatLngInfo.Split(',');

            PointLatLng point = new PointLatLng(double.Parse(LatLng[1]), double.Parse(LatLng[0]));
            GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.green_pushpin);
            //-------           
            marker.ToolTipText = label;       //这是标签
            marker.Tag = 1;
            marker.ToolTipMode = MarkerTooltipMode.Always;
            //-------
            markers_Overlay.Markers.Add(marker);
        }
        //-------------------打单个点--红色----------//
        //将字符串组LatLngInfo的点打标记，经纬度中用','隔开, 把点的颜色也写成形参
        /*
         * 备注: 这样打点不会消失了
         *     : 肯定可以用
         * 参数: LatLng: 经纬度字符串
         *       markersOverlay : markersOverlay_before or markersOverlay_after
         *       label: null是不显示
         * 
         */
        public void SetLableOnMap_before_red(string LatLngInfo, GMapOverlay markers_Overlay, string label)
        {
            string[] LatLng = null;
            LatLng = LatLngInfo.Split(',');

            PointLatLng point = new PointLatLng(double.Parse(LatLng[1]), double.Parse(LatLng[0]));
            GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red);
            //-------           
            marker.ToolTipText = label;       //这是标签
            marker.Tag = 1;
            marker.ToolTipMode = MarkerTooltipMode.Always;
            //-------
            markers_Overlay.Markers.Add(marker);
        }
        //-----------------------打一系列点-------------------------------------//
        /*
         * 备注: 形参markersOverlay=markersOverlay_before or markersOverlay_after
         *     : 也可以直接单独使用
         *     
         */
        public void SetLableOnMap_fore(string[] LatLngInfo, GMapOverlay markers_Overlay)
        {
            //创建图层  
            //GMapOverlay gMapOverlay = new GMapOverlay();
            //给每个坐标打点
            for (int i = 0; i < LatLngInfo.Length; i++)
            {
                string[] LatLng = LatLngInfo[i].Split(',');
                //在坐标点上绘制一绿色点并向图层中添加标签 
                markers_Overlay.Markers.Add(new GMarkerGoogle(new PointLatLng(double.Parse(LatLng[1]), double.Parse(LatLng[0])), GMarkerGoogleType.gray_small));
                //方便之后寻找到是第几个GMapMarker   
                markers_Overlay.Markers[i].Tag = i;
                markers_Overlay.Markers[i].Tag = "xxxx";
                markers_Overlay.Id = "markroad";
            }
            //GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.green_pushpin);
            //markers_Overlay.Markers.Add(marker);
        }
        //-----------------------生成预测报告方法-----------------//
        private void button13_Click(object sender, EventArgs e)
        {
            this.label28.Text = "目前___区有___个垃圾桶,\n经过粒子群算法预测:\n此区域共需放置___个垃圾用\n相比目前___了几个,\n分布具体位置是1.______________\n2.________________\n.....";
        }
        //-----------------------保存为图片----------------------//
        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "PNG (*.png)|*.png";
                    dialog.FileName = "GMap.NET image";
                    // Image image = this.gMapControl1.ToImage();
                    Image image = this.mapControl3.ToImage();
                    if (image != null)
                    {
                        using (image)
                        {
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                string fileName = dialog.FileName;
                                if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                                {
                                    fileName += ".png";
                                }
                                image.Save(fileName);
                                MessageBox.Show("图片已保存： " + dialog.FileName, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("图片保存失败： " + exception.Message, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        //------------------------------实时数据方法集---------------------------//
        //***********************************************************************//
        //***********************************************************************//
        //-----------------------------------------------------------------------//
        /*
         * 功能: 实时获取数据并打点
         * 调用: 参见具体方法
         * 备注: 注意添加关闭窗口关闭线程方法
         * 作者: later
         * 日期: 2018/08/04
         * 修改: ---------
         */
        private Thread thread1 = null;
        //-----------------该方法放入getToken中---------------------//
        public void startThread_drawmap()
        {
            thread1 = new Thread(new ThreadStart(get_data_drawmap));
            thread1.Start();
            thread1.IsBackground = true;
            Thread.Sleep(5000);
            Console.Read();
        }
        //---------------- 获取数据实时打点方法--------------------//
        public void get_data_drawmap()
        {         
            string device_Lir_01 = All_Devices.Lir_deviceId1;
            string device_Lir_02 = All_Devices.Lir_deviceId2;
            string device_Lir_03 = All_Devices.Lir_deviceId3;
            while (true)
            {
                onTime_Lir_device(device_Lir_01,1);
                onTime_Lir_device(device_Lir_02,2);
                onTime_Lir_device(device_Lir_03,3);
                Thread.Sleep(5000);//这里才是正真休眠的5s
            }           
        }
        //-----------------------图书馆设备一---------------------------//
        /*
         * 备注: 解决时间戳判别问题, 去除标记是一个问题
         * 思路: 貌似不需要解决, 一直打点还是只有一个点
         * GMap问题: 貌似使用多个图层, 缩放时会去掉之前的标记?/已解决
         * 问题: 时间戳判断问题, 是否写三个函数, 用三个label
         */
        public void onTime_Lir_device(string deviceId, int num)
        {
            
            HistoryDataResult result = getHistoryData(0, 1, deviceId, null);//获取某一设备的前7条数据
            List<DeviceDataHistoryDTOsItem> str = result.deviceDataHistoryDTOs;
            string result3 = "";
            int[] data = new int[5];
            string TimeStamp =null;//时间戳
            for (int i = 0; i < str.Count; i++)//i是记录条数, j是该条记录下的第j个值
            {
                int j = 0;
                foreach (KeyValuePair<string, string> item in str[i].data)
                {
                    result3 = item.Key + ":" + item.Value + "\r\n";
                    data[j++] = Convert.ToInt16(item.Value);
                    TimeStamp = str[i].timestamp;//时间戳, 判断是否是最后一次的数据, 这里还要解析为时间
                }
            }
            //-----解决三个垃圾桶问题--
            string TempString = null;
            switch (num)
            {
                case 1: TempString = this.txtTStamp1.Text; break;
                case 2: TempString = this.txtTStamp2.Text; break;
                case 3: TempString = this.txtTStamp3.Text; break;
                default:break;
            }
            //------------添加图层--------------
            if (data[0] == 100 )//20160804T170754Z  
            {
                //判断告警值是否达到要求, 再判断是不是最新的数值
                if (TimeStamp.Equals(TempString))
                { 
                    // do nothing
                }
                else
                {
                    //都放在switch中
                    //SetLableOnMap_before_green_pushpin("119.9192368984, 31.7503484356", markersOverlay, "这个桶满了1");
                    //SetLableOnMap_before_green_pushpin("31.7503484356, 119.9192368984", markersOverlay, "新增点2");
                    //MessageBox.Show(deviceId + "出现100");
                    //this.txtTStamp.Text = TimeStamp;
                    switch (num)
                    {
                        case 1:  this.txtTStamp1.Text= TimeStamp;
                            SetLableOnMap_before_green_pushpin(All_Points.LirPoints[0], markersOverlay1, "1号垃圾桶已满"); break;
                        case 2:  this.txtTStamp2.Text= TimeStamp;
                            SetLableOnMap_before_green_pushpin(All_Points.LirPoints[1], markersOverlay2, "2号垃圾桶已满"); break;
                        case 3:  this.txtTStamp3.Text= TimeStamp;
                            SetLableOnMap_before_green_pushpin(All_Points.LirPoints[2], markersOverlay3, "3号垃圾桶已满"); break;
                        default: break;
                    }
                }
                
            }
            //----------报警----------
            if (data[1] == 100)//20160804T170754Z  
            {
                //判断告警值是否达到要求, 再判断是不是最新的数值
                if (TimeStamp.Equals(TempString))
                {
                    // do nothing
                }
                else
                {
                    //都放在switch中
                    //SetLableOnMap_before_green_pushpin("119.9192368984, 31.7503484356", markersOverlay, "这个桶满了1");
                    //SetLableOnMap_before_green_pushpin("31.7503484356, 119.9192368984", markersOverlay, "新增点2");
                    //MessageBox.Show(deviceId + "出现100");
                    //this.txtTStamp.Text = TimeStamp;
                    switch (num)
                    {
                        case 1:
                            this.txtTStamp1.Text = TimeStamp;
                            MessageBox.Show("1号垃圾桶有求助信息");
                            SetLableOnMap_before_red(All_Points.LirPoints[0], markersOverlay, "1号垃圾桶有求助信息"); break;
                        case 2:
                            this.txtTStamp2.Text = TimeStamp;
                            MessageBox.Show("2号垃圾桶有求助信息");
                            SetLableOnMap_before_red(All_Points.LirPoints[1], markersOverlay, "2号垃圾桶有求助信息"); break;
                        case 3:
                            this.txtTStamp3.Text = TimeStamp;
                            MessageBox.Show("3号垃圾桶有求助信息");
                            SetLableOnMap_before_red(All_Points.LirPoints[2], markersOverlay, "3号垃圾桶有求助信息"); break;
                        default: break;
                    }
                }

            }
            //------------清理满溢图层--------------
            if (data[0] != 100)
            {
                //图层要先绑定MapControl
                switch (num)
                {
                    case 1:
                        clearMark(markersOverlay1); break;
                    case 2:
                        clearMark(markersOverlay2); break;
                    case 3:
                        clearMark(markersOverlay3); break;
                    default: break;
                }
                //clearMark(markersOverlay);
            }

        }
        //------------------------------菜单栏等其他方法---------------------------//
        //***********************************************************************//
        //***********************************************************************//
        //-----------------------------------------------------------------------//
        /*
         * 功能: 一些简单功能
         * 调用: 参见具体方法
         * 备注: 参阅之前的SkinTest工程
         * 作者: later
         * 日期: 2018/8/11
         * 修改: ---------
         */
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void 重启ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("要重新启动吗？", "提示", MessageBoxButtons.YesNoCancel,
              MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                System.Environment.Exit(0);
            }
        }

        private void 使用说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("NOTEPAD.exe", "E:\\onenet_C#\\Test_url\\Test_url\\Test_url\\aaaFinal\\final\\final_01\\final_01\\readme.txt");
        }

        private void 联系作者ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://blog.csdn.net/qq_37832932");
        }

        private void 学习交流ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("即将前往Github", "提示", MessageBoxButtons.YesNoCancel,
              MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://github.com/SCFMVP");
            }          
        }

        private void 团队简介ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("NOTEPAD.exe", "E:\\onenet_C#\\Test_url\\Test_url\\Test_url\\aaaFinal\\final\\final_01\\final_01\\readme.txt");      
        }

        private void 学校官网ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.jstu.edu.cn/");
        }

        private void 源码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("即将前往Github", "提示", MessageBoxButtons.YesNoCancel,
              MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://github.com/SCFMVP");
            }
        }

        private void 作者邮箱ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("laterniu@foxmail.com");
        }

        private void 作者博客ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("即将前往CSDN", "提示", MessageBoxButtons.YesNoCancel,
              MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://blog.csdn.net/qq_37832932");
            }
        }

       
    }
}
