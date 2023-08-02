from sklearn.model_selection import cross_val_score
from data.DataManager import DataManager
from sklearn import svm
from joblib import load

data = DataManager().data

X = data.drop("Label", axis=1)
X = X.apply(lambda x: x.str.replace(",", "."))
for col in X.columns:
    X[col] = X[col].astype(float)
Y = data["Label"]

clf = svm.SVC()
scores = cross_val_score(clf, X, Y, cv=10)
print("Crossval SVM:")
print(scores)
print("")

svm = load("core/ml-models/PreEvalModels/svm_model")
print("SVM on Trainset")
print(svm.score(X, Y))
print()

knn = load("core/ml-models/PreEvalModels/knn_model")
print("Knn on Trainset")
print(knn.score(X, Y))
print()
