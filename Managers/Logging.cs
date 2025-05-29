using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MelonLoader;
using MelonLoader.Utils;

namespace Managers
{
    public static class Log
    {
        // Contains logs from LazyLogger until it gets drained.
        private static Queue<string> logStack = new Queue<string>();
        
        // # of spaces before a message.
        private static string depth;

        // Is marked by Loud and Quiet whenever called in Lazy mode.
        private static bool shouldDrain;
        
        // Just tracks whether or no the current logger is LazyLogger.
        private static bool isLazy;
        
        // A minimal interface for logging used by EagerLogger and LazyLogger.
        private delegate void Logger(string message);
        
        // The current logging strategy: eager OR lazy.
        private static Logger activeLogger = EagerLogger;

        // Holds the relevant handle to the logfile to improve logging performance over File.AppendAllText (which is slow bc open/close every time)
        private static FileStream logfileStream;
        
        
        public static void Prepare()
        {
            logfileStream = File.Open(Env.LogfilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            Loud("[SoundBending.Managers.Log] Prepare: Logfile handle ready");
        }

        public static void Deinit()
        {
            logfileStream.Close();
            MelonLogger.Msg("|    [SoundBending.Managers.Log] Deinit: logfile stream closed");
        }
        
        
        
        // Just forwards to MelonLogger.Msg and the log file
        private static void EagerLogger(string message)
        {
            string formatted = depth + message;
            
            MelonLogger.Msg(formatted);
            logfileStream.WriteAsync(Encoding.UTF8.GetBytes("| " + formatted + "\n"), 0, 2 + formatted.Length + 1);
        }
        
        // Just pushes to logStack until Close is called, then either drains (if shouldDrain is marked) or gets cleared.
        private static void LazyLogger(string message)
        {
            logStack.Enqueue(depth + message);
        }

        // Only called if shouldDrain is true at the end of a Close call.
        private static void DrainLogStack()
        {
            while (logStack.Count > 0)
            {
                string log = logStack.Dequeue();
                
                MelonLogger.Msg(log);
                
                logfileStream.WriteAsync(Encoding.UTF8.GetBytes("+ " + log + "\n"), 0, 2 + log.Length + 1);
            }

            logStack.Clear();
        }
        
        
        // Log messages loudly and ALWAYS drain when any loglevel is on
        public static void Force(params string[] args)
        {
            if (Env.LogLevel > 0)
            {
                if (isLazy)
                {
                    shouldDrain = true;
                }
                
                activeLogger(args[0]);
            }
        }
        
        // Log messages loudly (in any debug mode)
        public static void Loud(params string[] args) {
            if (Env.LogLevel > 0)
            {
                if (isLazy && Env.LogLevel > 1) shouldDrain = true;
                
                activeLogger(args[0]);
            }
        }
        
        // Log messages quietly (only in debug mode 2 or higher)
        public static void Quiet(params string[] args) {
            if (Env.LogLevel > 1)
            {
                activeLogger(args[0]);
            }
        }
        
        

        // Increases logging depth (args are always quiet).
        public static void Open(params string[] args)
        {
            if (args.Length != 0 && Env.LogLevel > 0)
            {
                activeLogger(">> " + args[0]);
            }
            
            depth += "|   ";
        }
        
        // Decreases logging depth
        public static void Close(params string[] args)
        {
            if (depth.Length == 0)
            {
                Loud("[SoundBending.Managers.Log] Close: depth is already zero? how did this even happen?");
                return;
            }
            
            depth = depth.Remove(depth.Length - 4);
            
            if (args.Length != 0 && Env.LogLevel > 0)
            {
                activeLogger("<< " + args[0] + (depth.Length == 0 ? "\n" : ""));
            }
        }
        
        

        // Wraps a given Action in a logging block, further indenting logs from deeper in the call stack.
        public static void Wrap(string fromTag, string prefix, string postfix, Action action)
        {
            Open(fromTag + (prefix != null ? ": " + prefix : ";"));

            try
            {
                action();
            }
            catch (Exception e)
            {
                Force("ERROR " + e);
            }
            
            Close(fromTag + (postfix != null ? ": " + postfix : ";"));
        }

        
        
        // Same as Wrap, but for functions that run every frame. Very useful for reducing log sizes. Only triggers 1) on Loud calls AND 2) when DebugLevel is 2 or greater.
        public static void WrapLazy(string fromTag, string prefix, string postfix, Action action)
        {
            bool alreadyLazy = isLazy;

            if (!alreadyLazy)
            {
                activeLogger = LazyLogger;
                isLazy = true;
                shouldDrain = false;
            }
            
            Open(fromTag + (prefix != null ? ": " + prefix : ";"));

            try
            {
                action();
            }
            catch (Exception e)
            {
                Force(depth + "ERROR " + e);
            }
            
            Close(fromTag + (postfix != null ? ": " + postfix : ";"));

            if (!alreadyLazy) {
                if (shouldDrain)
                {
                    DrainLogStack();
                }
                
                logStack.Clear();
                activeLogger = EagerLogger;
                isLazy = false;
                shouldDrain = false;
            }
        }
    }
}