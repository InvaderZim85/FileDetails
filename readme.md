# FileDetails

**Content**
<!-- TOC -->

- [General](#general)
- [Installation](#installation)
    - [Regedit](#regedit)
    - [Registry file](#registry-file)
        - [Registry file for the file menu](#registry-file-for-the-file-menu)
        - [Registry file for the directory menu](#registry-file-for-the-directory-menu)

<!-- /TOC -->

## General
The program *FileDetails* is a small tool to show informations about a file or a directory via the windows context menu (shell).

**File**

![Context menu](images/004.png)

![Details](images/009.png)

When you're showing the details for a file you can also compare the hash values. For this click on the *Compare hash values* button (bottom left)

![Compare](images/010.png)

> **Note** The comparision of the hash values is case insensitive 

**Directory**

![Context menu dir](images/005.png)

![Details dir](images/011.png)

You can also save the details into a file (text of markdown) or copy them to the clipboard (text or markdown)

## Installation
When you've compiled the project you can copy the exe to any folder you want. To use the program via the windows context menu (shell) you have to add two entries to the registry.

### Regedit
> **Note**: You need administrator privileges to open the *Registry Editor*

To add a new entry you've to do the following:
1. Open the windows registry:
    1. Press the windows key (keyboard) or click on the symbol in the task bar
    2. Type `regedit` 

       ![regedit](images/001.png)

2. Add the key for files:
    1. Navigate to `HKEY_CLASSES_ROOT\*\shell`
    2. Right click on `shell` and select *New > Key*

       ![new key](images/006.png)

    3. Give the key the name you want. The name will be displayed in the context menu. I've named it *FileDetails*
    4. Add another key under the new key and name it `command`

       ![sub key](images/002.png)

    5. Right click the entry `(Default)` and select *Modify*

       ![Modify](images/007.png)

    6. Insert the path to the program and add `%1` to the end and click *OK*

       ![Path](images/008.png)

    7. Now you will see the entry in the context menu

       ![Context menu](images/004.png)
3. Add the key for directories:
    1. Navigate to `HKEY_CLASSES_ROOT\Directory\shell`
    2. Perform the steps 2.2 - 2.6

       ![Key](images/003.png)

    3. Now you will see the entry in the context menu

       ![Context menu](images/005.png)

### Registry file
If you don't want to perform all the above steps you can create two small files to add the values directly.

#### Registry file for the file menu
Create a new empty text file and copy the following. Save the file with the extension `reg` (e.G. *FileDetailsInstall.reg*)
```
Windows Registry Editor Version 5.00

[HKEY_CLASSES_ROOT\*\shell\Show details]

[HKEY_CLASSES_ROOT\*\shell\Show details\command]
@="PathToTheExe %1"
```

> **Note**: You have to change the path to the exe

#### Registry file for the directory menu
```
Windows Registry Editor Version 5.00

[HKEY_CLASSES_ROOT\Directory\shell\Show details]

[HKEY_CLASSES_ROOT\Directory\shell\Show details\command]
@="PathToTheExe %1"
```

> **Note**: You have to change the path to the exe
