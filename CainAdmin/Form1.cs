using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Drawing;
using System.Diagnostics;
using System.Linq;

namespace CainAdmin
{
    public partial class Form1 : Form
    {
        //============头部声明==========//
        int Year;
        int Month;
        int Day;
        int Hour;
        int Minute;
        int Second;
        string str4h;
        string str4m;
        string str4s;
        string ipv4addr;
        string ipv4mask;
        string ipv4gate;
        string ipv4dns1;
        string ipv4dns2;
        string timestamp;
        string str4ver;
        string vertxt = "无法获取";
        string ip1dvc;
        string ip2dvc;
        const long GB = 1073741824; // 1GB大小为 1024 * 1024 * 1024  = 1073741824
        long totalSpace;
        long freeSpace;
        long usedSpace;
        double usage;
        double dskusgalert = 0.9;
        string HDID;
        string HDSN;
        string vername;
        int ptnum = 0;//分区数量
        //============变量&方法分割线==========//
        void sysispro()//判断系统是否为专业版
        {
            if (vername.Contains("专业")|| vername.Contains("企业"))
            {
                btn4rungpedit.Enabled = true;
            }
        }
        void getvername()//显示系统全称
        {
            vername = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault().ToString();
            lb4vername.Text = vername != null ? vername : "无法获取";
        }
        void mainload()//主载入，载入logo图片
        {
            pic4logo.ImageLocation = @"img\logo.png";
        }
        void showmain()//显示版本号
        {
            FileInfo fi = new FileInfo(Application.StartupPath + "\\CainAdmin.exe");
            string fulldatetime = fi.LastWriteTime.ToString();
            string year = fulldatetime.Substring(0, 4);
            string month = fulldatetime.Substring(5, 2);
            string month1 = month.Substring(0, 1);
            string month2 = month.Substring(1, 1);
            string day = "";
            string day1 = "";
            string day2 = "";
            if (month2 == "/")
            {
                month = "0" + month1;
                day = fulldatetime.Substring(7, 2);
                day1 = day.Substring(0, 1);
                day2 = day.Substring(1, 1);
                if (day2 == " ")
                {
                    day = day1;
                }
            }
            else
            {
                day = fulldatetime.Substring(8, 2);
                day1 = day.Substring(0, 1);
                day2 = day.Substring(1, 1);
                if (day2 == " ")
                {
                    day = day1;
                }
            }
            lb4lsttime.Text = "build " + year + month + day;
        }
        void gethardinfo()//总干线显示硬件信息
        {
            getcpuid();
            getplcid();
            getdiskid();
            getusrname();
            getpctype();
            getpm();
            getpcname();
            getbiosid();
        }
        void getcpuid()
        {
            try
            {
                //cpu id
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                string strID = null;
                foreach (ManagementObject mo in moc)
                {
                    strID = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                moc = null;
                mc = null;
                lb4cpuid.Text = strID;
            }
            catch
            {
                lb4cpuid.Text = "无法获取";
            }
        }//显示CPU ID
        void getplcid()
        {
            try
            {
                //主板id
                ManagementClass mc = new ManagementClass("Win32_BaseBoard");
                ManagementObjectCollection moc = mc.GetInstances();
                string strID = null;
                foreach (ManagementObject mo in moc)
                {
                    strID = mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
                moc = null;
                mc = null;
                lb4plcid.Text = strID;
                if (lb4plcid.Text == "Default string")
                {
                    lb4plcid.Text = "无法获取";
                }
            }
            catch
            {
                lb4plcid.Text = "无法获取";
            }
        }//显示主板ID
        void getdiskid()
        {
            try//探针硬盘型号
            {
                ManagementClass cimobject = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc1 = cimobject.GetInstances();
                foreach (ManagementObject mo in moc1)
                {
                    HDSN = (string)mo.Properties["Model"].Value;
                }
                lb4diskmdl.Text = HDSN;
            }
            catch
            {
                lb4diskmdl.Text = "无法获取";
            }
            try//探针硬盘ID
            {
                ManagementClass mc1 = new ManagementClass("Win32_PhysicalMedia");
                ManagementObjectCollection moc2 = mc1.GetInstances();
                foreach (ManagementObject mo in moc2)
                {
                    HDID = mo.Properties["SerialNumber"].Value.ToString().Trim();
                    break;
                }
                lb4diskid.Text = HDID;
            }
            catch
            {
                lb4diskid.Text = "无法获取";
            }
        }//显示硬盘ID
        void getusrname()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["UserName"].ToString();
                }
                moc = null;
                mc = null;
                lb4usrname.Text = st;
            }
            catch
            {
                lb4usrname.Text = "无法获取";
            }
        }//显示用户名
        void getpctype()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["SystemType"].ToString();
                }
                moc = null;
                mc = null;
                lb4pctype.Text = st;
            }
            catch
            {
                lb4pctype.Text = "无法获取";
            }
        }//显示PC类型
        void getpm()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["TotalPhysicalMemory"].ToString();
                }
                moc = null;
                mc = null;
                double realpm = double.Parse(st) / 1024 / 1024 / 1024;
                int maypm = (int)Math.Round(realpm, MidpointRounding.AwayFromZero);
                lb4pm.Text = maypm.ToString("0.00") + "GB";
                lb4rm.Text = realpm.ToString("0.00") + "GB";
            }
            catch
            {
                lb4rm.Text = "无法获取";
                lb4pm.Text = "无法获取";
            }
        }//显示物理内存
        void getpcname()
        {
            try
            {
               lb4pcname.Text = System.Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                lb4pcname.Text = "无法获取";
            }
        }//显示PC名称
        void getbiosid()
        {
            try
            {
                //获取BIOS id
                ManagementClass mc = new ManagementClass("Win32_BIOS");
                ManagementObjectCollection moc = mc.GetInstances();
                string strID = null;
                foreach (ManagementObject mo in moc)
                {
                    strID = mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
                moc = null;
                mc = null;
                lb4biosid.Text = strID;
                if (lb4biosid.Text=="Default string")
                {
                    lb4biosid.Text = "无法获取";
                }
            }
            catch
            {
                lb4biosid.Text = "无法获取";
            }
        }//显示BIOS ID
        void showtime()
        {
            DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            Year = currentTime.Year;
            Month = currentTime.Month;
            Day = currentTime.Day;
            Hour = currentTime.Hour;
            Minute = currentTime.Minute;
            Second = currentTime.Second;
            if (Hour < 10)
            {
                str4h = "0" + Hour.ToString();
            }
            else
            {
                str4h = Hour.ToString();
            }
            if (Minute < 10)
            {
                str4m = "0" + Minute.ToString();
            }
            else
            {
                str4m = Minute.ToString();
            }
            if (Second < 10)
            {
                str4s = "0" + Second.ToString();
            }
            else
            {
                str4s = Second.ToString();
            }
        }//显示时间
        void gettime()
        {
            showtime();
            lb4showtime.Text = DateTime.Now.ToLongDateString() + " " + str4h + ":" + str4m + ":" + str4s;
        }//获取格式化的时间
        void gettimestamp()
        {
            showtime();
            timestamp = str4h + ":" + str4m + ":" + str4s;
        }//获取时间戳
        void getnetdeviceinfo()//获取适配器信息
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();//获取本地计算机上网络接口的对象
            txt4netapi.Text = "适配器总数：" + adapters.Length+ "\r\n===============================";
            foreach (NetworkInterface adapter in adapters)
            {
                txt4netapi.Text = txt4netapi.Text + "\r\n描述：" + adapter.Description;
                txt4netapi.Text = txt4netapi.Text + "\r\n标识符：" + adapter.Id;
                txt4netapi.Text = txt4netapi.Text + "\r\n名称：" + adapter.Name;
                txt4netapi.Text = txt4netapi.Text + "\r\n类型：" + adapter.NetworkInterfaceType;
                txt4netapi.Text = txt4netapi.Text + "\r\n速度：" + adapter.Speed * 0.001 * 0.001 + "M";
                txt4netapi.Text = txt4netapi.Text + "\r\n操作状态：" + adapter.OperationalStatus;
                //txt4netapi.Text = txt4netapi.Text + "\r\nMAC 地址：" + adapter.GetPhysicalAddress();

                // 格式化显示MAC地址                
                PhysicalAddress pa = adapter.GetPhysicalAddress();//获取适配器的媒体访问（MAC）地址
                byte[] bytes = pa.GetAddressBytes();//返回当前实例的地址
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("X2"));//以十六进制格式化
                    if (i != bytes.Length - 1)
                    {
                        sb.Append("-");
                    }
                }
                txt4netapi.Text = txt4netapi.Text + "\r\nMAC 地址：" + sb;
                txt4netapi.Text = txt4netapi.Text + "\r\n===============================";
            }
        }
        void getmyhosts()
        {
            try
            {
                FileStream fsrd = File.OpenRead("C:\\Windows\\System32\\drivers\\etc\\hosts");
                StreamReader sr = new StreamReader(fsrd, System.Text.Encoding.Default);
                string strrd = sr.ReadToEnd();
                sr.Close();
                fsrd.Close();
                txt4myhosts.Text = strrd;
            }
            catch
            {
                txt4myhosts.Text = "未发现hosts文件，请检查您的系统是否完整！";
                gettimestamp();
                lb4status.Text = "//未发现hosts文件，请检查您的系统是否完整！ "+timestamp;
            }
        }//获取hosts
        void getnetinfo()//获取网络信息
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();//获取本地计算机上网络接口的对象
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Description.Contains("TAP")|| adapter.Description.Contains("Virtual") || adapter.Description.Contains("VPN") || adapter.Description.Contains("Bluetooth") || adapter.Description.Contains("Loopback"))
                {
                    //过滤其他适配器
                }
                else
                {
                    if (adapter.OperationalStatus.ToString() == "Up")
                    {
                        IPInterfaceProperties ip = adapter.GetIPProperties();     //IP配置信息
                        UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                        foreach (UnicastIPAddressInformation ipadd in ipCollection)
                        {
                            //InterNetwork    IPV4地址      InterNetworkV6        IPV6地址
                            //Max            MAX 位址
                            if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                //判断是否为ipv4
                                ipv4addr = ipadd.Address.ToString();//获取ip
                                ipv4mask = ipadd.IPv4Mask.ToString();//子网掩码
                            }
                        }
                        if (ip.UnicastAddresses.Count > 0)
                        {
                            //ipv4addr = ip.UnicastAddresses[0].Address.ToString();   //IP地址
                            //ipv4mask = ip.UnicastAddresses[0].IPv4Mask.ToString();  //子网掩码
                        }
                        if (ip.GatewayAddresses.Count > 0)
                        {
                            ipv4gate = ip.GatewayAddresses[0].Address.ToString();   //默认网关
                        }
                        if (ip.DnsAddresses.Count > 0)
                        {
                            ipv4dns1 = ip.DnsAddresses[0].ToString();       //首选DNS服务器地址
                            if (ip.DnsAddresses.Count > 1)
                            {
                                ipv4dns2 = ip.DnsAddresses[1].ToString();  //备用DNS服务器地址
                            }
                            else
                            {
                                ipv4dns2 = "自动获取";
                            }
                        }
                        else
                        {
                            ipv4dns1 = "自动获取";
                            ipv4dns2 = "自动获取";
                        }
                        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
                        IPGlobalStatistics ipstat = properties.GetIPv4GlobalStatistics();
                        string ReceivedPackets = ipstat.ReceivedPackets.ToString();//接收数据包
                        string ReceivedPacketsForwarded = ipstat.ReceivedPacketsForwarded.ToString();//转发数据包
                        string ReceivedPacketsDelivered = ipstat.ReceivedPacketsDelivered.ToString();//传送数据包
                        string ReceivedPacketsDiscarded = ipstat.ReceivedPacketsDiscarded.ToString();//丢弃数据包

                        lb4netdevice.Text = adapter.Name;
                        lb4ipv4.Text = ipv4addr;
                        lb4mask.Text = ipv4mask;
                        lb4gate.Text = ipv4gate;
                        lb4dns1.Text = ipv4dns1;
                        lb4dns2.Text = ipv4dns2;
                        lb4datarec.Text = ReceivedPackets;
                        lb4datasend.Text = ReceivedPacketsDelivered;
                        lb4dataudp.Text = ReceivedPacketsForwarded;
                        lb4datalost.Text = ReceivedPacketsDiscarded;
                    }
                }
            }
        }
        void getsysver()//获取系统版本
        {
            str4ver = Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor;
            switch (str4ver)
            {
                case "10.0":
                    vertxt = "Win10";
                    break;
                case "6.3":
                    vertxt = "Win8.1";
                    break;
                case "6.2":
                    vertxt = "Win8";
                    break;
                case "6.1":
                    vertxt = "Win7";
                    break;
                case "6":
                    vertxt = "WinVista";
                    break;
                case "5.2":
                    vertxt = "WinXP x64";
                    break;
                case "5.1":
                    vertxt = "WinXP";
                    break;
                case "5":
                    vertxt = "Win2000";
                    break;
            }
            lb4sysver.Text = vertxt;
            bool is64Os = Environment.Is64BitOperatingSystem;
            if (is64Os == true)
            {
                lb4sysbit.Text = "64位 OS";
            }
            else
            {
                lb4sysbit.Text = "32位 OS";
            }
            lb4devicename.Text = Environment.MachineName;
            lb4username.Text = Environment.UserName;
        }
        void getnetcardinfo()//获取网卡信息
        {
            getlaninfo();
            getwlaninfo();
        }
        void getlaninfo()//获取有线网信息
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();//获取本地计算机上网络接口的对象
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Description.Contains("TAP") || adapter.Description.Contains("Virtual") || adapter.Description.Contains("VPN") || adapter.Description.Contains("Bluetooth") || adapter.Description.Contains("Loopback"))
                {
                    //过滤其他适配器
                }
                else
                {
                    if (adapter.NetworkInterfaceType.ToString().Contains("Ethernet"))
                    {
                        ip1dvc = adapter.Description;
                        PhysicalAddress pa = adapter.GetPhysicalAddress();// 格式化显示MAC地址      
                        byte[] bytes = pa.GetAddressBytes();//返回当前实例的地址
                        StringBuilder macaddr = new StringBuilder();
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            macaddr.Append(bytes[i].ToString("X2"));//以十六进制格式化
                            if (i != bytes.Length - 1)
                            {
                                macaddr.Append("-");
                            }
                        }
                        lb4dvc1.Text = ip1dvc;
                        lb4mac1.Text = macaddr.ToString();
                        lb4spd1.Text = adapter.Speed * 0.001 * 0.001 + "Mbps";
                        if (adapter.Speed.ToString()=="-1")
                        {
                            lb4spd1.Text = "该适配器未启用";
                        }
                    }
                }
            }
        }
        void getwlaninfo()//获取无线网信息
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();//获取本地计算机上网络接口的对象
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Description.Contains("TAP") || adapter.Description.Contains("Virtual") || adapter.Description.Contains("VPN") || adapter.Description.Contains("Bluetooth") || adapter.Description.Contains("Loopback"))
                {
                    //过滤其他适配器
                }
                else
                {
                    if (adapter.NetworkInterfaceType.ToString().Contains("Wireless"))
                    {
                        ip2dvc = adapter.Description;
                        PhysicalAddress pa = adapter.GetPhysicalAddress();// 格式化显示MAC地址      
                        byte[] bytes = pa.GetAddressBytes();//返回当前实例的地址
                        StringBuilder macaddr = new StringBuilder();
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            macaddr.Append(bytes[i].ToString("X2"));//以十六进制格式化
                            if (i != bytes.Length - 1)
                            {
                                macaddr.Append("-");
                            }
                        }
                        lb4dvc2.Text = ip2dvc;
                        lb4mac2.Text = macaddr.ToString();
                        lb4spd2.Text = adapter.Speed * 0.001 * 0.001 + "Mbps";
                        if (adapter.Speed.ToString() == "-1")
                        {
                            lb4spd2.Text = "该适配器未启用";
                        }
                    }
                }
            }
        }
        void clrdskinfo()//清空磁盘使用情况
        {
            pic4dsk1.Visible = false;
            lb4dsk1.Visible = false;
            lb4usg1.Visible = false;
            lb4dsktol1.Visible = false;
            lb4dskval1.Visible = false;
            pic4dsk2.Visible = false;
            lb4dsk2.Visible = false;
            lb4usg2.Visible = false;
            lb4dsktol2.Visible = false;
            lb4dskval2.Visible = false;
            pic4dsk3.Visible = false;
            lb4dsk3.Visible = false;
            lb4usg3.Visible = false;
            lb4dsktol3.Visible = false;
            lb4dskval3.Visible = false;
            pic4dsk4.Visible = false;
            lb4dsk4.Visible = false;
            lb4usg4.Visible = false;
            lb4dsktol4.Visible = false;
            lb4dskval4.Visible = false;
            pic4dsk5.Visible = false;
            lb4dsk5.Visible = false;
            lb4usg5.Visible = false;
            lb4dsktol5.Visible = false;
            lb4dskval5.Visible = false;
            pic4dsk6.Visible = false;
            lb4dsk6.Visible = false;
            lb4usg6.Visible = false;
            lb4dsktol6.Visible = false;
            lb4dskval6.Visible = false;
        }
        void getlocaldskinfo()//获取磁盘信息
        {
            ManagementClass diskClass = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection disks = diskClass.GetInstances();
            int dsknum = 1;
            if(ptnum != disks.Count)
            {
                ptnum = disks.Count;
                clrdskinfo();
            }
            foreach (ManagementObject disk in disks)
            {
                try
                {
                    void dskarr()
                    {
                        var pic4dsk = Controls.Find("pic4dsk" + dsknum.ToString(), true)[0] as PictureBox;
                        pic4dsk.ImageLocation = @"img\hddbd.png";
                        if (disk["Description"].ToString().Contains("移动"))
                        {
                            pic4dsk.ImageLocation = @"img\usb.png";
                        }
                        var lb4dsk = Controls.Find("lb4dsk" + dsknum.ToString(), true)[0] as Label;
                        var lb4dskval = Controls.Find("lb4dskval" + dsknum.ToString(), true)[0] as Label;
                        var lb4usg = Controls.Find("lb4usg" + dsknum.ToString(), true)[0] as Label;
                        var lb4dsktol = Controls.Find("lb4dsktol" + dsknum.ToString(), true)[0] as Label;
                        if(disk["VolumeName"].ToString()==null|| disk["VolumeName"].ToString() == "")
                        {
                            lb4dsk.Text = "本地磁盘（" + disk["Name"].ToString() + "）";
                        }
                        else
                        {
                            lb4dsk.Text = disk["VolumeName"].ToString() + "（" + disk["Name"].ToString() + "）";
                        }
                        lb4dskval.Width = (int)(usage * 160);
                        lb4dskval.Text = ((int)(usage * 100)).ToString() + "%";
                        lb4usg.Text = "已用" + usedSpace.ToString() + "GB；剩余" + freeSpace.ToString() + "GB";
                        lb4dsk.Visible = true;
                        lb4dskval.Visible = true;
                        lb4dsktol.Visible = true;
                        lb4usg.Visible = true;
                        pic4dsk.Visible = true;
                        if (usage > dskusgalert)
                        {
                            lb4dskval.BackColor = Color.DimGray;
                            lb4dskval.Text += " 危险";
                        }
                        else
                        {
                            lb4dskval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(87)))), ((int)(((byte)(78)))));
                            lb4dskval.Text = ((int)(usage * 100)).ToString() + "%";
                        }
                    }
                    // 磁盘名称
                    //txt4test.Text += "\r\n磁盘名称：" + disk["VolumeName"].ToString() + "（" + disk["Name"].ToString() + "）";
                    // 磁盘描述
                    //txt4test.Text += "\r\n磁盘描述：" + disk["Description"].ToString();
                    // 磁盘总容量，可用空间，已用空间
                    if (System.Convert.ToInt64(disk["Size"]) > 0)
                    {
                        totalSpace = System.Convert.ToInt64(disk["Size"]) / GB;
                        freeSpace = System.Convert.ToInt64(disk["FreeSpace"]) / GB;
                        usedSpace = totalSpace - freeSpace;
                        usage = (double)usedSpace / (double)totalSpace;
                        //txt4test.Text += "\r\n总空间：" + totalSpace.ToString() + "GB";
                        //txt4test.Text += "\r\n已用：" + usedSpace.ToString() + "GB";
                        //txt4test.Text += "\r\n空闲：" + freeSpace.ToString() + "GB";
                        //txt4test.Text += "\r\n使用率：" + (usage.ToString("0.00")).Substring(2) + "%";
                    }
                    //dskarr();
                    switch (dsknum)
                    {
                        case 1:
                            dskarr();

                            break;
                        case 2:
                            dskarr();
                            break;
                        case 3:
                            dskarr();
                            break;
                        case 4:
                            dskarr();
                            break;
                        case 5:
                            dskarr();
                            break;
                        case 6:
                            dskarr();
                            break;
                    }
                        dsknum += 1;
                }
                catch
                {
                    txt4test.Text += "\r\n无法获取该磁盘信息";
                }
            }
            if (ptnum >= 5)
            {
                hr4dsk2.Visible = true;
                hr4dsk1.Visible = true;
            }
            else
            {
                if (ptnum >= 3)
                {
                    hr4dsk1.Visible = true;
                }
            }
        }
        void test()//测试
        {
            txt4test.Text = "代码测试监控板\r\n====================";
            txt4test.Text += "\r\n工作域：" + Environment.UserDomainName;
            txt4test.Text += "\r\n系统目录：" + Environment.SystemDirectory;

        }
        //============头部声明==========//
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            mainload();
            showmain();
            gettime();
            gethardinfo();
            test();
            getnetdeviceinfo();
            getnetinfo();
            getsysver();
            getnetcardinfo();
            getlocaldskinfo();
            getvername();
            getvername();
            sysispro();
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)//切换标签页事件
        {
            timer4net.Enabled = false;
            timer4dskchk.Enabled = false;
            switch (this.tabControl1.SelectedIndex)
            {
                case 0:
                    //主页
                    timer4dskchk.Enabled = true;
                    break;
                case 1:
                    //硬件配置
                    break;
                case 2:
                    //网络状态
                    timer4net.Enabled = true;
                    break;
                case 3:
                    //hosts编辑器
                    getmyhosts();
                    break;
            }
        }
        private void timer4showtime_Tick(object sender, EventArgs e)
        {
            gettime();
        }
        private void btn4savehosts_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream fscr = File.Create("C:\\Windows\\System32\\drivers\\etc\\hosts");
                StreamWriter sw = new StreamWriter(fscr, System.Text.Encoding.Default);
                sw.Write(txt4myhosts.Text);
                sw.Close();
                fscr.Close();
                gettimestamp();
                lb4status.Text = "//hosts保存成功！ "+timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//hosts保存失败，请检查您的文件写入权限！ " + timestamp;
            }
        }
        private void btn4resethosts_Click(object sender, EventArgs e)
        {
            getmyhosts();
            gettimestamp();
            lb4status.Text = "//hosts还原成功！ " + timestamp;
        }
        private void timer4net_Tick(object sender, EventArgs e)
        {
            getnetinfo();
        }
        private void btn4flsnetdevice_Click(object sender, EventArgs e)
        {
            getnetdeviceinfo();
            gettimestamp();
            lb4status.Text = "//适配器列表刷新成功！ " + timestamp;
        }
        private void timer4dskchk_Tick(object sender, EventArgs e)
        {
            getlocaldskinfo();
        }
        private void btn4flsdns_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\flushdns.bat");
                gettimestamp();
                lb4status.Text = "//DNS缓存刷新成功！ " + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//DNS缓存刷新失败！" + timestamp;
            }
        }
        private void btn4runcmd_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("C:\\Windows\\System32\\cmd.exe");
                gettimestamp();
                lb4status.Text = "//CMD打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//CMD打开失败！" + timestamp;
            }
        }
        private void btn4runservices_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\services.bat");
                gettimestamp();
                lb4status.Text = "//服务打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//服务打开失败！" + timestamp;
            }
        }
        private void btn4rungpedit_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\gpedit.bat");
                gettimestamp();
                lb4status.Text = "//组策略打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//组策略打开失败！" + timestamp;
            }
        }
        private void btn4runreg_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\regedit.bat");
                gettimestamp();
                lb4status.Text = "//注册表打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//注册表打开失败！" + timestamp;
            }
        }
        private void btn4mstsc_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\mstsc.bat");
                gettimestamp();
                lb4status.Text = "//远程控制打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//远程控制打开失败！" + timestamp;
            }
        }
        private void btn4vqq_Click(object sender, EventArgs e)
        {
            Process.Start(@"hosts\vqq.txt");
        }
        private void btn4bhosts_Click(object sender, EventArgs e)
        {
            Process.Start("https://hosts.nfz.moe/127.0.0.1/basic/hosts");
        }
        private void btn4fhosts_Click(object sender, EventArgs e)
        {
            Process.Start("https://hosts.nfz.moe/127.0.0.1/full/hosts");
        }
        private void btn4devmgmt_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\devmgmt.bat");
                gettimestamp();
                lb4status.Text = "//设备管理器打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//设备管理器打开失败！" + timestamp;
            }
        }
        private void btn4diskmgmt_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\diskmgmt.bat");
                gettimestamp();
                lb4status.Text = "//磁盘管理打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//磁盘管理打开失败！" + timestamp;
            }
        }
        private void btn4control_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\control.bat");
                gettimestamp();
                lb4status.Text = "//控制面板打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//控制面板打开失败！" + timestamp;
            }
        }
        private void btn4appwiz_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\appwiz.bat");
                gettimestamp();
                lb4status.Text = "//程序和功能打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//程序和功能打开失败！" + timestamp;
            }
        }
        private void btn4taskmgr_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\taskmgr.bat");
                gettimestamp();
                lb4status.Text = "//任务管理器打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//任务管理器打开失败！" + timestamp;
            }
        }
        private void btn4powercfg_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\powercfg.bat");
                gettimestamp();
                lb4status.Text = "//电源选项打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//电源选项打开失败！" + timestamp;
            }
        }
        private void btn4explorer_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\explorer.bat");
                gettimestamp();
                lb4status.Text = "//文件系统打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//文件系统打开失败！" + timestamp;
            }
        }
        private void btn4compmgmt_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"cmd\compmgmt.bat");
                gettimestamp();
                lb4status.Text = "//计算机管理打开成功！" + timestamp;
            }
            catch
            {
                gettimestamp();
                lb4status.Text = "//计算机管理打开失败！" + timestamp;
            }
        }
    }
}
