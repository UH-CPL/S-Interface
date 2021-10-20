using System;
using System.Collections.Generic;
using System.Text;

namespace deformTracker
{
    class TrackingLogic
    {

        const float PI = (float)3.1415926;   
        int ParticleNumber = 100;
        const int LOWEST_TEMP = 0;
        float[] rotationAngel = new float[13] { -PI/4, -PI*5/24, -PI*1/6, -PI*1/8, -PI/12, -PI/24, 
                                                            0, PI/24, PI/12, PI/8, PI/6, 5*PI/24, PI/4};

        //int Amplify_x = 1;
        //int Amplify_y = 1;
        int Range_x = 5;
        int Range_y = 2;
        double[] Particle_x;
        double[] Particle_y;
        float[] similarityScore;
        float[] similarityScoreOfInitialT;
        int[] rotationIndexStore;
        int[] rotationIndexStoreOfInitialT;
        float trackerFailThreshold = 0.75f;


        public TrackingLogic()
        {
            Particle_x = new double[ParticleNumber];
            Particle_y = new double[ParticleNumber];
            similarityScore = new float[ParticleNumber];
            similarityScoreOfInitialT = new float[ParticleNumber];
            rotationIndexStore = new int[ParticleNumber];
            rotationIndexStoreOfInitialT = new int[ParticleNumber];

        }

        public void computeIndividualState(float[] normalizedImageData, SingleTracker[,] singleTrackerArray, int trackersColNum, int trackersRowNum, ImageInfo imageInfo, int localSearchRange)
        {
            
            generateRamdomNums();

            for (int i = 0; i < trackersColNum; i++)
            {
                for (int j = 0; j < trackersRowNum; j++)
                {
                    computeIndividualBestState(normalizedImageData, singleTrackerArray[i, j], imageInfo, localSearchRange);
                }
            }

        }

        public void computeGlobalIndividualState(float[] normalizedImageData, SingleTracker[,] singleTrackerArray, int trackersColNum, int trackersRowNum, ImageInfo imageInfo, int globalSearchRange)
        {
            bool local = false;
            generateRamdomNums();

            for (int i = 0; i < trackersColNum; i++)
            {
                for (int j = 0; j < trackersRowNum; j++)
                {
                    computeGlobalIndividualBestState(normalizedImageData, singleTrackerArray[i, j], imageInfo, globalSearchRange);
                }
            }

        }

