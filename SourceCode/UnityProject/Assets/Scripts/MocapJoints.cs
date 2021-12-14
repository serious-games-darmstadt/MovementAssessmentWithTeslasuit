using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class MocapJoints
    {
        private static MocapJoints INSTANCE;
        private Dictionary<String, Transform> JointTransforms;
        public List<String> JointNames = new List<string> {"pelvis", "lowerSpine", "middleSpine", "upperSpine",
            "rightClavicle", "rightShoulder", "rightElbow", "rightWrist", "leftClavicle", "leftShoulder", 
            "leftElbow", "leftWrist", "rightHip", "rightKnee", "rightAnkle", "leftHip", "leftKnee", "leftAnkle"};

        public Dictionary<String, Vector3> JointZeroPositions = new Dictionary<string, Vector3>();

        public Dictionary<Exercise, Transform> ExerciseReferenceJoints;

        public Dictionary<string, Color> JointColorMap;

        public static MocapJoints GetInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new MocapJoints();
            }

            return INSTANCE;
        }

        public Transform GetJoint(String joint)
        {
            return JointTransforms[joint];
        }
        
        private MocapJoints()
        {
            Transform pelvis = GameObject.Find("Teslasuit_Man").transform.Find("Maniken_skeletool:root")
                .Find("Maniken_skeletool:pelvis");
            Transform lowerSpine = pelvis.Find("Maniken_skeletool:spine_01");
            Transform middleSpine = lowerSpine.Find("Maniken_skeletool:spine_02");
            Transform upperSpine = middleSpine.Find("Maniken_skeletool:spine_04");
            
            Transform rightClavicle = upperSpine.Find("Maniken_skeletool:clavicle_r");
            Transform rightShoulder = rightClavicle.Find("Maniken_skeletool:shoulder_r");
            Transform rightElbow = rightShoulder.Find("Maniken_skeletool:elbow_r");
            Transform rightWrist = rightElbow.Find("Maniken_skeletool:hand_01_r");
            
            Transform leftClavicle = upperSpine.Find("Maniken_skeletool:clavicle_l");
            Transform leftShoulder = leftClavicle.Find("Maniken_skeletool:shoulder_l");
            Transform leftElbow = leftShoulder.Find("Maniken_skeletool:elbow_l");
            Transform leftWrist = leftElbow.Find("Maniken_skeletool:hand_01_l");
            
            Transform rightHip = pelvis.Find("Maniken_skeletool:hip_r");
            Transform rightKnee = rightHip.Find("Maniken_skeletool:knee_r");
            Transform rightAnkle = rightKnee.Find("Maniken_skeletool:foot_r");
            
            Transform leftHip = pelvis.Find("Maniken_skeletool:hip_r");
            Transform leftKnee = leftHip.Find("Maniken_skeletool:knee_r");
            Transform leftAnkle = leftKnee.Find("Maniken_skeletool:foot_r");


            JointTransforms = new Dictionary<string, Transform>
            {
                {"pelvis", pelvis},
                {"lowerSpine", lowerSpine},
                {"middleSpine", middleSpine},
                {"upperSpine", upperSpine},
                {"rightClavicle", rightClavicle},
                {"rightShoulder", rightShoulder},
                {"rightElbow", rightElbow},
                {"rightWrist", rightWrist},
                {"leftClavicle", leftClavicle},
                {"leftShoulder", leftShoulder},
                {"leftElbow", leftElbow},
                {"leftWrist", leftWrist},
                {"rightHip", rightHip},
                {"rightKnee", rightKnee},
                {"rightAnkle", rightAnkle},
                {"leftHip", leftHip},
                {"leftKnee", leftKnee},
                {"leftAnkle", leftAnkle}
            };
            
            foreach (var joint in JointNames)
            {
                JointZeroPositions.Add(joint, Vector3.zero);
            }
            
            ExerciseReferenceJoints = new Dictionary<Exercise, Transform>
            {
                {Exercise.Negative, pelvis},
                {Exercise.Pushup, rightWrist},
                {Exercise.Squat, rightAnkle},
                {Exercise.Lunge, rightAnkle},
            };
            
            JointColorMap = new Dictionary<string, Color>
            {
                {"pelvis", Color.red},
                {"lowerSpine", Color.blue},
                {"middleSpine", Color.black},
                {"upperSpine", Color.red},
                {"rightClavicle", Color.white},
                {"rightShoulder", Color.gray},
                {"rightElbow", Color.blue},
                {"rightWrist", Color.black},
                {"leftClavicle", Color.yellow},
                {"leftShoulder", Color.cyan},
                {"leftElbow", Color.blue},
                {"leftWrist", Color.black},
                {"rightHip", Color.gray},
                {"rightKnee", Color.blue},
                {"rightAnkle", Color.black},
                {"leftHip", Color.cyan},
                {"leftKnee", Color.blue},
                {"leftAnkle", Color.black}
            };
        }
    }
}