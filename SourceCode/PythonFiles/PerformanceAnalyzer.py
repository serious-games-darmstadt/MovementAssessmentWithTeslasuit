class PerformanceAnalyzer:
    totalTime = []
    denoiseTime = []
    classifyTime = []
    segmentTime = []
    evaluateTime = []
    logTime = []
    readDataTime = []

    @staticmethod
    def analyze():
        avgTotal = sum(PerformanceAnalyzer.totalTime[10:]) / len(PerformanceAnalyzer.totalTime[10:])
        avgDenoise = sum(PerformanceAnalyzer.denoiseTime[10:]) / len(PerformanceAnalyzer.denoiseTime[10:])
        avgClassify = sum(PerformanceAnalyzer.classifyTime[10:]) / len(PerformanceAnalyzer.classifyTime[10:])
        avgSegment = sum(PerformanceAnalyzer.segmentTime[10:]) / len(PerformanceAnalyzer.segmentTime[10:])
        avgEvaluate = sum(PerformanceAnalyzer.evaluateTime[10:]) / len(PerformanceAnalyzer.evaluateTime[10:])
        avgLog = sum(PerformanceAnalyzer.logTime[10:]) / len(PerformanceAnalyzer.logTime[10:])
        avgReadData = sum(PerformanceAnalyzer.readDataTime[10:]) / len(PerformanceAnalyzer.readDataTime[10:])

        print(len(PerformanceAnalyzer.totalTime[10:]), " samples processed.")
        result = [avgTotal, avgReadData / avgTotal, avgDenoise / avgTotal, avgClassify / avgTotal,
                  avgSegment / avgTotal, avgEvaluate / avgTotal, avgLog / avgTotal]
        return result

    @staticmethod
    def add_total_time_measurement(measurement):
        PerformanceAnalyzer.totalTime.append(measurement)

    @staticmethod
    def add_denoise_time_measurement(measurement):
        PerformanceAnalyzer.denoiseTime.append(measurement)

    @staticmethod
    def add_classify_time_measurement(measurement):
        PerformanceAnalyzer.classifyTime.append(measurement)

    @staticmethod
    def add_segment_time_measurement(measurement):
        PerformanceAnalyzer.segmentTime.append(measurement)

    @staticmethod
    def add_evaluate_time_measurement(measurement):
        PerformanceAnalyzer.evaluateTime.append(measurement)

    @staticmethod
    def add_log_time_measurement(measurement):
        PerformanceAnalyzer.logTime.append(measurement)

    @staticmethod
    def add_read_data_time_measurement(measurement):
        PerformanceAnalyzer.readDataTime.append(measurement)

    @staticmethod
    def add_a_time_measurement(measurement):
        PerformanceAnalyzer.a.append(measurement)

    @staticmethod
    def add_b_time_measurement(measurement):
        PerformanceAnalyzer.b.append(measurement)

    @staticmethod
    def add_c_time_measurement(measurement):
        PerformanceAnalyzer.c.append(measurement)

    @staticmethod
    def add_d_time_measurement(measurement):
        PerformanceAnalyzer.d.append(measurement)

    @staticmethod
    def add_e_time_measurement(measurement):
        PerformanceAnalyzer.e.append(measurement)
