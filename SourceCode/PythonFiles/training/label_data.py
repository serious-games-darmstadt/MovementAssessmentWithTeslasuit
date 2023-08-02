import sys

from PyQt5.QtWidgets import QApplication, QWidget, QMainWindow, QGridLayout, QComboBox, QSizePolicy, QPushButton
from matplotlib.backends.backend_qt5agg import FigureCanvasQTAgg

import Config
from ClassificationResult import ClassificationExercise
from data.DataAccess import DataAccess
from data.DataManager import DataManager
import matplotlib.pyplot as plt
import numpy as np
import pandas as pd


class MainWindow(QMainWindow):
    def __init__(self):
        super(MainWindow, self).__init__()
        self.lastLine1 = None
        self.lastLine2 = None
        self.indexStart = None
        self.clickDown = False
        self.main_widget = QWidget(self)
        self.dataManager = DataManager()
        self.selected_directory = self.dataManager.unlabeled_data_dir
        self.data = None
        self.x = None
        self.Mode = "Segmentation"

        fig, axs = plt.subplots(2, figsize=(15, 20))
        self.axis = axs
        self.fig = fig
        self.canvas = FigureCanvasQTAgg(fig)

        self.canvas.setSizePolicy(QSizePolicy.Expanding,
                                  QSizePolicy.Expanding)
        self.canvas.updateGeometry()

        self.canvas.mpl_connect("button_press_event", self.__onclick__)
        self.canvas.mpl_connect("button_release_event", self.__onrelease__)
        self.canvas.mpl_connect("motion_notify_event", self.__onmotion__)

        self.modeDropdown = QComboBox()
        self.modeDropdown.addItems(["Segmentation", "Labeling", "Negative Labeling"])
        self.modeDropdown.currentIndexChanged.connect(self.mode_changed)

        self.directoryDropdown = QComboBox()
        self.directoryDropdown.addItems(["Unlabeled", "Negativ", "Labeled", "Segmented", "Error"])
        self.directoryDropdown.currentIndexChanged.connect(self.directory_changed)

        self.fileDropdown = QComboBox()
        self.fileDropdown.addItems(self.dataManager.unlabeled_files)
        self.fileDropdown.currentIndexChanged.connect(self.file_changed)

        self.nodeDropdown = QComboBox()
        nodeDropdownItems = []
        for exercise in ClassificationExercise:
            if exercise == ClassificationExercise.NEGATIVE:
                continue
            nodeDropdownItems.append(exercise.name + "_" + Config.class_segmentation_map[exercise])
        self.nodeDropdown.addItems(nodeDropdownItems)
        self.nodeDropdown.currentIndexChanged.connect(self.update)

        self.labelDropdown = QComboBox()
        self.labelDropdown.addItems(["PUSHUP_POSE", "PUSHUP_REP",
                                     "SQUAT_POSE", "SQUAT_REP",
                                     "LUNGE_POSE", "LUNGE_REP",
                                     "NEGATIVE_NONE"])

        self.saveButton = QPushButton("Save")
        self.saveButton.clicked.connect(self.on_save)

        self.layout = QGridLayout(self.main_widget)
        self.layout.addWidget(self.modeDropdown, 0, 0, 1, 1)
        self.layout.addWidget(self.directoryDropdown, 0, 1, 1, 1)
        self.layout.addWidget(self.fileDropdown, 0, 2, 1, 1)
        self.layout.addWidget(self.nodeDropdown, 0, 3, 1, 1)
        self.layout.addWidget(self.labelDropdown, 0, 4, 1, 1)
        self.layout.addWidget(self.saveButton, 0, 5, 1, 1)
        self.layout.addWidget(self.canvas, 1, 0, 1, 6)

        self.setCentralWidget(self.main_widget)
        self.show()
        self.file_changed()

    def update(self):
        self.x = np.arange(len(self.data))

        exercise_node = self.nodeDropdown.currentText().split("_")
        selected_exercise = ClassificationExercise[exercise_node[0]]
        self.axis[0].clear()
        self.axis[1].clear()

        self.axis[0].plot(self.x, self.data[DataAccess.get_node_property_names(Config.class_segmentation_map[selected_exercise], "gyroscope")])
        self.axis[1].plot(self.x, self.data[DataAccess.get_node_property_names(Config.class_segmentation_map[selected_exercise], "accelerometer")])

        self.colors = ['gray', "sienna", "aquamarine", "darkcyan", "blueviolet", "fuchsia", "goldenrod", "lawngreen",
                       "palegreen", ]
        labelcolors = {}
        labels = self.data["label"].unique()
        for index, label in enumerate(labels):
            labelcolors[label] = self.colors[index]

        lastLabel = None
        startFill = 0
        labelAdded = []
        for index, row in self.data.iterrows():
            nextlabel = row["label"]

            if lastLabel is not None and (nextlabel != lastLabel or index == len(self.data) - 1):
                if lastLabel in labelAdded:
                    self.axis[0].axvspan(startFill, index, color=labelcolors[lastLabel], alpha=0.5)
                else:
                    self.axis[0].axvspan(startFill, index, color=labelcolors[lastLabel], label=lastLabel, alpha=0.5)
                    labelAdded.append(lastLabel)
                startFill = index

            lastLabel = nextlabel

        if "segment" in self.data.columns:
            segmentStarts = self.data.index[self.data["segment"] == "START"].values
            segmentEnds = self.data.index[self.data["segment"] == "END"].values
            for i in range(0, len(segmentStarts)):
                self.axis[0].axvline(segmentStarts[i], alpha=0.5, color="green")
                self.axis[1].axvline(segmentStarts[i], alpha=0.5, color="green")
                self.axis[0].axvline(segmentEnds[i], alpha=0.5, color="red")
                self.axis[1].axvline(segmentEnds[i], alpha=0.5, color="red")

        self.axis[0].legend()
        plt.draw()

    def mode_changed(self):
        self.Mode = self.modeDropdown.currentText()
        self.load_new_data()

        self.update()


    def load_new_data(self):
        path = self.selected_directory + self.fileDropdown.currentText()
        self.data = self.dataManager.get_data_from_file(path)

        if self.Mode == "Segmentation":
            self.data["segment"] = "NONE"

    def directory_changed(self):
        if self.directoryDropdown.currentText() == "Labeled":
            files = self.dataManager.labeled_files
            self.selected_directory = self.dataManager.labeled_data_dir
        elif self.directoryDropdown.currentText() == "Unlabeled":
            files = self.dataManager.unlabeled_files
            self.selected_directory = self.dataManager.unlabeled_data_dir
        elif self.directoryDropdown.currentText() == "Segmented":
            files = self.dataManager.segmented_files
            self.selected_directory = self.dataManager.segmented_data_dir
        elif self.directoryDropdown.currentText() == "Error":
            files = self.dataManager.error_files
            self.selected_directory = self.dataManager.error_data_dir
        else:
            files = self.dataManager.negativ_labeled_files
            self.selected_directory = self.dataManager.negativ_lebaled_data_dir

        self.fileDropdown.disconnect()
        self.fileDropdown.clear()
        self.fileDropdown.addItems(files)
        self.fileDropdown.currentIndexChanged.connect(self.file_changed)

        self.file_changed()

    def file_changed(self):
        self.load_new_data()
        self.update()

    def __onclick__(self, click):
        self.lastLine1 = self.axis[0].axvline(click.xdata)
        self.lastLine2 = self.axis[1].axvline(click.xdata)
        self.clickDown = True
        plt.draw()

    def __onrelease__(self, click):
        self.clickDown = False
        current_x = int(click.xdata)

        if self.indexStart is None:
            self.indexStart = current_x

            if self.Mode == "Segmentation":
                self.data.loc[current_x, "segment"] = "START"
        else:
            if self.Mode == "Labeling" or self.Mode == "Negative Labeling":
                self.data.loc[self.indexStart:current_x, "label"] = self.labelDropdown.currentText()
            elif self.Mode == "Segmentation":
                self.data.loc[current_x, "segment"] = "END"
            self.indexStart = None

        print(click.xdata)

    def __onmotion__(self, click):
        if self.clickDown:
            self.lastLine1.set_xdata([click.xdata, click.xdata])
            self.lastLine2.set_xdata([click.xdata, click.xdata])
            plt.draw()

    def on_save(self):
        if self.Mode == "Labeling":
            path = self.dataManager.labeled_data_dir + self.fileDropdown.currentText()
        elif self.Mode == "Segmentation":
            path = self.dataManager.segmented_data_dir + self.fileDropdown.currentText()
        elif self.Mode == "Negative Labeling":
            path = self.dataManager.negativ_lebaled_data_dir + self.fileDropdown.currentText()

        self.dataManager.save_data_to_path(path, self.data)



app = QApplication(sys.argv)
window = MainWindow()
window.show()

app.exec()

