import os
from joblib import load
import Config
from ClassificationResult import ClassificationExercise, ClassificationType


class PoseClassifier:
    def __init__(self):
        own_file_path = os.path.dirname(__file__)
        self.classifier = load(own_file_path + "/ml-models/" + Config.CLASSIFIER_MODEL)
        self.lastExercise = ClassificationExercise.NEGATIVE
        self.returnExercise = ClassificationExercise.NEGATIVE
        self.lastType = ClassificationType.NONE
        self.returnType = ClassificationType.NONE

        self.sameResultCount = 0

    def predict(self, suit_data):
        if not Config.CLASSIFY:
            return ClassificationExercise.NEGATIVE, ClassificationType.NONE

        # Classes are in the format of PUSHUP_POSE, PUSHUP_REP, NEGATIVE_NONE
        result = self.classifier.predict([suit_data])[0]
        result_values = result.split("_")

        exercise = ClassificationExercise[result_values[0]]
        _type = ClassificationType[result_values[1]]

        if exercise == self.lastExercise and _type == self.lastType:
            self.sameResultCount = self.sameResultCount + 1

            if self.sameResultCount >= 5:
                self.returnExercise = exercise
                self.returnType = _type
        else:
            self.sameResultCount = 0

        self.lastExercise = exercise
        self.lastType = _type

        return self.returnExercise, self.returnType
