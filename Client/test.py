import os
import time


#import GameSource

jumbo = '{"Event": 1, "Nesting": {"NestingAgain": ""}}{"Event": 2, "Nesting": "NestingAgain"}'

def GayTest():

    open_brackets = 0
    closing_brackets = 0

    for char_index in range(len(jumbo)):
        if(jumbo[char_index] == "{"):
            open_brackets += 1
            
        if(jumbo[char_index] == "}" and jumbo[char_index + 1] == "{"):
            