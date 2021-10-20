using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigEGrabber
{
        ///////////////////////////////////////////////////////////////////////////////////////////
        //Delegates
        ///////////////////////////////////////////////////////////////////////////////////////////
        public delegate void StringDelegate(string value);
        public delegate void DoubleDelegate(double value);
        public delegate void BooleanDelegate(bool value);
        public delegate void IntegerDelegate(int value);
        public delegate void VoidDelegate();
}
