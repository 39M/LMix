//------------------------------------------------------------------------------
// <copyright file="BoneOrientationDoubleExponentialFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Implementation of a Holt Double Exponential Smoothing filter for orientation. The double 
/// exponential smooths the curve and predicts. There is also noise jitter removal. And maximum
/// prediction bounds.  The parameters are commented in the Init function.
/// </summary>
public class BoneOrientationsFilter
{
    // The previous filtered orientation data.
    private FilterDoubleExponentialData[] history;

    // The transform smoothing parameters for this filter.
    private KinectWrapper.NuiTransformSmoothParameters smoothParameters;

    // True when the filter parameters are initialized.
    private bool init;

    // Initializes a new instance of the class.
    public BoneOrientationsFilter()
    {
        this.init = false;
    }

    // Initialize the filter with a default set of TransformSmoothParameters.
    public void Init()
    {
        // Set some reasonable defaults
        this.Init(0.5f, 0.8f, 0.75f, 0.1f, 0.1f);
    }

    /// <summary>
    /// Initialize the filter with a set of manually specified TransformSmoothParameters.
    /// </summary>
    /// <param name="smoothingValue">Smoothing = [0..1], lower values is closer to the raw data and more noisy.</param>
    /// <param name="correctionValue">Correction = [0..1], higher values correct faster and feel more responsive.</param>
    /// <param name="predictionValue">Prediction = [0..n], how many frames into the future we want to predict.</param>
    /// <param name="jitterRadiusValue">JitterRadius = The deviation angle in radians that defines jitter.</param>
    /// <param name="maxDeviationRadiusValue">MaxDeviation = The maximum angle in radians that filtered positions are allowed to deviate from raw data.</param>
    public void Init(float smoothingValue, float correctionValue, float predictionValue, float jitterRadiusValue, float maxDeviationRadiusValue)
    {
        this.smoothParameters = new KinectWrapper.NuiTransformSmoothParameters();

        this.smoothParameters.fMaxDeviationRadius = maxDeviationRadiusValue; // Size of the max prediction radius Can snap back to noisy data when too high
        this.smoothParameters.fSmoothing = smoothingValue;                   // How much soothing will occur.  Will lag when too high
        this.smoothParameters.fCorrection = correctionValue;                 // How much to correct back from prediction.  Can make things springy
        this.smoothParameters.fPrediction = predictionValue;                 // Amount of prediction into the future to use. Can over shoot when too high
        this.smoothParameters.fJitterRadius = jitterRadiusValue;             // Size of the radius where jitter is removed. Can do too much smoothing when too high
        
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

    /// Resets the filter to default values.
    public void Reset()
    {
        this.history = new FilterDoubleExponentialData[(int)KinectWrapper.NuiSkeletonPositionIndex.Count];
    }

    // Implements a double exponential smoothing filter on the skeleton bone orientation quaternions.
    public void UpdateFilter(ref KinectWrapper.NuiSkeletonData skeleton, ref Matrix4x4[] jointOrientations)
    {
//        if (null == skeleton)
//        {
//            return;
//        }

//        if (skeleton.eTrackingState != KinectWrapper.NuiSkeletonTrackingState.SkeletonTracked)
//        {
//            return;
//        }

        if (this.init == false)
        {
            this.Init(); // initialize with default parameters                
        }

        KinectWrapper.NuiTransformSmoothParameters tempSmoothingParams = new KinectWrapper.NuiTransformSmoothParameters();

        // Check for divide by zero. Use an epsilon of a 10th of a millimeter
        this.smoothParameters.fJitterRadius = Math.Max(0.0001f, this.smoothParameters.fJitterRadius);

        tempSmoothingParams.fSmoothing = smoothParameters.fSmoothing;
        tempSmoothingParams.fCorrection = smoothParameters.fCorrection;
        tempSmoothingParams.fPrediction = smoothParameters.fPrediction;
		
		int jointsCount = (int)KinectWrapper.NuiSkeletonPositionIndex.Count;
        for(int jointIndex = 0; jointIndex < jointsCount; jointIndex++)
        {
			//KinectWrapper.NuiSkeletonPositionIndex jt = (KinectWrapper.NuiSkeletonPositionIndex)jointIndex;
			
            // If not tracked, we smooth a bit more by using a bigger jitter radius
            // Always filter feet highly as they are so noisy
            if (skeleton.eSkeletonPositionTrackingState[jointIndex] != KinectWrapper.NuiSkeletonPositionTrackingState.Tracked || 
				jointIndex == (int)KinectWrapper.NuiSkeletonPositionIndex.FootLeft || jointIndex == (int)KinectWrapper.NuiSkeletonPositionIndex.FootRight)
            {
                tempSmoothingParams.fJitterRadius = smoothParameters.fJitterRadius * 2.0f;
                tempSmoothingParams.fMaxDeviationRadius = smoothParameters.fMaxDeviationRadius * 2.0f;
            }
            else
            {
                tempSmoothingParams.fJitterRadius = smoothParameters.fJitterRadius;
                tempSmoothingParams.fMaxDeviationRadius = smoothParameters.fMaxDeviationRadius;
            }

            FilterJoint(ref skeleton, jointIndex, ref tempSmoothingParams, ref jointOrientations);
        }
    }

    // Update the filter for one joint.  
    protected void FilterJoint(ref KinectWrapper.NuiSkeletonData skeleton, int jointIndex, ref KinectWrapper.NuiTransformSmoothParameters smoothingParameters, ref Matrix4x4[] jointOrientations)
    {
//        if (null == skeleton)
//        {
//            return;
//        }

//        int jointIndex = (int)jt;

        Quaternion filteredOrientation;
        Quaternion trend;
		
		Vector3 fwdVector = (Vector3)jointOrientations[jointIndex].GetColumn(2);
		if(fwdVector == Vector3.zero)
			return;

        Quaternion rawOrientation = Quaternion.LookRotation(fwdVector, jointOrientations[jointIndex].GetColumn(1));
        Quaternion prevFilteredOrientation = this.history[jointIndex].FilteredBoneOrientation;
        Quaternion prevTrend = this.history[jointIndex].Trend;
        Vector3 rawPosition = (Vector3)skeleton.SkeletonPositions[jointIndex];
        bool orientationIsValid = KinectHelper.JointPositionIsValid(rawPosition) && KinectHelper.IsTrackedOrInferred(skeleton, jointIndex) && KinectHelper.BoneOrientationIsValid(rawOrientation);

        if (!orientationIsValid)
        {
            if (this.history[jointIndex].FrameCount > 0)
            {
                rawOrientation = history[jointIndex].FilteredBoneOrientation;
                history[jointIndex].FrameCount = 0;
            }
        }

        // Initial start values or reset values
        if (this.history[jointIndex].FrameCount == 0)
        {
            // Use raw position and zero trend for first value
            filteredOrientation = rawOrientation;
            trend = Quaternion.identity;
        }
        else if (this.history[jointIndex].FrameCount == 1)
        {
            // Use average of two positions and calculate proper trend for end value
            Quaternion prevRawOrientation = this.history[jointIndex].RawBoneOrientation;
            filteredOrientation = KinectHelper.EnhancedQuaternionSlerp(prevRawOrientation, rawOrientation, 0.5f);

            Quaternion diffStarted = KinectHelper.RotationBetweenQuaternions(filteredOrientation, prevFilteredOrientation);
            trend = KinectHelper.EnhancedQuaternionSlerp(prevTrend, diffStarted, smoothingParameters.fCorrection);
        }
        else
        {
            // First apply a jitter filter
            Quaternion diffJitter = KinectHelper.RotationBetweenQuaternions(rawOrientation, prevFilteredOrientation);
            float diffValJitter = (float)Math.Abs(KinectHelper.QuaternionAngle(diffJitter));

            if (diffValJitter <= smoothingParameters.fJitterRadius)
            {
                filteredOrientation = KinectHelper.EnhancedQuaternionSlerp(prevFilteredOrientation, rawOrientation, diffValJitter / smoothingParameters.fJitterRadius);
            }
            else
            {
                filteredOrientation = rawOrientation;
            }

            // Now the double exponential smoothing filter
            filteredOrientation = KinectHelper.EnhancedQuaternionSlerp(filteredOrientation, prevFilteredOrientation * prevTrend, smoothingParameters.fSmoothing);

            diffJitter = KinectHelper.RotationBetweenQuaternions(filteredOrientation, prevFilteredOrientation);
            trend = KinectHelper.EnhancedQuaternionSlerp(prevTrend, diffJitter, smoothingParameters.fCorrection);
        }      

        // Use the trend and predict into the future to reduce latency
        Quaternion predictedOrientation = filteredOrientation * KinectHelper.EnhancedQuaternionSlerp(Quaternion.identity, trend, smoothingParameters.fPrediction);

        // Check that we are not too far away from raw data
        Quaternion diff = KinectHelper.RotationBetweenQuaternions(predictedOrientation, filteredOrientation);
        float diffVal = (float)Math.Abs(KinectHelper.QuaternionAngle(diff));

        if (diffVal > smoothingParameters.fMaxDeviationRadius)
        {
            predictedOrientation = KinectHelper.EnhancedQuaternionSlerp(filteredOrientation, predictedOrientation, smoothingParameters.fMaxDeviationRadius / diffVal);
        }

//        predictedOrientation.Normalize();
//        filteredOrientation.Normalize();
//        trend.Normalize();
          
        // Save the data from this frame
        history[jointIndex].RawBoneOrientation = rawOrientation;
        history[jointIndex].FilteredBoneOrientation = filteredOrientation;
        history[jointIndex].Trend = trend;
        history[jointIndex].FrameCount++;

        // Set the filtered and predicted data back into the bone orientation
		if(KinectHelper.BoneOrientationIsValid(predictedOrientation))
		{
			jointOrientations[jointIndex].SetTRS(Vector3.zero, predictedOrientation, Vector3.one);
		}
    }

    // Historical Filter Data.  
    private struct FilterDoubleExponentialData
    {
        // Gets or sets Historical Position.  
        public Quaternion RawBoneOrientation;

        // Gets or sets Historical Filtered Position.  
        public Quaternion FilteredBoneOrientation;

        // Gets or sets Historical Trend.  
        public Quaternion Trend;

        // Gets or sets Historical FrameCount.  
        public uint FrameCount;
    }
}
