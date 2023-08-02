import Config
from data.DataAccess import DataAccess
from data.DataManager import DataManager
from simulation.Simulator import Simulator
import pandas as pd
pd.options.mode.chained_assignment = None

class AutomaticLabeling:
    def __init__(self, pose_label_extend, rep_pose_gap_half):
        print("Start Labeling...")
        POSE_LABEL_EXTEND = pose_label_extend
        REP_POSE_GAP_HALF = rep_pose_gap_half

        dataManager = DataManager()

        for index, file in enumerate(dataManager.negativ_labeled_files):
            if Config.EVAL_USE_DATA == "GOOD" and file not in Config.EVAL_GOOD_DATA:
                continue
            # elif Config.EVAL_USE_DATA == "ALL" and int(file.split("_")[0]) < 6:
            #     continue

            print(file)

            data = dataManager.get_data_from_file(dataManager.negativ_lebaled_data_dir + file)

            if Config.EVAL_USE_SEGMENTS == "MANUAL":
                manual_segmented_data = dataManager.get_data_from_file(dataManager.segmented_data_dir + file)
                data["segment"] = manual_segmented_data["segment"]

            # Step 0: Determine Exercise:
            exercise_name = file.split('_')[1].upper()

            # Step 1: Simulate data to denoise.
            Config.CLASSIFY = False
            Config.SEGMENT = False
            Config.EVALUATE = False
            sim_data = data[DataAccess.simulation_columns()]
            simulator = Simulator(sim_data)
            sim_result, error_result = simulator.run()

            # Step 2: Label Data
            if "segment" not in data.columns:
                data["segment"] = "NONE"

            labels = data["label"]
            segmentStarts = data.index[data["segment"] == "START"].values
            segmentEnds = data.index[data["segment"] == "END"].values

            for index, (start, end) in enumerate(zip(segmentStarts, segmentEnds)):
                labels[start + REP_POSE_GAP_HALF:end - REP_POSE_GAP_HALF] = exercise_name + "_REP"

                if index > 0:
                    lastEnd = segmentEnds[index - 1]
                    labels[lastEnd - POSE_LABEL_EXTEND:start + POSE_LABEL_EXTEND] = exercise_name + "_POSE"

            sim_result["label"] = labels
            sim_result["segment"] = data["segment"]

            path = dataManager.labeled_data_dir + file
            sim_result.to_csv(path, sep=";", header=True, index=False)
            with open(path, "r+") as f:
                content = f.read()
                f.seek(0, 0)
                f.write("SEP=;\n" + content)


