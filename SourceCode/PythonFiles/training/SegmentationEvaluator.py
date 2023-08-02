import pathlib

from matplotlib.cbook import boxplot_stats

import Config
from TsDataUtil import extract_reps_from_data
from data.DataAccess import DataAccess
from data.DataManager import DataManager
from simulation.Simulator import Simulator
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np


class SegmentationEvaluator:
    def __init__(self):
        self.dataManager = DataManager()

    def evaluate_segmentation(self):
        print("Evaluating Segmentation...")

        results = pd.DataFrame(columns=["File", "Actual_Reps", "Detected_Reps", "Mapped_Reps"])
        paired_reps_result = pd.DataFrame(columns=["File", "Actual_Start", "Calculated_Start", "Difference"])

        for file in self.dataManager.labeled_files:
            # if int(file.split("_")[0]) < 6 or int(file.split("_")[0]) > 6:
            #     continue

            data = self.dataManager.get_data_from_file(self.dataManager.labeled_data_dir + file)

            sim_data = data[DataAccess.simulation_columns()]
            simulator = Simulator(sim_data)
            sim_result, error_result = simulator.run()

            labeled_segment_starts = data["index"][data["segment"] == "START"].values
            actual_detected_map = dict.fromkeys(labeled_segment_starts, None)
            calculated_segment_starts = []

            exercise_rep_dict = extract_reps_from_data(sim_result)
            for key, value in exercise_rep_dict.items():
                for rep in value:
                    calculated_start = rep["index"].iloc[0]
                    calculated_segment_starts.append(calculated_start)

                    if len(labeled_segment_starts) > 0:
                        closest_actual_rep = min(labeled_segment_starts, key=lambda x: abs(x - calculated_start))
                        if actual_detected_map[closest_actual_rep] is None or abs(calculated_start - closest_actual_rep) < abs(actual_detected_map[closest_actual_rep] - closest_actual_rep):
                            actual_detected_map[closest_actual_rep] = calculated_start

            paired_reps = np.NaN
            if len(labeled_segment_starts) > 0:
                # Filter reps that werent paired and are still None
                filtered_map = dict(filter(lambda x: x[1], actual_detected_map.items()))

                # Check if there is a one-to-one mapping with only additional reps filtered out or if some don't
                # get a pair even though they should have
                if len(labeled_segment_starts) >= len(calculated_segment_starts) and len(filtered_map) < len(calculated_segment_starts):
                    print("Problem")
                if len(labeled_segment_starts) <= len(calculated_segment_starts) and len(filtered_map) < len(labeled_segment_starts):
                    print("Problem")

                for key, value in filtered_map.items():
                    paired_reps_result = paired_reps_result.append({"File": file[3::],
                                               "Actual_Start": key,
                                               "Calculated_Start": value,
                                               "Difference": value - key}, ignore_index=True)

                paired_reps = len(filtered_map)

            results = results.append({"File": file[3::],
                                    "Actual_Reps": len(labeled_segment_starts),
                                    "Detected_Reps": len(calculated_segment_starts),
                                    "Mapped_Reps": paired_reps},
                                     ignore_index=True)

        self.dataManager.save_data_to_path("SegmentationEvaluation.csv", results)
        self.dataManager.save_data_to_path("SegmentationEvaluationReps.csv", paired_reps_result)

    def plot_segmentation_evaluation(self):
        print("Plotting Segmentation Evaluation...")
        plt.rcParams.update({'font.size': 15})

        data = self.dataManager.get_data_from_file("SegmentationEvaluation.csv", make_numeric=False)

        b = data.groupby("File", sort=False).sum()
        
        self.dataManager.save_data_to_path("BarsGrouped.csv", b)

        fig, ax = plt.subplots(figsize=(9, 7))
        x = np.arange(len(b.index))
        width = 0.3
        ax.bar(x - width, b["Mapped_Reps"], width, label="Mapped")
        ax.bar(x, b["Detected_Reps"], width, label="Detected")
        ax.bar(x + width, b["Actual_Reps"], width, label="Actual")
        ax.set_xticks(x)
        ax.set_xticklabels(b.index)
        ax.legend(["Mapped", "Detected", "Actual"], loc="lower right")
        plt.ylabel("Repetitions")
        plt.xlabel("Exercise & Pause Duration")
        plt.xticks(rotation=90)

        rep_ratio_string = "%d/%d" % (b["Detected_Reps"].sum(), b["Actual_Reps"].sum())
        stats_text = "Detected Reps / Actual Reps: %s" % rep_ratio_string
        # plt.xlabel(stats_text)
        plt.savefig("SegmentationEvaluationBars.pdf", bbox_inches="tight")

        data = self.dataManager.get_data_from_file("SegmentationEvaluationReps.csv", make_numeric=False)
        custom_dict = {'Lunge_Short': 1, 'Lunge_Medium': 2, 'Lunge_Long': 3,
                       'Pushup_Short': 4, 'Pushup_Medium': 5, 'Pushup_Long': 6,
                       'Squat_Short': 7, 'Squat_Medium': 8, 'Squat_Long': 9,}
        data['rank'] = data['File'].map(custom_dict)
        data.sort_values("rank", inplace=True)
        grouped_data = data.groupby("File", sort=False)
        plt.figure(figsize=(9, 7))
        boxplot = grouped_data.boxplot(column="Difference", subplots=False)
        plt.xticks(range(1, 10), grouped_data.groups.keys(), rotation=90)
        plt.ylabel("Difference in ms")
        ax = plt.gca()
        ax.set_ylim([-2000, 2000])

        plt.savefig("SegmentationEvaluationBoxesAdjusted.pdf", bbox_inches="tight")

    def plot_t_denoise_curves(self):
        path = str(pathlib.Path(__file__).parent) + "/../plots/SegmentationEvaluation/"
        x = range(1, 16)
        results = {
            "Short Detected": [],
            "Short Mapped" : [],
            "Medium Detected": [],
            "Medium Mapped": [],
            "Long Detected": [],
            "Long Mapped": [],
        }

        for i in x:
            filepath = path + "M_Good_D8_G%d/SegmentationEvaluation.csv" % i
            data = self.dataManager.get_data_from_file(filepath, make_numeric=False)
            pause_lengths = data["File"].apply(lambda x: x.split("_")[1])
            data["pause"] = pause_lengths

            grouped_data = data.groupby("pause")

            for name, group in grouped_data:
                if "Fast" in name:
                    key = "Short"
                elif "Medium" in name:
                    key = "Medium"
                elif "Slow" in name:
                    key = "Long"

                results[key + " Detected"].append(group["Detected_Reps"].sum())
                results[key + " Mapped"].append(group["Mapped_Reps"].sum())

        plt.figure(figsize=(7,7))
        for key, value in results.items():
            plt.plot(x, value, label=key)
        plt.grid(True, linestyle=':')
        plt.xticks(x)
        plt.xlabel("G_limit")
        plt.ylabel("Repetitions")
        plt.legend()
        plt.savefig("GlimitPauseCurves.pdf", bbox_inches="tight")

    def plot_segmentation_glimit_avg_iqr_range(self):
        path = str(pathlib.Path(__file__).parent) + "/../plots/SegmentationEvaluation/"
        average_iqr = []
        x = range(1, 16)

        for i in x:
            filepath = path + "M_Good_D8_G%d/SegmentationEvaluationReps.csv" % i
            data = self.dataManager.get_data_from_file(filepath, make_numeric=False)

            grouped_data = data.groupby("File")["Difference"]
            q1, q3, median, std = grouped_data.quantile(0.25), grouped_data.quantile(0.75), grouped_data.median(), grouped_data.std()
            iqr = q3 - q1
            print(i, "Median", median.mean())
            print(i, "Std", std.mean())

            # for name, group in grouped_data:
            #     median = grouped_data.median()
            #     offset_to_median = (grouped_data - median).abs()
            #
            #     for j in range(0, 300):
            #         percentage = (offset_to_median > j).sum()


            average_iqr.append(iqr.mean())

        plt.figure(figsize=(7, 7))
        plt.grid(True, linestyle=':')
        plt.plot(x, average_iqr)
        plt.xticks(x)
        plt.yticks([78, 88, 96, 103, 109, 120, 126, 141, 179, 196, 218, 286])
        plt.xlabel("G_limit")
        plt.ylabel("Average IQR (ms)")
        plt.savefig("AverageIQR.pdf", bbox_inches="tight")