        private void computeIndividualBestState(float[] normalizedImageData, SingleTracker singleTracker, ImageInfo imageInfo, int SearchRange)
        {
            //float[] similarityScore = new float[ParticleNumber];
            //int[] rotationIndexStore = new int[ParticleNumber];
            int colorbands = imageInfo.colorbands;
            int stride = imageInfo.stride;
            int dataLength = imageInfo.dataLength;
            int singleTrackerHeight = singleTracker.height;
            int singleTrackerWidth = singleTracker.width;
            float[,] tempCrop = new float[singleTrackerHeight, singleTrackerWidth];
            int halfWidth = singleTracker.width / 2;
            int halfHeight = singleTracker.height / 2;
            int startIndex;
            int endIndex;
            int currentParticleX;
            int currentParticleY;


            for (int ParticleIndex = 0; ParticleIndex < ParticleNumber; ParticleIndex++)
            {
               
                currentParticleX = (int)(singleTracker.centerX + SearchRange * singleTracker.searchRangeX * Particle_x[ParticleIndex]);
                currentParticleY = (int)(singleTracker.centerY + SearchRange * singleTracker.searchRangeY * Particle_y[ParticleIndex]);
                             
                //detect bad particles
                int addr = currentParticleX * imageInfo.colorbands + currentParticleY * imageInfo.stride;
                if (currentParticleX <= 0 || currentParticleX > imageInfo.imageWidth || currentParticleY <= 0 || currentParticleY > imageInfo.imageHeight)
                {
                    currentParticleX = singleTracker.centerX;
                    currentParticleY = singleTracker.centerY;
                }
                else if (addr < 0 || addr > imageInfo.dataLength )
                {
                    currentParticleX = singleTracker.centerX;
                    currentParticleY = singleTracker.centerY;
                }
                else if (addr > 0 && addr < imageInfo.dataLength)
                {
                    if (normalizedImageData[addr] < 10)
                    {
                    currentParticleX = singleTracker.centerX;
                    currentParticleY = singleTracker.centerY;
                    }
                }
          

                int center_x = currentParticleX;
                int center_y = currentParticleY;
                computeStartEndIndex(out startIndex, out endIndex, singleTracker);
                for (int Numb = startIndex; Numb <= endIndex; Numb++)
                {
      
                    float angle = rotationAngel[Numb];
                    double cosine = Math.Cos(angle);
                    double sinine = Math.Sin(angle);
                    int new_x, new_y;
                    int theAddress, h, w;

                    
                    for (int m = currentParticleX - halfWidth; m < currentParticleX + halfWidth; m++)
                    {
                        for (int n = currentParticleY - halfHeight; n < currentParticleY + halfHeight; n++)
                        {

                            new_x = (int)((m - center_x) * cosine - (n - center_y) * sinine);
                            new_y = (int)((m - center_x) * sinine + (n - center_y) * cosine);

                            theAddress = (currentParticleX + new_x) * colorbands + (currentParticleY + new_y) * stride;
                            h = n - currentParticleY + halfHeight;
                            w = m - currentParticleX + halfWidth;
                            if (h < singleTrackerHeight && w < singleTrackerWidth)
                            {
                                if (theAddress >= dataLength || theAddress < 0)
                                {
                                    tempCrop[h, w] = 0;
                                }
                                else
                                {
                                    tempCrop[h, w] = normalizedImageData[theAddress];
                                }
                            }
                        }
                    }
                    

     

                        //current score computation.
                        if (Numb == startIndex)
                        {
                            similarityScore[ParticleIndex] = computeCorrelation(singleTracker.currentTemplate, tempCrop, singleTrackerHeight, singleTrackerWidth);
                            similarityScoreOfInitialT[ParticleIndex] = computeCorrelation(singleTracker.originalTemplate, tempCrop, singleTrackerHeight, singleTrackerWidth);
                            //RotationScore1[ParticleIndex, Ind] = angle;
                            rotationIndexStore[ParticleIndex] = Numb;
                        }
                        else
                        {
                            float score = computeCorrelation(singleTracker.currentTemplate, tempCrop, singleTrackerHeight, singleTrackerWidth);
                            float score2 = computeCorrelation(singleTracker.originalTemplate, tempCrop, singleTrackerHeight, singleTrackerWidth);
                            if (score > similarityScore[ParticleIndex])
                            {
                                similarityScore[ParticleIndex] = score;
                                //RotationScore1[ParticleIndex] = angle;
                                rotationIndexStore[ParticleIndex] = Numb;
                            }
                            if (score2 > similarityScoreOfInitialT[ParticleIndex])
                            {
                                similarityScoreOfInitialT[ParticleIndex] = score2;
                                rotationIndexStoreOfInitialT[ParticleIndex] = Numb;
                            }
                        }

                }                                  
            }

            pickBestParticle(Particle_x, Particle_y, similarityScore, rotationIndexStore, similarityScoreOfInitialT, rotationIndexStoreOfInitialT, singleTracker, imageInfo, SearchRange);

            //int iterationCounter = 1;
            //int iterationMax = 10;
            updateBestROI(singleTracker, normalizedImageData, colorbands, stride, dataLength);
        }




