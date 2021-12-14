import Config
from ClassificationResult import ClassificationType, ClassificationExercise
from core.DenoiseProxy import DenoiseProxy
from data.DataAccess import DataAccess


class StreamSegmentor:
    def __init__(self):
        self.topDetected = False
        self.repStarted = False
        self.denoiseProxy = DenoiseProxy()
        self.state = "UNKNOWN"
        self.negativeCount = 0
        self.last_recognized_exercise = ClassificationExercise.PUSHUP

    '''
        Returns the state of a segment
    '''
    def onNewStreamData(self, node_data, classification_exercise: ClassificationExercise, classification_type: ClassificationType):
        segmentation_result = "NONE"
        if not Config.SEGMENT:
            return segmentation_result

        if classification_exercise is not ClassificationExercise.NEGATIVE:
            self.last_recognized_exercise = classification_exercise
            self.negativeCount = 0

        segmentation_node = Config.class_segmentation_map[self.last_recognized_exercise]
        segmentation_data = DataAccess.get_gyroscope_data(node_data, segmentation_node)


        if Config.SEGMENTATION_MODEL == "HEURISTIC":
            segmentation_result = self.segmentation_model_heuristic(segmentation_data, classification_exercise, classification_type)
        elif Config.SEGMENTATION_MODEL == "CLASSIFIER":
            segmentation_result = self.segmentation_model_classifier(classification_exercise, classification_type)

        return segmentation_result

    def segmentation_model_classifier(self, classification_exercise: ClassificationExercise, classification_type: ClassificationType):
        if (self.state == "UNKNOWN" or self.state == "REP_DONE") and classification_type == ClassificationType.POSE:
            self.state = "STARTING_POSE"
            return "NONE"

        if self.state == "STARTING_POSE" and classification_type == ClassificationType.REP:
            self.state = "IN_REP"
            return "START"

        if self.state == "IN_REP" and Config.predictedClass == classification_type == ClassificationType.POSE:
            self.state = "REP_DONE"
            return "END"

        if self.state == "STARTING_POSE" and classification_exercise == ClassificationExercise.NEGATIVE:
            self.state = "UNKNOWN"
            return "NONE"

        return "NONE"


    def segmentation_model_heuristic(self, segmentation_data, classification_exercise: ClassificationExercise, classification_type: ClassificationType):
        if classification_type == ClassificationType.POSE and abs(segmentation_data[0]) < Config.SEGMENT_THRESHOLD \
                and abs(segmentation_data[1]) < Config.SEGMENT_THRESHOLD and abs(segmentation_data[2]) < Config.SEGMENT_THRESHOLD:

            if self.state == "UNKNOWN" or self.state == "REP_DONE" or self.state == "REP_STARTED":
                self.state = "STARTING_POSE"
                return "NONE"

            if self.state == "IN_REP":
                self.state = "REP_DONE"
                return "END"

        # if self.state == "STARTING_POSE" and abs(segmentation_data[0]) > 2 and abs(segmentation_data[1]) > 2 and abs(segmentation_data[2]) > 2:
        if self.state == "STARTING_POSE" and sum(abs(segmentation_data)) > Config.SEGMENT_THRESHOLD * 3:
            self.state = "REP_STARTED"
            return "START"

        if self.state == "REP_STARTED" and classification_type == ClassificationType.REP:
            self.state = "IN_REP"
            return "NONE"

        if classification_exercise == ClassificationExercise.NEGATIVE:
            self.negativeCount = self.negativeCount + 1
            # if self.negativeCount >= 10:
            self.state = "UNKNOWN"
            return "NONE"

        return "NONE"
