//
// ilogging.cs
// This file is part of Stardust
//
// Author: Jonas Syrstad (jsyrstad2+StardustCore@gmail.com), http://no.linkedin.com/in/jonassyrstad/) 
// Copyright (c) 2014 Jonas Syrstad. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

namespace Stardust.Particles
{
    public static class Logging
    {
        public static ILogging CurrentLogger=>_logger;
        private static ILogging _logger;
        private static readonly object LockObject=new object();

        public static void SetLogger(ILogging logger)
        {
            lock (LockObject)
            {
                _logger = logger; 
            }
        }

        public static void DebugMessage(string message, LogType entryType = LogType.Information, string additionalDebugInformation = null)
        {
            try
            {
                if (CurrentLogger.IsNull()) return;
                CurrentLogger.DebugMessage(message, entryType, additionalDebugInformation);
            }
            catch
            {
            }
        }

        public static void DebugMessage(string format, params object[] args)
        {
            DebugMessage(string.Format(format, args));
        }

        public static void DebugMessage(string format, LogType entryType, params object[] args)
        {
            DebugMessage(string.Format(format, args), entryType);
        }

        public static void HeartBeat()
        {
            try
            {
                if (CurrentLogger.IsNull()) return;
                CurrentLogger.HeartBeat();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Use this to log the exception befrot rethrowing it.
        /// </summary>
        /// <code>
        /// catch(Exception ex)
        /// {
        ///     throw ex.LogAndRethrow();
        /// }
        /// </code>
        public static T LogAndRethrow<T>(this T self, string additionalDebugInformation = null) where T : Exception
        {
            Exception(self, additionalDebugInformation);
            return self;
        }

        public static void Exception(Exception exceptionToLog, string additionalDebugInformation = null)
        {
            try
            {
                if (CurrentLogger.IsNull()) return;
                CurrentLogger.Exception(exceptionToLog, additionalDebugInformation);
            }
            catch
            {
            }
        }

        public static void Log(this Exception self, string additionalDebugInformation = null)
        {
            Exception(self, additionalDebugInformation);
        }

    }
    /// <summary>
    /// This interface is used to ensure that this assembly supports the client profile. log4net does not support client profile.
    /// If you need logging and the client profile implement this interface and write all logging your self.
    /// </summary>
    public interface ILogging
    {
        void Exception(Exception exceptionToLog, string additionalDebugInformation = null);
        void HeartBeat();
        void DebugMessage(string message, LogType entryType = LogType.Information, string additionalDebugInformation = null);
        void SetCommonProperties(string logName);
    }
}