        private void computeGlobalIndividualBestState(float[] normalizedImageData, SingleTracker singleTracker, ImageInfo imageInfo, int SearchRange)
        {
            //float[] similarityScore = new float[ParticleNumber];
            //int[] rotationIndexStore = new int[ParticleNumber];
            int colorbands = imageInfo.colorbands;
            int stride = imageInfo.stride;
            int dataLength = imageInfo.dataLength;
            int singleTrackerHeight = singleTracker.height;
            int singleTrackerWidth = singleTracker.width;
            float[,] tempCrop = new float[singleTrackerHeight, singleTrackerWidth];
            int halfWidth = singleTracker.width / 2;
            int halfHeight = singleTracker.height / 2;
            int startIndex;
            int endIndex;
            int currentParticleX;
            int currentParticleY;


            for (int ParticleIndex = 0; ParticleIndex < ParticleNumber; ParticleIndex++)
            {

                currentParticleX = (int)( imageInfo.imageWidth / 2 + SearchRange * singleTracker.searchRangeX * Particle_x[ParticleIndex]);
                currentParticleY = (int)( imageInfo.imageHeight / 2 + SearchRange * singleTracker.searchRangeY * Particle_y[ParticleIndex]);

                //detect bad particles
                int addr = currentParticleX * imageInfo.colorbands + currentParticleY * imageInfo.stride;
                if (currentParticleX <= 0 || currentParticleX > imageInfo.imageWidth || currentParticleY <= 0 || currentParticleY > imageInfo.imageHeight)
                {
                    currentParticleX = imageInfo.imageWidth / 2;
                    currentParticleY = imageInfo.imageHeight / 2;
                }
                else if (addr < 0 || addr > imageInfo.dataLength || normalizedImageData[addr] < 10)
                {
                    currentParticleX = imageInfo.imageWidth / 2;
                    currentParticleY = imageInfo.imageHeight / 2;
                }


                int center_x = currentParticleX;
                int center_y = currentParticleY;
                startIndex = 4;
                endIndex = 8;
                //computeStartEndIndex(out startIndex, out endIndex, singleTracker);
                for (int Numb = startIndex; Numb <= endIndex; Numb++)
                {

                    float angle = rotationAngel[Numb];
                    double cosine = Math.Cos(angle);
                    double sinine = Math.Sin(angle);
                    int new_x, new_y;
                    int theAddress, h, w;


                    for (int m = currentParticleX - halfWidth; m < currentParticleX + halfWidth; m++)
                    {
                        for (int n = currentParticleY - halfHeight; n < currentParticleY + halfHeight; n++)
                        {

                            new_x = (int)((m - center_x) * cosine - (n - center_y) * sinine);
                            new_y = (int)((m - center_x) * sinine + (n - center_y) * cosine);

                            theAddress = (currentParticleX + new_x) * colorbands + (currentParticleY + new_y) * stride;
                            h = n - currentParticleY + halfHeight;
                            w = m - currentParticleX + halfWidth;
                            if (h < singleTrackerHeight && w < singleTrackerWidth)
                            {
                                if (theAddress >= dataLength || theAddress < 0)
                                {
                                    tempCrop[h, w] = 0;
                                }
                                else
                                {
                                    tempCrop[h, w] = normalizedImageData[theAddress];
                                }
                            }
                        }
                    }




                    //current score computation.
                    if (Numb == startIndex)
                    {
                        similarityScore[ParticleIndex] = computeCorrelation(singleTracker.currentTemplate, tempCrop, singleTrackerHeight, singleTrackerWidth);
                        similarityScoreOfInitialT[ParticleIndex] = computeCorrelation(singleTracker.originalTemplate, tempCrop, singleTrackerHeight, singleTrackerWidth);
                        //RotationScore1[ParticleIndex, Ind] = angle;
                        rotationIndexStore[ParticleIndex] = Numb;
                    }
                    else
                    {
                        float score = computeCorrelation(singleTracker.currentTemplate, tempCrop, singleTrackerHeight, singleTrackerWidth);
                        float score2 = computeCorrelation(singleTracker.originalTemplate, tempCrop, singleTrackerHeight, singleTrackerWidth);
                        if (score > similarityScore[ParticleIndex])
                        {
                            similarityScore[ParticleIndex] = score;
                            //RotationScore1[ParticleIndex] = angle;
                            rotationIndexStore[ParticleIndex] = Numb;
                        }
                        if (score2 > similarityScoreOfInitialT[ParticleIndex])
                        {
                            similarityScoreOfInitialT[ParticleIndex] = score2;
                            rotationIndexStoreOfInitialT[ParticleIndex] = Numb;
                        }
                    }

                }
            }

            pickBestParticle(Particle_x, Particle_y, similarityScore, rotationIndexStore, similarityScoreOfInitialT, rotationIndexStoreOfInitialT, singleTracker, imageInfo, SearchRange);

            //int iterationCounter = 1;
            //int iterationMax = 10;
            updateBestROI(singleTracker, normalizedImageData, colorbands, stride, dataLength);
            
        }




