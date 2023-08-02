# The nodes that are streamed from Unity to Python
from ClassificationResult import ClassificationExercise

streamedNodes = ["RightUpperArm", "RightLowerArm", "LeftUpperArm", "LeftLowerArm",
                 "Chest", "Spine", "RightUpperLeg", "RightLowerLeg", "LeftUpperLeg", "LeftLowerLeg"]
# The node properties that are streamed from Unity to Python
streamedNodeProperties = ["gyroscope", "accelerometer"]
streamedJoints = ["pelvis", "lowerSpine", "middleSpine", "upperSpine", "rightClavicle", "rightShoulder",
                  "rightElbow", "rightWrist", "leftClavicle", "leftShoulder", "leftElbow", "leftWrist", "rightHip",
                  "rightKnee", "rightAnkle", "leftHip", "leftKnee", "leftAnkle"]

simulatedNodes = streamedNodes
simulatedProperties = streamedNodeProperties
simulatedJoints = streamedJoints

resultNodes = streamedNodes
resultProperties = streamedNodeProperties
resultJoints = {
    "Torso": ["pelvis", "lowerSpine", "middleSpine", "upperSpine", "rightClavicle", "leftClavicle"],
    "Right Arm": ["rightShoulder", "rightElbow", "rightWrist"],
    "Left Arm": ["leftShoulder", "leftElbow", "leftWrist"],
    "Right Leg": ["rightHip", "rightKnee", "rightAnkle"],
    "Left Leg": ["leftHip", "leftKnee", "leftAnkle"]
}

classifierNodes = streamedNodes
classifierProperties = ["gyroscope", "accelerometer"]

proMpJoints = streamedJoints

class_segmentation_map = {
    ClassificationExercise.PUSHUP: "RightUpperArm",
    ClassificationExercise.SQUAT: "RightUpperLeg",
    ClassificationExercise.LUNGE: "RightUpperLeg"
}


CLASSIFIER_MODEL = "svm_model"
SEGMENTATION_MODEL = "HEURISTIC" # CLASSIFIER, HEURISTIC
predictedClass = None
STD_MULTIPLIER = 2

DENOISE = True
CLASSIFY = True
SEGMENT = True
EVALUATE = True

# AUDIO; MANUAL
EVAL_USE_SEGMENTS = "AUDIO"
DENOISE_LENGTH = 8
SEGMENT_THRESHOLD = 8
# GOOD; ALL; PARTIAL
EVAL_USE_DATA = "ALL"

EVAL_GOOD_DATA = [
    "01_Pushup_Slow.csv", "01_Pushup_Medium.csv", "01_Pushup_Fast.csv",
    "02_Squat_Slow.csv", "02_Squat_Medium.csv", "02_Squat_Fast.csv", "02_Lunge_Slow.csv", "02_Lunge_Medium.csv", "02_Lunge_Fast.csv",
    "03_Squat_Slow.csv", "03_Squat_Medium.csv", "03_Squat_Fast.csv",
    "04_Squat_Slow.csv", "04_Squat_Medium.csv", "04_Squat_Fast.csv", "04_Lunge_Slow.csv", "04_Lunge_Medium.csv", "04_Lunge_Fast.csv",
    "07_Lunge_Slow.csv", "07_Lunge_Medium.csv", "07_Lunge_Fast.csv",
    "08_Squat_Slow.csv", "08_Squat_Medium.csv", "08_Squat_Fast.csv", "08_Lunge_Slow.csv", "08_Lunge_Medium.csv", "08_Lunge_Fast.csv",
    "10_Pushup_Slow.csv", "10_Pushup_Medium.csv", "10_Pushup_Fast.csv",
    "12_Squat_Slow.csv", "12_Squat_Medium.csv", "12_Squat_Fast.csv", "12_Pushup_Slow.csv", "12_Pushup_Medium.csv", "12_Pushup_Fast.csv",
    "13_Squat_Slow.csv", "13_Squat_Medium.csv", "13_Squat_Fast.csv", "13_Pushup_Slow.csv", "13_Pushup_Medium.csv", "13_Pushup_Fast.csv", "13_Lunge_Slow.csv", "13_Lunge_Medium.csv", "13_Lunge_Fast.csv",

    # Good Error Data
    "01_Pushup_Error1.csv", "01_Pushup_Error2.csv", "01_Pushup_Error3.csv",
    "02_Squat_Error1.csv", "02_Squat_Error2.csv", "02_Squat_Error3.csv", "02_Lunge_Error1.csv", "02_Lunge_Error2.csv", "02_Lunge_Error3.csv",
    "03_Squat_Error1.csv", "03_Squat_Error2.csv", "03_Squat_Error3.csv",
    "04_Squat_Error1.csv", "04_Squat_Error2.csv", "04_Squat_Error3.csv", "04_Lunge_Error1.csv", "04_Lunge_Error2.csv", "04_Lunge_Error3.csv",
    "07_Lunge_Error1.csv", "07_Lunge_Error2.csv", "07_Lunge_Error3.csv",
    "08_Squat_Error1.csv", "08_Squat_Error2.csv", "08_Lunge_Error2.csv",
    "10_Pushup_Error1.csv", "10_Pushup_Error2.csv", "10_Pushup_Error3.csv",
    "12_Squat_Error1.csv", "12_Squat_Error2.csv", "12_Squat_Error3.csv", "12_Pushup_Error1.csv", "12_Pushup_Error2.csv", "12_Pushup_Error3.csv",
    "13_Squat_Error1.csv", "13_Squat_Medium.csv", "13_Squat_Fast.csv", "13_Pushup_Slow.csv", "13_Pushup_Medium.csv", "13_Pushup_Fast.csv", "13_Lunge_Slow.csv", "13_Lunge_Medium.csv", "13_Lunge_Fast.csv",
]   











