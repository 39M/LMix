//------------------------------------------------------------------------------
// <copyright file="SkeletonJointsSelfIntersectionConstraint.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Filter to prevent skeleton arm joints from intersecting the "body".
/// </summary>
public class SelfIntersectionConstraint
{
	// cylinder creation parameters
    public float ShoulderExtend = 0.5f;
    public float HipExtend = 6.0f;
    public float CollisionTolerance = 1.01f;
    public float RadiusMultiplier = 1.3f; // increase for bulky avatars
	
	
	// Initializes a new instance of the class.
	public SelfIntersectionConstraint()
	{
	}
	
    // ConstrainSelfIntersection collides joints with the skeleton to keep the skeleton's hands and wrists from puncturing its body
    // A cylinder is created to represent the torso. Intersecting joints have their positions changed to push them outside the torso.
    public void Constrain(ref KinectWrapper.NuiSkeletonData skeleton)
    {
//        if (null == skeleton)
//        {
//            return;
//        }

		int shoulderCenterIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter;
		int hipCenterIndex = (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter;

        if (skeleton.eSkeletonPositionTrackingState[shoulderCenterIndex] != KinectWrapper.NuiSkeletonPositionTrackingState.NotTracked &&
            skeleton.eSkeletonPositionTrackingState[hipCenterIndex] != KinectWrapper.NuiSkeletonPositionTrackingState.NotTracked)
        {
            Vector3 shoulderDiffLeft = KinectHelper.VectorBetween(ref skeleton, shoulderCenterIndex, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
            Vector3 shoulderDiffRight = KinectHelper.VectorBetween(ref skeleton, shoulderCenterIndex, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);
            float shoulderLengthLeft = shoulderDiffLeft.magnitude;
            float shoulderLengthRight = shoulderDiffRight.magnitude;

            // The distance between shoulders is averaged for the radius
            float cylinderRadius = (shoulderLengthLeft + shoulderLengthRight) * 0.5f;
    
            // Calculate the shoulder center and the hip center.  Extend them up and down respectively.
            Vector3 shoulderCenter = (Vector3)skeleton.SkeletonPositions[shoulderCenterIndex];
            Vector3 hipCenter = (Vector3)skeleton.SkeletonPositions[hipCenterIndex];
            Vector3 hipShoulder = hipCenter - shoulderCenter;
            hipShoulder.Normalize();

            shoulderCenter = shoulderCenter - (hipShoulder * (ShoulderExtend * cylinderRadius));
            hipCenter = hipCenter + (hipShoulder * (HipExtend * cylinderRadius));
    
            // Optionally increase radius to account for bulky avatars
            cylinderRadius *= RadiusMultiplier;
   
            // joints to collide
            int[] collisionIndices = 
			{ 
				(int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft, 
				(int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft, 
				(int)KinectWrapper.NuiSkeletonPositionIndex.WristRight, 
				(int)KinectWrapper.NuiSkeletonPositionIndex.HandRight 
			};
    
            foreach (int j in collisionIndices)
            {
                Vector3 collisionJoint = (Vector3)skeleton.SkeletonPositions[j];
                
                Vector4 distanceNormal = KinectHelper.DistanceToLineSegment(shoulderCenter, hipCenter, collisionJoint);

                Vector3 normal = new Vector3(distanceNormal.x, distanceNormal.y, distanceNormal.z);

                // if distance is within the cylinder then push the joint out and away from the cylinder
                if (distanceNormal.w < cylinderRadius)
                {
                    collisionJoint += normal * ((cylinderRadius - distanceNormal.w) * CollisionTolerance);

                    skeleton.SkeletonPositions[j] = (Vector4)collisionJoint;
                }
            }
        }
    }
}
