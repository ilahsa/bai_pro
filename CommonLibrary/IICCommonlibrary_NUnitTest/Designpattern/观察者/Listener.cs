using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.观察者
{
    public class Listener
    {
        private List<Observer> _observers = new List<Observer>();

        public void RegisterOb(Observer ob)
        {
            _observers.Add(ob);
        }

        public void RemoveOb(Observer ob)
        {
            _observers.Remove(ob);
        }

        public void ChangeInfo()
        {
            //do something

            NotifyOb();

        }

        private void NotifyOb()
        {
            foreach (Observer ob in _observers)
            {
                ob.Update();
            }
        }
    }
}
