import os
import signal
import sys
import time

# from joblib import load

# from ClassificationResult import ClassificationExercise
# from PerformanceAnalyzer import PerformanceAnalyzer
# from Plotter import Plotter
# from core.DataGateway import DataGateway
from Server import Server
os.environ['FOR_DISABLE_CONSOLE_CTRL_HANDLER'] = '1'

# dataGateway = DataGateway()
server = Server()

def run_program():
    # data = open(r"C:\StudentProjects\Burakhan\Tesla Suit\Assets\JsonAttempts\burak_Lunge.json","r")
    # print(data.read())
    server.start()


def on_exit(signum, frame):
    print("Stopping...")
    server.stop()
    print("All stopped. Exiting.")

    # data = dataGateway.getRecordedData()
    # plotter = Plotter(showLabels=True)
    # plotter.plot_node_data(data)
    # plotter.plot_joint_data(data)

    # performanceAnalysis = PerformanceAnalyzer.analyze()
    # plotter.plotPerformanceAnalysis(performanceAnalysis)

    # own_file_path = os.path.dirname(__file__)
    # proMps = {}
    # for name, member in ClassificationExercise.__members__.items():
    #     if member == ClassificationExercise.NEGATIVE:
    #         continue
    #
    #     # Files need to be called e.g. proMp_PUSHUP and need to provide only joint data
    #     # as a numpy array
    #     try:
    #         proMp = load(own_file_path + "/../core/ml-models/proMp_" + name)
    #         proMps[name] = proMp
    #     except FileNotFoundError:
    #         print("Missing ProMP: ", name)
    # plotter.plot_movement_error(proMps, data)

    sys.exit(1)


if __name__ == "__main__":
    signal.signal(signal.SIGINT, on_exit)
    run_program()
    print("Startup complete")
    while True:
        time.sleep(10)