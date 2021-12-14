import Config
from Quat4f import Quat4f
from Vector3f import Vector3f
from data.DataAccess import DataAccess


class TsMocapData:
    def __init__(self, node, row):
        dataAccess = DataAccess()

        self.quat9x = Quat4f(row[dataAccess.get_node_property_names(node, "quat9x")])
        self.quat6x = Quat4f(row[dataAccess.get_node_property_names(node, "quat6x")])
        self.linearAccel = Vector3f(row[dataAccess.get_node_property_names(node, "linearAccel")])
        self.accelerometer = Vector3f(row[dataAccess.get_node_property_names(node, "accelerometer")])
        self.gyroscope = Vector3f(row[dataAccess.get_node_property_names(node, "gyroscope")])
        self.magnetometer = Vector3f(row[dataAccess.get_node_property_names(node, "magnetometer")])
