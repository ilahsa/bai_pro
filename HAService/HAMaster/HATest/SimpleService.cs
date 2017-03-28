//using System;
//using System.IO;
//using System.Net;
//using System.Net.Sockets;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using Imps.Services.HA;

//namespace HATest
//{
//    public class SimpleService : IHAComponent
//    {
//        private bool _isRunning = false;
//        private Socket s1 = null;
//        private Socket s2 = null;
//        private List<string[]> buffer = new List<string[]>();

//        private Thread _worker;

//        private object _syncRoot = new object();

//        public bool IsRunning
//        {
//            get { return _isRunning; }
//        }

//        public void Start(object sender, EventArgs e)
//        {
//                _isRunning = true;

//                s1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                s1.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8003));

//                s2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                s2.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8004));

//                StreamWriter sw = File.AppendText(@"HATest.txt");
//                sw.WriteLine("HATest.Start");
//                sw.Close();

//                s1.Listen(10);
//                s2.Listen(10);



//                _worker = new Thread(new ThreadStart(ThreadProc));
//                _worker.Name = "HA Test Worker";
//                _worker.IsBackground = true;
//                _worker.Start();
//        }

//        private void ThreadProc()
//        {
//            while (true)
//            {
//                try
//                {
//                    Thread.Sleep(1000);
//                    UserMemory();
//                }
//                catch (Exception ex)
//                {
//                    StreamWriter sw = File.AppendText(@"HATest.txt");
//                    sw.WriteLine("HA Test Worker : " + ex.ToString());
//                    sw.Close();
//                }
//            }
//        }

//        private void UserMemory()
//        {
//            string[] ss = new string[5120];
//            for (int i = 0; i < 5120; i++)
//            {
//                ss[i] = i.ToString();
//            }
//            buffer.Add(ss);
//        }

//        public void Pause(object sender, EventArgs e)
//        {
//            _isRunning = false;

//            _worker.Suspend();

//            StreamWriter sw = File.AppendText(@"HATest2.txt");
//            sw.WriteLine("HATest.Pause");
//            sw.Close();

//            Thread.Sleep(3000);

//            s1.Close();
//            s2.Close();

//            s1 = null;
//            s2 = null;
//        }

//        public void Resume(object sender, EventArgs e)
//        {
//            _isRunning = true;
//            StreamWriter sw = File.AppendText(@"HATest.txt");
//            sw.WriteLine("HATest.Resume");
//            sw.Close();

//            s1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            s1.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8003));

//            s2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            s2.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8004));

//            s1.Listen(10);
//            s2.Listen(10);

//            _worker.Resume();
//        }

//        public void Stop(object sender, EventArgs e)
//        {
//            _isRunning = false;
//            StreamWriter sw = File.AppendText(@"HATest1.txt");
//            sw.WriteLine("HATest.Stop");
//            sw.Close();

//            s1.Close();
//            s2.Close();
//        }
//    }
//}