import time

SleepTimer = 0.00000000000000000000001

class APIHandler:

    LastCall : float = time.time()

    @staticmethod
    def TimeSinceLastSend():
        return time.time() - APIHandler.LastCall

    @staticmethod
    def Sendable():
        """ If API Errors persist with events not being handled, increase this timer """
        return APIHandler.TimeSinceLastSend() > 0.010