using System.Diagnostics;
using NUnit.Framework;

namespace NUnitTestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "print";
            info.FileName = @"C:\Users\Andrew\Desktop\for printing\MSG Manifesting FFTIN _4.1_v0.8.docx";
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Normal;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();

            p.WaitForInputIdle();
            System.Threading.Thread.Sleep(3000);
            if (false == p.CloseMainWindow())
                p.Kill();
        }
    }
}