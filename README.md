# DataProcessor
Data Processor for Executing Instructions

Highly configurable for almost any means, Almost entirely package free

| Features | Description | Packages |
| --- | --- | --- |
| `image utilities`| Manipulate/Analyze Images               | |
| `math utilities` | Algorithms and Chains                   | | 
| `plugin bus`     | Customizable Global Plugin Action       | |
| `tcp stream`     | TCP Stream for Relaying Data            | | 
| `json structure` | Json/Dictionary Based Data Transmission | [Newtonsoft(13.0.2~nuget)](https://www.nuget.org/packages/Newtonsoft.Json/) |

---
# [Python Client Implementation](www.google.com)
---
# Create Plugin
- Navigate to Examples.PluginExample.cs, and Copy the File
- Copy File into Essentials.Plugins, Filename Matching the Context of your Implementation
- Navigate to Essentials.PluginLoader.cs, under Class PluginLoader
- Add Reference of your new Plugin
- Begin Developing the Plugin

---
# Create Module
---
# Create Query
---

# Packet Documentation

# Task
## Initialization of a Task
Initializes a Task Setup for Executing

- Json Structure
- Capitilized Keynames

```
Key[int]    : Event = 0
Key[string] : Tag = "TaskName"
Key[string] : ModuleName = "FunctionToExecute"
```
To Json
```json
{"Event": 0, "Tag": "TaskName", "ModuleName": "FunctionToExecute"}
```



---


## Feeding Data to a Task
Feeds a set of Arguments for a Task to Execute
- Json Structure
- Capatalized Keynames

```
Key[int]    : Event = 1
Key[string] : Tag = "TaskName"
Key[string] : ModuleName = "FunctionToExecute"
```
To Json
```json
{"Event": 1, "Tag": "TaskName", "Arguments": {"ARGS": ...} }
```

---


## Updating Runtime of a Task
- Json Structure
- Capatalized Keynames

```
Set bool once

Key[int]    : Event = 2
Key[string] : Tag = "TaskName"
Key[bool]   : VerboseOutput
Key[bool]   : StopTask
Key[bool]   : PauseTask
Key[bool]   : ResumeTask
```
To Json
```json
{"Event": 2, "Tag": "TaskName", "VerboseOutput": BOOL, "StopTask": BOOL, "PauseTask": BOOL, "ResumeTask": BOOL }
```


---


## Querying Data
Query and Receive Data about a Task
- Json Structure
- Capatalized Keynames

```
Key[int]    : Event = 3
Key[string] : Tag = "TaskName"
Key[string] : Var = "SomeVar"
```
To Json
```json
{"Event": 1, "Tag": "TaskName", "Var": "SomeVar" }
```