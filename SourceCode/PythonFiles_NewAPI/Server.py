import threading
import time

import numpy as np
import zmq

#from PerformanceAnalyzer import PerformanceAnalyzer


class Server:
    def __init__(self):
        #self.dataGateway = dataGateway
        self.context = zmq.Context()
        self.receive_socket = self.context.socket(zmq.SUB)
        self.receive_socket.connect("tcp://localhost:5555")
        self.receive_socket.setsockopt_string(zmq.SUBSCRIBE, "SuitDataStream")
        self.receive_socket.setsockopt(zmq.CONFLATE, 1)
        self.send_socket = self.context.socket(zmq.PUB)
        self.send_socket.bind("tcp://*:6666")

        self.queue = []
        self.thread1 = None
        self.thread2 = None
        self.threadsRunning = False

    def receive_thread(self):
        print("Receive Thread Started")
        while self.threadsRunning:
            #  Wait for next request from client
            message = self.receive_socket.recv(copy=True)
            print(message)
            self.send_socket.send_string("ErrorResponseStream " + " hello unity")
            # t = time.process_time()
            # topic, payload = message.split()
            #
            # stringPayload = str(payload, "utf-8")
            # data = stringPayload.split(";")
            #
            # try:
            #     row = np.array(data, dtype=np.single)
            # except:
            #     print("Count not process: ", message)
            # #PerformanceAnalyzer.add_read_data_time_measurement(time.process_time() - t)
            #
            # error = self.dataGateway.onNewTeslasuitData(row)
            #
            #
            #
            #
            #
            # name = error[0]
            # errorData = error[1]
            # csvString = ",".join(["%f" % num for num in errorData])
            # csvString = name + "," + csvString
            # self.send_socket.send_string("ErrorResponseStream " + csvString)
            #
            #


            # self.pushResult(error)
            #PerformanceAnalyzer.add_total_time_measurement(time.process_time() - t)
            # time.sleep(0.001)
        print("Receive Thread Stopped")

    def send_thread(self):
        print("Send Thread Started")
        while self.threadsRunning:
            if len(self.queue) > 0:
                name_error = self.queue.pop(0)
                name = name_error[0]
                errorData = name_error[1]
                csvString = ",".join(["%f" % num for num in errorData])
                csvString = name + ","+csvString
                self.send_socket.send_string("ErrorResponseStream " + csvString)
            time.sleep(0.001)
        print("Send Thread Stopped")

    def start(self):
        self.threadsRunning = True
        self.thread1 = threading.Thread(target=self.receive_thread)
        self.thread1.start()

        # self.thread2 = threading.Thread(target=self.send_thread)
        # self.thread2.start()

    def stop(self):
        self.threadsRunning = False
        self.thread1.join()
        # self.thread2.join()

    def pushResult(self, name_error):
        self.queue.append(name_error)





