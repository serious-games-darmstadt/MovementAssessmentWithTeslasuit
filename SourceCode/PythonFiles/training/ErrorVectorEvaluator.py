import Config
from TsDataUtil import extract_reps_from_data
from data.DataAccess import DataAccess
from data.DataManager import DataManager
from simulation.Simulator import Simulator
import numpy as np
import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt


class ErrorVectorEvaluator:
    def __init__(self):
        self.dataManager = DataManager()

        self.vectorBins = {
            "up" : np.array([0, 1, 0]),
            "down" : np.array([0, -1, 0]),

            "front" : np.array([0, 0, -1]),
            "front_up" : np.array([0, 1, -1]),
            "front_down" : np.array([0, -1, -1]),

            "front_left" : np.array([1, 0, -1]),
            "front_left_up" : np.array([1, 1, -1]),
            "front_left_down" : np.array([1, -1, -1]),

            "front_right" : np.array([-1, 0, -1]),
            "front_right_up" : np.array([-1, 1, -1]),
            "front_right_down" : np.array([-1, -1, -1]),

            "left" : np.array([1, 0, 0]),
            "left_up" : np.array([1, 1, 0]),
            "left_down" : np.array([1, -1, 0]),

            "back_left" : np.array([1, 0, 1]),
            "back_left_up" : np.array([1, 1, 1]),
            "back_left_down" : np.array([1, -1, 1]),

            "back" : np.array([0, 0, 1]),
            "back_up" : np.array([0, 1, 1]),
            "back_down" : np.array([0, -1, 1]),

            "back_right" : np.array([-1, 0, 1]),
            "back_right_up" : np.array([-1, 1, 1]),
            "back_right_down" : np.array([-1, -1, 1]),

            "right" : np.array([-1, 0, 0]),
            "right_up" : np.array([-1, 1, 0]),
            "right_down" : np.array([-1, -1, 0]),
        }
        


    def evaluateErrorVectors(self):
        print("Evaluating Error Vectors...")

        result = []

        for file in self.dataManager.error_files:
            if Config.EVAL_USE_DATA == "GOOD" and file not in Config.EVAL_GOOD_DATA:
                continue
            if Config.EVAL_USE_DATA == "PARTIAL" and int(file.split("_")[0]) <= 5:
                continue

            data = self.dataManager.get_data_from_file(self.dataManager.error_data_dir + file)

            sim_data = data[DataAccess.simulation_columns()]
            simulator = Simulator(sim_data)
            sim_result, error_data = simulator.run()
            error_data = error_data.drop(["timestamp"], axis=1)

            if Config.EVAL_USE_SEGMENTS == "AUDIO":
                sim_result["segment"] = data["segment"]

            exercise_rep_dict = extract_reps_from_data(sim_result)
            for key, value in exercise_rep_dict.items():
                for rep in value:
                    rep_start = rep.iloc[0].name
                    rep_end = rep.iloc[-1].name + 1
                    rep_error = error_data.iloc[rep_start:rep_end]
                    abs_rep_error = abs(rep_error)

                    max_error_joint = abs_rep_error.max().idxmax()
                    max_error_index = abs_rep_error.idxmax()[max_error_joint]
                    max_error_row = error_data.iloc[max_error_index]

                    rep_result = [file[3:-4]]
                    for joint in Config.streamedJoints:
                        joint_error = max_error_row[DataAccess.get_joint_properties_names(joint)].to_numpy()
                        rep_result.append(self.bin3dVector(joint_error))

                    result.append(rep_result)

        columns = ["File"]
        columns.extend(Config.streamedJoints)
        result_dataframe = pd.DataFrame(result, columns=columns)
        self.dataManager.save_data_to_path("BinnedErrors.csv", result_dataframe)


    def bin3dVector(self, vector):
        if (vector == [0, 0, 0]).all():
            return "None"

        min_direction = None
        min_angle = 7 # angle is in radians so just use something bigger than 2pi to start
        for key, value in self.vectorBins.items():
            angle = self.angle(vector, value)
            if angle < min_angle:
                min_direction = key
                min_angle = angle

        return min_direction

    def angle(self, vector_1, vector_2):
        unit_vector_1 = vector_1 / np.linalg.norm(vector_1)
        unit_vector_2 = vector_2 / np.linalg.norm(vector_2)
        dot_product = np.dot(unit_vector_1, unit_vector_2)
        angle = np.arccos(dot_product)
        return angle

    def plot_error_evaluation(self):
        print("Plotting Error Evaluation")
        plt.rcParams.update({'font.size': 15})

        data = self.dataManager.get_data_from_file("BinnedErrors.csv", make_numeric=False)
        grouped_data = data.groupby("File")

        for name, group in grouped_data:
            plt.figure(figsize=(12, 10))
            numeric = group.drop(["File", "leftHip", "leftKnee", "leftAnkle"], axis=1)
            heatmap = numeric.apply(pd.Series.value_counts)
            heatmap = heatmap.fillna(0)

            for key in self.vectorBins.keys():
                if key not in heatmap.index:
                    heatmap.loc[key] = 0

            heatmap["cat"] = pd.Categorical(heatmap.index,
                                            categories=self.vectorBins.keys(),
                                            ordered=True)
            heatmap = heatmap.sort_values("cat")
            heatmap_final = heatmap.drop(["cat"], axis=1)
            f = sns.heatmap(heatmap_final, annot=True, xticklabels=1, yticklabels=1)
            plt.savefig(name + "_Heat.pdf", bbox_inches="tight")

