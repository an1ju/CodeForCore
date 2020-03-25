using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CoreLibrary
{
    public class PublicClassTools
    {

        #region 拷贝文件夹
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="moveToPath"></param>
        public static void Copy(string sourcePath, string moveToPath)
        {
            #region 创建文件夹
            if (Directory.Exists(moveToPath))
            {
                //存在，不用管，继续！
            }
            else
            {
                //不存在，创建
                Directory.CreateDirectory(moveToPath);
            }
            #endregion

            #region 复制文件
            CopyDirectory(sourcePath, moveToPath);
            #endregion


        }

        /// <summary>
        /// 删除文件夹操作
        /// </summary>
        /// <param name="fullPathName"></param>
        /// <returns></returns>
        public static string Delete(string fullPathName)
        {
            string r = "";
            if (Directory.Exists(fullPathName))
            {
                //存在，删除
                Directory.Delete(fullPathName, true);
                r = "删除成功";
            }
            else
            {
                //没有文件夹
                r = "该文件夹不存在";
            }
            return r;
        }

        /// <summary>
        /// 递归方法把文件夹中的文件全部复制过去
        /// 包含文件夹中的文件夹
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="destPath"></param>
        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        #endregion

        #region 字节处理器
        /// <summary>
        /// 读取配置文件，输入输出都是什么数据。
        /// </summary>
        /// <param name="HR_id"></param>
        /// <param name="tcpReceiveSendBytesName">tcpReceiveBytes/tcpSendBytes</param>
        /// <returns></returns>
        public static int Read_Setting_Length(int HR_id, string tcpReceiveSendBytesName)
        {
            string Rstrs = PublicClassRule.GetValues(HR_id.ToString(), tcpReceiveSendBytesName);//接收数据类型            

            string[] rArray = Rstrs.Split(',');

            int rLength = rArray.Length;
            //rLength = GetTCP_Receive_Length(rArray, rLength);//这个方法用错了！！留着启示后人 2019年12月13日17:02:03 就不应该用

            return rLength;
        }

        private static int GetTCP_Receive_Length(string[] rArray, int rLength)
        {
            for (int i = 0; i < rArray.Length; i++)
            {
                rLength = rLength + GetTypeByteLength(rArray[i].Trim());
            }

            return rLength;
        }

        public static int GetTypeByteLength(string type)
        {
            int rLength = 0;
            switch (type.Trim())
            {
                case "int8":
                    rLength = rLength + 1;
                    break;
                case "int16":
                case "uint16":
                    rLength = rLength + 2;
                    break;
                case "int32":
                case "uint32":
                    rLength = rLength + 4;
                    break;
                case "float":
                    rLength = rLength + 4;
                    break;
                case "double":
                    rLength = rLength + 8;
                    break;
                default:
                    break;
            }
            return rLength;
        }

        #endregion


        #region 字节合并操作

        /// <summary>
        /// 两个byte[]数组合并成一个
        /// </summary>
        /// <param name="bList1">1</param>
        /// <param name="bList2">2</param>
        /// <returns></returns>
        public static byte[] MergeBytes(byte[] bList1, byte[] bList2)
        {
            byte[] r = new byte[bList1.Count() + bList2.Count()];
            for (int i = 0; i < r.Count(); i++)
            {
                if (i < bList1.Count())
                {
                    r[i] = bList1[i];
                }
                else
                {
                    r[i] = bList2[i - bList1.Count()];
                }
            }

            return r;
        }

        /// <summary>
        /// 把N个byte[]合并到一起，这个方法中会同时调用 两个合并的方法~此处是为了方便起见好用
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static byte[] MergeBytes(List<byte[]> list)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (i > 0)
                {
                    list[i] = MergeBytes(list[i - 1], list[i]);
                }
            }

            return list[list.Count - 1];
        }
        #endregion

    }
}