        private void updateBestROI(SingleTracker singleTracker, float[] normalizedImageData, int colorbands, int stride, int dataLength)
        {
            float angle = singleTracker.rotation;
            int halfWidth = singleTracker.width / 2;
            int halfHeight = singleTracker.height / 2;
            double cosine = Math.Cos(angle);
            double sinine = Math.Sin(angle);
            int new_x, new_y;
            int theAddress, h, w;

            try
            {
                for (int m = singleTracker.centerX - halfWidth; m < singleTracker.centerX + halfWidth; m++)
                {
                    for (int n = singleTracker.centerY - halfHeight; n < singleTracker.centerY + halfHeight; n++)
                    {

                        new_x = (int)((m - singleTracker.centerX) * cosine - (n - singleTracker.centerY) * sinine);
                        new_y = (int)((m - singleTracker.centerX) * sinine + (n - singleTracker.centerY) * cosine);

                        theAddress = (singleTracker.centerX + new_x) * colorbands + (singleTracker.centerY + new_y) * stride;
                        h = n - singleTracker.centerY + halfHeight;
                        w = m - singleTracker.centerX + halfWidth;
                        if (h < singleTracker.height && w < singleTracker.width)
                        {
                            if (theAddress >= dataLength || theAddress < 0)
                            {
                                singleTracker.bestROI[h, w] = 0;
                            }
                            else
                            {
                                singleTracker.bestROI[h, w] = normalizedImageData[theAddress];
                            }
                        }
                    }
                }
            }
            catch(System.IndexOutOfRangeException)
            {
                return;
            }
        }


        float computeCorrelation(float[,] matrix1, float[,] matrix2, int matrixHeight, int matrixWidth)
        {
            int i, j;
            float mean1 = 0;
            float mean2 = 0;
            float r;
            double child = 0;
            double mother1 = 0;
            double mother2 = 0;
            float count = matrixHeight * matrixWidth;


            for (i = 0; i < matrixHeight; i++)
            //for (i = 0; i < MatrixHeight; i = i + DownSampleRate)
            {
                for (j = 0; j < matrixWidth; j++)
                //for (j = 0; j < MatrixWidth; j = j + DownSampleRate)
                {

                    //count = count + 1;
                    mean1 = mean1 + matrix1[i, j];
                    mean2 = mean2 + matrix2[i, j];

                }
            }
            mean1 = mean1 / count;
            mean2 = mean2 / count;

            for (int m = 0; m < matrixHeight; m++)
            //for (int m = 0; m < MatrixHeight; m = m + DownSampleRate)
            {
                for (int n = 0; n < matrixWidth; n++)
                //for (int n = 0; n < MatrixWidth; n = n + DownSampleRate)
                {
                    child = child + (matrix1[m, n] - mean1) * (matrix2[m, n] - mean2);
                    mother1 = mother1 + (matrix1[m, n] - mean1) * (matrix1[m, n] - mean1);
                    mother2 = mother2 + (matrix2[m, n] - mean2) * (matrix2[m, n] - mean2);
                }
            }

            return (float)(child / Math.Sqrt(mother1 * mother2));

        }


