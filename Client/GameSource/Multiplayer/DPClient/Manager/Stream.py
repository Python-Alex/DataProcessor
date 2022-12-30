import os
import sys
import time
import json
import socket
import threading

from GameSource.Multiplayer.DPClient.Manager.Receiving import Incoming, QueryResponse, EventType

class DataProcessorStream(threading.Thread):

	Host : tuple[str, int]
	Socket : socket.socket

	def __init__(self):
		threading.Thread.__init__(self, name=self.__class__.__name__)

		self.Host = ("127.0.0.1", 65500)
		self.Socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		
		try:
			self.Socket.connect(self.Host)
		except socket.error:
			if(__debug__ == False):
				sys.stderr.write("[ERROR] Connection Failed to DataProcessor Stream %s:%d\n" % (self.Host[0], self.Host[1]))

	def ProcessIncomingData(self, event: dict)->bool:
		if("Event" not in event.keys()):
			return False

		incoming = Incoming(event.get("Event"), event.get("Tag"))

		match incoming.Event:
			case EventType.INITIALIZED:
				print("TASK INITIALIZED")
			
			case EventType.DATA_FED:
				print("DATA FED")

			case EventType.UPDATED:
				print("TASK UPDATED")

			case EventType.QUERY_RESPONSE:
				query = QueryResponse(event.get("Tag"), event.get("Response"))

	def run(self) -> None:
		while(self.Socket):
			bytedata = self.Socket.recv(2048)

			jsondata = {}
			try:
				jsondata = json.loads(bytedata.decode())
			except Exception:
				if(not __debug__):
					sys.stderr.write("[ERROR] DataProcessor Stream Sent Bad Data\n")
				
				continue

			finally:
				print("handling", jsondata)
				result = self.ProcessIncomingData(jsondata)
				
def InitializeConnection()->DataProcessorStream:
	dps = DataProcessorStream()
	dps.start()

	return dps