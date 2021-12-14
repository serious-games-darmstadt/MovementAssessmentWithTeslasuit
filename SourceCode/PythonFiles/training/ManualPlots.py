import pathlib
import sys

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
from joblib import load

import Config
import TsDataUtil
from Plotter import Plotter
from data.DataAccess import DataAccess
from data.DataManager import DataManager
from simulation.Simulator import Simulator

plotter = Plotter(showLabels=False)
dataManager = DataManager()

Config.DENOISE_LENGTH = 8
Config.SEGMENT_THRESHOLD = 8
Config.resultProperties = ["gyroscope"]
data = dataManager.get_data_from_file(dataManager.unlabeled_data_dir + "13_Squat_Slow.csv")
sim_data = data[DataAccess.simulation_columns()]
simulator = Simulator(sim_data)
sim_result, error_result = simulator.run()
plotter.plot_node_data(sim_result, show_segments=True)
sys.exit(1)



# ProMP Plots
data = dataManager.get_data_from_file(dataManager.error_data_dir + "13_Pushup_Error3.csv")
sim_data = data[DataAccess.simulation_columns()]
simulator = Simulator(sim_data)
sim_result, error_result = simulator.run()
exercise_rep_dict = TsDataUtil.extract_reps_from_training_data(sim_result)
zero_aligned_reps = TsDataUtil.to_zero_aligned_timesteps(exercise_rep_dict["PUSHUP"])
uniform_timestep_reps = TsDataUtil.to_uniform_timesteps(zero_aligned_reps)
equalized_length_reps = TsDataUtil.equalize_length(uniform_timestep_reps)
rep = equalized_length_reps[0][:4541]

# lunge = load("proMp_LUNGE")
pushup = load("proMp_PUSHUP")
# squat = load("proMp_SQUAT")

Config.resultJoints = {
    "Left Arm": ["leftShoulder"],
}
# plotter.plot_pro_mp(lunge, reps=None, postfix="LUNGE")
# plotter.plot_pro_mp(pushup, reps=[rep], postfix="PUSHUP", plot_std=False, plot_mean=False)
# plotter.plot_pro_mp(squat, reps=None, postfix="SQUAT")



# Plot for Segmentation evaluation of T_denoise
x = np.arange(1, 11)
detected = [322, 344, 353, 358, 361, 361, 360, 359, 358, 356]
mapped = [314, 334, 341, 348, 350, 352, 350, 352, 350, 348]

plt.rcParams["figure.figsize"] = (7, 7)
plt.grid(True, linestyle=':')
plt.plot(x, detected, label="Detected")
plt.plot(x, mapped, label="Mapped")
plt.xticks(x)
plt.yticks([314, 322, 334, 341, 344, 352, 361])
plt.xlabel("T_denoise")
plt.ylabel("Repetitions")
plt.legend()
# plt.savefig("SegmentationDenoise.pdf", bbox_inches="tight")



# Plot for Segmentation evaluation of G_limit
x = np.arange(1, 16)
detected = [284, 359, 395, 420, 431, 442, 448, 450, 454, 453, 455, 456, 453, 451, 445]
mapped = [279, 352, 386, 407, 421, 430, 437, 439, 443, 444, 445, 446, 444, 441, 435]

plt.figure()
plt.grid(True, linestyle=':')
plt.plot(x, detected)
plt.plot(x, mapped)
plt.xticks(x)
plt.yticks([279, 284, 352, 359, 386, 395, 407, 420, 430, 439, 446, 456])
plt.legend(["Detected", "Mapped"])
plt.xlabel("G_limit")
plt.ylabel("Repetitions")
# plt.savefig("SegmentationGlimit.pdf", bbox_inches="tight")

# result = []
# for file in dataManager.labeled_files:
#     data = dataManager.get_data_from_file(dataManager.labeled_data_dir + file)
#     repetitions = (data["segment"] == "START").sum()
#     file_result = [file[3:-4], repetitions]
#     result.append(file_result)
#
# df = pd.DataFrame(result, columns=["Exercise", "count"])
# group = df.groupby("Exercise").sum()
# print(group)




correct = []
actual = []
slow_path = str(pathlib.Path(__file__).parent) + "/../plots/Classifier Evaluation/M_D1_AllSlow/RepetitionResults.csv"
medium_path = str(pathlib.Path(__file__).parent) + "/../plots/Classifier Evaluation/M_D1_AllMedium/RepetitionResults.csv"
fast_path = str(pathlib.Path(__file__).parent) + "/../plots/Classifier Evaluation/M_D1_AllFast/RepetitionResults.csv"

for exercise in ("LUNGE", "PUSHUP", "SQUAT"):
    for path in (fast_path, medium_path, slow_path):
        data = dataManager.get_data_from_file(path, make_numeric=False)

        reps = data.loc[data["actual"] == exercise]
        correct.append(len(reps))
        actual.append((reps["predicted"] == exercise).sum())


fig, ax = plt.subplots(figsize=(8, 8))
x = np.arange(9)
width = 0.4
ax.bar(x - width / 2, actual, width, label="Correct")
ax.bar(x + width / 2, correct, width, label="Total")
ax.set_xticks(x)
ax.set_xticklabels(["Lunge_Short", "Lunge_Medium", "Lunge_Long", "Pushup_Short", "Pushup_Medium", "Pushup_Long", "Squat_Short", "Squat_Medium", "Squat_Long"])
ax.set_ylabel("Repetitions")
plt.legend(["Correct Repetitions", "Total Repetitions"], loc="lower right")
plt.xticks(rotation=90)
# plt.savefig("ExerciseBars.pdf", bbox_inches="tight")