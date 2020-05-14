using System;
using System.IO;
using CoreLibrary;

namespace ConsoleApp {
    class Program {
        static void Main (string[] args) {
            SdkVersion () ;
            //Test();
            //TestKill(2203);//现在就想实验这个
            ManagerMessage manager=new ManagerMessage();
            MakeRun("",ref manager);

            string str = ClassTest.SayHello ();


            Console.WriteLine ("Hello World!" + str);

            Console.ReadLine ();

        }

        private static void Test () {
            string path1 = Environment.CurrentDirectory;
            string path2 = Directory.GetCurrentDirectory ();
            ///home/qi/Documents/vscode/CodeForCore/CoreWebAPI/bin/Debug/netcoreapp3.1/HR_Models.ini
            Ini ini = new Ini ("/home/qi/Documents/vscode/CodeForCore/ConsoleApp/bin/Debug/netcoreapp3.1/HR_Models.ini");
            //Ini ini = new Ini("HR_Models.ini");

            string strrr = ini.ReadValue ("root", "tcpSendLogName");

            Service_Main a = new Service_Main ();
        }

        private static void TestKill (int pid) {
            CoreLibrary.CMDClass.KillProcess (pid);
        }

        public static void SdkVersion () {
            var sdkVersion = string.Empty;
            var psi = new System.Diagnostics.ProcessStartInfo ("dotnet", " --version");
            psi.RedirectStandardOutput = true;
            using (var process = System.Diagnostics.Process.Start (psi)) {
                var output = process.StandardOutput.ReadLine();//.ReadToEnd ();
                Console.WriteLine(output);
            }
            //return Content(sdkVersion);
        }

private static void MakeRun(string arguments, ref ManagerMessage row)
        {
            try
            {
                //string arguments = @"-nosplash -nodesktop -sd D:\MyWorkGGTest\one\MGtcp -r test";


                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("matlab", arguments);

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

    }
}