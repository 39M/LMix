//------------------------------------------------------------------------------
// <copyright file="SkeletonJointsPositionDoubleExponentialFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Implementation of a Holt Double Exponential Smoothing filter. The double exponential
/// smooths the curve and predicts.  There is also noise jitter removal. And maximum
/// prediction bounds.  The parameters are commented in the Init function.
/// </summary>
public class JointPositionsFilter
{
    // The history data.
    private FilterDoubleExponentialData[] history;

    // The transform smoothing parameters for this filter.
    private KinectWrapper.NuiTransformSmoothParameters smoothParameters;

    // True when the filter parameters are initialized.
    private bool init;
	
	
    /// Initializes a new instance of the class.
    public JointPositionsFilter()
    {
        this.init = false;
    }

    // Initialize the filter with a default set of TransformSmoothParameters.
    public void Init()
    {
        // Specify some defaults
        //this.Init(0.25f, 0.25f, 0.25f, 0.03f, 0.05f);
		this.Init(0.5f, 0.5f, 0.5f, 0.05f, 0.04f);
    }

    /// <summary>
    /// Initialize the filter with a set of manually specified TransformSmoothParameters.
    /// </summary>
    /// <param name="smoothingValue">Smoothing = [0..1], lower values is closer to the raw data and more noisy.</param>
    /// <param name="correctionValue">Correction = [0..1], higher values correct faster and feel more responsive.</param>
    /// <param name="predictionValue">Prediction = [0..n], how many frames into the future we want to predict.</param>
    /// <param name="jitterRadiusValue">JitterRadius = The deviation distance in m that defines jitter.</param>
    /// <param name="maxDeviationRadiusValue">MaxDeviation = The maximum distance in m that filtered positions are allowed to deviate from raw data.</param>
    public void Init(float smoothingValue, float correctionValue, float predictionValue, float jitterRadiusValue, float maxDeviationRadiusValue)
    {
        this.smoothParameters = new KinectWrapper.NuiTransformSmoothParameters();

        this.smoothParameters.fSmoothing = smoothingValue;                   // How much soothing will occur.  Will lag when too high
        this.smoothParameters.fCorrection = correctionValue;                 // How much to correct back from prediction.  Can make things springy
        this.smoothParameters.fPrediction = predictionValue;                 // Amount of prediction into the future to use. Can over shoot when too high
        this.smoothParameters.fJitterRadius = jitterRadiusValue;             // Size of the radius where jitter is removed. Can do too much smoothing when too high
        this.smoothParameters.fMaxDeviationRadius = maxDeviationRadiusValue; // Size of the max prediction radius Can snap back to noisy data when too high
        
		this.Reset();
        this.init = true;
    }

    // Initialize the filter with a set of TransformSmoothParameters.
    public void Init(KinectWrapper.NuiTransformSmoothParameters smoothingParameters)
    {
        this.smoothParameters = smoothingParameters;
		
        this.Reset();
        this.init = true;
    }

    // Resets the filter to default values.
    public void Reset()
    {
        this.history = new FilterDoubleExponentialData[(int)KinectWrapper.NuiSkeletonPositionIndex.Count];
    }

    // Update the filter with a new frame of data and smooth.
    public void UpdateFilter(ref KinectWrapper.NuiSkeletonData skeleton)
    {
//        if (null == skeleton)
//        {
//            return;
//        }

        if (skeleton.eTrackingState != KinectWrapper.NuiSkeletonTrackingState.SkeletonTracked)
        {
            return;
        }

        if (this.init == false)
        {
            this.Init();    // initialize with default parameters                
        }

        //Array jointTypeValues = Enum.GetValues(typeof(KinectWrapper.NuiSkeletonPositionIndex));

        KinectWrapper.NuiTransformSmoothParameters tempSmoothingParams = new KinectWrapper.NuiTransformSmoothParameters();

        // Check for divide by zero. Use an epsilon of a 10th of a millimeter
        this.smoothParameters.fJitterRadius = Math.Max(0.0001f, this.smoothParameters.fJitterRadius);

        tempSmoothingParams.fSmoothing = smoothParameters.fSmoothing;
        tempSmoothingParams.fCorrection = smoothParameters.fCorrection;
        tempSmoothingParams.fPrediction = smoothParameters.fPrediction;
		
		int jointsCount = (int)KinectWrapper.NuiSkeletonPositionIndex.Count;
        for(int jointIndex = 0; jointIndex < jointsCount; jointIndex++)
        {
			//KinectWrapper.NuiSkeletonPositionIndex jt = (KinectWrapper.NuiSkeletonPositionIndex)jointTypeValues.GetValue(jointIndex);
			
            // If not tracked, we smooth a bit more by using a bigger jitter radius
            // Always filter feet highly as they are so noisy
            if (skeleton.eSkeletonPositionTrackingState[jointIndex] != KinectWrapper.NuiSkeletonPositionTrackingState.Tracked)
            {
                tempSmoothingParams.fJitterRadius = smoothParameters.fJitterRadius * 2.0f;
                tempSmoothingParams.fMaxDeviationRadius = smoothParameters.fMaxDeviationRadius * 2.0f;
            }
            else
            {
                tempSmoothingParams.fJitterRadius = smoothParameters.fJitterRadius;
                tempSmoothingParams.fMaxDeviationRadius = smoothParameters.fMaxDeviationRadius;
            }

            FilterJoint(ref skeleton, jointIndex, ref tempSmoothingParams);
        }
    }

