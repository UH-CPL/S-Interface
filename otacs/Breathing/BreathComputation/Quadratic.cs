using System;
using System.Collections.Generic;
using System.Text;

namespace BreathImageProcessing
{
    class Quadratic
    {

        private double b0, b1, b2, msqError;
        private float[,] a;
        private float[,] inv_a;

        private float[] c;

        bool isSingularMatrix;// Singular matrix does not have inverse as determinant is 0

        public Quadratic()
        {
            a = new float[3,3];
            inv_a = new float[3, 3];

            c = new float[3];

            b0 = 0;
            b1 = 0;
            b2 = 0;

            isSingularMatrix = false;

        }

        public void ComputeQuadraticFitting(int n, float[] x, float[] y)
        {
            //1: Populate matrix, a;
            LoadMatrix(x, y);

            //2: Compute inverse matrix, inv_a 
            ComputeInverseMatrix();

            //3: Compute parameters, b0, b1, b2
            ComputeParameters();

            //4: Compute mean squared error
            computeMeanSquaredError(x, y);
        }

        public double get_b0()
        { return b0; }
        public double get_b1()
        { return b1; }
        public double get_b2()
        { return b2; }
        public double get_msqError()
        { return msqError; }

        public void LoadMatrix(float[] x, float[] y)
        {
            // Clean the matrix a
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    a[i, j] = 0;
                    inv_a[i, j] = 0;
                }

            //Clean Matrix c
            for (int j = 0; j < 3; j++)
            { c[j] = 0; }


            a[0, 0] =  x.Length; // n
            for (int i = 0; i < x.Length; i++)
            {
                a[0, 1] = a[0, 1] + x[i]; //xi
                a[0, 2] = a[0, 2] + x[i] * x[i]; //xi^2

                a[1, 0] = a[1, 0] + x[i]; //xi
                a[1, 1] = a[1, 1] + x[i] * x[i]; //xi^2 
                a[1, 2] = a[1, 2] + x[i] * x[i] * x[i]; //xi^3

                a[2, 0] = a[2, 0] + x[i] * x[i]; //xi^2 
                a[2, 1] = a[2, 1] + x[i] * x[i] * x[i]; //xi^3
                a[2, 2] = a[2, 2] + x[i] * x[i] * x[i] * x[i]; //xi^4


                c[0] = c[0] + y[i];
                c[1] = c[1] + x[i]*y[i];
                c[2] = c[2] + x[i]*x[i]*y[i];

            } //end of for loop
        }

        public void ComputeInverseMatrix()
        {
            float D;
            
            D  =  a[0,0]*(a[1,1]*a[2,2]-a[2,1]*a[1,2])
                 -a[0,1]*(a[1,0]*a[2,2]-a[1,2]*a[2,0])
                 +a[0,2]*(a[1,0]*a[2,1]-a[1,1]*a[2,0]);

            double invD; 

            if(D != 0) // Matrix inverse is possible
            {
                isSingularMatrix = false;
                invD = 1/D;
            }
            else // Matrix inverse is not possible
            {
                isSingularMatrix = true;
                invD = 1000000; // set dummy value
            }

            inv_a[0, 0] =  (a[1, 1] * a[2, 2] - a[2, 1] * a[1, 2]) * (float)invD;
            inv_a[1, 0] = -(a[0, 1] * a[2, 2] - a[0, 2] * a[2, 1]) * (float)invD;
            inv_a[2, 0] =  (a[0, 1] * a[1, 2] - a[0, 2] * a[1, 1]) * (float)invD;
            inv_a[0, 1] = -(a[1, 0] * a[2, 2] - a[1, 2] * a[2, 0]) * (float)invD;
            inv_a[1, 1] =  (a[0, 0] * a[2, 2] - a[0, 2] * a[2, 0]) * (float)invD;
            inv_a[2, 1] = -(a[0, 0] * a[1, 2] - a[1, 0] * a[0, 2]) * (float)invD;
            inv_a[0, 2] =  (a[1, 0] * a[2, 1] - a[2, 0] * a[1, 1]) * (float)invD;
            inv_a[1, 2] = -(a[0, 0] * a[2, 1] - a[2, 0] * a[0, 1]) * (float)invD;
            inv_a[2, 2] =  (a[0, 0] * a[1, 1] - a[1, 0] * a[0, 1]) * (float)invD;

        }

        public void ComputeParameters()
        {
            if (isSingularMatrix == false) // if matrix is inversible, compute parameters 
            {
                b0 = inv_a[0, 0] * c[0] + inv_a[0, 1] * c[1] + inv_a[0, 2] * c[2];
                b1 = inv_a[1, 0] * c[0] + inv_a[1, 1] * c[1] + inv_a[1, 2] * c[2];
                b2 = inv_a[2, 0] * c[0] + inv_a[2, 1] * c[1] + inv_a[2, 2] * c[2];
            }
        }

        private void computeMeanSquaredError(float[] x, float[] y)
        {
            msqError = 0.0;
            float Temp = (float)0.0;
            for (int i = 0; i < x.Length; i++)
            { 
                Temp = (float)(b0 + b1*x[i] + b2*x[i]*x[i] - y[i]);
                msqError += (Temp * Temp);
            }
            msqError /= (double)x.Length;
        }

    }
}
