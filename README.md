# SharpZipRunner
Executes position independent shellcode from an encrypted zip 
Get PIC code from your assembly either by using donut or metasploit or cobaltstrike RAW format.  

zip the .bin file and encrypt it with a password, this assembly decrypts the zip entry in memory and executes it using D/Invokes injection API. 

tested by dropping the encrypted zip on disk, but could probably also work entirely in memory with some modifications. 
only supports PIC payloads, tried creating a runPE variant but failed miserably :) 


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
