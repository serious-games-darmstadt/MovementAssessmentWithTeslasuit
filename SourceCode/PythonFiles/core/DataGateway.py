import time
import numpy as np

import Config
from PerformanceAnalyzer import PerformanceAnalyzer
from core.DataRecorder import DataRecorder
from core.DenoiseProxy import DenoiseProxy
from core.PoseClassifier import PoseClassifier
from core.RepEvaluator import RepEvaluator
from core.StreamSegmentor import StreamSegmentor
from data.DataAccess import DataAccess


class DataGateway:
    def __init__(self):
        self.streamSegmentor = StreamSegmentor()
        self.classifier = PoseClassifier()
        self.dataRecorder = DataRecorder()
        self.denoiseProxy = DenoiseProxy()
        self.repEvaluator = RepEvaluator()


    def onNewTeslasuitData(self, suitData):
        t = time.process_time()
        timestamp = DataAccess.get_timestamp(suitData)
        denoisedData = self.denoiseProxy.denoise(DataAccess.get_data_without_timesamp(suitData))
        PerformanceAnalyzer.add_denoise_time_measurement(time.process_time() - t)

        nodeData = DataAccess.get_node_data(denoisedData)
        jointData = DataAccess.get_joint_data(denoisedData)

        t = time.process_time()
        exercise, _type = self.classifier.predict(nodeData)
        PerformanceAnalyzer.add_classify_time_measurement(time.process_time() - t)

        t = time.process_time()
        segmentDetected = self.streamSegmentor.onNewStreamData(nodeData, exercise, _type)
        PerformanceAnalyzer.add_segment_time_measurement(time.process_time() - t)

        t = time.process_time()
        if segmentDetected == "START":
            self.repEvaluator.repStarted(exercise, timestamp)
        error, repEnded = self.repEvaluator.evaluate(timestamp, jointData, exercise, _type)
        if repEnded:
            segmentDetected = "END"
        error = np.insert(error, 0, suitData[0]).tolist()
        PerformanceAnalyzer.add_evaluate_time_measurement(time.process_time() - t)

        t = time.process_time()
        self.dataRecorder.log_data(np.insert(denoisedData, 0, timestamp), exercise.name + "_" + _type.name, segmentDetected, error)
        PerformanceAnalyzer.add_log_time_measurement(time.process_time() - t)

        return [exercise.name, error]


    def getRecordedData(self):
        return self.dataRecorder.dataToDataframe()

    def get_error_data(self):
        return self.dataRecorder.errorToDataframe()