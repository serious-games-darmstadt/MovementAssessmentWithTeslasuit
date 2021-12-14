import time
from core.DataGateway import DataGateway


class Simulator:
    def __init__(self, data):
        self.dataGateway = DataGateway()
        self.sim_data = data

    def run(self):
        t = time.process_time()
        self.sim_data.apply(lambda x: self.dataGateway.onNewTeslasuitData(x), axis=1)
        print("Simulation took ", time.process_time() - t , " seconds.")

        simulation_result = self.dataGateway.getRecordedData()
        error_data = self.dataGateway.get_error_data()
        return simulation_result, error_data

        # plotter = Plotter(showLabels=True)
        # plotter.plot_node_data(simulation_result)
        #
        # result_joint_data = simulation_result[DataAccess.get_joints_properties_names(Config.streamedJoints)]
        # proMpBuilder = ProMpBuilder()
        # proMP = proMpBuilder.buildProMp(result_joint_data)
        # plotter.plot_pro_mp(proMP)

        # trainer = ModelTrainer()
        # trainer.retrain_model_with_segments(simulation_result)

        # own_file_path = os.path.dirname(__file__)
        # proMps = load(own_file_path + "/../core/ml-models/proMpPushup")
        # plotter.plot_movement_error(proMps, simulation_result)



