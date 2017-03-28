using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.HA;

namespace HAWorker.egg
{
    public static class Process
    {
        private static Dictionary<string, Action> _actionDic = null;
        private static byte[] _handShakeByte = null;
        static Process() {
            _actionDic = new Dictionary<string, Action>();
            _actionDic.Add("checkupdate", CheckUpdate);
            _actionDic.Add("checktask", CheckTask);
            _handShakeByte = Encoding.UTF8.GetBytes(@"{""event"":""handshake""}");
        }
        public static void DoProcess(string action) {
            if (!_actionDic.ContainsKey(action)) {
                throw new Exception("not sport action " + action);
            }
            Action doAction = _actionDic[action];
            try
            {
                doAction();
            }
            catch (Exception ex) {
                EggLog.Info(string.Format("doprocess error {0} {1}", action, ex.Message));
            }
        }

        private static void CheckUpdate() {
        }


        private static void CheckTask()
        {
        }

        public static void HandShake(out TcpSession tcpSession, out string key) {
            tcpSession = new TcpSession(Constv.Instance.ServerAddr);
            byte[] sendByte = Protocol.Encode(Constv.Instance.DefaultAesKey, _handShakeByte, Constv.Instance.ProtocolVersion, false);
            byte[] receive = tcpSession.Send(sendByte);
            string respStr = Protocol.Decode(Constv.Instance.DefaultAesKey, receive);
            //{"rc":0,"payroll":{"key":"OAMBYKWBVSZDWYLX"}}
            Response<Payroll_HandkShake> resp = JsonSerializer.JString2Object<Response<Payroll_HandkShake>>(respStr);
            if (resp.rc == 0 && resp.payroll != null)
            {
                key = resp.payroll.key;
            }
            else {
                key = null;
            }
        }

    }
}
