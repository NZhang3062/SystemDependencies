# System Dependencies
## 1. Description
Components of computer systems often have dependencies--other components that must be installed before they will function properly. These dependencies are frequently shared by multiple components.
## 2. Function
### 2.1 Input
The input will contain a sequence of commands (as described below), each on a separate line containing no more than eighty characters. Item names are case sensitive, and each is no longer than ten characters. The command names (**DEPEND, INSTALL, REMOVE and LIST**) always appear in uppercase starting in column one, and item names are separated from the command name and each other by one or more spaces. All appropriate **DEPEND** commands will appear before the occurrence of any **INSTALL** dependencies. The end of the input is marked by a line containing only the word **END**.
### 2.2 Output
Echo each line of input. Follow each echoed **INSTALL** or **REMOVE** line with the actions taken in response, making certain that the actions are given in the proper order. Also identify exceptional conditions (see Expected Output, below, for examples of all cases). For the **LIST** command, display the names of the currently installed components. No output, except the echo, is produced for a **DEPEND** command or the line containing **END**. There will be at most one dependency list per item.
## 3. Command Syntax
### 3.1 DEPEND item1 item2 [item3 ...]
item1 depends on item2 (and item3 ...)
### 3.2 INSTALL item1
install item1 and those on which it depends
### 3.3 REMOVE item1
remove item1, and those on whch it depends, if possible
### 3.4 LIST
list the names of all currently-installed components
## 4. Author
Nan Zhang，
Ronglin Liu
