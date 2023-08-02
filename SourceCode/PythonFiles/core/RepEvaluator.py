import os
import time

from joblib import load
import numpy as np

import ClassificationResult
from ClassificationResult import ClassificationExercise
import Config
from data.DataAccess import DataAccess


class RepEvaluator:
    def __init__(self):
        own_file_path = os.path.dirname(__file__)
        # self.proMps = {"PushupStart": load(own_file_path + "/ml-models/proMpPushup")}
        # self.proMps["PushupStart"]["means"] = self.proMps["PushupStart"]["means"].iloc[:, 60:].to_numpy()
        # self.proMps["PushupStart"]["std"] = self.proMps["PushupStart"]["std"].iloc[:, 60:].to_numpy()

        self.proMps = {}
        for name, member in ClassificationExercise.__members__.items():
            if member == ClassificationExercise.NEGATIVE:
                continue

            # Files need to be called e.g. proMp_PUSHUP and need to provide only joint data
            # as a numpy array
            try:
                proMp = load(own_file_path + "/ml-models/proMp_" + name)
                proMp["means"] = proMp["means"].to_numpy()
                proMp["std"] = proMp["std"].to_numpy()
                self.proMps[name] = proMp
            except FileNotFoundError:
                if Config.EVALUATE:
                    print("Missing ProMP: ", name)

        self.activeProMp = None
        self.noError = np.zeros(len(Config.proMpJoints) * 3)
        self.repStartedTimestamp = None
        self.in_rep = False

    def repStarted(self, exercise: ClassificationExercise, timestamp):
        if not Config.EVALUATE:
            return

        if exercise == ClassificationExercise.NEGATIVE:
            print("ERROR: Rep Start was detected but the classified exercise is NEGATIVE!")
            return

        self.activeProMp = self.proMps[exercise.name]
        self.repStartedTimestamp = timestamp


    def evaluate(self, timestamp, data, exercise: ClassificationExercise, _type: ClassificationResult.ClassificationType):
        if not Config.EVALUATE:
            return self.noError, False

        if _type == ClassificationResult.ClassificationType.REP:
            self.in_rep = True

        if self.activeProMp is None or exercise == ClassificationExercise.NEGATIVE or not self.in_rep:
            return self.noError, False

        repTimeIndex = timestamp - self.repStartedTimestamp + 210
        if repTimeIndex >= len(self.activeProMp["means"]):
            self.activeProMp = None
            self.in_rep = False
            return self.noError, True

        mean = self.activeProMp["means"][repTimeIndex]
        std = self.activeProMp["std"][repTimeIndex]

        # Calculate absolute difference, subtract standard deviation and set all negative values to zero.
        # Result: By how much is the standard deviation exceeded, i.e. how big is the error? Within std equals no error.
        difference = mean-data
        absDiff = abs(difference)
        error = absDiff - Config.STD_MULTIPLIER * std
        filteredError = error
        np.clip(error, 0, None, out=filteredError)

        # Then take all values where deviation from mean was downwards, i.e. actual smaller than mean
        # and multiply by -1. No we have positive values for upwards deviation and negative values
        # for downwards deviation.
        filteredError[data < mean] = filteredError[data < mean] * (-1)

        # then make the error relative. Add 0.01 to avoid division by zero
        relativeError = filteredError / (std + 0.01)

        return relativeError, False
