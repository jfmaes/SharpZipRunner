# SharpZipRunner
Executes position independent shellcode from an encrypted zip 


``` 
 ___  _                   ____ _       ___
/ __>| |_  ___  _ _  ___ |_  /<_> ___ | . \ _ _ ._ _ ._ _  ___  _ _
\__ \| . |<_> || '_>| . \ / / | || . \|   /| | || ' || ' |/ ._>| '_>
<___/|_|_|<___||_|  |  _//___||_||  _/|_\_\`___||_|_||_|_|\___.|_|
                    |_|          |_|


An Encrypted zip on your computer? what could possibly go wrong?

 Usage:
  -z, --zip-file=VALUE       The path on disk to the encrypted zip

  -e, --entry=VALUE          The specific zip entry to put in mem (optional),
                               if not provided assumes only one zip entry is
                               present

  -p, --password=VALUE       The password of the encrypted zip

  -i, --process=VALUE        The process to inject into (if not used will
                               inject into self (not recommended)

  -c, --create               Create a new process, and injects into that
                               process (requires the process argument)

  -h, --help                 shows this menu

``` 
