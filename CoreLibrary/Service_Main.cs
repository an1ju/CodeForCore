using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TcpCSFramework_To_Core;

namespace CoreLibrary
{
    public class Service_Main 
    {
        /// <summary>
        /// 硬核心数据列表。
        /// 这个服务的主要操作区全在这里了。
        /// 2019年12月5日15:07:36
        /// </summary>
        private List<ManagerMessage> YingHeXinList = null;

        /// <summary>
        /// TCP通讯硬核部分。
        /// 这是与simlink通讯的硬核了。
        /// 2019年12月5日15:27:21
        /// </summary>
        private TcpSvr svr = null;
        /// <summary>
        /// TCP监听端口
        /// </summary>
        private int TCPport = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["tcpPort"]);
        /// <summary>
        /// TCP连接最大数
        /// </summary>
        private int TCPConnectMax = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["tcpConnectMax"]);

        /// <summary>
        /// 逻辑流程计时器，试验创建后，会进行拷贝到新文件夹的操作
        /// </summary>
        private Timer timer_LiuCheng;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Service_Main()
        {
            YingHeXinList = new List<ManagerMessage>();

            svr = new TcpSvr((ushort)TCPport, (ushort)TCPConnectMax, new Coder(Coder.EncodingMothord.UTF8));


            svr.Resovlver = new DatagramResolver("0");//这里的设置没啥用，因为我不准备使用它的解析器。
            //处理客户端连接数已满事件
            svr.ServerFull += new NetEvent(ServerFull);
            //处理新客户端连接事件
            svr.ClientConn += new NetEvent(ClientConn);
            //处理客户端关闭事件
            svr.ClientClose += new NetEvent(ClientClose);
            //处理接收到数据事件
            svr.RecvData += new NetEvent(RecvData);

            svr.Start();
            Console.WriteLine("Server is listen...{0}", svr.ServerSocket.LocalEndPoint.ToString());

            timer_LiuCheng = new Timer(2000);
            timer_LiuCheng.Elapsed += Timer_LiuCheng_Elapsed;
            timer_LiuCheng.Start();
        }



        /// <summary>
        /// 理想丰满
        /// 这里没被执行
        /// 2019年12月5日16:12:02
        /// </summary>
        ~Service_Main()
        {
            svr.Stop();
            for (int i = 0; i < YingHeXinList.Count(); i++)
            {
                if (YingHeXinList[i].PID != 0)
                {
                    CMDClass.KillProcess(YingHeXinList[i].PID);
                }

            }
            YingHeXinList.Clear();
            timer_LiuCheng.Stop();
        }



        public string SayHello()
        {
            return string.Format("Hello Omega: Now {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
        }

        public ManagerMessage GetMessageByID(int id)
        {
            ManagerMessage r = null;
            for (int i = 0; i < YingHeXinList.Count; i++)
            {
                if (YingHeXinList[i].ID == id)
                {
                    r = YingHeXinList[i];
                    YingHeXinList[i].Web_Heart++;//web心跳
                    break;
                }
            }
            return r;
        }
        public List<ManagerMessage> GetAllMessage()
        {
            return YingHeXinList;
        }

        public string SetSendDataStr(string message, int id)
        {
            string r = "无效操作";

            //首先验证数据是否符合规范：解析数量、查看是否都是数字，id是否存在
            var rows = YingHeXinList.Where(o => o.ID == id).ToList();
            if (rows.Count > 0)
            {
                //查到有ID
                string[] sArray = message.Split(',');//这是当前想要发的数据

                string Sstrs = PublicClassRule.GetValues(rows[0].HR_Make_ID.ToString(), "tcpSendBytes");//配置中要发送数据类型[这是正确的数据]
                string[] sRightArray = Sstrs.Split(',');

                if (sArray.Length == sRightArray.Length)
                {
                    //与配置项相符，再验证数据是否与要求类型相符
                    try
                    {
                        byte[] sendByteDataList = PublicClassRule.MakeDataToBytes(rows[0].HR_Make_ID, message);

                        if (sendByteDataList == null)
                        {
                            //无效
                            r = "数据无效";
                        }
                        else
                        {
                            //可以记录，发送
                            rows[0].SendData = message;
                            rows[0].Web_Heart++;//web心跳
                            r = "发送数据变更成功";
                        }
                    }
                    catch (Exception)
                    {

                        r = "数据转换不正确，可能有的数据超过类型限制。";
                    }

                }
                else
                {
                    r = "Message字符结构不正确，请正确输入正确数量的数据。";
                }
            }
            else
            {
                r = "ID无效";
            }


            return r;
        }

        public string SetStatus(ShiYanStatus t, int id)
        {
            string r = "无效操作";
            for (int i = 0; i < YingHeXinList.Count; i++)
            {
                if (YingHeXinList[i].ID == id)
                {
                    YingHeXinList[i].Status = t;
                    r = "状态变更成功";
                    break;
                }
            }
            return r;
        }



        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="motherPathName"></param>
        /// <returns></returns>
        public string DeleteFolder(string motherPathName)
        {
            string r = "无效操作";
            try
            {
                string iniPath = System.AppDomain.CurrentDomain.BaseDirectory + "/HR_Models.ini";
                Ini ini = new Ini(iniPath);
                string tempPath = ini.ReadValue("root", "UserRootPath") + PublicClassRule.JieXiMuLu(motherPathName);//解析目录
                //已经有路径了，再查一下该文件夹是否正在使用中
                bool isUsing = false;
                for (int i = 0; i < YingHeXinList.Count; i++)
                {
                    if (YingHeXinList[i].CopyModel_To_TargetPath == tempPath)
                    {
                        isUsing = true;
                        break;
                    }
                }
                if (isUsing)
                {
                    r = "该模型正在被使用，请停止试验后再试。";
                }
                else
                {
                    r = PublicClassTools.Delete(tempPath);
                }

            }
            catch (Exception)
            {

                r = "文件夹名称解析异常";
            }

            return r;
        }

        /// <summary>
        /// 此方法提供单个试验配置的协议，提供给网站使用，便于动态创建页面
        /// </summary>
        /// <param name="hrID"></param>
        /// <param name="xieyi"></param>
        /// <returns></returns>
        public string GetTag_By_HRID_And_XIEYI(int hrID, ShiYanXieYi xieyi)
        {
            return PublicClassRule.GetValues(hrID.ToString(), xieyi.ToString());//读取配置文件
        }

        //// <summary>
        /// 创建新的试验
        /// 这里有几个关键步骤：创建管理连接、复制文件、启动文件、实现通讯
        /// </summary>
        /// <param name="uid">用户编号</param>
        /// <param name="type">试验类型</param>
        /// <param name="hrID">华人设定的模板模型编号</param>
        /// <param name="look_uid">如果看其他人的试验，这里写别人的uid，默认不写为0</param>
        /// <param name="motherPathName">被查看的用户路径名称  eg.55_20191206144001New</param>
        /// <returns></returns>
        public ManagerMessage CreateNew_ShiYan(int uid, ShiYanCreateType type, int hrID, int look_uid = 0, string motherPathName = "")
        {
            //
            ManagerMessage r = new ManagerMessage();
            r.ID = PublicClassRule.MakeMessageID(YingHeXinList, TCPConnectMax);//运算生成
            r.UID = uid;
            r.CreateType = type;
            r.HR_Make_ID = hrID;
            r.Status = ShiYanStatus.New;
            r.Look_Other_UID = look_uid;
            r.SourceModelPath = motherPathName;

            #region 路径处理
            try
            {
                PublicClassRule.Self_Make_Path(ref r, motherPathName);//根据创建类型，分配相关路径
            }
            catch (Exception e)
            {

                Read_Exception(ref r, e);

            }
            #endregion



            YingHeXinList.Add(r);
            return r;
        }

        private void Read_Exception(ref ManagerMessage r, Exception e)
        {
            if (e.Message == "无此试验模型配置")
            {
                r.Status = ShiYanStatus.Error_No_THIS_HR_ID;
            }
            else if (e.Message == "母文件夹无效")
            {
                r.Status = ShiYanStatus.Error_No_MotherPath;
            }
            else if (e.Message == "没找到原模型路径")
            {
                //没找到原模型路径
                r.Status = ShiYanStatus.Error_NoFind_SourcePath;
            }
            else if (e.Message == "没找到自己的试验模型数据路径")
            {
                //没找到自己的试验模型数据路径
                r.Status = ShiYanStatus.Error_NoFind_FuPanPath;
            }
            else if (e.Message == "复盘试验时：UID与Look_Other_UID不一致")
            {
                r.Status = ShiYanStatus.Error_UID_NOT_Equal_LookOtherUID;
            }
            else if (e.Message == "【motherPathName】文件夹名称无效")
            {
                //文件夹名称不合格
                r.Status = ShiYanStatus.Error_Write_MotherPathName;
            }
        }


        /// <summary>
        /// 逻辑流程：这里控制我们整个试验系统的状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_LiuCheng_Elapsed(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < YingHeXinList.Count; i++)
            {
                bool ThisTimeChanged = false;//本次循环有变化。有变化时跳出循环，等待下一次循环再做处理。
                //因为有可能使用remove操作，这里去掉一行，后续可能导致顺序异常

                switch (YingHeXinList[i].Status)
                {
                    case ShiYanStatus.New:
                        {
                            #region 是新创建的，开始拷贝
                            YingHeXinList[i].Status = ShiYanStatus.FilesCopying;
                            if (YingHeXinList[i].CreateType != ShiYanCreateType.ReView)
                            {
                                _ = Copy(YingHeXinList[i]);//【使用丢弃】不知道啥意思，反正这里我用异步操作了。2019年12月6日14:01:41
                            }
                            else
                            {
                                //看自己的试验复盘，不需要复制。直接跳到复制完成。
                                YingHeXinList[i].Status = ShiYanStatus.FilesCopyCompleted;
                            }


                            #endregion
                        }
                        break;
                    case ShiYanStatus.FilesCopying:
                        {
                            //这里不管，等待拷贝完成
                        }
                        break;
                    case ShiYanStatus.FilesCopyCompleted:
                        {
                            #region 已经拷贝好了，开始执行运行命令
                            YingHeXinList[i].Status = ShiYanStatus.CmdCommand;

                            // TODO:这里将来要做重要操作，启动MATLAB的仿真程序，并且把仿真程序的ID记录下来。
                            _ = MakeSimlinkRun(YingHeXinList[i]);

                            #endregion
                        }
                        break;
                    case ShiYanStatus.CmdCommand:
                        {
                            //无需操作，等待通讯开始                            
                        }
                        break;
                    case ShiYanStatus.Running:
                        {
                            #region 正常运行的试验要查看通讯是否超时。包含simlink通讯超时和 web端通讯超时
                            PublicClassRule.CheckTimeOut(YingHeXinList[i]);

                            #endregion
                        }
                        break;
                    case ShiYanStatus.Error:
                    case ShiYanStatus.Error_NoFind_FuPanPath:
                    case ShiYanStatus.Error_NoFind_SourcePath:
                    case ShiYanStatus.Error_No_MotherPath:
                    case ShiYanStatus.Error_No_THIS_HR_ID:
                    case ShiYanStatus.Error_UID_NOT_Equal_LookOtherUID:
                    case ShiYanStatus.Error_Write_MotherPathName:
                        {
                            #region 错误状态的试验自动结束
                            YingHeXinList[i].Status = ShiYanStatus.End;
                            #endregion
                        }
                        break;
                    case ShiYanStatus.End:
                        {
                            #region MyRegion
                            CMDClass.KillProcess(YingHeXinList[i].PID);//此处注意，可能要改为sleep或多线程。
                            YingHeXinList.RemoveAt(i);
                            ThisTimeChanged = true;
                            #endregion
                        }
                        break;
                    default:
                        break;
                }

                if (ThisTimeChanged)
                {
                    break;
                }
            }
        }


        #region 异步方法集合
        /// <summary>
        /// 异步操作文件夹复制
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private async Task Copy(ManagerMessage row)
        {
            await Task.Run(() => {
                PublicClassTools.Copy(row.SourceModelPath, row.CopyModel_To_TargetPath);
                row.Status = ShiYanStatus.FilesCopyCompleted;
            });
        }
        /// <summary>
        /// 启动simlink的过程
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private async Task MakeSimlinkRun(ManagerMessage row)
        {
            await Task.Run(() => {
                //要运行的路径是row.CopyModel_To_TargetPath
                //文件夹里面的命令需要复制过来、你看看怎么能启动起来

                #region 将来不会用这里的代码 2019年12月9日09:40:17

                //System.Diagnostics.Process cmd_MakeRun = new System.Diagnostics.Process();
                //cmd_MakeRun.StartInfo.FileName = @"D:\uuu\UID44\44_20191209114531New\MGtcp\hello.bat";
                ////cmd_MakeRun.StartInfo.UserName = "OmegaTest";
                //cmd_MakeRun.StartInfo.UseShellExecute = false;

                //cmd_MakeRun.Start();



                //row.PID = CMDClass.FindPID("MATLAB R2018a", @"C:\Program Files\MATLAB\R2018a\bin\win64\MATLAB.exe");

                ////row.PID = cmd_MakeRun.Id;//把程序启动起来的PID记录下来

                ////cmd_MakeRun.Kill();
                //CMDClass.KillProcess("QQ", cmd_MakeRun.StartInfo.FileName);//两种方式都能把！QQ干掉！！
                ////CMDClass.KillProcess(row.PID);
                #endregion


                string arguments = PublicClassRule.GetValues(row.HR_Make_ID.ToString(), "arguments");//原始参数

                arguments = PublicClassRule.MakeArguments(arguments, row.HR_Make_ID, row.CopyModel_To_TargetPath);//结构化之后的最终使用参数

                //修改M文件中的  input 的ID  本段属于文件操作，修改.m文件内容
                string m_path = string.Format("{0}{1}", row.CopyModel_To_TargetPath, PublicClassRule.GetValues(row.HR_Make_ID.ToString(), "mFileName"));
                PublicClassRule.Edit_M_File(m_path, row.HR_Make_ID, row.ID);

                MakeRun(arguments, ref row);
                //等待程序和服务TCP通讯，！！！注意，此时，这个进程不会停止，不会自动关闭。只能等待kill   PID  2019年12月9日14:40:17


            });
        }




        /// <summary>
        /// 启动simlink模型
        /// </summary>
        /// <param name="arguments">启动参数，这是启动MATLAB的名称参数，在配置文件的属性中可以看到DEMO</param>
        /// <param name="row">PID记录在这里</param>
        private void MakeRun(string arguments, ref ManagerMessage row)
        {
            try
            {
                //string arguments = @"-nosplash -nodesktop -sd D:\MyWorkGGTest\one\MGtcp -r test";


                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(PublicClassRule.GetValues("root", "MATLAB_Process_FullPath"), arguments);

                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                System.Diagnostics.Process myprocess = new System.Diagnostics.Process();
                myprocess.StartInfo = startInfo;
                myprocess.StartInfo.UseShellExecute = false;
                myprocess.StartInfo.CreateNoWindow = true;
                myprocess.StartInfo.RedirectStandardOutput = true;

                myprocess.Start();


                row.PID = myprocess.Id;//获取PID

                myprocess.WaitForExit();//进程会一直处于等待退出状态。


            }
            catch (Exception ex)
            {


            }
        }

        #endregion

        #region TCP通讯

        private void ServerFull(object sender, NetEventArgs e)
        {
            string info = string.Format("链接数已满一个客户端链接被拒:\t{0} is refused",
             e.Client.ClientSocket.RemoteEndPoint.ToString());


            //Must do it
            e.Client.Close();

            Console.WriteLine(info);
            Console.Write(">");
        }

        private void ClientConn(object sender, NetEventArgs e)
        {
            string info = string.Format("一个客户端正在链接:\t\t{0} connect server Session:{1}. Socket Handle:{2}",
             e.Client.ClientSocket.RemoteEndPoint.ToString(),
             e.Client.ID, e.Client.ClientSocket.Handle);


            //TODO:这里可以判断客户机IP，如果想要限制指定IP连接我们，就在这里做限制。 eg.被限制的直接踢掉   e.Client.Close();
        }

        private void ClientClose(object sender, NetEventArgs e)
        {
            string info;
            if (e.Client.TypeOfExit == TcpCSFramework_To_Core.Session.ExitType.ExceptionExit)
            {
                info = string.Format("一个客户端异常退出链接:\t{0} Exception Closed.",
                 e.Client.ClientSocket.RemoteEndPoint.ToString());

            }
            else
            {
                info = string.Format("一个客户端正常退出链接:\t{0} Normal Closed.",
                    e.Client.ClientSocket.RemoteEndPoint.ToString());

            }

            if (e.Client != null)
            {
                //Services_Have.omegaRemove(e.Client, ref serivce_have);

            }

            Console.WriteLine(info);
            Console.Write(">");

        }

        private void RecvData(object sender, NetEventArgs e)
        {

            #region 接收数据应对措施

            int id = MakeDataTCP(e);//查询ID
            if (id == -1)
            {
                //不处理
                //【注意】程序中写了。如果返回值是-1说明没查到，此链接会被关闭。所以要处理不等于-1的即可
            }
            else
            {
                //处理收到的数据
                var row = YingHeXinList.Where(o => o.ID == id).ToList();
                TimeSpan ts = DateTime.Now - row[0].TCP_OK_Start;
                row[0].TCP_GoOn_Time = string.Format("{0}天{1}小时{2}分{3}秒 {4}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

                //int length = PublicClassTools.Read_Setting_Length(row[0].HR_Make_ID, "tcpReceiveBytes");
                //根据这个长度处理数据
                List<double> receiveDataList = PublicClassRule.MakebytesToData(e.Client.RecvDataBuffer, row[0].HR_Make_ID);
                //把数据放置在对应的位置上
                string rDataStr = "";
                for (int i = 0; i < receiveDataList.Count; i++)
                {
                    rDataStr = string.Format("{0},{1}", rDataStr, receiveDataList[i]);
                }
                //循环结束，最前方都会去掉
                if (rDataStr.StartsWith(","))
                {
                    rDataStr = rDataStr.Substring(1);
                }
                row[0].ReceivedData = rDataStr;
                row[0].ReceivedTotalCount++;

                //下面该回数据了，有来不往非君子。

                //首先验证一下要发送的数据 2019年12月10日11:42:46
                int slength = PublicClassTools.Read_Setting_Length(row[0].HR_Make_ID, "tcpSendBytesNames");//协议中要发送的数据长度
                if (row[0].SendData == null)
                {
                    //如果是空，给一个初始值

                    MakeSendDataStr(row, slength);
                }
                //再验证一下，这个数据的数量是不是正确
                string[] sCount = row[0].SendData.Split(',');
                if (sCount.Length != slength)
                {
                    //长度不正确
                    MakeSendDataStr(row, slength);

                }
                else
                {
                    //长度正确
                }

                //现在有要发送的数据了。开始制作byte[]
                byte[] sendByteDataList = PublicClassRule.MakeDataToBytes(row[0].HR_Make_ID, row[0].SendData);
                if (sendByteDataList == null)
                {
                    //没数据可发
                }
                else
                {
                    //发送数据异常
                    SendData111(sendByteDataList, e.Client);
                    row[0].SendTotalCount++;
                    if (row[0].CreateType == ShiYanCreateType.New || true) //先不管了，不管怎么用，都记录吧
                    {
                        //新建的试验记录日志
                        PublicClassRule.MakeLog(row[0]);//写tcp通讯的命令和返回值日志
                    }
                    else
                    {
                        //复盘自己和别人的试验，不做日志记录
                    }

                }

            }

            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="_data"></param>
            /// <param name="sessionClient"></param>
            void SendData111(byte[] _data, Session sessionClient)
            {
                svr.Send(sessionClient, _data);

                string byteStr = "";
                for (int i = 0; i < _data.Length; i++)
                {

                    byteStr = byteStr + " " + _data[i].ToString("X2");
                }

                string info = string.Format("发送数据:\tOmegaS Send data:\t{0} \tTo:{1}.", byteStr, sessionClient);
                Console.WriteLine(info);
                Console.Write(">");
            }
            #endregion


        }
        /// <summary>
        /// 制作要发送数据的字符串
        /// </summary>
        /// <param name="row"></param>
        /// <param name="slength"></param>
        private static void MakeSendDataStr(List<ManagerMessage> row, int slength)
        {
            List<double> sendDataList = new List<double>();
            string sDataStr = "";
            for (int i = 0; i < slength; i++)
            {
                sendDataList.Add(0);//制作默认数据
                sDataStr = string.Format("{0},{1}", sDataStr, sendDataList[i]);
            }
            //循环结束，最前方都会去掉
            if (sDataStr.StartsWith(","))
            {
                sDataStr = sDataStr.Substring(1);
            }
            row[0].SendData = sDataStr;
        }

        /// <summary>
        /// 处理接收来的TCP通讯数据，获取ID
        /// </summary>
        /// <param name="e"></param>
        /// <returns>返回的ID是 管理试验的ID，注意！不是索引</returns>
        private int MakeDataTCP(NetEventArgs e)
        {
            int ID = -1;

            //默认不知道对应我们的  哪行数据ID，先查一下SESSION，看看有没有。有就可以直接用了。
            //没有进行对比ID操作，进行匹配，数据长度也能获取到，依据配置文件
            //如果还是没有，就中断此链接
            System.Net.EndPoint remote = e.Client.ClientSocket.RemoteEndPoint;
            bool canFindEndPoint = false;
            for (int i = 0; i < YingHeXinList.Count; i++)
            {
                if (string.Format("{0}:{1}", YingHeXinList[i].Remote_IP == null ? "0.0.0.0" : YingHeXinList[i].Remote_IP, YingHeXinList[i].Remote_Port) == remote.ToString())
                {
                    //找到了
                    canFindEndPoint = true;
                    ID = YingHeXinList[i].ID;
                    break;
                }
            }
            if (canFindEndPoint)
            {
                //找到了
            }
            else
            {
                //没找到
                //int id = e.Client.RecvDataBuffer[0];//通讯协议规定，第一个字节就是程序发出的ID指令。这个ID对应管理服务的0-200个可复用试验接口台。【注意，不是索引！】 
                int id = Convert.ToInt32(PublicClassRule.MakeOneData("double", 0, e.Client.RecvDataBuffer));

                bool canFindID = false;
                for (int i = 0; i < YingHeXinList.Count; i++)
                {
                    if (YingHeXinList[i].ID == id)
                    {
                        //找到了，EndPoint绑定进去
                        canFindID = true;
                        //YingHeXinList[i].RemoteEndPoint = remote;//不知道为什么，此行注释的不行，程序中可以，但到了wcf，客户端收不到。
                        YingHeXinList[i].Remote_IP = ((System.Net.IPEndPoint)remote).Address.ToString();
                        YingHeXinList[i].Remote_Port = ((System.Net.IPEndPoint)remote).Port;
                        YingHeXinList[i].TCP_OK_Start = DateTime.Now;
                        ID = YingHeXinList[i].ID;
                        YingHeXinList[i].Status = ShiYanStatus.Running;
                        YingHeXinList[i].Tcp_TimeOut_Test = new TimeOutPlay();
                        YingHeXinList[i].Web_TimeOut_Test = new TimeOutPlay();
                        break;
                    }
                }
                if (canFindID)
                {
                    //找到了，数据绑定进去，哦上面EndPoint已经写入了

                }
                else
                {
                    //没找到，关闭此链接
                    e.Client.Close();
                }
            }

            return ID;
        }

        #endregion


        #region 上传文件

        public string UpLoad_Mfile(int id, byte[] data, string fileName)
        {
            string r = "无效操作";

            //先查id是否在正在试验管理中
            var row = YingHeXinList.Where(o => o.ID == id).ToList();
            if (row.Count > 0)
            {
                //有此试验，然后保存在该试验路径下
                string path = string.Format(@"{0}\{1}", row[0].CopyModel_To_TargetPath/*,DateTime.Now.ToString("yyyyMMddHHmmss")*/, fileName);

                bool r2 = FileHelper.ByteToFile(data, path);
                if (r2)
                {
                    r = "上传成功";
                    row[0].PY_Path = path;//上传成功后，记录此路径，可能要用。
                }
                else
                {
                    r = "上传失败";
                }
            }
            else
            {
                //无效ID
                r = "无效ID";
            }


            return r;
        }
        /// <summary>
        /// 直接向文件夹中传文件
        /// </summary>
        /// <param name="motherPathName"></param>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        //public string UpLoad_Mfile(string motherPathName, byte[] data, string fileName)
        //{
        //    string r = "无效操作";

        //    string iniPath = System.AppDomain.CurrentDomain.BaseDirectory + "/HR_Models.ini";
        //    Ini ini = new Ini(iniPath);
        //    string tempPath = ini.ReadValue("root", "UserRootPath") + PublicClassRule.JieXiMuLu(motherPathName);//解析目录

        //    string path = string.Format(@"{0}\{1}", tempPath/*,DateTime.Now.ToString("yyyyMMddHHmmss")*/, fileName);
        //    bool r2 = FileHelper.ByteToFile(data, path);
        //    if (r2)
        //    {
        //        r = "上传成功";

        //    }
        //    else
        //    {
        //        r = "上传失败";
        //    }

        //    return r;
        //}



        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="motherPathName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] DownLoad_Mfile(string motherPathName, string fileName)
        {
            string r = "无效操作";
            try
            {
                string iniPath = System.AppDomain.CurrentDomain.BaseDirectory + "/HR_Models.ini";
                Ini ini = new Ini(iniPath);
                string tempPath = ini.ReadValue("root", "UserRootPath") + PublicClassRule.JieXiMuLu(motherPathName);//解析目录
                //已经有路径了，再查一下该文件夹是否正在使用中
                //拼接文件完整路径
                string fullfilename = string.Format(@"{0}{1}", tempPath, fileName);
                return FileHelper.FileToByte(fullfilename);
            }
            catch (Exception)
            {

                r = "文件夹名称解析异常";
            }

            return null;
        }

        #endregion
    }
}

