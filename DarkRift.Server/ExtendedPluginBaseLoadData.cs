﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using DarkRift.Server.Metrics;
using System;
using System.Collections.Specialized;

namespace DarkRift.Server
{
    /// <summary>
    ///     Base load data class for plugins inheriting <see cref="ExtendedPluginBase"/>.
    /// </summary>
    public abstract class ExtendedPluginBaseLoadData : PluginBaseLoadData
    {
        /// <summary>
        ///     The server's metrics manager.
        /// </summary>
        public IMetricsManager MetricsManager { get; set; }

        /// <summary>
        ///     The metrics collector this plugin will use.
        /// </summary>
        public MetricsCollector MetricsCollector { get; set; }

        internal ExtendedPluginBaseLoadData(string name, DarkRiftServer server, NameValueCollection settings, Logger logger, MetricsCollector metricsCollector
        )
            : base(name, server, settings, logger)
        {
            MetricsManager = server.MetricsManager;
            MetricsCollector = metricsCollector;
        }

        /// <summary>
        ///     Creates new load data with the given properties.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="settings">The settings to pass the plugin.</param>
        /// <param name="serverInfo">The runtime details about the server.</param>
        /// <param name="threadHelper">The server's thread helper.</param>
        /// <param name="logger">The logger this plugin will use.</param>
        /// <remarks>
        ///     This constructor ensures that the legacy <see cref="WriteEventHandler"/> field is initialised to <see cref="Logger.Log(string, LogType, Exception)"/> for backwards compatibility.
        /// </remarks>
        public ExtendedPluginBaseLoadData(string name, NameValueCollection settings, DarkRiftInfo serverInfo, DarkRiftThreadHelper threadHelper, Logger logger)
            : base(name, settings, serverInfo, threadHelper)
        {
            Logger = logger;
        }
    }
}
