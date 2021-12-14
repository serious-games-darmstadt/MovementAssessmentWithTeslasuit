import os
import pathlib

import pandas as pd
import numpy as np
import Config
from data.DataAccess import DataAccess


class DataManager:
    def __init__(self):
        self.unlabeled_data_dir = str(pathlib.Path(__file__).parent) + "/../training/1-unlabelled-data/"
        self.unlabeled_files = []
        for filename in os.listdir(self.unlabeled_data_dir):
            self.unlabeled_files.append(filename)

        self.negativ_lebaled_data_dir = str(pathlib.Path(__file__).parent) + "/../training/2-negativ-labeled-data/"
        self.negativ_labeled_files = []
        for filename in os.listdir(self.negativ_lebaled_data_dir):
            self.negativ_labeled_files.append(filename)

        self.labeled_data_dir = str(pathlib.Path(__file__).parent) + "/../training/3-labeled-data/"
        self.labeled_files = []
        for filename in os.listdir(self.labeled_data_dir):
            self.labeled_files.append(filename)

        self.segmented_data_dir = str(pathlib.Path(__file__).parent) + "/../training/4-segmented-data/"
        self.segmented_files = []
        for filename in os.listdir(self.segmented_data_dir):
            self.segmented_files.append(filename)

        self.error_data_dir = str(pathlib.Path(__file__).parent) + "/../training/5-error-data/"
        self.error_files = []
        for filename in os.listdir(self.error_data_dir):
            self.error_files.append(filename)

        self.predicted_data_dir = str(pathlib.Path(__file__).parent) + "/../training/4-predicted-data/"
        self.predicted_files = []
        for filename in os.listdir(self.predicted_data_dir):
            self.predicted_files.append(filename)

        simulation_data_dir = str(pathlib.Path(__file__).parent) + "/../simulation/simulation-data/"
        self.simulation_files = []
        for filename in os.listdir(simulation_data_dir):
            self.simulation_files.append(simulation_data_dir + filename)

    def get_simulation_data(self):
        dataframes = []

        for filename in self.simulation_files:
            partial_data = pd.read_csv(filename, sep=";", header=0, skiprows=1)
            dataframes.append(partial_data)
        combined_data = pd.concat(dataframes)
        simulation_column_names = DataAccess.simulation_columns()
        return self.makeDataNumeric(combined_data[simulation_column_names])

    def get_data_from_file(self, filename, make_numeric=True):
        data = pd.read_csv(filename, sep=";", header=0, skiprows=1)

        if make_numeric:
            numeric_data = self.makeDataNumeric(data)
            return numeric_data

        return data

    def get_classifier_training_data(self):
        print("Getting Classifier Training Data...")
        dataframes = []
        groups = []
        labels_by_files = []

        for filename in self.labeled_files:
            partial_training_data = pd.read_csv(self.labeled_data_dir + filename, sep=";", header=0, skiprows=1)
            dataframes.append(partial_training_data)

            subjectId = int(filename.split("_")[0])
            exercise_name = filename[3:-4]
            groups.extend(np.repeat(subjectId, len(partial_training_data)))
            labels_by_files.extend([exercise_name] * len(partial_training_data))

        combined_data = pd.concat(dataframes)
        combined_data = combined_data.reset_index(drop=True)
        segment_starts = combined_data["segment"] == "START"
        print("Repetitions: ", segment_starts.sum())

        classifierColumns = DataAccess.classifier_columns(include_label=True)
        return self.makeDataNumeric(combined_data[classifierColumns]), groups, segment_starts, pd.Series(labels_by_files)

    def get_labeled_data(self):
        dataframes = []

        for filename in self.labeled_files:
            if Config.EVAL_USE_DATA == "GOOD" and filename not in Config.EVAL_GOOD_DATA:
                continue

            partial_training_data = pd.read_csv(self.labeled_data_dir + filename, sep=";", header=0, skiprows=1)
            dataframes.append(partial_training_data)

        combined_data = pd.concat(dataframes)
        combined_data = combined_data.reset_index(drop=True)

        return self.makeDataNumeric(combined_data)

    def makeDataNumeric(self, data):
        hasLabel = "label" in data.columns
        hasSegment = "segment" in data.columns
        if hasLabel:
            label = data["label"]
            data = data.drop("label", axis=1)

        if hasSegment:
            segment = data["segment"]
            data = data.drop("segment", axis=1)

        for col in data.select_dtypes([np.object]).columns:
            columnAsFloat = data[col].str.replace(",", ".").astype(float)
            data.loc[:, col] = columnAsFloat

        if hasLabel:
            data.insert(1, "label", label)

        if hasSegment:
            data.insert(2, "segment", segment)

        return data

    def save_data_to_path(self, path, data):
        print("Saving ", path)
        data.to_csv(path, sep=";", header=True, index=False)
        with open(path, "r+") as f:
            content = f.read()
            f.seek(0, 0)
            f.write("SEP=;\n" + content)
        print("Saved")