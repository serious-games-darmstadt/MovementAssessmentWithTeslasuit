import numpy as np
import pandas as pd

from ClassificationResult import ClassificationExercise, ClassificationType


def filter_segments(data):
    segmentStart = None

    for index, row in data.iterrows():
        if row["segment"] == "START":
            if segmentStart is not None:
                data[segmentStart]["segment"] = "NONE"
            segmentStart = index

        if row["segment"] == "END":
            if segmentStart is None:
                data[index]["segment"] = "NONE"
            else:
                segmentStart = None

    return data


'''
Simpler Method that assumes that START and END of segments are always properly marked and labeled
'''
def extract_reps_from_training_data(data):
    exercise_rep_dict = {}
    segmentStart = 0
    exercise = None

    for index, row in data.iterrows():
        if row["segment"] == "START":
            segmentStart = index
            exercise = ClassificationExercise[row["label"].split("_")[0]]

        if row["segment"] == "END":
            repData = data.loc[segmentStart:index, :]
            if exercise.name not in exercise_rep_dict:
                exercise_rep_dict[exercise.name] = []

            numeric_rep_data = repData.drop(["label", "segment"], axis=1)
            exercise_rep_dict[exercise.name].append(numeric_rep_data)

    return exercise_rep_dict


def extract_reps_from_data(data):
    exercise_rep_dict = {}
    segmentStart = 0
    exercise = None
    in_rep_count = 0

    for index, row in data.iterrows():
        if ClassificationType[row["label"].split("_")[1]] == ClassificationType.REP:
            in_rep_count = in_rep_count + 1

        if row["segment"] == "START":
            segmentStart = index
            exercise = ClassificationExercise[row["label"].split("_")[0]]
            in_rep_count = 0

        if row["segment"] == "END":
            if in_rep_count > 10 and segmentStart is not None:
                repData = data.loc[segmentStart:index, :]
                if exercise.name not in exercise_rep_dict:
                    exercise_rep_dict[exercise.name] = []

                numeric_rep_data = repData.drop(["label", "segment"], axis=1)
                exercise_rep_dict[exercise.name].append(numeric_rep_data)
                segmentStart = None
            in_rep_count = 0

    return exercise_rep_dict


'''
Zero-aligns the data so that the first entry has index 0.
'''


def to_zero_aligned_timesteps(data):
    zero_aligned_data = []

    for entry in data:
        time_offset = int(entry.iloc[0]["index"])
        entry.loc[:, "index"] = entry["index"].apply(lambda x: int(x) - time_offset)
        zero_aligned_data.append(entry)

    return zero_aligned_data


'''
Duration in ms.
Data is an array of DataFrames, each with columns index, Segment, Label, ...numeric columns...
'''


def to_uniform_timesteps(data):
    uniform_timestep_data = []

    for entry in data:
        index = np.arange(int(entry.iloc[-1]["index"]) + 1)

        # Empty Dataframe
        adjustedData = pd.DataFrame(index=index, columns=entry.columns, dtype=float)

        # Make Segment and Label columns strings
        # adjustedData.segment = adjustedData.segment.astype(str)
        # adjustedData.label = adjustedData.label.astype(str)

        # Copy Data for indexes that exist
        adjustedData.iloc[entry["index"]] = entry

        # Interpolate numeric data for indexes that don't exist yet
        adjustedData.interpolate(inplace=True, axis=0)

        adjustedData.replace("nan", np.nan, inplace=True)
        # adjustedData["segment"].fillna("NONE", inplace=True)
        # adjustedData["label"].ffill(inplace=True)

        adjustedData.drop("index", inplace=True, axis=1)
        uniform_timestep_data.append(adjustedData)

    return uniform_timestep_data


def equalize_length(data):
    targetDuration = 0
    for entry in data:
        if targetDuration < len(entry):
            targetDuration = len(entry)

    adjustedData = []
    for entry in data:
        if len(entry) - 1 < targetDuration:
            entry_as_numpy = entry.to_numpy()
            lastDataPoint = entry_as_numpy[-1]

            for j in range(targetDuration - len(entry)):
                entry_as_numpy = np.append(entry_as_numpy, [lastDataPoint], axis=0)
            adjustedData.append(pd.DataFrame(entry_as_numpy, columns=entry.columns, dtype=float))
        else:
            adjustedData.append(entry)

    return adjustedData
