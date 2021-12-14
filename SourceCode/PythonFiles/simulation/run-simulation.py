import os

from joblib import load

from ClassificationResult import ClassificationExercise
from Plotter import Plotter
from data.DataAccess import DataAccess
from data.DataManager import DataManager
from simulation.Simulator import Simulator

dataManager = DataManager()
data = dataManager.get_simulation_data()

simulator = Simulator(data)
simulation_result = simulator.run()

plotter = Plotter(showLabels=True)
plotter.plot_node_data(simulation_result)

own_file_path = os.path.dirname(__file__)
proMps = {}
for name, member in ClassificationExercise.__members__.items():
    if member == ClassificationExercise.NEGATIVE:
        continue

    # Files need to be called e.g. proMp_PUSHUP and need to provide only joint data
    # as a numpy array
    try:
        proMp = load(own_file_path + "/../core/ml-models/proMp_" + name)
        proMps[name] = proMp
    except FileNotFoundError:
        print("Missing ProMP: ", name)
plotter.plot_movement_error(proMps, simulation_result)


