# System Monitor

Monitor of file system changes in a computer. It also monitors Windows registry on that OS.

## Design

The program is formed by one command that has two ways of monitoring the system:

- Monitoring the changes on all the folders of the file system.
- Monitoring the changes on a specific folder.

### Output

The output of the command has the following structure:

- `FileChanges`
  - `AllFileChanges.txt`: all files that were changed, created, deleted or renamed.
  - `ChangedFiles.txt`: files that were changed.
  - `CreatedFiles.txt`: files that were created.
  - `DeletedFiles.txt`: files that were deleted.
  - `RenamedFiles.txt`: files that were renamed.
- `Log.txt`: : all monitored changes by time.

## Functioning

The application creates subscriptions to system events to be notified of changes on the computer. All these changes are saved in a file.

## Options

Option|Description
-|-
-f Folder|The folder to monitor. The whole file system is monitored if it is not specified.