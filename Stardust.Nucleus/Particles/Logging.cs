﻿//
// Logging.cs
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
using System.Diagnostics;
using Stardust.Nucleus;
using Stardust.Nucleus.ObjectActivator;

namespace Stardust.Particles
{
    /// <summary>
    /// Provides a common log service for the stardust framework. Implement ILogging if more advanced logging is needed and reset logger to your implementation
    /// </summary>
    public static class LoggingExtentions
    {
        private static ILogging Logger1
        {
            get => Logging.CurrentLogger;
            set => Logging.SetLogger(value);
        }

        private static bool ModuleCreatorLoggerSet;

        public static ILogging Logger
        {
            get
            {
                if (Logger1.IsNull())
                {
                    return Logger1 = Resolver.ResolverKernel.GetService<ILogging>();
                }
                return Logger1;
            }
        }

        public static void ResetLogger(Action<ILogging> initializationHandler = null)
        {
            ResetLogger<LoggingDefaultImplementation>();
        }

        public static void ResetLogger<T>(Action<ILogging> initializationHandler = null) where T : ILogging
        {
            Resolver.GetConfigurator().UnBind<ILogging>().AllAndBind().To<T>();
            Logger1 = Resolver.ResolverKernel.GetService<ILogging>();//.Activate(initializationHandler);
            initializationHandler?.Invoke(Logger1);
            if (ModuleCreatorLoggerSet)
                InitializeModuleCreatorWithDefalutLogger();
        }

        public static void InitializeModuleCreatorWithDefalutLogger()
        {
            LogHelper.SetLogger(Logger1);
            ModuleCreatorLoggerSet = true;
        }

        public static void SetCommonProperties(string logName)
        {
            if (Logger.IsNull()) return;
            Logger.SetCommonProperties(logName);
        }


        public static bool Initialized()
        {
            return Logger.IsInstance();
        }

        public static void SetLogger(Type loggerType)
        {
            Resolver.GetConfigurator().UnBind<ILogging>().AllAndBind().To(loggerType).SetSingletonScope();
            Logger1 = Resolver.ResolverKernel.GetService<ILogging>();//.Activate(initializationHandler);

            if (ModuleCreatorLoggerSet)
                InitializeModuleCreatorWithDefalutLogger();
        }

        public static void SetLogger(ILogging logger)
        {
            Resolver.GetConfigurator().UnBind<ILogging>().AllAndBind().ToConstructor(s => logger);
            Logger1 = Resolver.ResolverKernel.GetService<ILogging>();//.Activate(initializationHandler);

            if (ModuleCreatorLoggerSet)
                InitializeModuleCreatorWithDefalutLogger();
        }
    }
}