    // Update the filter for one joint.  
    protected void FilterJoint(ref KinectWrapper.NuiSkeletonData skeleton, int jointIndex, ref KinectWrapper.NuiTransformSmoothParameters smoothingParameters)
    {
//        if (null == skeleton)
//        {
//            return;
//        }

        //int jointIndex = (int)jt;

        Vector3 filteredPosition;
        Vector3 diffvec;
        Vector3 trend;
        float diffVal;

        Vector3 rawPosition = (Vector3)skeleton.SkeletonPositions[jointIndex];
        Vector3 prevFilteredPosition = this.history[jointIndex].FilteredPosition;
        Vector3 prevTrend = this.history[jointIndex].Trend;
        Vector3 prevRawPosition = this.history[jointIndex].RawPosition;
        bool jointIsValid = KinectHelper.JointPositionIsValid(rawPosition);

        // If joint is invalid, reset the filter
        if (!jointIsValid)
        {
            history[jointIndex].FrameCount = 0;
        }

        // Initial start values
        if (this.history[jointIndex].FrameCount == 0)
        {
            filteredPosition = rawPosition;
            trend = Vector3.zero;
        }
        else if (this.history[jointIndex].FrameCount == 1)
        {
            filteredPosition = (rawPosition + prevRawPosition) * 0.5f;
            diffvec = filteredPosition - prevFilteredPosition;
            trend = (diffvec * smoothingParameters.fCorrection) + (prevTrend * (1.0f - smoothingParameters.fCorrection));
        }
        else
        {              
            // First apply jitter filter
            diffvec = rawPosition - prevFilteredPosition;
            diffVal = Math.Abs(diffvec.magnitude);

            if (diffVal <= smoothingParameters.fJitterRadius)
            {
                filteredPosition = (rawPosition * (diffVal / smoothingParameters.fJitterRadius)) + (prevFilteredPosition * (1.0f - (diffVal / smoothingParameters.fJitterRadius)));
            }
            else
            {
                filteredPosition = rawPosition;
            }

            // Now the double exponential smoothing filter
            filteredPosition = (filteredPosition * (1.0f - smoothingParameters.fSmoothing)) + ((prevFilteredPosition + prevTrend) * smoothingParameters.fSmoothing);

            diffvec = filteredPosition - prevFilteredPosition;
            trend = (diffvec * smoothingParameters.fCorrection) + (prevTrend * (1.0f - smoothingParameters.fCorrection));
        }      

        // Predict into the future to reduce latency
        Vector3 predictedPosition = filteredPosition + (trend * smoothingParameters.fPrediction);

        // Check that we are not too far away from raw data
        diffvec = predictedPosition - rawPosition;
        diffVal = Mathf.Abs(diffvec.magnitude);

        if (diffVal > smoothingParameters.fMaxDeviationRadius)
        {
            predictedPosition = (predictedPosition * (smoothingParameters.fMaxDeviationRadius / diffVal)) + (rawPosition * (1.0f - (smoothingParameters.fMaxDeviationRadius / diffVal)));
        }

        // Save the data from this frame
        history[jointIndex].RawPosition = rawPosition;
        history[jointIndex].FilteredPosition = filteredPosition;
        history[jointIndex].Trend = trend;
        history[jointIndex].FrameCount++;
        
        // Set the filtered data back into the joint
//        Joint j = skeleton.Joints[jt];
//        j.Position = KinectHelper.Vector3ToSkeletonPoint(predictedPosition);
//        skeleton.Joints[jt] = j;
		skeleton.SkeletonPositions[jointIndex] = (Vector4)predictedPosition;
    }
	

    // Historical Filter Data.  
    private struct FilterDoubleExponentialData
    {
        // Gets or sets Historical Position.  
        public Vector3 RawPosition;

        // Gets or sets Historical Filtered Position.  
        public Vector3 FilteredPosition;

        // Gets or sets Historical Trend.  
        public Vector3 Trend;

        // Gets or sets Historical FrameCount.  
        public uint FrameCount;
    }
}
