import os
import sys
import json
import time
import socket

Master = socket.socket()
Master.connect(("127.0.0.1", 65500))

InitializeTask = {"Event": 0, "Tag": "ImageCompare", "ModuleName": "ImageRatioCompare"}
FeedTask = {"Event": 1, "Tag": "ImageCompare", "Arguments": {
        "A": [[0, 0, 0, 0], [0, 0, 0, 0]], 
        "B": [[0, 1, 1, 1, 0], [0, 0, 0, 0, 0]]
    }
}

FeedTaskMulti = {"Event": 1, "Tag": "ImageCompare", "Arguments": {
    "Images": {
        "A": [[0, 0, 0, 0], [0, 0, 0, 0]], 
        "B": [[0, 1, 1, 1, 0], [0, 0, 0, 0, 0]],
        "C": [[0, 0, 0, 1], [0, 1, 0, 0]]}
    }
}
QueryTaskResult = {"Event": 3, "Tag": "ImageCompare", "Var": "TaskResults"}


Master.send(json.dumps(InitializeTask).encode())
received_init = Master.recv(1024)
time.sleep(1.5)

Master.send(json.dumps(FeedTask).encode())
received_data_fed = Master.recv(1024)
time.sleep(1.5)

Master.send(json.dumps(QueryTaskResult).encode())
received_query_response = Master.recv(1024)

print(received_init)
print(received_data_fed)
print(received_query_response)

while(True):
    received = Master.recv(1024)
    print(received)