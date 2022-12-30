import json
import enum
from threading import Event

ByteType = [str, list, dict, bool, int, float]

class Packet:
    """ Represents an Outgoing/Incoming Packet Structure
  
    Returns:
        object: Packet Object
    """

    def SerializeJson(self) -> bytes:
        """Serializes class into json bytes sendable over the socket

        Returns:
            bytes: serialized bytes
        """
        return json.dumps(self.__dict__).encode()

    def Deserialize(self) -> dict:
        """Returns dictionary"""
        return self.__dict__


class EventType(enum.Enum):
    """ Define Packet EventTypes """
    INITIALIZED = 0
    DATA_FED = 1
    UPDATED = 2
    QUERY_RESPONSE = 3
EventTypeMap = {
    EventType.INITIALIZED.value: EventType.INITIALIZED,
    EventType.DATA_FED.value: EventType.DATA_FED,
    EventType.UPDATED.value: EventType.UPDATED,
    EventType.QUERY_RESPONSE.value: EventType.QUERY_RESPONSE
}

class Incoming(Packet):
    """ Further details an Incoming Packet Object 

    Returns:
        object: Incoming Packet
    """
    Event: EventType
    Tag: str

    def __init__(self, event: int, tag: str) -> None:
        super().__init__()

        self.Event = EventTypeMap[event]
        self.Tag = tag
        
class QueryResponse(Packet):
    """ Response to a given Query

    Returns:
        object: Query Response
    """

    Event : EventType
    Tag : str
    Response : list[type]

    def __init__(self, tag: str, response: list[type]) -> None:
        super().__init__()
        
        self.Event = EventType.QUERY_RESPONSE
        self.Tag = tag
        self.Response = response
