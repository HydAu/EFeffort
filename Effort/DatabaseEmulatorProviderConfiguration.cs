﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFProviderWrapperToolkit;

namespace Effort
{
    public static class DatabaseEmulatorProviderConfiguration
    {
        public static readonly string ProviderInvariantName = "EffortDatabaseEmulatorProvider";

        private static volatile bool registered = false;
        private static object sync = new object();

        public static void RegisterProvider()
        {
            if (!registered)
            {
                lock (sync)
                {
                    if (!registered)
                    {
                        DbProviderFactoryBase.RegisterProvider("Effort Database Emulator Provider", ProviderInvariantName, typeof(DatabaseEmulatorProviderFactory));
                        registered = true;
                    }
                }
            }
        }
    }
}
