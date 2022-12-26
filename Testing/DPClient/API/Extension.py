import time

SleepTimer = 0.00000000000000000000001

class APIHandler:

    LastCall : float = time.time()

    def TimeSinceLastSend():
        return time.time() - APIHandler.LastCall

    def Sendable():
        return APIHandler.TimeSinceLastSend() > 0.10