# System Monitor

Monitor of file system changes in a computer. It also monitors Windows registry on that OS.

## Design

The program is formed by one command that has two ways of monitoring the system:

- Monitoring the changes on all the directories of the file system.
- Monitoring the changes on a specific directory.

### Output

If all the directories of the file system are monitored, the output of the command has the following structure:

- `Starting date and time`
  - `Drive1`
    - `FileChanges`
      - `ChangedFiles.txt`: files that were changed on Drive1.
      - `CreatedFiles.txt`: files that were created on Drive1.
      - `DeletedFiles.txt`: files that were deleted on Drive1.
      - `RenamedFiles.txt`: files that were renamed on Drive1.
    - `AllFileChanges.txt`: all files that were changed, created, deleted or renamed on Drive1.
    - `Events.txt`: all monitored changes on Drive1 by time.
  - `Drive2`
  - ...
  - `AllFileChanges.txt`: all files that were changed, created, deleted or renamed on all drives.
  - `Events.txt`: all monitored changes on all drives by time.

In case a specific directory is monitored:

- `Starting date and time`
  - `FileChanges`
    - `ChangedFiles.txt`: files that were changed on the directory.
    - `CreatedFiles.txt`: files that were created on the directory.
    - `DeletedFiles.txt`: files that were deleted on the directory.
    - `RenamedFiles.txt`: files that were renamed on the directory.
  - `AllFileChanges.txt`: all files that were changed, created, deleted or renamed on the directory.
  - `Events.txt`: all monitored changes on the directory by time.

In both cases, all monitored changes are displayed in the console as they occur.

## Functioning

The application creates subscriptions to system events to be notified of changes on the computer. All these changes are saved in a file.

## Options

Option|Description
-|-
-d directory|The directory to monitor. The whole file system is monitored if it is not specified.