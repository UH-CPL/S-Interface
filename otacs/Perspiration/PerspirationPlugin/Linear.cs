using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerspirationPlugin
{
    class Linear
    {
        private double m_a, m_b, m_coeff;
        public Linear(int n, double[] x, double[] y)
        {
         
                // calculate the averages of arrays x and y
                double xa = 0, ya = 0;
                for (int i = 0; i < n; i++) 
                {
                    xa += x[i];
                    ya += y[i];
                }
                xa /= n;
                ya /= n;
     
                // calculate auxiliary sums
                double xx = 0, yy = 0, xy = 0;
                for (int i = 0; i < n; i++) 
                {
                    double tmpx = x[i] - xa, tmpy = y[i] - ya;
                    xx += tmpx * tmpx;
                    yy += tmpy * tmpy;
                    xy += tmpx * tmpy;
                }
     
                // calculate regression line parameters
     
                // make sure slope is not infinite
                //assert(fabs(xx) != 0);
     
                    m_b = xy / xx;
                    m_a = ya - m_b * xa;
                m_coeff = (Math.Abs(yy) == 0) ? 1 : xy / Math.Sqrt(xx * yy);
     
        }
 

        //! Evaluates the linear regression function at the given abscissa.
        /*!
        \param x the abscissa used to evaluate the linear regression function
        */
        public double getValue(double x)
        {return m_a + m_b * x;}
         
        //! Returns the slope of the regression line
        public double getSlope()
        {return m_b;}
         
        //! Returns the intercept on the Y axis of the regression line
        public double getIntercept()
        {return m_a;}
         
        //! Returns the linear regression coefficient
        /*!
        The regression coefficient indicated how well linear regression fits to the original data. It is an expression of error in the fitting and is defined as:
         
        \f[ r = \frac{S_{xy}}{\sqrt{S_{x} \cdot S_{y}}} \f]
         
        This varies from 0 (no linear trend) to 1 (perfect linear fit). If \f$ |S_y| = 0\f$ and \f$ |S_x| \neq 0 \f$,   then \e r is considered to be equal to 1.
        */
        public double getCoefficient()
        {return m_coeff;}
    }
}
