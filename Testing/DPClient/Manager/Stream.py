import os
import sys
import time
import json
import socket
import threading

from DPClient.Manager.Receiving import Incoming, QueryResponse, EventType

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
		if(not event.get("Event")):
			return False

		match event.get("Event"):
			case EventType.INITIALIZED.value:
				print("TASK INITIALIZED")
			
			case EventType.DATA_FED.value:
				print("DATA FED")

			case EventType.UPDATED.value:
				print("TASK UPDATED")

			case EventType.QUERY_RESPONSE.value:
				print("QUERY RESPONSE")

	def run(self) -> None:
		while(self.Socket):
			bytedata = self.Socket.recv(2048)

			jsondata = {}
			try:
				jsondata = json.loads(bytedata.decode())
				print(jsondata)
			except Exception:
				if(__debug__ == False):
					sys.stderr.write("[ERROR] DataProcessor Stream Sent Bad Data\n")
				
				continue

			finally:
				result = self.ProcessIncomingData(jsondata)
				

def InitializeConnection()->DataProcessorStream:
	dps = DataProcessorStream()
	dps.start()

	return dps