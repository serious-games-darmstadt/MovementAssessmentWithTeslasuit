import os

import numpy as np
import matplotlib.pyplot as plt
import pandas as pd

import Config
import TsDataUtil
from data.DataAccess import DataAccess


class Plotter:
    def __init__(self, showLabels=False):
        self.showLabels = showLabels
        self.colors = ['gray', "sienna", "aquamarine", "darkcyan", "blueviolet", "fuchsia", "goldenrod", "lawngreen",
                       "palegreen", ]
        self.own_file_path = os.path.dirname(__file__)

    def plot_node_data(self, data, show_segments=True):
        x = np.arange(len(data))

        labelcolors = {}
        if 'label' in data.columns:
            labels = data["label"].unique()
            for index, label in enumerate(labels):
                labelcolors[label] = self.colors[index]

        for node in Config.resultNodes:
            numPlots = len(Config.resultProperties)

            fig, axs = plt.subplots(numPlots, figsize=(12, 3 * numPlots))
            fig.tight_layout()
            fig.suptitle = node

            verticalLines = {}
            for index, segment in data["segment"].iteritems():
                if segment == "START":
                    verticalLines[index] = 'green'
                elif segment == "END":
                    verticalLines[index] = "red"

            for propertyIndex, prop in enumerate(Config.resultProperties):
                relevantColumnLabels = DataAccess.get_node_property_names(node, prop)
                relevantData = data[relevantColumnLabels]

                if len(Config.resultProperties) > 1:
                    currentAxis = axs[propertyIndex]
                else:
                    currentAxis = axs

                currentAxis.title.set_text(prop)
                if prop == "gyroscope":
                    currentAxis.set_ylabel("° / s")
                if prop == "accelerometer":
                    currentAxis.set_ylabel("Sensor Output")
                currentAxis.set_xlabel("Time (samples)")
                currentAxis.plot(x, relevantData, label=("X", "Y", "Z"))

                if show_segments:
                    for key, value in verticalLines.items():
                        currentAxis.axvline(key, color=value)

                if self.showLabels:
                    lastLabel = None
                    startFill = 0
                    addedLabels = []

                    for index, row in data.iterrows():
                        nextlabel = row["label"]

                        if lastLabel is not None and (nextlabel != lastLabel or index == len(data) - 1):
                            if lastLabel not in addedLabels:
                                currentAxis.axvspan(startFill, index, color=labelcolors[lastLabel], label=lastLabel, alpha=0.5)
                                addedLabels.append(lastLabel)
                            else:
                                currentAxis.axvspan(startFill, index, color=labelcolors[lastLabel], alpha=0.5)
                            startFill = index

                        lastLabel = nextlabel

                currentAxis.legend()

            plt.subplots_adjust(hspace=0.3)
            plt.savefig(self.own_file_path + "/plots/" + node + ".pdf", bbox_inches="tight")

    def plotRepData(self, data):
        x = np.arange(len(data[0]))

        for node in Config.resultNodes:
            numPlots = len(Config.resultProperties)

            if node == "RightUpperArm":
                numPlots = numPlots + 2

            fig, axs = plt.subplots(numPlots, figsize=(15, 20))
            fig.suptitle = node

            for propertyIndex, prop in enumerate(Config.resultProperties):
                relevantColumnLabels = DataAccess.get_node_property_names(node, prop)

                relevantData = []
                for index, rep in enumerate(data):
                    relevantData.append(rep[relevantColumnLabels])

                df = pd.concat(relevantData, axis=1)
                axs[propertyIndex].title.set_text(prop)
                axs[propertyIndex].plot(x, df)

                if node == "RightUpperArm":
                    for jointindex, joint in enumerate(Config.resultJoints):
                        relevantColumnLabels = DataAccess.get_joint_properties_names(joint)

                        relevantData = []
                        for index, rep in enumerate(data):
                            relevantData.append(rep[relevantColumnLabels])

                        df = pd.concat(relevantData, axis=1)

                        axs[numPlots - (jointindex + 1)].title.set_text(joint)
                        axs[numPlots - (jointindex + 1)].plot(x, df)
                        axs[numPlots - (jointindex + 1)].legend(relevantColumnLabels)

            plt.savefig(self.own_file_path + "/plots/" + node + ".jpg")

    def plot_joint_data(self, data):
        x = np.arange(len(data))

        labelcolors = {}
        if 'label' in data.columns:
            labels = data["label"].unique()
            for index, label in enumerate(labels):
                labelcolors[label] = self.colors[index]

        for jointGroupName, jointList in Config.resultJoints.items():
            numPlots = len(jointList)

            fig, axs = plt.subplots(numPlots, figsize=(15, 20))
            fig.suptitle = jointGroupName

            verticalLines = {}
            for index, segment in data["segment"].iteritems():
                if segment == "START":
                    verticalLines[index] = 'green'
                elif segment == "END":
                    verticalLines[index] = "red"

            for plotIndex, joint in enumerate(jointList):
                relevantColumnLabels = DataAccess.get_joint_properties_names(joint)
                relevantData = data[relevantColumnLabels]

                axs[plotIndex].title.set_text(joint)
                axs[plotIndex].plot(x, relevantData)

                for key, value in verticalLines.items():
                    axs[plotIndex].axvline(key, color=value)

                if self.showLabels:
                    lastLabel = None
                    startFill = 0
                    for index, row in data.iterrows():
                        nextlabel = row["label"]

                        if lastLabel is not None and (nextlabel != lastLabel or index == len(data) - 1):
                            axs[plotIndex].axvspan(startFill, index, color=labelcolors[lastLabel], alpha=0.5)
                            startFill = index

                        lastLabel = nextlabel

            plt.savefig(self.own_file_path + "/plots/" + jointGroupName + ".jpg")

    def plot_pro_mp(self, proMp, reps=None, plot_mean=True, postfix='', plot_std=True):
        if reps is None:
            reps = []
        means = proMp["means"]
        std = proMp["std"]

        x = np.arange(len(means))
        plt.rcParams.update({'font.size': 12})

        for jointGroupName, jointList in Config.resultJoints.items():
            numPlots = len(jointList)

            fig, axs = plt.subplots(numPlots, figsize=(8, 4*numPlots))
            fig.suptitle = jointGroupName


            for plotIndex, joint in enumerate(jointList):
                relevantColumnLabels = DataAccess.get_joint_properties_names(joint)

                if len(jointList) > 1:
                    current_axis = axs[plotIndex]
                else:
                    current_axis = axs

                current_axis.title.set_text(joint)
                current_axis.set_ylabel("Position")
                current_axis.set_xlabel("Time (ms)")
                current_axis.set_xlim([0, 5000])
                current_axis.set_ylim([-1, 2.5])
                labels = []

                for index, rep in enumerate(reps):
                    repData = rep[relevantColumnLabels]
                    current_axis.plot(x, repData)
                    labels.append(str(index) + "_x")
                    labels.append(str(index) + "_y")
                    labels.append(str(index) + "_z")
                labels.append("X")
                labels.append("Y")
                labels.append("Z")

                for prop in ["x", "y", "z"]:
                    if plot_mean:
                        current_axis.plot(x, means[joint+"_"+prop])
                    if plot_std:
                        lowerStd = means[joint+"_"+prop] - Config.STD_MULTIPLIER * std[joint+"_"+prop]
                        upperStd = means[joint+"_"+prop] + Config.STD_MULTIPLIER * std[joint+"_"+prop]
                        current_axis.fill_between(x, lowerStd, upperStd, alpha=0.4)

                plt.xlabel("Time (samples)")
                plt.ylabel("Position")
                current_axis.legend(labels, loc="upper right")

            plt.subplots_adjust(hspace=0.4)
            plt.savefig(self.own_file_path + "/plots/" + jointGroupName + "_ProMP_" + postfix +".pdf", bbox_inches="tight")

    def plotPerformanceAnalysis(self, data):
        labels = ["Read Data", "Denoise", "Classfiy", "Segment", "Evaluate", "Log", "Other"]
        other = 1.0 - sum(data[1:])
        data.append(other)
        fig, ax = plt.subplots(1, 1, figsize=(5, 5))
        wedges, text = ax.pie(data[1:], normalize=False, wedgeprops=dict(width=0.5))

        bbox_props = dict(boxstyle="square,pad=0.3", fc="w", ec="k", lw=0.72)
        kw = dict(arrowprops=dict(arrowstyle="-"), zorder=0, va="center")
        for i, p in enumerate(wedges):
            ang = (p.theta2 - p.theta1) / 2. + p.theta1
            y = np.sin(np.deg2rad(ang))
            x = np.cos(np.deg2rad(ang))
            horizontalalignment = {-1: "right", 1: "left"}[int(np.sign(x))]
            connectionstyle = "angle,angleA=0,angleB={}".format(ang)
            kw["arrowprops"].update({"connectionstyle": connectionstyle})
            ax.annotate(["%.2f %%" % (num*100) for num in data[1:]][i], xy=(x, y), xytext=(1.35 * np.sign(x), 1.4 * y),
                        horizontalalignment=horizontalalignment, **kw)

        title = "Avg. processing time: %f ms" % (data[0] * 1000)
        plt.title(title)
        plt.legend(labels)
        plt.savefig(self.own_file_path + "/plots/performance.pdf", bbox_inches="tight")

    def plot_movement_error(self, proMps, data):
        exercise_rep_dict = TsDataUtil.extract_reps_from_data(data)

        for key, value in exercise_rep_dict.items():
            print("Plot Movement Error for ", key)
            proMp = proMps[key]

            print("Zero align reps")
            zero_aligned_reps = TsDataUtil.to_zero_aligned_timesteps(value)
            print("Uniform timesteps")
            uniform_timestep_reps = TsDataUtil.to_uniform_timesteps(zero_aligned_reps)
            print("Equalize length")
            equalized_length_reps = TsDataUtil.equalize_length([proMp["means"], proMp["std"], *uniform_timestep_reps])

            proMp["means"] = equalized_length_reps[0]
            proMp["std"] = equalized_length_reps[1]

            print("Plot")
            self.plot_pro_mp(proMp, reps=equalized_length_reps[2:], plot_mean=False, postfix=key)


        # reps = TsDataUtil.extract_reps_from_data(data)
        # zero_aligned_reps = TsDataUtil.to_zero_aligned_timesteps(reps)
        # uniform_time_reps = TsDataUtil.to_uniform_timesteps(zero_aligned_reps)
        # equalized_length_data = TsDataUtil.equalize_length([proMp["means"], proMp["std"], *uniform_time_reps])

        # proMp["means"] = equalized_length_data[0]
        # proMp["std"] = equalized_length_data[1]
        # proMp["reps"] = equalized_length_data[2:]
        #
        # self.plot_pro_mp(proMp, plot_mean=False)