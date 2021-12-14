from typing import List
import Config


class DataAccess:
    __nodeSubPropertyMap = {
            "quat9x": ["_quat9x_w", "_quat9x_x", "_quat9x_y", "_quat9x_z"],
            "quat6x": ["_quat6x_w", "_quat6x_x", "_quat6x_y", "_quat6x_z"],
            "linearAccel": ["_linearAccel_x", "_linearAccel_y", "_linearAccel_z"],
            "accelerometer": ["_accelerometer_x", "_accelerometer_y", "_accelerometer_z"],
            "gyroscope": ["_gyroscope_x", "_gyroscope_y", "_gyroscope_z"],
            "magnetometer": ["_magnetometer_x", "_magnetometer_y", "_magnetometer_z"],
            "temperature": ["_temperature"]
        }

    @staticmethod
    def classifier_columns(include_label=False) -> List[str]:
        names = []
        if include_label:
            names.append("label")

        names.extend(DataAccess.__get_nodes_properties_names(Config.classifierNodes, Config.classifierProperties))
        return names

    @staticmethod
    def result_columns() -> List[str]:
        names = []
        names.extend(DataAccess.simulation_columns())
        return names

    @staticmethod
    def result_promp_columns() -> List[str]:
        names = ["index", "label", "segment"]
        names.extend(DataAccess.get_joints_properties_names(Config.streamedJoints))
        return names

    @staticmethod
    def streamed_columns():
        names = []
        node_property_columns = DataAccess.__get_nodes_properties_names(Config.streamedNodes,
                                                                        Config.streamedNodeProperties)
        joint_property_columns = DataAccess.get_joints_properties_names(Config.streamedJoints)

        names.extend(node_property_columns)
        names.extend(joint_property_columns)
        return names

    @staticmethod
    def simulation_columns():
        names = ["index"]
        node_property_columns = DataAccess.__get_nodes_properties_names(Config.simulatedNodes, Config.simulatedProperties)
        joint_property_columns = DataAccess.get_joints_properties_names(Config.simulatedJoints)

        names.extend(node_property_columns)
        names.extend(joint_property_columns)
        return names

    @staticmethod
    def get_joints_properties_names(joint_list: List[str]) -> List[str]:
        names = []
        for joint in joint_list:
            names.extend(DataAccess.get_joint_properties_names(joint))
        return names

    @staticmethod
    def get_joint_properties_names(joint: str) -> List[str]:
        return [joint+"_x", joint+"_y", joint+"_z"]

    @staticmethod
    def __get_nodes_properties_names(node_list: List[str], property_list: List[str]) -> List[str]:
        """
        Returns a list of fully qualified names for the specified nodes and properties.
        :param node_list: List of node names
        :param property_list: List of node property names
        :return:
        """
        names = []
        for node in node_list:
            names.extend(DataAccess.get_node_properties_names(node, property_list))
        return names

    @staticmethod
    def get_node_properties_names(node: str, property_list: List[str]) -> List[str]:
        """
        Return a list of fully qualified names for the specified node and properties
        :param node: name of a node
        :param property_list: list of node property names
        :return:
        """
        names = []
        for nodeProperty in property_list:
            names.extend(DataAccess.get_node_property_names(node, nodeProperty))
        return names

    @staticmethod
    def get_node_property_names(node: str, node_property: str):
        """
        Returns a list of fully qualified names for the specified node and property
        Example Input: RightUpperLeg, quat9x
        Example Output: [RightUpperLeg_quat9x_w, RightUpperLeg_quat9x_x, RightUpperLeg_quat9x_y, RightUpperLeg_quat9x_z]
        :param node: name of a node
        :param node_property: name of a property
        :return:
        """
        names = []
        subproperties = DataAccess.__nodeSubPropertyMap[node_property]
        for subProb in subproperties:
            names.append(node + subProb)
        return names

    @staticmethod
    def get_timestamp(suit_data):
        """
        Extracts the timestamp from the suitData array.
        Timestamp is found at index 0.
        :param suit_data: Array of the complete data send to Python
        :return: Timestamp
        """
        return int(suit_data[0])

    @staticmethod
    def get_data_without_timesamp(suit_data):
        """
        Returns all data apart from the timestamp
        :param suit_data:
        :return:
        """
        return suit_data[1:]

    @staticmethod
    def get_node_data(denoised_data):
        """
        Extracts all data that belongs to nodes from the denoised data. I.e timestamp is no
        longer included in the data
        There are 10 nodes streamed with 2 properties each. Each property has 3 axis.
        Thus 60 data fields belong to the nodes.
        :param denoised_data:
        :return:
        """
        return denoised_data[:60]

    @staticmethod
    def get_joint_data(denoised_data):
        """
        Extracts all data that belongs to joints from the denoised data. I.e timestamp is no
        longer included in the data
        There are 18 joints streamed with 3 axis each, thus 54 data fields belong to the joints.
        The first 60 fields belong to the nodes and everything after that belongs to the joints.
        :param denoised_data:
        :return:
        """
        return denoised_data[60:]

    @staticmethod
    def get_gyroscope_data(node_data, node):
        node_index = Config.streamedNodes.index(node)
        # * 6 because every node has two properties with 3 axis each
        return node_data[node_index*6:node_index*6+3]
