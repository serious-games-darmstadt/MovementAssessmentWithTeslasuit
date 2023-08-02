import Config
from data.DataAccess import DataAccess
from data.DataManager import DataManager
from simulation.Simulator import Simulator

dataManager = DataManager()

for file in dataManager.labeled_files:
    data = dataManager.get_data_from_file(dataManager.labeled_data_dir + file)

    # Step 1: Simulate data to denoise.
    Config.EVALUATE = False
    sim_data = data[DataAccess.simulation_columns()]
    simulator = Simulator(sim_data)
    sim_result, error_result = simulator.run()

    sim_result["segment"] = data["segment"]

    path = dataManager.predicted_data_dir + file
    sim_result.to_csv(path, sep=";", header=True, index=False)
    with open(path, "r+") as f:
        content = f.read()
        f.seek(0, 0)
        f.write("SEP=;\n" + content)