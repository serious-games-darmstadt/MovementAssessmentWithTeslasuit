import Config
from data.DataAccess import DataAccess
from data.DataManager import DataManager
from training.ErrorVectorEvaluator import ErrorVectorEvaluator
from training.ModelTrainer import ModelTrainer
from training.ProMpBuilder import ProMpBuilder
from training.SegmentationEvaluator import SegmentationEvaluator
from training.automatic_labeling import AutomaticLabeling

Config.EVAL_USE_SEGMENTS = "MANUAL"
Config.EVAL_USE_DATA = "GOOD"
Config.DENOISE_LENGTH = 8
Config.SEGMENT_THRESHOLD = 8

# Labeling: Takes Data  from 2-negativ-labeled-data and De-noises it and labels according to the included segments
# AutomaticLabeling(0, 0)

model_trainer = ModelTrainer()
dataManager = DataManager()
# data, groups, segment_starts, segment_labels = dataManager.get_classifier_training_data()
# model_trainer.evaluate_model(data, groups, segment_starts, segment_labels)
# model_trainer.plot_classifier_evaluation()
# model_trainer.train_model(data)
#

Config.resultJoints = {
    "Left Arm": ["leftShoulder"],
}
data = dataManager.get_labeled_data()
proMpBuilder = ProMpBuilder()
proMpBuilder.buildProMp(data[DataAccess.result_promp_columns()])

Config.DENOISE_LENGTH = 8
Config.SEGMENT_THRESHOLD = 8
segmentation_evaluator = SegmentationEvaluator()
# segmentation_evaluator.evaluate_segmentation()
# segmentation_evaluator.plot_segmentation_evaluation()
# segmentation_evaluator.plot_segmentation_glimit_avg_iqr_range()
# segmentation_evaluator.plot_t_denoise_curves()
#
error_vector_evaluator = ErrorVectorEvaluator()
# error_vector_evaluator.evaluateErrorVectors()
# error_vector_evaluator.plot_error_evaluation()



