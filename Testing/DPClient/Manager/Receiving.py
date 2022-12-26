import json
import enum


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


class Incoming(Packet):
    """ Further details an Incoming Packet Object 

    Returns:
        object: Incoming Packet
    """
    Event: int
    Tag: str

    def __init__(self, event: int, tag: str) -> None:
        super().__init__()

        self.Event = event
        self.Tag = tag

    @property
    def Type(self) -> EventType:
        return EventType[self.Event]

        
class QueryResponse(Packet):
    """ Response to a given Query

    Returns:
        object: Query Response
    """

    Event : int
    Tag : str
    Response : list[any]

    def __init__(self, tag: str, response: list[any]) -> None:
        super().__init__()
        
        self.Event = EventType.QUERY_RESPONSE
        self.Tag = tag
        self.Response = response
