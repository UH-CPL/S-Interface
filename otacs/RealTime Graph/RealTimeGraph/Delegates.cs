using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeGraph
{
    ///////////////////////////////////////////////////////////////////////////////////////////
    //Delegates
    ///////////////////////////////////////////////////////////////////////////////////////////
    public delegate void TwoValuesFloatDelegate(float value1, float value2, float frame);
    public delegate void OneValueFloatDelegate(float value1, float frame);
    public delegate void StringDelegate(string value);
    public delegate void DoubleDelegate(double value);
    public delegate void BooleanDelegate(bool value);
    public delegate void IntegerDelegate(int value);
    public delegate void BiofeedbackDelegate(Biofeedback value);

}
