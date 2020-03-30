using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace CoreLibrary
{

    /// <summary>
    /// 试验管理核心类。
    /// 在这里记录每一条新试验的连接状态。
    /// 在这里还将会把已经结束的试验关闭
    /// </summary>
    [DataContract]
    public class ManagerMessage
    {
        /// <summary>
        /// 序号，做计数器使用。
        /// **注意！！这个ID越来越重要了！！
        /// 2019年12月5日16:39:36
        /// 可以把这当做秘钥来用：网站让我开始一个实验，我就创建了一个实验。1-255 这个数字，循环获取，哪个空了，就用哪个。
        /// 回头，还得让模型运行起来发送这个数据回来，验证是我的这个实验，否则不行。模型回发给我的数据还要有数据长度，便于数据解析使用。
        /// 2019年12月5日16:42:01
        /// </summary>
        private int _ID;
        /// <summary>
        /// 用户编号，标记谁要做试验。
        /// 其实根据这个值，就可以生产目标路径了。
        /// </summary>
        private int _UID;
        /// <summary>
        /// 试验类型，是新试验还是复盘或者是看其他人的试验
        /// </summary>
        private ShiYanCreateType _CreateType;
        /// <summary>
        /// 华人风电的模型ID，网站上线第一个模型就是1，根据配置文件可以读出原始路径
        /// 如果用户使用的其他用户试验，或者复盘，这里可以写0
        /// </summary>
        private int _HR_Make_ID;
        /// <summary>
        /// 原始模型路径
        /// </summary>
        private string _SourceModelPath;
        /// <summary>
        /// 模型要复制到的目标路径
        /// </summary>
        private string _CopyModel_To_TargetPath;
        /// <summary>
        /// 试验状态
        /// </summary>
        private ShiYanStatus _Status;
        /// <summary>
        /// TCP通讯启动时刻的时间
        /// 用这个计时，时刻与新的时间对比，能记录试验开始后的流程和步骤。便于复盘使用试验数据。
        /// </summary>
        private DateTime _TCP_OK_Start;
        /// <summary>
        /// TCP连接持续时长，这个数据根据上面的_TCP_OK_Start及时计算
        /// </summary>
        private string _TCP_GoOn_Time;

        /// <summary>
        /// 远程连接地址
        /// </summary>
        private string _Remote_IP;
        /// <summary>
        /// 远程端口
        /// </summary>
        private int _Remote_Port;

        /// <summary>
        /// 发送的数据和接收的数据。【暂时用string表述】多个数据用逗号【,】分割
        /// </summary>
        private string _SendData;
        private string _ReceivedData;

        /// <summary>
        /// 发送总次数
        /// </summary>
        private int _SendTotalCount;
        /// <summary>
        /// 发送数据与前一次对比变化的总次数
        /// 这样可以减少要保存的数据文件。看看效果
        /// </summary>
        private int _SendChangedCount;

        private int _ReceivedTotalCount;
        private int _ReceivedChangedCount;

        /// <summary>
        /// web心跳机制
        /// 这个总不变，就当做用户离开了。
        /// </summary>
        private int _Web_Heart;
        /// <summary>
        /// 批量命令路径
        /// </summary>
        private string _CommandsPath;
        /// <summary>
        /// 脚本文件路径
        /// </summary>
        private string _PY_Path;
        /// <summary>
        /// 模型运行的进程ID
        /// </summary>
        private int _PID;
        /// <summary>
        /// 被查看用户ID，如果试验是看其他人的试验，为了表示尊重，注明来源。
        /// </summary>
        private int _Look_Other_UID;

        /// <summary>
        /// 超时检查，simlink
        /// </summary>
        private TimeOutPlay _Tcp_TimeOut_Test;
        /// <summary>
        /// 超时检查，web端
        /// </summary>
        private TimeOutPlay _Web_TimeOut_Test;
        /// <summary>
        /// M文件必备参数
        /// </summary>
        private string _M_Setting_Arguments;

        [DataMember]
        public int ID { get => _ID; set => _ID = value; }
        [DataMember]
        public int UID { get => _UID; set => _UID = value; }
        [DataMember]
        public ShiYanCreateType CreateType { get => _CreateType; set => _CreateType = value; }
        [DataMember]
        public int HR_Make_ID { get => _HR_Make_ID; set => _HR_Make_ID = value; }
        [DataMember]
        public string SourceModelPath { get => _SourceModelPath; set => _SourceModelPath = value; }
        [DataMember]
        public string CopyModel_To_TargetPath { get => _CopyModel_To_TargetPath; set => _CopyModel_To_TargetPath = value; }
        [DataMember]
        public ShiYanStatus Status { get => _Status; set => _Status = value; }
        [DataMember]
        public DateTime TCP_OK_Start { get => _TCP_OK_Start; set => _TCP_OK_Start = value; }
        [DataMember]
        public string TCP_GoOn_Time { get => _TCP_GoOn_Time; set => _TCP_GoOn_Time = value; }
        [DataMember]
        public string Remote_IP { get => _Remote_IP; set => _Remote_IP = value; }
        [DataMember]
        public int Remote_Port { get => _Remote_Port; set => _Remote_Port = value; }
        [DataMember]
        public string SendData { get => _SendData; set => _SendData = value; }
        [DataMember]
        public string ReceivedData { get => _ReceivedData; set => _ReceivedData = value; }
        [DataMember]
        public int SendTotalCount { get => _SendTotalCount; set => _SendTotalCount = value; }
        [DataMember]
        public int SendChangedCount { get => _SendChangedCount; set => _SendChangedCount = value; }
        [DataMember]
        public int ReceivedTotalCount { get => _ReceivedTotalCount; set => _ReceivedTotalCount = value; }
        [DataMember]
        public int ReceivedChangedCount { get => _ReceivedChangedCount; set => _ReceivedChangedCount = value; }
        [DataMember]
        public int Web_Heart { get => _Web_Heart; set => _Web_Heart = value; }
        [DataMember]
        public string CommandsPath { get => _CommandsPath; set => _CommandsPath = value; }
        [DataMember]
        public string PY_Path { get => _PY_Path; set => _PY_Path = value; }
        [DataMember]
        public int PID { get => _PID; set => _PID = value; }
        [DataMember]
        public int Look_Other_UID { get => _Look_Other_UID; set => _Look_Other_UID = value; }
        [DataMember]
        public TimeOutPlay Tcp_TimeOut_Test { get => _Tcp_TimeOut_Test; set => _Tcp_TimeOut_Test = value; }
        [DataMember]
        public TimeOutPlay Web_TimeOut_Test { get => _Web_TimeOut_Test; set => _Web_TimeOut_Test = value; }
        /// <summary>
        ///  M文件必备参数
        /// </summary>
        [DataMember]
        public string M_Setting_Arguments { get => _M_Setting_Arguments; set => _M_Setting_Arguments = value; }
    }

    /// <summary>
    /// 超时模型
    /// </summary>
    [DataContract]
    public class TimeOutPlay
    {
        /// <summary>
        /// 上一次的数值
        /// </summary>
        int _LastNum;
        /// <summary>
        /// 超时次数
        /// </summary>
        int _TimeOutCount;

        [DataMember]
        public int LastNum { get => _LastNum; set => _LastNum = value; }
        [DataMember]
        public int TimeOutCount { get => _TimeOutCount; set => _TimeOutCount = value; }
    }

    /// <summary>
    /// 试验创建类型
    /// </summary>
    public enum ShiYanCreateType
    {
        /// <summary>
        /// 新试验
        /// 这种类型需要复制模型到自己的文件夹
        /// </summary>
        New = 101,
        /// <summary>
        /// 复盘试验，复盘自己的试验
        /// 试验本身就是自己的，无需复制
        /// </summary>
        ReView,
        /// <summary>
        /// 看其他人的试验
        /// 把其他人的试验模型以及试验数据复制到自己的文件夹中
        /// </summary>
        LookOthers,
    }
    /// <summary>
    /// 试验状态
    /// </summary>
    public enum ShiYanStatus
    {
        /// <summary>
        /// 创建
        /// </summary>
        [Description("创建")]
        New = 0,
        /// <summary>
        /// 文档复制中
        /// </summary>
        [Description("复制中")]
        FilesCopying = 201,
        /// <summary>
        /// 拷贝完成
        /// </summary>
        [Description("拷贝完成")]
        FilesCopyCompleted,
        /// <summary>
        /// 正在运行cmd命令
        /// </summary>
        [Description("执行命令")]
        CmdCommand,
        /// <summary>
        /// 运行中
        /// </summary>
        [Description("运行中")]
        Running,
        /// <summary>
        /// 故障
        /// 现在预计在内部TCP通讯长久没发生变化时触发这个
        /// 还有就是网页离开后，如果这个进程还没停，也要触发
        /// </summary>
        [Description("异常")]
        Error,
        [Description("没找到圆形路径")]
        Error_NoFind_SourcePath,
        [Description("没找到复盘路径")]
        Error_NoFind_FuPanPath,
        /// <summary>
        /// 错误， 缺失母路径，复盘和查看他人试验时可能出现
        /// </summary>
        [Description("未传递母级路径")]
        Error_No_MotherPath,
        /// <summary>
        /// 没有这个HRID试验编号
        /// </summary>
        [Description("没有这个HR试验编号")]
        Error_No_THIS_HR_ID,
        /// <summary>
        /// 错误的文件夹名称
        /// </summary>
        [Description("母路径拼写错误")]
        Error_Write_MotherPathName,
        /// <summary>
        /// 复盘条件下，uid与lookotheruid不等
        /// </summary>
        [Description("查看自己复盘时，双ID不一致")]
        Error_UID_NOT_Equal_LookOtherUID,
        /// <summary>
        /// 试验结束
        /// </summary>
        [Description("试验结束")]
        End,

    }

    public enum ShiYanXieYi
    {
        tcpReceiveBytes,
        tcpReceiveBytesNames,
        tcpSendBytes,
        tcpSendBytesNames
    }

}
