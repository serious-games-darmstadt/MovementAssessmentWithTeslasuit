import pandas as pd
from sklearn import svm
from joblib import dump
from sklearn.metrics import confusion_matrix, plot_confusion_matrix, accuracy_score, precision_recall_fscore_support

import Config
from data.DataAccess import DataAccess
from data.DataManager import DataManager
import numpy as np
import matplotlib.pyplot as plt
from sklearn.neighbors import KNeighborsClassifier
from sklearn.model_selection import cross_val_score, LeaveOneGroupOut
import seaborn as sns

pd.set_option('mode.chained_assignment', None)

class ModelTrainer:

    @staticmethod
    def train_model(training_data):
        print("Training Model...")

        strippedData = training_data[training_data["label"] != "NotLabelled"]

        X = strippedData.drop(["label"], axis=1)
        Y = strippedData["label"]

        print("Building SVM Model with ", len(Y), " data points.")
        supportVectorMachine = svm.SVC()
        supportVectorMachine.fit(X, Y)

        print("Dumping Results")
        dump(supportVectorMachine, "../core/ml-models/svm_model")

    @staticmethod
    def evaluate_model(training_data, groups, segment_starts, segment_labels):
        print("Evaluating Classifier...")
        labeled_data_index = training_data["label"] != "NotLabelled"
        X = training_data.drop(["label"], axis=1)
        Y = training_data["label"]

        # Used to collect actual class and predicted class for individual samples over all groups
        actual_excl_unlabeled_aggregated = []
        prediction_aggregated = []

        # Used to collection actual class and predicted class for the repetitions
        repetition_results_actual = []
        repetition_results_predicted = []

        group_results = pd.DataFrame(columns=["Correct_Reps", "Total_Reps"])

        split_index = 1
        logo = LeaveOneGroupOut()
        for train_index, test_index in logo.split(X, Y, groups):
            print("Split ", split_index)
            split_index = split_index + 1
            labeled_data_index_part_train = labeled_data_index[labeled_data_index.index.isin(train_index)]
            labeled_data_index_part_test = labeled_data_index[labeled_data_index.index.isin(test_index)]

            X_train_incl_unlabeled = X[X.index.isin(train_index)]
            X_train = X_train_incl_unlabeled[labeled_data_index_part_train]
            X_test_incl_unlabeled = X[X.index.isin(test_index)]
            X_test = X_test_incl_unlabeled[labeled_data_index_part_test]

            Y_train_incl_unlabeled = Y[Y.index.isin(train_index)]
            Y_train = Y_train_incl_unlabeled[labeled_data_index_part_train]
            Y_test_incl_unlabeled = Y[Y.index.isin(test_index)]
            Y_test = Y_test_incl_unlabeled[labeled_data_index_part_test]

            supportVectorMachine = svm.SVC()
            supportVectorMachine.fit(X_train, Y_train)
            result = supportVectorMachine.predict(X_test)
            prediction_aggregated.extend(result)
            actual_excl_unlabeled_aggregated.extend(Y_test)

            result_incl_unlabeled = supportVectorMachine.predict(X_test_incl_unlabeled)
            segment_start_part = segment_starts[segment_starts.index.isin(test_index)]
            segment_labels_part = segment_labels[segment_labels.index.isin(test_index)]
            predicted_labels = result_incl_unlabeled[segment_start_part]

            split_function = lambda x: x.split("_")[0].upper()
            transformed_predicted_labels = np.array([split_function(x) for x in predicted_labels])
            actual_labels = segment_labels_part[segment_start_part].to_numpy()
            repetition_results_actual.extend(actual_labels)
            repetition_results_predicted.extend(transformed_predicted_labels)

            group_results = group_results.append({"Correct_Reps": (transformed_predicted_labels==actual_labels).sum(),
                                                  "Total_Reps": segment_start_part.sum()}, ignore_index=True)

        sample_results = pd.DataFrame({"actual": actual_excl_unlabeled_aggregated, "predicted": prediction_aggregated})
        repetition_results = pd.DataFrame({"actual": repetition_results_actual, "predicted": repetition_results_predicted})
        dataManager = DataManager()
        dataManager.save_data_to_path("GroupResults.csv", group_results)
        dataManager.save_data_to_path("RepetitionResults.csv", repetition_results)
        dataManager.save_data_to_path("SampleResults.csv", sample_results)

    @staticmethod
    def plot_classifier_evaluation(add_statistics=False):
        plt.rcParams.update({'font.size': 12})
        dataManager = DataManager()

        print("Repetition Classification Results")
        repetition_results = dataManager.get_data_from_file("RepetitionResults.csv", make_numeric=False)
        actual_exercises = repetition_results["actual"].apply(lambda x: x.split("_")[0].upper())
        accuracy = accuracy_score(actual_exercises, repetition_results["predicted"])
        precision, recall, f1, support = precision_recall_fscore_support(actual_exercises, repetition_results["predicted"], average='macro', zero_division=1)
        print("Accuracy: ", accuracy, "Precision: ", precision, " Recall: ", recall, " F1: ", f1, "\n")

        plt.figure(figsize=(9, 7))
        labels = ["NEGATIVE", "SQUAT", "PUSHUP", "LUNGE"]
        cm = confusion_matrix(actual_exercises, repetition_results["predicted"], labels=labels)
        g = sns.heatmap(cm, annot=True, xticklabels=labels, yticklabels=labels, fmt="d")
        g.set_xticklabels(g.get_xmajorticklabels(), fontsize=10)
        g.set_yticklabels(g.get_ymajorticklabels(), fontsize=10)
        plt.ylabel("True Label")
        plt.xlabel("Predicted Label")
        plt.savefig("RepetitionConfusionMatrix.pdf", bbox_inches="tight")

        print("Sample Classification Results")
        sample_results = dataManager.get_data_from_file("SampleResults.csv", make_numeric=False)
        accuracy = accuracy_score(sample_results["actual"], sample_results["predicted"])
        precision, recall, f1, support = precision_recall_fscore_support(sample_results["actual"], sample_results["predicted"], average='macro', zero_division=1)
        print("Accuracy: ", accuracy, "Precision: ", precision, " Recall: ", recall, " F1: ", f1, "\n")

        plt.figure(figsize=(7.4, 6.2))
        labels = ["NEGATIVE_NONE", "SQUAT_POSE", "SQUAT_REP", "PUSHUP_POSE", "PUSHUP_REP", "LUNGE_POSE", "LUNGE_REP", ]
        cm = confusion_matrix(sample_results["actual"], sample_results["predicted"], labels=labels)
        g = sns.heatmap(cm, annot=True, xticklabels=labels, yticklabels=labels, fmt="d")
        g.set_xticklabels(g.get_xmajorticklabels(), fontsize=10, rotation=35)
        g.set_yticklabels(g.get_ymajorticklabels(), fontsize=10, rotation=35)
        plt.ylabel("True Label")
        plt.xlabel("Predicted Label")
        plt.savefig("SampleConfusionMatrix.pdf", bbox_inches="tight")

        print("Classification Subjects Result")
        group_results = dataManager.get_data_from_file("GroupResults.csv", make_numeric=False)

        fig, ax = plt.subplots(figsize=(20, 10))
        x = np.arange(len(group_results.index))
        width = 0.2
        ax.bar(x - width/2, group_results["Correct_Reps"], width, label="Correct")
        ax.bar(x + width/2, group_results["Total_Reps"], width, label="Total")
        ax.set_xticks(x)
        plt.legend(["Correct Repetitions", "Total Repetitions"])

        if add_statistics:
            rep_ratio_string = "%d/%d" % (group_results["Correct_Reps"].sum(), group_results["Total_Reps"].sum())
            rep_ratio = group_results["Correct_Reps"].sum() / group_results["Total_Reps"].sum()
            stats_text = "\n\nCorrect Reps: %s\nAccuracy: %f" % (rep_ratio_string, rep_ratio)
        else:
            stats_text = ""
        plt.xlabel("Subject ID" + stats_text)

        if Config.EVAL_USE_DATA == "PARTIAL":
            ax.set_xticklabels([6, 7, 8, 9, 10, 11, 12, 13, 14, 15])
        elif Config.EVAL_USE_DATA == "ALL":
            ax.set_xticklabels([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15])
        elif Config.EVAL_USE_DATA == "GOOD":
            ax.set_xticklabels([1, 2, 3, 4, 7, 8, 9, 10, 12, 13])
        plt.savefig("Bars.pdf", bbox_inches="tight")

        print("Results Based On Exercise and Speed")
        number_actual = []
        number_correct = []
        for name, group in repetition_results.groupby("actual"):
            true_exercises = group["actual"].apply(lambda x: x.split("_")[0].upper())
            accuracy = accuracy_score(true_exercises, group["predicted"])
            precision, recall, f1, support = precision_recall_fscore_support(true_exercises,
                                                                             group["predicted"],
                                                                             average='macro', zero_division=1)
            print(name, "Accuracy: ", accuracy, "Precision: ", precision, " Recall: ", recall, " F1: ", f1, "\n")
            number_actual.append(len(group))
            number_correct.append((group["predicted"] == name.split("_")[0].upper()).sum())
            print(name, (group["predicted"] == name.split("_")[0].upper()).sum(), "/", len(group))

        fig, ax = plt.subplots(figsize=(8, 8))
        x = np.arange(9)
        width = 0.4
        ax.bar(x - width / 2, number_correct, width, label="Correct")
        ax.bar(x + width / 2, number_actual, width, label="Total")
        ax.set_xticks(x)
        ax.set_xticklabels(
            ["Lunge_Short", "Lunge_Medium", "Lunge_Long", "Pushup_Short", "Pushup_Medium", "Pushup_Long", "Squat_Short",
             "Squat_Medium", "Squat_Long"])
        ax.set_ylabel("Repetitions")
        plt.legend(["Correct Repetitions", "Total Repetitions"], loc="lower right")
        plt.xticks(rotation=90)
        plt.savefig("ExerciseBars.pdf", bbox_inches="tight")

        pause_lengths = repetition_results["actual"].apply(lambda x: x.split("_")[1])
        repetition_results["pause"] = pause_lengths

        number_actual = []
        number_correct = []
        for name, group in repetition_results.groupby("pause"):
            true_exercises = group["actual"].apply(lambda x: x.split("_")[0].upper())
            accuracy = accuracy_score(true_exercises, group["predicted"])
            precision, recall, f1, support = precision_recall_fscore_support(true_exercises,
                                                                             group["predicted"],
                                                                             average='macro', zero_division=1)
            print(name, "Accuracy: ", accuracy, "Precision: ", precision, " Recall: ", recall, " F1: ", f1, "\n")
            number_actual.append(len(group))
            number_correct.append((group["predicted"] == true_exercises).sum())

        fig, ax = plt.subplots(figsize=(8, 8))
        x = np.arange(3)
        width = 0.4
        ax.bar(x - width / 2, number_correct, width, label="Correct")
        ax.bar(x + width / 2, number_actual, width, label="Total")
        ax.set_xticks(x)
        ax.set_xticklabels(
            ["Short", "Medium", "Long"])
        ax.set_ylabel("Repetitions")
        plt.legend(["Correct Repetitions", "Total Repetitions"], loc="lower right")
        plt.xticks(rotation=90)
        plt.savefig("ExerciseBars2.pdf", bbox_inches="tight")

    def retrain_model_with_segments(self, data):
        classifierColumnNames = DataAccess.classifier_columns(include_label=True)
        classifierColumnNames.append("Segment")

        trainingData = data[classifierColumnNames]
        segmentStart = 0
        for index, row in trainingData.iterrows():
            if row["Segment"] == "START":
                segmentStart = index

            if row["Segment"] == "END":
               trainingData[segmentStart:index]["Label"] = "PushupRep"

        X = trainingData.drop(["Label", "Segment"], axis=1)
        Y = trainingData["Label"]

        supportVectorMachine = svm.SVC()
        supportVectorMachine.fit(X, Y)
        dump(supportVectorMachine, "core/ml-models/svm_model")

