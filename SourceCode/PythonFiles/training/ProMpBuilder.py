import numpy as np
import pandas as pd
from joblib import dump

import TsDataUtil
from Plotter import Plotter


class ProMpBuilder:

    def buildProMp(self, data):
        """
        :param data: Only the Joint Data!
        :return:
        """
        plotter = Plotter()
        exercise_rep_dict = TsDataUtil.extract_reps_from_training_data(data)
        proMps = {}

        for key, value in exercise_rep_dict.items():
            print("Build ProMP for", key)
            zero_aligned_reps = TsDataUtil.to_zero_aligned_timesteps(value)
            uniform_timestep_reps = TsDataUtil.to_uniform_timesteps(zero_aligned_reps)
            equalized_length_reps = TsDataUtil.equalize_length(uniform_timestep_reps)

            df = pd.concat(equalized_length_reps)
            means = df.groupby(df.index).mean()
            std = df.groupby(df.index).std()

            proMP = {"means": means, "std": std}
            proMps[key] = proMP
            dump(proMP, "proMp_" + key)

            print("Plotting ProMPs")
            # Dont plot reps because its too cluttered
            plotter.plot_pro_mp(proMP, reps=equalized_length_reps, postfix=key, plot_std=False)

        return proMps
