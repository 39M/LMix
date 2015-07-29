using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Filter to correct the joint locations and joint orientations to constraint to range of viable human motion.
/// </summary>
public class BoneOrientationsConstraint
{
	public enum ConstraintAxis { X = 0, Y = 1, Z = 2 }
	
    // The Joint Constraints.  
    private readonly List<BoneOrientationConstraint> jointConstraints = new List<BoneOrientationConstraint>();

	//private GameObject debugText;
	

    /// Initializes a new instance of the BoneOrientationConstraints class.
    public BoneOrientationsConstraint()
    {
		//debugText = GameObject.Find("CalibrationText");
    }
	
	// find the bone constraint structure for given joint
	// returns the structure index in the list, or -1 if the bone structure is not found
	private int FindBoneOrientationConstraint(int joint)
	{
		for(int i = 0; i < jointConstraints.Count; i++)
		{
			if(jointConstraints[i].joint == joint)
				return i;
		}
		
		// not found
		return -1;
	}

    // AddJointConstraint - Adds a joint constraint to the system.  
    public void AddBoneOrientationConstraint(int joint, ConstraintAxis axis, float angleMin, float angleMax)
    {
		int index = FindBoneOrientationConstraint(joint);
		
		BoneOrientationConstraint jc = index >= 0 ? jointConstraints[index] : new BoneOrientationConstraint(joint);
		
		if(index < 0)
		{
			index = jointConstraints.Count;
			jointConstraints.Add(jc);
		}
		
		AxisOrientationConstraint constraint = new AxisOrientationConstraint(axis, angleMin, angleMax);
		jc.axisConstrainrs.Add(constraint);
		
		jointConstraints[index] = jc;
     }

    // AddDefaultConstraints - Adds a set of default joint constraints for normal human poses.  
    // This is a reasonable set of constraints for plausible human bio-mechanics.
    public void AddDefaultConstraints()
    {
//        // Spine
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, ConstraintAxis.X, -10f, 45f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, ConstraintAxis.Y, -10f, 30f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, ConstraintAxis.Z, -10f, 30f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, ConstraintAxis.X, -90f, 95f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, ConstraintAxis.Z, -90f, 90f);

        // ShoulderCenter
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter, ConstraintAxis.X, -30f, 10f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter, ConstraintAxis.Y, -20f, 20f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter, ConstraintAxis.Z, -20f, 20f);

        // ShoulderLeft, ShoulderRight
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft, ConstraintAxis.X, 0f, 30f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight, ConstraintAxis.X, 0f, 30f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft, ConstraintAxis.Y, -60f, 60f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight, ConstraintAxis.Y, -30f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft, ConstraintAxis.Z, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight, ConstraintAxis.Z, -90f, 90f);

//        // ElbowLeft, ElbowRight
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.X, 300f, 360f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.X, 300f, 360f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.Y, 210f, 340f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.Y, 0f, 120f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.Z, -90f, 30f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.Z, 0f, 120f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.X, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.X, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.Z, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.Z, -90f, 90f);

        // WristLeft, WristRight
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft, ConstraintAxis.X, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.WristRight, ConstraintAxis.X, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.WristRight, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft, ConstraintAxis.Z, -90f, 90f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.WristRight, ConstraintAxis.Z, -90f, 90f);

