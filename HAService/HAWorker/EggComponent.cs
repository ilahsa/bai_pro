using System;
using System.Collections.Generic;
using System.Text;
using Imps.Services.HA;
using System.Threading;

namespace Imps.Services.HA
{
    public class EggComponent : IHAComponent
    {
        private bool _running = false;

        private Thread _thead;
        public bool IsRunning
        {
            get { return _running; }
        }
        public void Pause(object sender, EventArgs e)
        {
            _running = false;
        }

        public void Resume(object sender, EventArgs e)
        {

            _running = true;

        }
        public void Start(object sender, EventArgs e)
        {
            EggLog.Info("work start");
            _thead = new Thread(new ThreadStart(this.ThreadWork));
            _thead.IsBackground = true;
            _thead.Name = "thead_egg3_work";
            _thead.Start();
            _running = true;
        }

        public void Stop(object sender, EventArgs e)
        {
            EggLog.Info("work stop");
            if (_thead != null) {
                _thead.Abort();
                _thead = null;
            }
             _running = false;

        }

        private void ThreadWork() {
            while (true) {
                EggLog.Info(DateTime.Now.ToString());
                Thread.Sleep(4 * 1000);
            }
        }
    }
}
