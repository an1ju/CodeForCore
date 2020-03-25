using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace CoreLibrary
{
    /// <summary>
    /// 本项目的公共类库
    /// 规则设定
    /// </summary>
    public class PublicClassRule
    {

        /// <summary>
        /// 制作消息ID
        /// 0-200 可复用，没被占用的可被复用。2019年12月6日09:36:42
        /// 从当前列表中查找未被占用的ID
        /// </summary>
        /// <returns></returns>
        public static int MakeMessageID(List<ManagerMessage> YingHeXinList, int TCPConnectMax)
        {
            int r = 0;

            for (int i = 0; i < TCPConnectMax; i++)
            {
                bool canfind_ID = false;//在最大列表数量中进行查询，默认没找到

                for (int iList_index = 0; iList_index < YingHeXinList.Count; iList_index++)
                {
                    if (i == YingHeXinList[iList_index].ID)
                    {
                        canfind_ID = true;//找到已被占用，不继续找。进行下一个。
                        continue;
                    }
                    else
                    {
                        //没找到，继续找。
                    }
                }

                if (!canfind_ID)
                {
                    //这里就成功找到没使用的ID了
                    r = i;
                    break;
                }
            }

            return r;
        }


        static string iniPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\HR_Models.ini";
        static Ini ini = new Ini(iniPath);
        /// <summary>
        /// 通过自身初始化设置，根据创建类型，分配相关路径
        /// 【注意】文件夹规则设计
        /// UID1336\\1336_20191212131356NEW   这是某个用户的新建试验   也可以用这个复盘
        /// UID1336\\1336_20191212131356From1222_20191111111213New  这是1336号用户从1222号用户拿取11月11日创建的试验
        /// </summary>
        /// <param name="self">一条消息</param>
        /// <param name="motherPathName">被查看的目录名称</param>
        public static void Self_Make_Path(ref ManagerMessage self, string motherPathName)
        {
            switch (self.CreateType)
            {
                case ShiYanCreateType.New:
                    {
                        self.SourceModelPath = ini.ReadValue(self.HR_Make_ID.ToString(), "path");//从配置文件中读取
                        if (self.SourceModelPath == "")
                        {
                            //读出来是空格，说明没找到这个配置。如果这里不报错，下面就会把我们所有的试验都复制过去给这个用户了。
                            throw new Exception("无此试验模型配置");

                        }
                        self.CopyModel_To_TargetPath = string.Format(@"UID{0}\{0}_{1}New\", self.UID, DateTime.Now.ToString("yyyyMMddHHmmss")); //文件夹copy规则
                        //补充完成：
                        self.SourceModelPath = ini.ReadValue("root", "ModelRootPath") + self.SourceModelPath;
                        self.CopyModel_To_TargetPath = ini.ReadValue("root", "UserRootPath") + self.CopyModel_To_TargetPath;


                        //只有需要复制文件时做这个判断
                        if (!System.IO.Directory.Exists(self.SourceModelPath))
                        {
                            throw new Exception("没找到原模型路径");

                        }
                    }
                    break;
                case ShiYanCreateType.ReView:
                    {
                        if (self.UID == self.Look_Other_UID)
                        {
                            //自己看自己
                            if (motherPathName == "" || motherPathName == null)
                            {
                                throw new Exception("母文件夹无效");
                            }
                            self.CopyModel_To_TargetPath = JieXiMuLu(motherPathName);//解析目录
                            //补充完成：
                            self.CopyModel_To_TargetPath = ini.ReadValue("root", "UserRootPath") + self.CopyModel_To_TargetPath;
                        }
                        else
                        {
                            //不是自己看自己
                            throw new Exception("复盘试验时：UID与Look_Other_UID不一致");
                        }

                        //不需要复制，但要看看自己的文件在不在
                        if (!System.IO.Directory.Exists(self.CopyModel_To_TargetPath))
                        {
                            throw new Exception("没找到自己的试验模型数据路径");

                        }

                    }
                    break;
                case ShiYanCreateType.LookOthers:
                    {
                        if (self.SourceModelPath == "" || self.SourceModelPath == null)
                        {
                            throw new Exception("母文件夹无效");
                        }
                        self.SourceModelPath = JieXiMuLu(motherPathName);//解析目录
                        self.CopyModel_To_TargetPath = string.Format(@"UID{0}\{0}_{1}_From_{2}\", self.UID, DateTime.Now.ToString("yyyyMMddHHmmss"), self.SourceModelPath.Split('\\')[1]);
                        //补充完成：
                        self.SourceModelPath = ini.ReadValue("root", "UserRootPath") + self.SourceModelPath;
                        self.CopyModel_To_TargetPath = ini.ReadValue("root", "UserRootPath") + self.CopyModel_To_TargetPath;

                        //只有需要复制文件时做这个判断
                        if (!System.IO.Directory.Exists(self.SourceModelPath))
                        {
                            throw new Exception("没找到原模型路径");

                        }

                    }
                    break;
                default:
                    break;
            }




        }

        /// <summary>
        /// 目录解析：作用是
        /// </summary>
        /// <param name="motherPathName">55_20191206144001New</param>
        /// <returns></returns>
        internal static string JieXiMuLu(string motherPathName)
        {
            string r = "";
            string[] temp = motherPathName.Split('_');
            if (temp.Length >= 2)
            {
                //合格的文件夹名称
                r = string.Format(@"UID{0}\{1}\", temp[0], motherPathName);
            }
            else
            {
                //文件夹名称不合理
                throw new Exception("【motherPathName】文件夹名称无效");
            }

            return r;
        }

        public static string GetValues(string mother, string child)
        {
            return ini.ReadValue(mother, child);
        }

        /// <summary>
        /// 路径替换规则，
        /// arguments=-nosplash -nodesktop -sd D:\MyWorkGGTest\zero\MGtcp -r test
        /// D:\MyWorkGGTest\ + zero\   这是原始路径
        /// 新路径可以从上层传递过来
        /// <param name="arguments">参数</param>
        /// <param name="HR_ID"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public static string MakeArguments(string arguments, int HR_ID, string newPath)
        {
            string oldP = string.Format("{0}{1}", GetValues("root", "ModelRootPath"), GetValues(HR_ID.ToString(), "path"));
            arguments = arguments.Replace(oldP, newPath);

            return arguments;
        }

        /// <summary>
        /// 编辑修改批处理文件
        /// </summary>
        /// <param name="path">m文件路径</param>
        /// <param name="HR_Make_ID">华人根试验模型编号</param>
        /// <param name="keyID">这个是重要的ID，是simlink将来要返回给我的ID</param>
        public static void Edit_M_File(string path, int HR_Make_ID, int keyID)
        {
            FileStream fs = null;
            StreamWriter writer = null;
            //string path = "D:\\test.txt";

            if (!File.Exists(path))
            {
                //fs = File.Create(path);
                Console.WriteLine("该文件不存在：{0}", path);
            }
            else
            {


                string s = File.ReadAllText(path);

                string oldS = GetValues(HR_Make_ID.ToString(), "inputIDValue");
                string newS = string.Format("input = {0};", keyID);

                s = s.Replace(oldS, newS);

                byte[] mybyte = System.Text.Encoding.UTF8.GetBytes(s);

                fs = File.Open(path, FileMode.Open);
                Console.WriteLine("文件已存在，直接打开");

                writer = new StreamWriter(fs);
                fs.Write(mybyte, 0, mybyte.Length);
                //writer.WriteLine("测试文本");
                Console.WriteLine("像文件中写入文本数据流");
                writer.Flush();
                writer.Close();
            }


        }


        #region 数据操作



        /// <summary>
        /// 数据解析：从byte[]到数据
        /// </summary>
        /// <param name="recvDataBuffer"></param>
        /// <param name="HR_Make_ID"></param>
        /// <returns></returns>
        internal static List<double> MakebytesToData(byte[] recvDataBuffer, int HR_Make_ID)
        {
            List<double> r = new List<double>();
            //int length = PublicClassTools.Read_Setting_Length(HR_Make_ID, "tcpReceiveBytes");//计算长度
            string Rstrs = PublicClassRule.GetValues(HR_Make_ID.ToString(), "tcpReceiveBytes");//接收数据类型

            string[] rArray = Rstrs.Split(',');

            int startIndex = 0;
            for (int i = 0; i < rArray.Length; i++)
            {

                //做数据
                r.Add(MakeOneData(rArray[i], startIndex, recvDataBuffer));

                //重置位置
                startIndex = startIndex + PublicClassTools.GetTypeByteLength(rArray[i].Trim());

            }


            return r;
        }
        /// <summary>
        /// 单个数据解析：本代码中未考虑超出索引
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startIndex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static double MakeOneData(string type, int startIndex, byte[] data)
        {
            double r = 0;

            switch (type.Trim())
            {
                case "int8":
                    r = data[startIndex];
                    break;
                case "int16":
                    {
                        Array.Reverse(data, startIndex, 2);
                        r = BitConverter.ToInt16(data, startIndex);
                    }
                    break;
                case "uint16":
                    {
                        Array.Reverse(data, startIndex, 2);
                        r = BitConverter.ToUInt16(data, startIndex);
                    }
                    break;
                case "int32":
                    {
                        Array.Reverse(data, startIndex, 4);
                        r = BitConverter.ToInt32(data, startIndex);
                    }
                    break;
                case "uint32":
                    {
                        Array.Reverse(data, startIndex, 4);
                        r = BitConverter.ToUInt32(data, startIndex);
                    }
                    break;
                case "float":
                    {
                        Array.Reverse(data, startIndex, 4);
                        r = BitConverter.ToSingle(data, startIndex);
                    }
                    break;
                case "double":
                    {
                        Array.Reverse(data, startIndex, 8);
                        r = BitConverter.ToDouble(data, startIndex);
                    }
                    break;
                default:
                    break;
            }


            return r;
        }



        /// <summary>
        /// 制作要发送的字节
        /// </summary>
        /// <param name="HR_Make_ID"></param>
        /// <param name="sendData"></param>
        /// <returns></returns>
        internal static byte[] MakeDataToBytes(int HR_Make_ID, string sendData)
        {
            byte[] realByteOut = null;


            string Sstrs = PublicClassRule.GetValues(HR_Make_ID.ToString(), "tcpSendBytes");//要发送数据类型

            string[] sArray = Sstrs.Split(',');//这是数据类型数组
            string[] dataArray = sendData.Split(',');//这是要发送的数据数组

            List<byte[]> outBytesList = new List<byte[]>();//这里最后要发送出去的byte[]组

            //标注是否翻转，true不翻转  false翻转
            bool isTCP_IEC = IsTCP_IEC(HR_Make_ID);
            if (sArray.Length == dataArray.Length)
            {
                //数据数量相同
                #region 制作byte
                for (int i = 0; i < dataArray.Length; i++)
                {
                    byte[] temp = MakeOneBytes(sArray[i], dataArray[i], isTCP_IEC);
                    if (temp != null)
                    {
                        outBytesList.Add(temp);
                    }
                    else
                    {

                    }
                }
                //制作结束，封装开始。
                realByteOut = PublicClassTools.MergeBytes(outBytesList);

                #endregion
            }
            else
            {
                //数据数量不同，不执行发送指令
            }


            return realByteOut;
        }
        /// <summary>
        /// 依据数据类型制作byte[]
        /// </summary>
        /// <param name="type">数据类型的字符串表达式 ：int16 int32 等</param>
        /// <param name="data"></param>
        /// <param name="Is_TcpIEC">是否标准tcp通讯，关键在于是否翻转。标准的不用我程序翻转。</param>
        /// <returns></returns>
        internal static byte[] MakeOneBytes(string type, string data, bool Is_TcpIEC)
        {
            byte[] r = null;
            switch (type.Trim())
            {
                case "int8":
                    r = new byte[] { Convert.ToByte(data) }; //因为本身就是byte
                    break;
                case "int16":
                    {
                        if (!Is_TcpIEC)
                            r = BitConverter.GetBytes(Convert.ToInt16(data));
                        Array.Reverse(r);
                    }
                    break;
                case "uint16":
                    {
                        if (!Is_TcpIEC)
                            r = BitConverter.GetBytes(Convert.ToUInt16(data));
                        Array.Reverse(r);
                    }
                    break;
                case "int32":
                    {
                        if (!Is_TcpIEC)
                            r = BitConverter.GetBytes(Convert.ToInt32(data));
                        Array.Reverse(r);
                    }
                    break;
                case "uint32":
                    {
                        if (!Is_TcpIEC)
                            r = BitConverter.GetBytes(Convert.ToUInt32(data));
                        Array.Reverse(r);
                    }
                    break;
                case "float":
                    {
                        if (!Is_TcpIEC)
                            r = BitConverter.GetBytes(Convert.ToSingle(data));
                        Array.Reverse(r);
                    }
                    break;
                case "double":
                    {
                        if (!Is_TcpIEC)
                            r = BitConverter.GetBytes(Convert.ToDouble(data));
                        Array.Reverse(r);
                    }
                    break;
                default:
                    break;
            }

            return r;
        }

        /// <summary>
        /// 是否是TCP标准协议，是标准协议的话，本程序解析和制作byte数据时，不做翻转
        /// </summary>
        /// <param name="HR_ID">华人ID</param>
        /// <returns></returns>
        internal static bool IsTCP_IEC(int HR_ID)
        {
            bool r = false;
            string tcpIEC = GetValues(HR_ID.ToString(), "TCP_IEC");
            if (tcpIEC == "true")
            {
                r = true;
            }
            else
            {
                r = false;
            }
            return r;
        }

        #endregion


        #region 超时处理机制
        /// <summary>
        /// 超时机制处理，之后根据配置文件处理超时问题。自动销毁试验管理
        /// </summary>
        /// <param name="managerMessage"></param>
        internal static void CheckTimeOut(ManagerMessage managerMessage)
        {
            //int lastTcp = managerMessage.Tcp_TimeOut_Test.LastNum;
            //int nowTcp = managerMessage.ReceivedTotalCount;
            #region simlink tcp超时测试

            if (managerMessage.Tcp_TimeOut_Test.LastNum == managerMessage.ReceivedTotalCount)//对比
            {
                //没变化，就记超时
                managerMessage.Tcp_TimeOut_Test.TimeOutCount++;
            }
            else
            {
                managerMessage.Tcp_TimeOut_Test.LastNum = managerMessage.ReceivedTotalCount;
                managerMessage.Tcp_TimeOut_Test.TimeOutCount = 0;
            }
            if (System.Configuration.ConfigurationManager.AppSettings["tcpAllowTimeOut"] == "true")
            {
                //不予理睬
            }
            else
            {
                //验证次数
                if (managerMessage.Tcp_TimeOut_Test.TimeOutCount > Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["tcpAllowTimeOutMaxCount"]))
                {
                    //超时，退出
                    managerMessage.Status = ShiYanStatus.End;
                }
                else
                {
                    //未超时
                }
            }
            #endregion

            #region web端超时测试

            if (managerMessage.Web_TimeOut_Test.LastNum == managerMessage.Web_Heart)//对比
            {
                //没变化，就记超时
                managerMessage.Web_TimeOut_Test.TimeOutCount++;
            }
            else
            {
                managerMessage.Web_TimeOut_Test.LastNum = managerMessage.Web_Heart;
                managerMessage.Web_TimeOut_Test.TimeOutCount = 0;
            }
            if (System.Configuration.ConfigurationManager.AppSettings["webAllowTimeOut"] == "true")
            {
                //不予理睬
            }
            else
            {
                //验证次数
                if (managerMessage.Web_TimeOut_Test.TimeOutCount > Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["webAllowTimeOutMaxCount"]))
                {
                    //超时，退出
                    managerMessage.Status = ShiYanStatus.End;
                }
                else
                {
                    //未超时
                }
            }
            #endregion


        }


        #endregion

        /// <summary>
        /// 日志操作
        /// </summary>
        /// <param name="managerMessage"></param>
        internal static void MakeLog(ManagerMessage managerMessage)
        {
            //首先确定文件路径
            string sLogPath = managerMessage.CopyModel_To_TargetPath + GetValues("root", "tcpSendLogName");
            string rLogPath = managerMessage.CopyModel_To_TargetPath + GetValues("root", "tcpReceivedLogName");

            //CopyModel_To_TargetPath = "D:\\uuu\\UID0\\0_20191211152935New\\"

            //然后写入文件内容
            try
            {
                #region send日志

                if (File.Exists(sLogPath))
                {
                    //原本就存在，继续写数据，不再写标题
                    FileStream fsc2 = new FileStream(sLogPath, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter m_streamWriterc2 = new StreamWriter(fsc2, Encoding.Default);
                    m_streamWriterc2.BaseStream.Seek(0, SeekOrigin.End);

                    string b = string.Format("{0},{1}", managerMessage.TCP_GoOn_Time, managerMessage.SendData);
                    m_streamWriterc2.WriteLine(b);
                    m_streamWriterc2.Flush();
                    m_streamWriterc2.Close();
                    fsc2.Close();
                }
                else
                {
                    //不存在，创建
                    FileStream fsc2 = new FileStream(sLogPath, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter m_streamWriterc2 = new StreamWriter(fsc2, Encoding.Default);
                    m_streamWriterc2.BaseStream.Seek(0, SeekOrigin.End);

                    //这里写个标题
                    string title = GetValues(managerMessage.HR_Make_ID.ToString(), "tcpSendBytesNames");

                    string a = string.Format("{0},{1}", "时间", title);
                    string b = string.Format("{0},{1}", managerMessage.TCP_GoOn_Time, managerMessage.SendData);
                    m_streamWriterc2.WriteLine(a);
                    m_streamWriterc2.WriteLine(b);
                    m_streamWriterc2.Flush();
                    m_streamWriterc2.Close();
                    fsc2.Close();
                }
                #endregion

                #region received日志

                if (File.Exists(rLogPath))
                {
                    //原本就存在，继续写数据，不再写标题
                    FileStream fsc2 = new FileStream(rLogPath, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter m_streamWriterc2 = new StreamWriter(fsc2, Encoding.Default);
                    m_streamWriterc2.BaseStream.Seek(0, SeekOrigin.End);

                    string b = string.Format("{0},{1}", managerMessage.TCP_GoOn_Time, managerMessage.ReceivedData);
                    m_streamWriterc2.WriteLine(b);
                    m_streamWriterc2.Flush();
                    m_streamWriterc2.Close();
                    fsc2.Close();
                }
                else
                {
                    //不存在，创建
                    FileStream fsc2 = new FileStream(rLogPath, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter m_streamWriterc2 = new StreamWriter(fsc2, Encoding.Default);
                    m_streamWriterc2.BaseStream.Seek(0, SeekOrigin.End);

                    //这里写个标题
                    string title = GetValues(managerMessage.HR_Make_ID.ToString(), "tcpReceiveBytesNames");

                    string a = string.Format("{0},{1}", "时间", title);
                    string b = string.Format("{0},{1}", managerMessage.TCP_GoOn_Time, managerMessage.ReceivedData);
                    m_streamWriterc2.WriteLine(a);
                    m_streamWriterc2.WriteLine(b);
                    m_streamWriterc2.Flush();
                    m_streamWriterc2.Close();
                    fsc2.Close();
                }
                #endregion
            }
            catch (Exception)
            {
                //咱们不用管出了什么错误。我估摸着就是咱们写文件时，这个文件被打开了
            }


        }
    }
}
