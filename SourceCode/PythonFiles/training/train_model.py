from training.ModelTrainer import ModelTrainer
from data.DataManager import DataManager

model_trainer = ModelTrainer()

dataManager = DataManager()
data, groups, segment_starts = dataManager.get_classifier_training_data()
model_trainer.train_model(data)