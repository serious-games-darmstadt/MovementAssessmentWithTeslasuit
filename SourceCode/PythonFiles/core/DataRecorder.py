import pandas as pd

import Config
from Plotter import Plotter
from data.DataAccess import DataAccess


class DataRecorder:
    def __init__(self):
        self.recordedData = []
        self.recordedLabels = []
        self.recordedSegments = []
        self.recordedErrors = []
        self.plotter = Plotter()

    def log_data(self, suit_data, label, segmentDetected, error):
        self.recordedData.append(suit_data)
        self.recordedLabels.append(label)
        self.recordedSegments.append(segmentDetected)
        self.recordedErrors.append(error)

    def dataToDataframe(self):
        dataFrame = pd.DataFrame(self.recordedData, columns=DataAccess.result_columns())
        dataFrame["label"] = self.recordedLabels
        dataFrame["segment"] = self.recordedSegments
        return dataFrame

    def errorToDataframe(self):
        columns = ["timestamp"]
        columns.extend(DataAccess.get_joints_properties_names(Config.streamedJoints))
        dataFrame = pd.DataFrame(self.recordedErrors, columns=columns)
        return dataFrame