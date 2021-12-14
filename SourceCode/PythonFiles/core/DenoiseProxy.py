import Config
import numpy as np


class DenoiseProxy:
    def __init__(self):
        self.initialized = False
        self.buffer = None
        self.bufferLength = Config.DENOISE_LENGTH
        self.bufferIndex = 0

    def denoise(self, data):
        if not Config.DENOISE:
            return data

        if not self.initialized:
            self.buffer = np.tile(data, (self.bufferLength, 1))
            self.initialized = True

        self.buffer[self.bufferIndex] = data
        self.bufferIndex = (self.bufferIndex + 1) % self.bufferLength
        mean = self.buffer.mean(axis=0)
        return mean
