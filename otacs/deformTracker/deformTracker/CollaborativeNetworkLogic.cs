using System;
using System.Collections.Generic;
using System.Text;

namespace deformTracker
{
    class CollaborativeNetworkLogic
    {
        float trackerFailThreshold = 0.6f;

        public void computeSurvivorGroup(SingleTracker[,] singleTrackerArray, int trackersColNum, int trackersRowNum, InterestPoint interestPoint)
        {
            //decide for each tracker
            if (trackersColNum > 1 || trackersRowNum > 1)
            {
                for (int i = 0; i < trackersColNum; i++)
                {
                    for (int j = 0; j < trackersRowNum; j++)
                    {
                        singleTrackerArray[i, j].isSurvivor = decideSurviving(i, j, singleTrackerArray, trackersColNum, trackersRowNum, interestPoint);
                    }
                }

            }
            else
            {
                if (singleTrackerArray[0, 0].bestMatchScore >= trackerFailThreshold)
                {
                    singleTrackerArray[0, 0].isSurvivor = true;
                }
                else
                {
                    singleTrackerArray[0, 0].isSurvivor = false;
                }
            }
            //post prosessing the survivor group
            //eliminateNullGroup(singleTrackerArray, trackersColNum, trackersRowNum);
        }

        


        bool decideSurviving(int colInd, int rowInd, SingleTracker[,] singleTrackerArray, int trackersColNum, int trackersRowNum, InterestPoint interestPoint)
        {

            double score;
            double varianceX = 10;
            double varianceY = 10;
            double varianceZ = 2;
            //threshold is 0.025 for visual, 0.04 for thermal.
            
            double threshold = 0.04;
            double avgNeighborsMatchScore = 0;
            double neighborsMotionSimilarity = 0; 
               

            int neighborsNum = 4;
            int i, j;
            double mean_x, mean_y, mean_z;
            double x = singleTrackerArray[colInd, rowInd].centerX - singleTrackerArray[colInd, rowInd].previousCenterX;
            double y = singleTrackerArray[colInd, rowInd].centerY - singleTrackerArray[colInd, rowInd].previousCenterY;
            double z = singleTrackerArray[colInd, rowInd].rotationIndex - singleTrackerArray[colInd, rowInd].previousRotationIndex;

            i = colInd - 1;
            j = rowInd;
            if (i < 0)
            {
                neighborsNum--;
            }
            else
            {
   
                mean_x = singleTrackerArray[i, j].centerX - singleTrackerArray[i, j].previousCenterX;
                mean_y = singleTrackerArray[i, j].centerY - singleTrackerArray[i, j].previousCenterY;
                mean_z = singleTrackerArray[i, j].rotationIndex - singleTrackerArray[i, j].previousRotationIndex;
                neighborsMotionSimilarity = neighborsMotionSimilarity + compute2dPDF(x, y, z, mean_x, mean_y, mean_z, varianceX, varianceY, varianceZ);
                avgNeighborsMatchScore = avgNeighborsMatchScore + singleTrackerArray[i, j].bestMatchScore;
            }

            i = colInd + 1;
            j = rowInd;
            if (i >= trackersColNum)
            {
                neighborsNum--;
            }
            else
            {
                mean_x = singleTrackerArray[i, j].centerX - singleTrackerArray[i, j].previousCenterX;
                mean_y = singleTrackerArray[i, j].centerY - singleTrackerArray[i, j].previousCenterY;
                mean_z = singleTrackerArray[i, j].rotationIndex - singleTrackerArray[i, j].previousRotationIndex;
                neighborsMotionSimilarity = neighborsMotionSimilarity + compute2dPDF(x, y, z, mean_x, mean_y, mean_z, varianceX, varianceY, varianceZ);
                avgNeighborsMatchScore = avgNeighborsMatchScore + singleTrackerArray[i, j].bestMatchScore;
            
            }

            i = colInd;
            j = rowInd - 1;
            if (j < 0)
            {
                neighborsNum--;
            }
            else
            {
                mean_x = singleTrackerArray[i, j].centerX - singleTrackerArray[i, j].previousCenterX;
                mean_y = singleTrackerArray[i, j].centerY - singleTrackerArray[i, j].previousCenterY;
                mean_z = singleTrackerArray[i, j].rotationIndex - singleTrackerArray[i, j].previousRotationIndex;
                neighborsMotionSimilarity = neighborsMotionSimilarity + compute2dPDF(x, y, z, mean_x, mean_y, mean_z, varianceX, varianceY, varianceZ);
                avgNeighborsMatchScore = avgNeighborsMatchScore + singleTrackerArray[i, j].bestMatchScore;
            
            }

            i = colInd;
            j = rowInd + 1;
            if (j >= trackersRowNum)
            {
                neighborsNum--;
            }
            else
            {
                mean_x = singleTrackerArray[i, j].centerX - singleTrackerArray[i, j].previousCenterX;
                mean_y = singleTrackerArray[i, j].centerY - singleTrackerArray[i, j].previousCenterY;
                mean_z = singleTrackerArray[i, j].rotationIndex - singleTrackerArray[i, j].rotationIndex;
                neighborsMotionSimilarity = neighborsMotionSimilarity + compute2dPDF(x, y, z, mean_x, mean_y, mean_z, varianceX, varianceY, varianceZ);
                avgNeighborsMatchScore = avgNeighborsMatchScore + singleTrackerArray[i, j].bestMatchScore;
            
            }

            neighborsMotionSimilarity = neighborsMotionSimilarity / neighborsNum;
            avgNeighborsMatchScore = avgNeighborsMatchScore / neighborsNum;

            
            //score = singleTrackerArray[colInd, rowInd].bestMatchScore * neighborsMotionSimilarity * avgNeighborsMatchScore;

            //double distToPoint = Math.Sqrt((singleTrackerArray[colInd, rowInd].centerX - interestPoint.x) * (singleTrackerArray[colInd, rowInd].centerX - interestPoint.x) + (singleTrackerArray[colInd, rowInd].centerY - interestPoint.y) * (singleTrackerArray[colInd, rowInd].centerY - interestPoint.y));

            score = singleTrackerArray[colInd, rowInd].bestMatchScore * neighborsMotionSimilarity;

            if (score < threshold)
                return false;
            else
                return true;
        }

        private double compute2dPDF(double x, double y, double z, double mean_x, double mean_y, double mean_z, double variance_x, double variance_y, double variance_z)
        {
            return Math.Exp(-((x - mean_x) * (x - mean_x) / (2 * variance_x) + (y - mean_y) * (y - mean_y) / (2 * variance_y) + (z - mean_z) / (2 * variance_z)));
        }
   
    }
}