        public void pickBestParticle(double[] Particle_x, double[] Particle_y, float[] similarityScore, int[] rotationIndexStore, float[] similarityScoreOfInitialT, int[] rotationIndexStoreOfIntialT, SingleTracker singleTracker, ImageInfo imageInfo, int SearchRange)
        {
            singleTracker.useInitialT = false;
            int Index = ComputeMaxIndex(similarityScore, ParticleNumber);
            int Index2 = ComputeMaxIndex(similarityScoreOfInitialT, ParticleNumber);
            if (similarityScore[Index] < similarityScoreOfInitialT[Index2] && similarityScoreOfInitialT[Index2] > trackerFailThreshold)
            {
                singleTracker.useInitialT = true;
                Index = Index2;
            }
            //testing start
            //Index = 0;
            //testing end
            int result_x = (int)(singleTracker.centerX + SearchRange * singleTracker.searchRangeX * Particle_x[Index]);
            int result_y = (int)(singleTracker.centerY + SearchRange * singleTracker.searchRangeY * Particle_y[Index]);
                
            if (result_x > 0 && result_x < imageInfo.imageWidth && result_y > 0 && result_y < imageInfo.imageHeight)
            {
                singleTracker.previousCenterX = singleTracker.centerX;
                singleTracker.previousCenterY = singleTracker.centerY;
                singleTracker.previousRotationIndex = singleTracker.rotationIndex;

                singleTracker.centerX = result_x;
                singleTracker.centerY = result_y;
                singleTracker.searchRangeX = (int)(Math.Abs(singleTracker.centerX - singleTracker.previousCenterX) / (Range_x + 1)) + 1;
                singleTracker.searchRangeY = (int)(Math.Abs(singleTracker.centerY - singleTracker.previousCenterY) / (Range_y + 1)) + 1;
                int tempRotationIndex = rotationIndexStore[Index];
                singleTracker.bestMatchScore = similarityScore[Index];
                if (singleTracker.useInitialT == true)
                {
                    tempRotationIndex = rotationIndexStoreOfIntialT[Index];
                    singleTracker.bestMatchScore = similarityScoreOfInitialT[Index];
                }
                singleTracker.rotationIndex = tempRotationIndex;
                singleTracker.rotation = rotationAngel[tempRotationIndex];
                
               

            }   
         
        }

        private void computeStartEndIndex(out int startIndex, out int endIndex, SingleTracker singleTracker)
        {
            int currentIndex = singleTracker.rotationIndex;
            if (currentIndex == 0)
            {
                startIndex = 0;
                //endIndex = 2;
                endIndex = 6;
            }
            else if (currentIndex == 12)
            {
                //startIndex = 10;
                startIndex = 6;
                endIndex = 12;
            }
            else
            {
                startIndex = currentIndex - 1;
                endIndex = currentIndex + 1;
            }
        }

        public void generateRamdomNums()
        {


            Random randomNo = new Random((int)DateTime.Now.Millisecond * (int)DateTime.Now.Millisecond);

            for (int ParticleIndex = 0; ParticleIndex < ParticleNumber; ParticleIndex++)
            {
                if (ParticleIndex == 0)
                {
                    Particle_x[ParticleIndex] = 0;
                    Particle_y[ParticleIndex] = 0;
                }
                else
                {

                    Particle_x[ParticleIndex] = Range_x * RandomGenerator(0, 1, randomNo);
                    Particle_y[ParticleIndex] = Range_y * RandomGenerator(0, 1, randomNo);

                }
            }
        }


        double RandomGenerator(double mean, double stdDev, Random r)
        {
            // Get a random number between 0 and 1
            double rnd = r.NextDouble();
            double rmd = r.NextDouble();
            // Transform the result to normal distribution
            double NDRN = mean + (stdDev * (Math.Sqrt(-2 * Math.Log(rnd)) * Math.Cos(2 * PI * rmd)));

            return NDRN;
        }

        int ComputeMaxIndex(float[] Score, int Total)
        {
            int Index = 0;
            float max = Score[0];

            for (int i = 0; i < Total; i++)
            {
                if (Score[i] > max)
                {
                    max = Score[i];
                    Index = i;
                }
            }
            return Index;
        }


    }
}
