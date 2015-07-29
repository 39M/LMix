using UnityEngine;
using System.Collections;

public class KinectHelper 
{

    // JointPositionIsValid checks whether a skeleton joint is all 0's which can indicate not valid.  
    public static bool JointPositionIsValid(Vector3 jointPosition)
    {
        return jointPosition.x != 0.0f || jointPosition.y != 0.0f || jointPosition.z != 0.0f;
    }

    // BoneOrientationIsValid checks whether a skeleton bone rotation is NaN which can indicate an invalid rotation.  
    public static bool BoneOrientationIsValid(Quaternion boneOrientation)
    {
        return !(float.IsNaN(boneOrientation.x) || float.IsNaN(boneOrientation.y) || float.IsNaN(boneOrientation.z) || float.IsNaN(boneOrientation.w));
    }

    // IsTracked checks whether the skeleton joint is tracked.
    public static bool IsTracked(KinectWrapper.NuiSkeletonData skeleton, int jointIndex)
    {
//            if (null == skeleton)
//            {
//                return false;
//            }

		return skeleton.eSkeletonPositionTrackingState[jointIndex] == KinectWrapper.NuiSkeletonPositionTrackingState.Tracked;
    }

    // IsTrackedOrInferred checks whether the skeleton joint is tracked or inferred.
    public static bool IsTrackedOrInferred(KinectWrapper.NuiSkeletonData skeleton, int jointIndex)
    {
//            if (null == skeleton)
//            {
//                return false;
//            }

		return skeleton.eSkeletonPositionTrackingState[jointIndex] != KinectWrapper.NuiSkeletonPositionTrackingState.NotTracked;
    }

    // RotationBetweenQuaternions returns a quaternion that represents a rotation qR such that qA * qR = quaternionB.
    public static Quaternion RotationBetweenQuaternions(Quaternion quaternionA, Quaternion quaternionB)
    {
        Quaternion modifiedB = EnsureQuaternionNeighborhood(quaternionA, quaternionB);
        return Quaternion.Inverse(quaternionA) * modifiedB;
    }

    // EnhancedQuaternionSlerp performs a quaternion Slerp, after placing both input quaternions in the same 3D sphere.
    public static Quaternion EnhancedQuaternionSlerp(Quaternion quaternionA, Quaternion quaternionB, float amount)
    {
        Quaternion modifiedB = EnsureQuaternionNeighborhood(quaternionA, quaternionB);
        return Quaternion.Slerp(quaternionA, modifiedB, amount);
    }

    // EnsureQuaternionNeighborhood ensures that quaternions qA and quaternionB are in the same 3D sphere in 4D space.
    public static Quaternion EnsureQuaternionNeighborhood(Quaternion quaternionA, Quaternion quaternionB)
    {
        if (Quaternion.Dot(quaternionA, quaternionB) < 0)
        {
            // Negate the second quaternion, to place it in the opposite 3D sphere.
            //return -quaternionB;
			return new Quaternion(-quaternionB.x, -quaternionB.y, -quaternionB.z, -quaternionB.w);
        }

        return quaternionB;
    }

    // QuaternionAngle returns the amount of rotation in the given quaternion, in radians.
    public static float QuaternionAngle(Quaternion rotation)
    {
        //rotation.Normalize();
        float angle = 2.0f * (float)Mathf.Acos(rotation.w);
        return angle;
    }

    // LerpAndApply performs a Lerp and applies the Lerped vector to the skeleton joint.
    public static void LerpAndApply(ref KinectWrapper.NuiSkeletonData skeleton, int jointIndex, Vector3 newJointPos, float lerpValue, KinectWrapper.NuiSkeletonPositionTrackingState finalTrackingState)
    {
//            if (null == skeleton)
//            {
//                return;
//            }

		Vector3 jointPos = (Vector3)skeleton.SkeletonPositions[jointIndex];
        jointPos = Vector3.Lerp(jointPos, newJointPos, lerpValue);
	
		skeleton.SkeletonPositions[jointIndex] = (Vector4)jointPos;
		skeleton.eSkeletonPositionTrackingState[jointIndex] = finalTrackingState;
    }

    // CopySkeleton copies the data from another skeleton.
    public static void CopySkeleton(ref KinectWrapper.NuiSkeletonData source, ref KinectWrapper.NuiSkeletonData destination)
    {
//        if (null == source)
//        {
//            return;
//        }
//
//        if (null == destination)
//        {
//            destination = new Skeleton();
//        }

        destination.eTrackingState = source.eTrackingState;
        destination.dwTrackingID = source.dwTrackingID;
        destination.Position = source.Position;
		destination.dwQualityFlags = source.dwQualityFlags;

        int jointsCount = (int)KinectWrapper.NuiSkeletonPositionIndex.Count;
		
		if(destination.SkeletonPositions == null)
			destination.SkeletonPositions = new Vector4[jointsCount];
		if(destination.eSkeletonPositionTrackingState == null)
			destination.eSkeletonPositionTrackingState = new KinectWrapper.NuiSkeletonPositionTrackingState[jointsCount];
		
        for(int jointIndex = 0; jointIndex < jointsCount; jointIndex++)
        {
            destination.SkeletonPositions[jointIndex] = source.SkeletonPositions[jointIndex];
			destination.eSkeletonPositionTrackingState[jointIndex] = source.eSkeletonPositionTrackingState[jointIndex];
        }
    }

    /// VectorBetween calculates the Vector3 from start to end == subtract start from end 
    public static Vector3 VectorBetween(ref KinectWrapper.NuiSkeletonData skeleton, int startJoint, int endJoint)
    {
//        if (null == skeleton)
//        {
//            return Vector3.Zero;
//        }

        Vector3 startPosition = (Vector3)skeleton.SkeletonPositions[startJoint];
        Vector3 endPosition = (Vector3)skeleton.SkeletonPositions[endJoint];
		
        return endPosition - startPosition;
    }

    // DistanceToLineSegment finds the distance from a point to a line.
    public static Vector4 DistanceToLineSegment(Vector3 linePoint0, Vector3 linePoint1, Vector3 point)
    {
        // find the vector from x0 to x1
        Vector3 lineVec = linePoint1 - linePoint0;
        float lineLength = lineVec.magnitude;
        Vector3 lineToPoint = point - linePoint0;

        const float Epsilon = 0.0001f;

        // if the line is too short skip
        if (lineLength > Epsilon)
        {
            float t = Vector3.Dot(lineVec, lineToPoint) / lineLength;

            // projection is longer than the line itself so find distance to end point of line
            if (t > lineLength)
            {
                lineToPoint = point - linePoint1;
            }
            else if (t >= 0.0f)
            {
                // find distance to line
                Vector3 normalPoint = lineVec;

                // Perform the float->vector conversion once by combining t/fLineLength
                normalPoint *= t / lineLength;
                normalPoint += linePoint0;
                lineToPoint = point - normalPoint;
            }
        }

        // The distance is the size of the final computed line
        float distance = lineToPoint.magnitude;

        // The normal is the final line normalized
        Vector3 normal = lineToPoint / distance;

        return new Vector4(normal.x, normal.y, normal.z, distance);
    }

}
