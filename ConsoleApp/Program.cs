using System.IO;
using System;
using CoreLibrary;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();

            string str = ClassTest.SayHello();

            Console.WriteLine("Hello World!" + str);

            Console.ReadLine();

        }

        private static void Test()
        {
            string path1= Environment.CurrentDirectory;
            string path2= Directory.GetCurrentDirectory();
                                            ///home/qi/Documents/vscode/CodeForCore/CoreWebAPI/bin/Debug/netcoreapp3.1/HR_Models.ini
            Ini ini = new Ini("/home/qi/Documents/vscode/CodeForCore/ConsoleApp/bin/Debug/netcoreapp3.1/HR_Models.ini");
            //Ini ini = new Ini("HR_Models.ini");

            string strrr=ini.ReadValue("root","tcpSendLogName");

            Service_Main a = new Service_Main();
        }

        
    }
}
