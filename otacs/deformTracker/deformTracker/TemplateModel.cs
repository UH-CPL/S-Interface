using System;
using System.Collections.Generic;
using System.Text;

namespace deformTracker
{
    class TemplateModel
    {
        public float[,] A;                                                     //matrix A for solving the linear equation.
        public float[] b;                                                      //rhs of linear equation.
        public float[] X;                                                      //linear equation solution.
        public float[] X0;
        public float[] rho;
        public float[,] Iconstmap;                                             //Iconstmap.
        public float[,] Iconstvals;                                            //Iconstvals. 
        int Tol = 15;
        int iterationCount;
        int lambda_1 = 25;
        int lambda_2 = 2;
        int lambda = 100; 
        int MaxIterationNo = 150;
        float unstableRatio = 0.2F;

        public TemplateModel()
        {

        }

        public void UpdateTemplates(SingleTracker[,] singleTrackerArray, int trackersColNum, int trackersRowNum)
        {


            for (int i = 0; i < trackersColNum; i++)
            {
                for (int j = 0; j < trackersRowNum; j++)
                {
                    if (singleTrackerArray[i, j].isSurvivor == true)
                    {
                        if (singleTrackerArray[i, j].useInitialT == false)
                        {
                            computeTemplate(singleTrackerArray[i, j]);
                        }
                        else
                        {
                            setToInitialTemplate(singleTrackerArray[i, j]);
                        }
                    }
                }
            }

        }

        private void setToInitialTemplate(SingleTracker singleTracker)
        {
            for (int i = 0; i < singleTracker.height; i++)
            {
                for (int j = 0; j < singleTracker.width; j++)
                {
                    singleTracker.currentTemplate[i, j] = singleTracker.originalTemplate[i, j];
                }
            }
        }

