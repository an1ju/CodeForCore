using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CoreLibrary
{
    /// <summary>
    /// CMD命令
    /// </summary>
    public class CMDClass
    {
        /// <summary>
        /// 运行cmd命令
        /// 会显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        public static bool RunCmd(string cmdExe, string cmdStr)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                    myPro.StartInfo = psi;
                    myPro.Start();
                    myPro.WaitForExit();
                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }

        /// <summary>
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        public static bool RunCmd2(string cmdExe, string cmdStr)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    string str = string.Format(@"""{0}"" {1} {2}", cmdExe, cmdStr, "&exit");

                    myPro.StandardInput.WriteLine(str);
                    myPro.StandardInput.AutoFlush = true;
                    myPro.WaitForExit();

                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }


        public static void KillProcess(int pid)
        {

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("kill", pid.ToString());
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process myprocess = new System.Diagnostics.Process();
            myprocess.StartInfo = startInfo;
            myprocess.StartInfo.UseShellExecute = false;
            myprocess.StartInfo.CreateNoWindow = true;
            myprocess.StartInfo.RedirectStandardOutput = true;

            myprocess.Start();



            #region windows 下 kill方法
            //string cmdStr = string.Format("taskkill /pid  {0} /f &exit", pid);
            //using (Process process = new Process())
            //{
            //    process.StartInfo.FileName = "cmd.exe";
            //    process.StartInfo.UseShellExecute = false;
            //    process.StartInfo.RedirectStandardInput = true;
            //    process.StartInfo.RedirectStandardOutput = true;
            //    process.StartInfo.RedirectStandardError = true;
            //    process.StartInfo.CreateNoWindow = true;
            //    process.Start();
            //    process.StandardInput.WriteLine(cmdStr);
            //    process.StandardInput.AutoFlush = true;
            //}
            #endregion
        }



        /// <summary>
        /// 结束指定名称进程
        /// </summary>
        /// <param name="processName"></param>
        public static bool KillProcess(string processName, string path = "")
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(processName);
                foreach (var p in proc)
                {
                    if (path != "" && p.MainModule.FileName == path)
                    {
                    }

                    if (p.ProcessName == processName)
                    {
                        KillProcess(p.Id);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            

        }

        /// <summary>
        /// 根据进程名称和进程ID，返回出PID
        /// 为什么要这个方法呢？因为执行程序是复制出来的，名称可能(一定会重复，)我要关闭哪个，需要路径确认。
        /// </summary>
        /// <param name="processName">exe名称</param>
        /// <param name="path">程序所在路径</param>
        /// <returns></returns>
        public static int FindPID(string processName, string path = "")
        {
            int r = -1;
            try
            {
                Process[] proc = Process.GetProcessesByName(processName);

                for (int i = 0; i < proc.Length; i++)
                {
                    if (path != "" && proc[i].MainModule.FileName == path)
                    {
                        r = i;
                        break;
                    }

                }

            }
            catch
            {
                r = -999;
            }

            return r;
        }
    }
}
