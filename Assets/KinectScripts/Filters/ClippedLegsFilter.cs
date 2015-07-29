//------------------------------------------------------------------------------
// <copyright file="SkeletonJointsFilterClippedLegs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// FilterClippedLegs smooths out leg joint positions when the skeleton is clipped
/// by the bottom of the camera FOV.  Inferred joint positions from the skeletal tracker
/// can occasionally be noisy or erroneous, based on limited depth image pixels from the
/// parts of the legs in view.  This filter applies a lot of smoothing using a double
/// exponential filter, letting through just enough leg movement to show a kick or high step.
/// Based on the amount of leg that is clipped/inferred, the smoothed data is feathered into the
/// skeleton output data.
/// </summary>
public class ClippedLegsFilter
{
    // The blend weights when all leg joints are tracked.
    private readonly Vector3 allTracked;

    // The blend weights when the foot is inferred or not tracked.
    private readonly Vector3 footInferred;

    // The blend weights when ankle and below are inferred or not tracked.
    private readonly Vector3 ankleInferred;

    // The blend weights when knee and below are inferred or not tracked.
    private readonly Vector3 kneeInferred;

    // The joint position filter.
    private JointPositionsFilter filterJoints;

    // The timed lerp for the left knee.
    private TimedLerp lerpLeftKnee;

    // The timed lerp for the left ankle.
    private TimedLerp lerpLeftAnkle;

    // The timed lerp for the left foot.
    private TimedLerp lerpLeftFoot;

    /// The timed lerp for the right knee.
    private TimedLerp lerpRightKnee;

    // The timed lerp for the right ankle.
    private TimedLerp lerpRightAnkle;

    // The timed lerp for the right foot.
    private TimedLerp lerpRightFoot;

    // The local skeleton with leg filtering applied.
    private KinectWrapper.NuiSkeletonData filteredSkeleton;
	

    // Initializes a new instance of the class.
    public ClippedLegsFilter()
    {
        this.lerpLeftKnee = new TimedLerp();
        this.lerpLeftAnkle = new TimedLerp();
        this.lerpLeftFoot = new TimedLerp();
        this.lerpRightKnee = new TimedLerp();
        this.lerpRightAnkle = new TimedLerp();
        this.lerpRightFoot = new TimedLerp();

        this.filterJoints = new JointPositionsFilter();
        this.filteredSkeleton = new KinectWrapper.NuiSkeletonData();

        // knee, ankle, foot blend amounts
        this.allTracked = new Vector3(0.0f, 0.0f, 0.0f); // All joints are tracked
        this.footInferred = new Vector3(0.0f, 0.0f, 1.0f); // foot is inferred
        this.ankleInferred = new Vector3(0.5f, 1.0f, 1.0f);  // ankle is inferred
        this.kneeInferred = new Vector3(1.0f, 1.0f, 1.0f);   // knee is inferred

        Reset();
    }

    // Resets filter state to defaults.
    public void Reset()
    {
        // set up a really floaty double exponential filter - we want maximum smoothness
        this.filterJoints.Init(0.5f, 0.3f, 1.0f, 1.0f, 1.0f);

        this.lerpLeftKnee.Reset();
        this.lerpLeftAnkle.Reset();
        this.lerpLeftFoot.Reset();
        this.lerpRightKnee.Reset();
        this.lerpRightAnkle.Reset();
        this.lerpRightFoot.Reset();
    }