        public void computeTemplate(SingleTracker singleTracker)
        {
            Iconstmap = new float[singleTracker.height, singleTracker.width];
            Iconstvals = new float[singleTracker.height, singleTracker.width];
            X = new float[singleTracker.height * singleTracker.width];
            X0 = new float[singleTracker.height * singleTracker.width];
            rho = new float[singleTracker.height * singleTracker.width];

            //step1: set the const_map and const_vals.
            int No_unstable = 0;
            int No_stable = 0;
            //consts_map and consts_vals.
            for (int y = 0; y < singleTracker.height; y++)
            {
                for (int x = 0; x < singleTracker.width; x++)
                {
                    Iconstvals[y, x] = ComputeAffinity(singleTracker.bestROI[y, x], singleTracker.currentTemplate[y, x]);
                    if (Iconstvals[y, x] == 1) //|| Iconstvals[y, x] == 0)
                    {
                        No_unstable++;
                        Iconstmap[y, x] = 1;
                    }
                    else if (Iconstvals[y, x] == 0)
                    {
                        No_stable++;
                        Iconstmap[y, x] = 1;
                    }
                    else
                    {
                        Iconstvals[y, x] = 0;
                        Iconstmap[y, x] = 0;   //don't know if this line is necessary.
                    }
                }
            }

                    
            if (No_unstable > unstableRatio * singleTracker.height * singleTracker.width)
            {
                //step2: compute the Laplacian matrix and store it in band sparse matrix format.

                int img_size = singleTracker.height * singleTracker.width;
                A = new float[img_size, 25];
                b = new float[img_size];

                int len = GetLaplacian(singleTracker.bestROI, Iconstmap, Iconstvals, singleTracker.height, singleTracker.width);

                //step3: GMRES linear equation solver.
                //!set the initial guess to be [b1/a11, b2/a22, ..., bn/ann]'
                for (int i = 0; i < img_size; i++)
                {
                    X[i] = b[i] / A[i, 12];
                }

                iterationCount = 0;
                //GMRES(A, b, Single_height, Single_width, Tol, X);
                GMRES(A, b, singleTracker.height, singleTracker.width, Tol, X);


                //step4: template update.
                float alpha;
                for (int i = 0; i < img_size; i++)
                {
                    //int h = i % Single_height;
                    //int w = i / Single_height;
                    int h = i % singleTracker.height;
                    int w = i / singleTracker.height;
                    alpha = X[i];
                    if (alpha < 0)
                    {
                        alpha = 0;
                    }
                    else if (alpha > 1)
                    {
                        alpha = 1;
                    }
                    singleTracker.currentTemplate[h, w] = (1 - alpha) * singleTracker.currentTemplate[h, w] + alpha * singleTracker.bestROI[h, w];
                }
            }
     
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------
        float ComputeAffinity(float x, float y)
        {
            float diff = Math.Abs(x - y);
            float a = 0;

            if (diff > lambda_1)
            {
                a = 1;
            }
            else if (diff < lambda_2)
            {
                a = 0;
            }
            else
            {
                a = 0.5F;
            }

            return a;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------
        #region GMRES
        void GMRES(float[,] A, float[] b, int height, int width, float Tol, float[] X)
        {
            iterationCount++;
            int N = width * height;

            //float[] rho = new float[N];

            int kmax;
            int k;

            if (width * height > 10)
            {
                kmax = 10;
            }
            else
            {
                kmax = width;

            }
            float[,] H = new float[kmax + 1, kmax];
            float[,] V = new float[N, kmax + 1];
            double beta, errtol;
            float[] mul_matrix = new float[25];
            float c, s;
            float norm_rho, norm_b;

            //float[] X0 = new float[height * width];
            //for (int i = 0; i < N; i++)
            //{
            //X0[i] = X[i];
            //}

            //multiply A and X to compute rho
            for (int i = 0; i < N; i++)
            {
                X0[i] = X[i];

                if ((i - 2 * height - 2) < 0)
                {
                    mul_matrix[0] = 0;
                }
                else
                {
                    mul_matrix[0] = X[i - 2 * height - 2];
                }
                if ((i - 2 * height - 1) < 0)
                {
                    mul_matrix[1] = 0;
                }
                else
                {
                    mul_matrix[1] = X[i - 2 * height - 1];
                }
                if ((i - 2 * height) < 0)
                {
                    mul_matrix[2] = 0;
                }
                else
                {
                    mul_matrix[2] = X[i - 2 * height];
                }
                if ((i - 2 * height + 1) < 0)
                {
                    mul_matrix[3] = 0;
                }
                else
                {
                    mul_matrix[3] = X[i - 2 * height + 1];
                }
                if ((i - 2 * height + 2) < 0)
                {
                    mul_matrix[4] = 0;
                }
                else
                {
                    mul_matrix[4] = X[i - 2 * height + 2];
                }
                if ((i - height - 2) < 0)
                {
                    mul_matrix[5] = 0;
                }
                else
                {
                    mul_matrix[5] = X[i - height - 2];
                }
                if ((i - height - 1) < 0)
                {
                    mul_matrix[6] = 0;
                }
                else
                {
                    mul_matrix[6] = X[i - height - 1];
                }
                if ((i - height) < 0)
                {
                    mul_matrix[7] = 0;
                }
                else
                {
                    mul_matrix[7] = X[i - height];
                }
                if ((i - height + 1) < 0)
                {
                    mul_matrix[8] = 0;
                }
                else
                {
                    mul_matrix[8] = X[i - height + 1];
                }
                if ((i - height + 2) < 0)
                {
                    mul_matrix[9] = 0;
                }
                else
                {
                    mul_matrix[9] = X[i - height + 2];
                }
                if ((i - 2) < 0)
                {
                    mul_matrix[10] = 0;
                }
                else
                {
                    mul_matrix[10] = X[i - 2];
                }
                if ((i - 1) < 0)
                {
                    mul_matrix[11] = 0;
                }
                else
                {
                    mul_matrix[11] = X[i - 1];
                }
                mul_matrix[12] = X[i];
                if ((i + 1) >= N)
                {
                    mul_matrix[13] = 0;
                }
                else
                {
                    mul_matrix[13] = X[i + 1];
                }
                if ((i + 2) >= N)
                {
                    mul_matrix[14] = 0;
                }
                else
                {
                    mul_matrix[14] = X[i + 2];
                }
                if ((i + height - 2) >= N)
                {
                    mul_matrix[15] = 0;
                }
                else
                {
                    mul_matrix[15] = X[i + height - 2];
                }
                if ((i + height - 1) >= N)
                {
                    mul_matrix[16] = 0;
                }
                else
                {
                    mul_matrix[16] = X[i + height - 1];
                }
                if ((i + height) >= N)
                {
                    mul_matrix[17] = 0;
                }
                else
                {
                    mul_matrix[17] = X[i + height];
                }
                if ((i + height + 1) >= N)
                {
                    mul_matrix[18] = 0;
                }
                else
                {
                    mul_matrix[18] = X[i + height + 1];
                }
                if ((i + height + 2) >= N)
                {
                    mul_matrix[19] = 0;
                }
                else
                {
                    mul_matrix[19] = X[i + height + 2];
                }
                if ((i + 2 * height - 2) >= N)
                {
                    mul_matrix[20] = 0;
                }
                else
                {
                    mul_matrix[20] = X[i + 2 * height - 2];
                }
                if ((i + 2 * height - 1) >= N)
                {
                    mul_matrix[21] = 0;
                }
                else
                {
                    mul_matrix[21] = X[i + 2 * height - 1];
                }
                if ((i + 2 * height) >= N)
                {
                    mul_matrix[22] = 0;
                }
                else
                {
                    mul_matrix[22] = X[i + 2 * height];
                }
                if ((i + 2 * height + 1) >= N)
                {
                    mul_matrix[23] = 0;
                }
                else
                {
                    mul_matrix[23] = X[i + 2 * height + 1];
                }
                if ((i + 2 * height + 2) >= N)
                {
                    mul_matrix[24] = 0;
                }
                else
                {
                    mul_matrix[24] = X[i + 2 * height + 2];
                }

                rho[i] = b[i] - A[i, 0] * mul_matrix[0] - A[i, 1] * mul_matrix[1] - A[i, 2] * mul_matrix[2] - A[i, 3] * mul_matrix[3]
                         - A[i, 4] * mul_matrix[4] - A[i, 5] * mul_matrix[5] - A[i, 6] * mul_matrix[6] - A[i, 7] * mul_matrix[7] - A[i, 8] * mul_matrix[8]
                         - A[i, 9] * mul_matrix[9] - A[i, 10] * mul_matrix[10] - A[i, 11] * mul_matrix[11] - A[i, 12] * mul_matrix[12]
                         - A[i, 13] * mul_matrix[13] - A[i, 14] * mul_matrix[14] - A[i, 15] * mul_matrix[15] - A[i, 16] * mul_matrix[16]
                         - A[i, 17] * mul_matrix[17] - A[i, 18] * mul_matrix[18] - A[i, 19] * mul_matrix[19] - A[i, 20] * mul_matrix[20]
                         - A[i, 21] * mul_matrix[21] - A[i, 22] * mul_matrix[22] - A[i, 23] * mul_matrix[23] - A[i, 24] * mul_matrix[24];


            }

            //beta = sqrt(dot_product(rho, rho))
            beta = 0;
            for (int i = 0; i < N; i++)
            {
                beta = beta + rho[i] * rho[i];
            }
            beta = Math.Sqrt(beta);

            //V(1:N, 1) = rho/beta
            for (int i = 0; i < N; i++)
            {
                V[i, 0] = (float)(rho[i] / beta);
            }

            //errtol = 0.5**Tol
            errtol = Math.Pow(0.5, Tol);

            k = -1;

            //GMRES iterations.
            //if the residual is small enough or iteration times is greater than the maximum, then stop iterating.
            //norm_rho = sqrt(dot_product(rho, rho))
            norm_rho = 0;
            for (int i = 0; i < N; i++)
            {
                norm_rho = norm_rho + rho[i] * rho[i];
            }
            norm_rho = (float)(Math.Sqrt(norm_rho));

            //norm_b = sqrt(dot_product(B, B))
            norm_b = 0;
            for (int i = 0; i < N; i++)
            {
                norm_b = norm_b + b[i] * b[i];
            }
            norm_b = (float)(Math.Sqrt(norm_b));

            while ((norm_rho / norm_b > errtol) && (k < (kmax - 1)))
            {

                k = k + 1;

                //multiply A and V(1:N, k)
                for (int i = 0; i < N; i++)
                {
                    if ((i - 2 * height - 2) < 0)
                    {
                        mul_matrix[0] = 0;
                    }
                    else
                    {
                        mul_matrix[0] = V[i - 2 * height - 2, k];
                    }
                    if ((i - 2 * height - 1) < 0)
                    {
                        mul_matrix[1] = 0;
                    }
                    else
                    {
                        mul_matrix[1] = V[i - 2 * height - 1, k];
                    }
                    if ((i - 2 * height) < 0)
                    {
                        mul_matrix[2] = 0;
                    }
                    else
                    {
                        mul_matrix[2] = V[i - 2 * height, k];
                    }
                    if ((i - 2 * height + 1) < 0)
                    {
                        mul_matrix[3] = 0;
                    }
                    else
                    {
                        mul_matrix[3] = V[i - 2 * height + 1, k];
                    }
                    if ((i - 2 * height + 2) < 0)
                    {
                        mul_matrix[4] = 0;
                    }
                    else
                    {
                        mul_matrix[4] = V[i - 2 * height + 2, k];
                    }
                    if ((i - height - 2) < 0)
                    {
                        mul_matrix[5] = 0;
                    }
                    else
                    {
                        mul_matrix[5] = V[i - height - 2, k];
                    }
                    if ((i - height - 1) < 0)
                    {
                        mul_matrix[6] = 0;
                    }
                    else
                    {
                        mul_matrix[6] = V[i - height - 1, k];
                    }
                    if ((i - height) < 0)
                    {
                        mul_matrix[7] = 0;
                    }
                    else
                    {
                        mul_matrix[7] = V[i - height, k];
                    }
                    if ((i - height + 1) < 0)
                    {
                        mul_matrix[8] = 0;
                    }
                    else
                    {
                        mul_matrix[8] = V[i - height + 1, k];
                    }
                    if ((i - height + 2) < 0)
                    {
                        mul_matrix[9] = 0;
                    }
                    else
                    {
                        mul_matrix[9] = V[i - height + 2, k];
                    }
                    if ((i - 2) < 0)
                    {
                        mul_matrix[10] = 0;
                    }
                    else
                    {
                        mul_matrix[10] = V[i - 2, k];
                    }
                    if ((i - 1) < 0)
                    {
                        mul_matrix[11] = 0;
                    }
                    else
                    {
                        mul_matrix[11] = V[i - 1, k];
                    }
                    mul_matrix[12] = V[i, k];
                    if ((i + 1) >= N)
                    {
                        mul_matrix[13] = 0;
                    }
                    else
                    {
                        mul_matrix[13] = V[i + 1, k];
                    }
                    if ((i + 2) >= N)
                    {
                        mul_matrix[14] = 0;
                    }
                    else
                    {
                        mul_matrix[14] = V[i + 2, k];
                    }
                    if ((i + height - 2) >= N)
                    {
                        mul_matrix[15] = 0;
                    }
                    else
                    {
                        mul_matrix[15] = V[i + height - 2, k];
                    }
                    if ((i + height - 1) >= N)
                    {
                        mul_matrix[16] = 0;
                    }
                    else
                    {
                        mul_matrix[16] = V[i + height - 1, k];
                    }
                    if ((i + height) >= N)
                    {
                        mul_matrix[17] = 0;
                    }
                    else
                    {
                        mul_matrix[17] = V[i + height, k];
                    }
                    if ((i + height + 1) >= N)
                    {
                        mul_matrix[18] = 0;
                    }
                    else
                    {
                        mul_matrix[18] = V[i + height + 1, k];
                    }
                    if ((i + height + 2) >= N)
                    {
                        mul_matrix[19] = 0;
                    }
                    else
                    {
                        mul_matrix[19] = V[i + height + 2, k];
                    }
                    if ((i + 2 * height - 2) >= N)
                    {
                        mul_matrix[20] = 0;
                    }
                    else
                    {
                        mul_matrix[20] = V[i + 2 * height - 2, k];
                    }
                    if ((i + 2 * height - 1) >= N)
                    {
                        mul_matrix[21] = 0;
                    }
                    else
                    {
                        mul_matrix[21] = V[i + 2 * height - 1, k];
                    }
                    if ((i + 2 * height) >= N)
                    {
                        mul_matrix[22] = 0;
                    }
                    else
                    {
                        mul_matrix[22] = V[i + 2 * height, k];
                    }
                    if ((i + 2 * height + 1) >= N)
                    {
                        mul_matrix[23] = 0;
                    }
                    else
                    {
                        mul_matrix[23] = V[i + 2 * height + 1, k];
                    }
                    if ((i + 2 * height + 2) >= N)
                    {
                        mul_matrix[24] = 0;
                    }
                    else
                    {
                        mul_matrix[24] = V[i + 2 * height + 2, k];
                    }

                    V[i, k + 1] = A[i, 0] * mul_matrix[0] + A[i, 1] * mul_matrix[1] + A[i, 2] * mul_matrix[2] + A[i, 3] * mul_matrix[3]
                             + A[i, 4] * mul_matrix[4] + A[i, 5] * mul_matrix[5] + A[i, 6] * mul_matrix[6] + A[i, 7] * mul_matrix[7] + A[i, 8] * mul_matrix[8]
                             + A[i, 9] * mul_matrix[9] + A[i, 10] * mul_matrix[10] + A[i, 11] * mul_matrix[11] + A[i, 12] * mul_matrix[12]
                             + A[i, 13] * mul_matrix[13] + A[i, 14] * mul_matrix[14] + A[i, 15] * mul_matrix[15] + A[i, 16] * mul_matrix[16]
                             + A[i, 17] * mul_matrix[17] + A[i, 18] * mul_matrix[18] + A[i, 19] * mul_matrix[19] + A[i, 20] * mul_matrix[20]
                             + A[i, 21] * mul_matrix[21] + A[i, 22] * mul_matrix[22] + A[i, 23] * mul_matrix[23] + A[i, 24] * mul_matrix[24];



                }

                //Modified Gram-Schmidt method to find orthogonal basis
                //do i = 1, k
                //H(i:i, k:k) = matmul( transpose( V(1:N, i:i)), V(1:N, k+1:k+1))
                //V(1:N, k+1) = V(1:N, k+1) - H(i, k)* V(1:N, i)
                //end do 
                for (int i = 0; i <= k; i++)
                {
                    float temp = 0;
                    for (int j = 0; j < N; j++)
                    {
                        temp = temp + V[j, i] * V[j, k + 1];
                    }
                    H[i, k] = temp;

                    for (int j = 0; j < N; j++)
                    {
                        V[j, k + 1] = V[j, k + 1] - H[i, k] * V[j, i];
                    }
                }

                //H(k+1, k) = sqrt(dot_product(V(1:N, k+1), V(1:N, k+1)))
                float temp4 = 0;
                for (int i = 0; i < N; i++)
                {
                    temp4 = temp4 + V[i, k + 1] * V[i, k + 1];
                }
                H[k + 1, k] = (float)(Math.Sqrt(temp4));

                //V(1:N, k+1) = V(1:N, k+1)/H(k+1, k)
                for (int i = 0; i < N; i++)
                {
                    V[i, k + 1] = V[i, k + 1] / H[k + 1, k];
                }

                //Solve the least square problem H_H*y=beta*e by QR factorization for H_H
                //such that H_H=Q*R
                //allocate(y(k))
                //if (k .eq. 1) then 
                //y(1) = beta*H(1,1)/(H(1,1)**2+H(2,1)**2)
                float[] y = new float[k + 1];
                if (k == 0)
                {
                    y[0] = (float)(beta * H[0, 0] / (Math.Pow(H[0, 0], 2) + Math.Pow(H[1, 0], 2)));
                }
                else
                {
                    //allocate(G(k+1, k+1))     ! G is the givens rotation matrix
                    //allocate(Q(k+1, k+1))     ! Q part of QR factorization
                    //allocate(R(k+1, k))       ! R part of QR factorization
                    //allocate(eye(k+1))
                    //allocate(H_H(k+1, k))
                    float[,] G = new float[k + 2, k + 2];
                    float[,] Q = new float[k + 2, k + 2];
                    float[,] R = new float[k + 2, k + 1];
                    float[] eye = new float[k + 2];
                    float[,] H_H = new float[k + 2, k + 1];

                    //eye(1) = 1
                    //eye(2:k+1) = 0
                    eye[0] = 1;

                    //initialize Q.
                    for (int i = 0; i < k + 2; i++)
                    {
                        Q[i, i] = 1;
                    }

                    //H_H = H(1:k+1, 1:k)
                    for (int i = 0; i < k + 2; i++)
                    {
                        for (int j = 0; j < k + 1; j++)
                        {
                            H_H[i, j] = H[i, j];
                        }
                    }

                    //QR factorization of H_H
                    for (int i = 0; i <= k; i++)
                    {
                        //initialize G with an eye matrix.
                        for (int m = 0; m < k + 2; m++)
                        {
                            for (int n = 0; n < k + 2; n++)
                            {
                                G[m, n] = 0;
                            }
                        }

                        for (int j = 0; j < k + 2; j++)
                        {
                            G[j, j] = 1;
                            //Q[j, j] = 1;
                        }

                        //Use Given rotations method to do QR factorization.
                        c = (float)(H_H[i, i] / Math.Sqrt(Math.Pow(H_H[i, i], 2) + Math.Pow(H_H[i + 1, i], 2)));
                        s = (float)((-1) * H_H[i + 1, i] / Math.Sqrt(Math.Pow(H_H[i + 1, i], 2) + Math.Pow(H_H[i, i], 2)));
                        G[i, i] = c;
                        G[i, i + 1] = s;
                        G[i + 1, i] = (-1) * s;
                        G[i + 1, i + 1] = c;

                        //R = matmul(transpose(G), H_H)
                        for (int m = 0; m < k + 2; m++)
                        {
                            for (int n = 0; n < k + 1; n++)
                            {
                                float temp1 = 0;
                                for (int count = 0; count < k + 2; count++)
                                {
                                    temp1 = temp1 + G[count, m] * H_H[count, n];
                                }
                                R[m, n] = temp1;
                            }
                        }

                        //Q = matmul(Q, G)
                        for (int m = 0; m < k + 2; m++)
                        {
                            for (int n = 0; n < k + 2; n++)
                            {
                                float temp2 = 0;
                                for (int count = 0; count < k + 2; count++)
                                {
                                    temp2 = temp2 + Q[m, count] * G[count, n];
                                }
                                Q[m, n] = temp2;
                            }
                        }

                        //H_H = R
                        for (int m = 0; m < k + 2; m++)
                        {
                            for (int n = 0; n < k + 1; n++)
                            {
                                H_H[m, n] = R[m, n];
                            }
                        }
                    }

                    //solve R*y = Q'*beta*e
                    float[] rhy = new float[k + 2];

                    //rhy = beta * matmul(transpose(Q), eye)
                    for (int i = 0; i < k + 2; i++)
                    {
                        rhy[i] = (float)(beta * Q[0, i]);
                    }
                    y[k] = rhy[k] / R[k, k];

                    for (int i = 0; i <= k - 1; i++)
                    {
                        y[k - i - 1] = rhy[k - i - 1];
                        for (int j = 0; j <= i; j++)
                        {
                            y[k - i - 1] = y[k - i - 1] - R[k - i - 1, k - j] * y[k - j];
                        }
                        y[k - i - 1] = y[k - i - 1] / R[k - i - 1, k - i - 1];
                    }

                }

                //X = X0 + matmul(V(1:N, 1:k), y)      !! new X of this iteration
                for (int i = 0; i < N; i++)
                {
                    float temp3 = 0;
                    for (int j = 0; j <= k; j++)
                    {
                        temp3 = temp3 + V[i, j] * y[j];
                    }
                    X[i] = X0[i] + temp3;
                }

                //multiply A and X to compute rho
                for (int i = 0; i < N; i++)
                {
                    if ((i - 2 * height - 2) < 0)
                    {
                        mul_matrix[0] = 0;
                    }
                    else
                    {
                        mul_matrix[0] = X[i - 2 * height - 2];
                    }
                    if ((i - 2 * height - 1) < 0)
                    {
                        mul_matrix[1] = 0;
                    }
                    else
                    {
                        mul_matrix[1] = X[i - 2 * height - 1];
                    }
                    if ((i - 2 * height) < 0)
                    {
                        mul_matrix[2] = 0;
                    }
                    else
                    {
                        mul_matrix[2] = X[i - 2 * height];
                    }
                    if ((i - 2 * height + 1) < 0)
                    {
                        mul_matrix[3] = 0;
                    }
                    else
                    {
                        mul_matrix[3] = X[i - 2 * height + 1];
                    }
                    if ((i - 2 * height + 2) < 0)
                    {
                        mul_matrix[4] = 0;
                    }
                    else
                    {
                        mul_matrix[4] = X[i - 2 * height + 2];
                    }
                    if ((i - height - 2) < 0)
                    {
                        mul_matrix[5] = 0;
                    }
                    else
                    {
                        mul_matrix[5] = X[i - height - 2];
                    }
                    if ((i - height - 1) < 0)
                    {
                        mul_matrix[6] = 0;
                    }
                    else
                    {
                        mul_matrix[6] = X[i - height - 1];
                    }
                    if ((i - height) < 0)
                    {
                        mul_matrix[7] = 0;
                    }
                    else
                    {
                        mul_matrix[7] = X[i - height];
                    }
                    if ((i - height + 1) < 0)
                    {
                        mul_matrix[8] = 0;
                    }
                    else
                    {
                        mul_matrix[8] = X[i - height + 1];
                    }
                    if ((i - height + 2) < 0)
                    {
                        mul_matrix[9] = 0;
                    }
                    else
                    {
                        mul_matrix[9] = X[i - height + 2];
                    }
                    if ((i - 2) < 0)
                    {
                        mul_matrix[10] = 0;
                    }
                    else
                    {
                        mul_matrix[10] = X[i - 2];
                    }
                    if ((i - 1) < 0)
                    {
                        mul_matrix[11] = 0;
                    }
                    else
                    {
                        mul_matrix[11] = X[i - 1];
                    }
                    mul_matrix[12] = X[i];
                    if ((i + 1) >= N)
                    {
                        mul_matrix[13] = 0;
                    }
                    else
                    {
                        mul_matrix[13] = X[i + 1];
                    }
                    if ((i + 2) >= N)
                    {
                        mul_matrix[14] = 0;
                    }
                    else
                    {
                        mul_matrix[14] = X[i + 2];
                    }
                    if ((i + height - 2) >= N)
                    {
                        mul_matrix[15] = 0;
                    }
                    else
                    {
                        mul_matrix[15] = X[i + height - 2];
                    }
                    if ((i + height - 1) >= N)
                    {
                        mul_matrix[16] = 0;
                    }
                    else
                    {
                        mul_matrix[16] = X[i + height - 1];
                    }
                    if ((i + height) >= N)
                    {
                        mul_matrix[17] = 0;
                    }
                    else
                    {
                        mul_matrix[17] = X[i + height];
                    }
                    if ((i + height + 1) >= N)
                    {
                        mul_matrix[18] = 0;
                    }
                    else
                    {
                        mul_matrix[18] = X[i + height + 1];
                    }
                    if ((i + height + 2) >= N)
                    {
                        mul_matrix[19] = 0;
                    }
                    else
                    {
                        mul_matrix[19] = X[i + height + 2];
                    }
                    if ((i + 2 * height - 2) >= N)
                    {
                        mul_matrix[20] = 0;
                    }
                    else
                    {
                        mul_matrix[20] = X[i + 2 * height - 2];
                    }
                    if ((i + 2 * height - 1) >= N)
                    {
                        mul_matrix[21] = 0;
                    }
                    else
                    {
                        mul_matrix[21] = X[i + 2 * height - 1];
                    }
                    if ((i + 2 * height) >= N)
                    {
                        mul_matrix[22] = 0;
                    }
                    else
                    {
                        mul_matrix[22] = X[i + 2 * height];
                    }
                    if ((i + 2 * height + 1) >= N)
                    {
                        mul_matrix[23] = 0;
                    }
                    else
                    {
                        mul_matrix[23] = X[i + 2 * height + 1];
                    }
                    if ((i + 2 * height + 2) >= N)
                    {
                        mul_matrix[24] = 0;
                    }
                    else
                    {
                        mul_matrix[24] = X[i + 2 * height + 2];
                    }

                    rho[i] = b[i] - A[i, 0] * mul_matrix[0] - A[i, 1] * mul_matrix[1] - A[i, 2] * mul_matrix[2] - A[i, 3] * mul_matrix[3]
                             - A[i, 4] * mul_matrix[4] - A[i, 5] * mul_matrix[5] - A[i, 6] * mul_matrix[6] - A[i, 7] * mul_matrix[7] - A[i, 8] * mul_matrix[8]
                             - A[i, 9] * mul_matrix[9] - A[i, 10] * mul_matrix[10] - A[i, 11] * mul_matrix[11] - A[i, 12] * mul_matrix[12]
                             - A[i, 13] * mul_matrix[13] - A[i, 14] * mul_matrix[14] - A[i, 15] * mul_matrix[15] - A[i, 16] * mul_matrix[16]
                             - A[i, 17] * mul_matrix[17] - A[i, 18] * mul_matrix[18] - A[i, 19] * mul_matrix[19] - A[i, 20] * mul_matrix[20]
                             - A[i, 21] * mul_matrix[21] - A[i, 22] * mul_matrix[22] - A[i, 23] * mul_matrix[23] - A[i, 24] * mul_matrix[24];


                }

                //recompute norm_rho.
                //norm_rho = sqrt(dot_product(rho, rho))
                norm_rho = 0;
                for (int i = 0; i < N; i++)
                {
                    norm_rho = norm_rho + rho[i] * rho[i];
                }
                norm_rho = (float)(Math.Sqrt(norm_rho));

            }


            if (norm_rho / norm_b > errtol && iterationCount < MaxIterationNo)
            {
                GMRES(A, b, height, width, Tol, X);
            }

        }
        #endregion GMRES
        //--------------------------------------------------------------------------------------------------------------------------------------------
        #region getLaplacian
        int GetLaplacian(float[,] Image, float[,] Iconstmap, float[,] Iconstvals, int tempHeight, int tempWidth)
        {
            //int tlen = Single_height * Single_width * 9 * 9;
            int tlen = tempHeight * tempWidth * 9 * 9;
            int[] row_indicy = new int[tlen];
            int[] col_indicy = new int[tlen];
            float[] values = new float[tlen];
            int win_size = 1;
            int neb_size = (win_size * 2 + 1) * (win_size * 2 + 1);
            //int img_height = Single_height;
            //int img_width = Single_width;
            int img_height = tempHeight;
            int img_width = tempWidth;
            int img_size = img_width * img_height;
            float epsilon = 0.0000001F;
            int length = 0;
            int m, n, num, i, j;

            //compute the indicy matrix.
            int[,] inds_Matrix = new int[img_height, img_width];
            int indsM = 0;
            for (m = 0; m < img_width; m++)
            {
                for (n = 0; n < img_height; n++)
                {
                    inds_Matrix[n, m] = indsM;
                    indsM++;
                }
            }

            //variables.
            int[] win_inds = new int[neb_size];
            float[] winI = new float[neb_size];
            float win_mu;
            float win_var;
            float[,] tvals = new float[neb_size, neb_size];

            //compute row_inds, col_inds, and vals.
            for (m = win_size; m < img_width - win_size; m++)
            {
                for (n = win_size; n < img_height - win_size; n++)
                {

                    //set win_inds and winI.
                    for (num = 0; num < neb_size; num++)
                    {
                        win_inds[num] = inds_Matrix[n - win_size + num % 3, m - win_size + num / 3];
                        winI[num] = Image[n - win_size + num % 3, m - win_size + num / 3] / 255;
                    }

                    //count win_mu
                    float data_tmp = 0;
                    float temp1 = 0;
                    for (j = 0; j < neb_size; j++)
                    {
                        data_tmp = data_tmp + winI[j];
                        temp1 = temp1 + winI[j] * winI[j];
                    }
                    win_mu = data_tmp / neb_size;

                    win_var = 1 / (temp1 / neb_size - win_mu * win_mu + epsilon / neb_size);

                    //compute winI - win_mu.
                    for (num = 0; num < neb_size; num++)
                    {
                        winI[num] = winI[num] - win_mu;
                    }

                    //compute tvals.
                    for (i = 0; i < neb_size; i++)
                    {
                        for (j = 0; j < neb_size; j++)
                        {
                            tvals[i, j] = (1 + winI[i] * winI[j]) / neb_size;
                        }
                    }

                    //put the win_inds and tvals into row_inds, col_inds and vals.
                    for (int Nm = length; Nm < neb_size * neb_size + length; Nm++)
                    {
                        row_indicy[Nm] = win_inds[(Nm - length) % neb_size];
                        col_indicy[Nm] = win_inds[(Nm - length) / neb_size];
                        values[Nm] = tvals[(Nm - length) % neb_size, (Nm - length) / neb_size];
                    }

                    length = length + neb_size * neb_size;

                }
            }

            //construct matrix A.
            //compute sum(A, 2)
            float[] tmp_value = new float[img_size];
            for (j = 0; j < length; j++)
            {
                tmp_value[row_indicy[j]] = tmp_value[row_indicy[j]] + values[j];
            }

            for (j = 0; j < length; j++)
            {
                int delta = col_indicy[j] - row_indicy[j];

                if (delta == ((-2) * img_height - 2))
                {
                    A[row_indicy[j], 0] = A[row_indicy[j], 0] - values[j];
                }
                else if (delta == ((-2) * img_height - 1))
                {
                    A[row_indicy[j], 1] = A[row_indicy[j], 1] - values[j];
                }
                else if (delta == (-2) * img_height)
                {
                    A[row_indicy[j], 2] = A[row_indicy[j], 2] - values[j];
                }
                else if (delta == (-2) * img_height + 1)
                {
                    A[row_indicy[j], 3] = A[row_indicy[j], 3] - values[j];
                }
                else if (delta == (-2) * img_height + 2)
                {
                    A[row_indicy[j], 4] = A[row_indicy[j], 4] - values[j];
                }
                else if (delta == (-1) * img_height - 2)
                {
                    A[row_indicy[j], 5] = A[row_indicy[j], 5] - values[j];
                }
                else if (delta == (-1) * img_height - 1)
                {
                    A[row_indicy[j], 6] = A[row_indicy[j], 6] - values[j];
                }
                else if (delta == (-1) * img_height)
                {
                    A[row_indicy[j], 7] = A[row_indicy[j], 7] - values[j];
                }
                else if (delta == (-1) * img_height + 1)
                {
                    A[row_indicy[j], 8] = A[row_indicy[j], 8] - values[j];
                }
                else if (delta == (-1) * img_height + 2)
                {
                    A[row_indicy[j], 9] = A[row_indicy[j], 9] - values[j];
                }
                else if (delta == -2)
                {
                    A[row_indicy[j], 10] = A[row_indicy[j], 10] - values[j];
                }
                else if (delta == -1)
                {
                    A[row_indicy[j], 11] = A[row_indicy[j], 11] - values[j];
                }
                else if (delta == 0)
                {
                    A[row_indicy[j], 12] = A[row_indicy[j], 12] - values[j];
                }
                else if (delta == 1)
                {
                    A[row_indicy[j], 13] = A[row_indicy[j], 13] - values[j];
                }
                else if (delta == 2)
                {
                    A[row_indicy[j], 14] = A[row_indicy[j], 14] - values[j];
                }
                else if (delta == img_height - 2)
                {
                    A[row_indicy[j], 15] = A[row_indicy[j], 15] - values[j];
                }
                else if (delta == img_height - 1)
                {
                    A[row_indicy[j], 16] = A[row_indicy[j], 16] - values[j];
                }
                else if (delta == img_height)
                {
                    A[row_indicy[j], 17] = A[row_indicy[j], 17] - values[j];
                }
                else if (delta == img_height + 1)
                {
                    A[row_indicy[j], 18] = A[row_indicy[j], 18] - values[j];
                }
                else if (delta == img_height + 2)
                {
                    A[row_indicy[j], 19] = A[row_indicy[j], 19] - values[j];
                }
                else if (delta == 2 * img_height - 2)
                {
                    A[row_indicy[j], 20] = A[row_indicy[j], 20] - values[j];
                }
                else if (delta == 2 * img_height - 1)
                {
                    A[row_indicy[j], 21] = A[row_indicy[j], 21] - values[j];
                }
                else if (delta == 2 * img_height)
                {
                    A[row_indicy[j], 22] = A[row_indicy[j], 22] - values[j];
                }
                else if (delta == 2 * img_height + 1)
                {
                    A[row_indicy[j], 23] = A[row_indicy[j], 23] - values[j];
                }
                else if (delta == 2 * img_height + 2)
                {
                    A[row_indicy[j], 24] = A[row_indicy[j], 24] - values[j];
                }
                else
                {
                    //bug = true;
                }

            }


            for (i = 0; i < img_size; i++)
            {
                A[i, 12] = tmp_value[i] + A[i, 12] + lambda * Iconstmap[i % img_height, i / img_height];
            }

            //construct b.
            for (i = 0; i < img_size; i++)
            {
                b[i] = lambda * Iconstvals[i % img_height, i / img_height];
            }

            return length;

        }
        #endregion getLaplacian
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

 
    }
}
