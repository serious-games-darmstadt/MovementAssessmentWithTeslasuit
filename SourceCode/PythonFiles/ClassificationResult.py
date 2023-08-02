from enum import Enum


class ClassificationType(Enum):
    NONE = 1
    POSE = 2
    REP = 3


class ClassificationExercise(Enum):
    NEGATIVE = 1
    PUSHUP = 2
    SQUAT = 3
    LUNGE = 4