    // Implements the per-frame filter logic for the arms up patch.
    public bool FilterSkeleton(ref KinectWrapper.NuiSkeletonData skeleton, float deltaNuiTime)
    {
//        if (null == skeleton)
//        {
//            return false;
//        }

        // exit early if we lose tracking on the entire skeleton
        if (skeleton.eTrackingState != KinectWrapper.NuiSkeletonTrackingState.SkeletonTracked)
        {
            filterJoints.Reset();
        }

        KinectHelper.CopySkeleton(ref skeleton, ref filteredSkeleton);
        filterJoints.UpdateFilter(ref filteredSkeleton);

        // Update lerp state with the current delta NUI time.
        this.lerpLeftKnee.Tick(deltaNuiTime);
        this.lerpLeftAnkle.Tick(deltaNuiTime);
        this.lerpLeftFoot.Tick(deltaNuiTime);
        this.lerpRightKnee.Tick(deltaNuiTime);
        this.lerpRightAnkle.Tick(deltaNuiTime);
        this.lerpRightFoot.Tick(deltaNuiTime);

        // Exit early if we do not have a valid body basis - too much of the skeleton is invalid.
        if ((!KinectHelper.IsTracked(skeleton, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter)) || 
			(!KinectHelper.IsTrackedOrInferred(skeleton, (int)KinectWrapper.NuiSkeletonPositionIndex.HipLeft)) || 
			(!KinectHelper.IsTrackedOrInferred(skeleton, (int)KinectWrapper.NuiSkeletonPositionIndex.HipRight)))
        {
            return false;
        }

        // Determine if the skeleton is clipped by the bottom of the FOV.
        bool clippedBottom = (skeleton.dwQualityFlags & (int)KinectWrapper.FrameEdges.Bottom) != 0;

        // Select a mask for the left leg depending on which joints are not tracked.
        // These masks define how much of the filtered joint positions should be blended
        // with the raw positions.  Based on the tracking state of the leg joints, we apply
        // more filtered data as more joints lose tracking.
        Vector3 leftLegMask = this.allTracked;

        if (!KinectHelper.IsTracked(skeleton, (int)KinectWrapper.NuiSkeletonPositionIndex.KneeLeft))
        {
            leftLegMask = this.kneeInferred;
        }
        else if (!KinectHelper.IsTracked(skeleton, (int)KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft))
        {
            leftLegMask = this.ankleInferred;
        }
        else if (!KinectHelper.IsTracked(skeleton, (int)KinectWrapper.NuiSkeletonPositionIndex.FootLeft))
        {
            leftLegMask = this.footInferred;
        }

        // Select a mask for the right leg depending on which joints are not tracked.
        Vector3 rightLegMask = this.allTracked;

        if (!KinectHelper.IsTracked(skeleton, (int)KinectWrapper.NuiSkeletonPositionIndex.KneeRight))
        {
            rightLegMask = this.kneeInferred;
        }
        else if (!KinectHelper.IsTracked(skeleton, (int)KinectWrapper.NuiSkeletonPositionIndex.AnkleRight))
        {
            rightLegMask = this.ankleInferred;
        }
        else if (!KinectHelper.IsTracked(skeleton, (int)KinectWrapper.NuiSkeletonPositionIndex.FootRight))
        {
            rightLegMask = this.footInferred;
        }

        // If the skeleton is not clipped by the bottom of the FOV, cut the filtered data
        // blend in half.
        float clipMask = clippedBottom ? 1.0f : 0.5f;

        // Apply the mask values to the joints of each leg, by placing the mask values into the lerp targets.
        this.lerpLeftKnee.SetEnabled(leftLegMask.x * clipMask);
        this.lerpLeftAnkle.SetEnabled(leftLegMask.y * clipMask);
        this.lerpLeftFoot.SetEnabled(leftLegMask.z * clipMask);
        this.lerpRightKnee.SetEnabled(rightLegMask.x * clipMask);
        this.lerpRightAnkle.SetEnabled(rightLegMask.y * clipMask);
        this.lerpRightFoot.SetEnabled(rightLegMask.z * clipMask);

        // The bSkeletonUpdated flag tracks whether we have modified the output skeleton or not.
        bool skeletonUpdated = false;

        // Apply lerp to the left knee, which will blend the raw joint position with the filtered joint position based on the current lerp value.
        if (this.lerpLeftKnee.IsLerpEnabled())
        {
			int jointIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.KneeLeft;
            KinectHelper.LerpAndApply(ref skeleton, jointIndex, (Vector3)filteredSkeleton.SkeletonPositions[jointIndex], lerpLeftKnee.SmoothValue, KinectWrapper.NuiSkeletonPositionTrackingState.Tracked);
            skeletonUpdated = true;
        }

        // Apply lerp to the left ankle.
        if (this.lerpLeftAnkle.IsLerpEnabled())
        {
			int jointIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft;
            KinectHelper.LerpAndApply(ref skeleton, jointIndex, (Vector3)filteredSkeleton.SkeletonPositions[jointIndex], lerpLeftAnkle.SmoothValue, KinectWrapper.NuiSkeletonPositionTrackingState.Tracked);
            skeletonUpdated = true;
        }

        // Apply lerp to the left foot.
        if (this.lerpLeftFoot.IsLerpEnabled())
        {
			int jointIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.FootLeft;
            KinectHelper.LerpAndApply(ref skeleton, jointIndex, (Vector3)filteredSkeleton.SkeletonPositions[jointIndex], lerpLeftFoot.SmoothValue, KinectWrapper.NuiSkeletonPositionTrackingState.Inferred);
            skeletonUpdated = true;
        }

        // Apply lerp to the right knee.
        if (this.lerpRightKnee.IsLerpEnabled())
        {
			int jointIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.KneeRight;
            KinectHelper.LerpAndApply(ref skeleton, jointIndex, (Vector3)filteredSkeleton.SkeletonPositions[jointIndex], lerpRightKnee.SmoothValue, KinectWrapper.NuiSkeletonPositionTrackingState.Tracked);
            skeletonUpdated = true;
        }

        // Apply lerp to the right ankle.
        if (this.lerpRightAnkle.IsLerpEnabled())
        {
			int jointIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.AnkleRight;
            KinectHelper.LerpAndApply(ref skeleton, jointIndex, (Vector3)filteredSkeleton.SkeletonPositions[jointIndex], lerpRightAnkle.SmoothValue, KinectWrapper.NuiSkeletonPositionTrackingState.Tracked);
            skeletonUpdated = true;
        }

        // Apply lerp to the right foot.
        if (this.lerpRightFoot.IsLerpEnabled())
        {
			int jointIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.FootRight;
            KinectHelper.LerpAndApply(ref skeleton, jointIndex, (Vector3)filteredSkeleton.SkeletonPositions[jointIndex], lerpRightFoot.SmoothValue, KinectWrapper.NuiSkeletonPositionTrackingState.Inferred);
            skeletonUpdated = true;
        }

        return skeletonUpdated;
    }
}