//        // HipLeft, HipRight
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipLeft, ConstraintAxis.X, 0f, 90f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipRight, ConstraintAxis.X, 0f, 90f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipLeft, ConstraintAxis.Y, 0f, 0f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipRight, ConstraintAxis.Y, 0f, 0f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipLeft, ConstraintAxis.Z, 270f, 360f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipRight, ConstraintAxis.Z, 0f, 90f);

        // KneeLeft, KneeRight
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.KneeLeft, ConstraintAxis.X, 270f, 360f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.KneeRight, ConstraintAxis.X, 270f, 360f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.KneeLeft, ConstraintAxis.Y, 0f, 0f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.KneeRight, ConstraintAxis.Y, 0f, 0f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.KneeLeft, ConstraintAxis.Z, 0f, 0f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.KneeRight, ConstraintAxis.Z, 0f, 0f);

        // AnkleLeft, AnkleRight
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft, ConstraintAxis.X, 0f, 0f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.AnkleRight, ConstraintAxis.X, 0f, 0f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft, ConstraintAxis.Y, -40f, 40f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.AnkleRight, ConstraintAxis.Y, -40f, 40f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft, ConstraintAxis.Z, 0f, 0f);
        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.AnkleRight, ConstraintAxis.Z, 0f, 0f);

	}

    // ApplyBoneOrientationConstraints and constrain rotations.
    public void Constrain(ref Matrix4x4[] jointOrientations, ref bool[] jointTracked)
    {
        // Constraints are defined as a vector with respect to the parent bone vector, and a constraint angle, 
        // which is the maximum angle with respect to the constraint axis that the bone can move through.

        // Calculate constraint values. 0.0-1.0 means the bone is within the constraint cone. Greater than 1.0 means 
        // it lies outside the constraint cone.
        for (int i = 0; i < this.jointConstraints.Count; i++)
        {
            BoneOrientationConstraint jc = this.jointConstraints[i];

            if (!jointTracked[i] || jc.joint == (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter) 
            {
                // End joint is not tracked or Hip Center has no parent to perform this calculation with.
                continue;
            }

            // If the joint has a parent, constrain the bone direction to be within the constraint cone
            int parentIdx = KinectWrapper.GetSkeletonJointParent(jc.joint);

            // Local bone orientation relative to parent
            Matrix4x4 localOrientationMatrix = jointOrientations[parentIdx].inverse * jointOrientations[jc.joint];
			
			Vector3 localOrientationZ = (Vector3)localOrientationMatrix.GetColumn(2);
			Vector3 localOrientationY = (Vector3)localOrientationMatrix.GetColumn(1);
			if(localOrientationZ == Vector3.zero || localOrientationY == Vector3.zero)
				continue;
			
            Quaternion localRotation = Quaternion.LookRotation(localOrientationZ, localOrientationY);
			Vector3 eulerAngles = localRotation.eulerAngles;
			bool isConstrained = false;
			
			//Matrix4x4 globalOrientationMatrix = jointOrientations[jc.joint];
			//Quaternion globalRotation = Quaternion.LookRotation(globalOrientationMatrix.GetColumn(2), globalOrientationMatrix.GetColumn(1));
			
			for(int a = 0; a < jc.axisConstrainrs.Count; a++)
			{
				AxisOrientationConstraint ac = jc.axisConstrainrs[a];
				
				Quaternion axisRotation = Quaternion.AngleAxis(localRotation.eulerAngles[ac.axis], ac.rotateAround);
				//Quaternion axisRotation = Quaternion.AngleAxis(globalRotation.eulerAngles[ac.axis], ac.rotateAround);
				float angleFromMin = Quaternion.Angle(axisRotation, ac.minQuaternion);
				float angleFromMax = Quaternion.Angle(axisRotation, ac.maxQuaternion);
				 
				if(!(angleFromMin <= ac.angleRange && angleFromMax <= ac.angleRange))
				{
					// Keep the current rotations around other axes and only
					// correct the axis that has fallen out of range.
					//Vector3 euler = globalRotation.eulerAngles;
					
					if(angleFromMin > angleFromMax)
					{
						eulerAngles[ac.axis] = ac.angleMax;
					}
					else
					{
						eulerAngles[ac.axis] = ac.angleMin;
					}
					
					isConstrained = true;
				}
			}
			
			if(isConstrained)
			{
				Quaternion constrainedRotation = Quaternion.Euler(eulerAngles);

                // Put it back into the bone orientations
				localOrientationMatrix.SetTRS(Vector3.zero, constrainedRotation, Vector3.one); 
				jointOrientations[jc.joint] = jointOrientations[parentIdx] * localOrientationMatrix;
				//globalOrientationMatrix.SetTRS(Vector3.zero, constrainedRotation, Vector3.one); 
				//jointOrientations[jc.joint] = globalOrientationMatrix;
				
				switch(jc.joint)
				{
					case (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter:
						jointOrientations[(int)KinectWrapper.NuiSkeletonPositionIndex.Head] = jointOrientations[jc.joint];
						break;
					case (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft:
						jointOrientations[(int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft] = jointOrientations[jc.joint];
						break;
					case (int)KinectWrapper.NuiSkeletonPositionIndex.WristRight:
						jointOrientations[(int)KinectWrapper.NuiSkeletonPositionIndex.HandRight] = jointOrientations[jc.joint];
						break;
					case (int)KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft:
						jointOrientations[(int)KinectWrapper.NuiSkeletonPositionIndex.FootLeft] = jointOrientations[jc.joint];
						break;
					case (int)KinectWrapper.NuiSkeletonPositionIndex.AnkleRight:
						jointOrientations[(int)KinectWrapper.NuiSkeletonPositionIndex.FootRight] = jointOrientations[jc.joint];
						break;
				}
			}
			
//			globalRotation = Quaternion.LookRotation(globalOrientationMatrix.GetColumn(2), globalOrientationMatrix.GetColumn(1));
//			string stringToDebug = string.Format("{0}, {2}", (KinectWrapper.NuiSkeletonPositionIndex)jc.joint, 
//				globalRotation.eulerAngles, localRotation.eulerAngles);
//			Debug.Log(stringToDebug);
//			
//			if(debugText != null)
//				debugText.guiText.text = stringToDebug;
			
        }
    }


	// Joint Constraint structure to hold the constraint axis, angle and cone direction and associated joint.
    private struct BoneOrientationConstraint
    {
		// skeleton joint
		public int joint;
		
		// the list of axis constraints for this bone
		public List<AxisOrientationConstraint> axisConstrainrs;
		
		
        public BoneOrientationConstraint(int joint)
        {
            this.joint = joint;
			axisConstrainrs = new List<AxisOrientationConstraint>();
        }
    }
	
	
	private struct AxisOrientationConstraint
	{
		// the axis to rotate around
		public int axis;
		public Vector3 rotateAround;
				
		// min, max and range of allowed angle
		public float angleMin;
		public float angleMax;
		
		public Quaternion minQuaternion;
		public Quaternion maxQuaternion;
		public float angleRange;
				
		
		public AxisOrientationConstraint(ConstraintAxis axis, float angleMin, float angleMax)
		{
			// Set the axis that we will rotate around
			this.axis = (int)axis;
			
			switch(axis)
			{
				case ConstraintAxis.X:
					this.rotateAround = Vector3.right;
					break;
				 
				case ConstraintAxis.Y:
					this.rotateAround = Vector3.up;
					break;
				 
				case ConstraintAxis.Z:
					this.rotateAround = Vector3.forward;
					break;
			
				default:
					this.rotateAround = Vector3.zero;
					break;
			}
			
			// Set the min and max rotations in degrees
			this.angleMin = angleMin;
			this.angleMax = angleMax;
			
				 
			// Set the min and max rotations in quaternion space
			this.minQuaternion = Quaternion.AngleAxis(angleMin, this.rotateAround);
			this.maxQuaternion = Quaternion.AngleAxis(angleMax, this.rotateAround);
			this.angleRange = angleMax - angleMin;
		}
	}
	
}