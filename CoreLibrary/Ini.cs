using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLibrary {

    /* 演示 如下：
        static void Main(string[] args)
        {
            string Current;

            Current = Directory.GetCurrentDirectory();//获取当前根目录
            Console.WriteLine("Current directory {0}", Current);
            // 写入ini
            Ini ini=new Ini(Current+"/config.ini");
            ini.WriteValue("Setting","key1","hello word!");
            ini.WriteValue("Setting","key2","hello ini!");
            ini.WriteValue("SettingImg", "Path", "IMG.Path");
            // 读取ini
            string stemp = ini.ReadValue("Setting","key2");
            Console.WriteLine(stemp);


            Console.ReadKey();
        }

     */

    /// <summary>
    /// Ini操作类，读写操作简单，容易控制，是居家旅行，必备之首选。
    /// 2013年1月30日9:49:28 整理 By Omega。
    /// 写入就是写入，没有就创建，有就修改。连没有ini文件时都直接给你创建出来。
    /// 
    /// 例子见本源文件上面的示例
    /// 
    /// 
    /// 关于异常，读不到的节点，就会返回空""，不会报错，请安心使用
    /// </summary>
    public class Ini {
        // 声明INI文件的写操作函数 WritePrivateProfileString()
        [System.Runtime.InteropServices.DllImport ("kernel32")]
        private static extern long WritePrivateProfileString (string section, string key, string val, string filePath);

        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport ("kernel32")]
        private static extern int GetPrivateProfileString (string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        IniParser.Model.IniData data = null;
        private string sPath = null;
        public Ini (string path) {
            this.sPath = path;
            IniParser.FileIniDataParser iniDataParser = new IniParser.FileIniDataParser ();
            data = iniDataParser.ReadFile (path, Encoding.ASCII);
        }

        /// <summary>
        /// 写ini文件
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="value">键值</param>
        public void WriteValue (string section, string key, string value) {
            // section=配置节，key=键名，value=键值，path=路径
            //WritePrivateProfileString (section, key, value, sPath);

            data[section][key]=value;
        }

        /// <summary>
        /// 读ini文件
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <returns>键值</returns>
        public string ReadValue (string section, string key) {
            // 每次从ini中读取多少字节
            //System.Text.StringBuilder temp = new System.Text.StringBuilder (65535);
            // section=配置节，key=键名，temp=上面，path=路径
            //GetPrivateProfileString(section, key, "", temp, 65535, sPath);
            //return temp.ToString();

            string linuxData = data[section][key];
            return linuxData;

        }

    }
}