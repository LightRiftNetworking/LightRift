﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using DarkRift.Dispatching;
using System.Collections.Specialized;

namespace DarkRift.Server
{
    /// <summary>
    ///     Base class for plugin load data.
    /// </summary>
    public abstract class PluginBaseLoadData
    {
        /// <summary>
        ///     The name to give the plugin.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The dispatcher to pass to the plugin.
        /// </summary>
        public IDispatcher Dispatcher { get; set; }

        /// <summary>
        ///     The server info to pass to the plugin.
        /// </summary>
        public DarkRiftInfo ServerInfo { get; }

        /// <summary>
        ///     The settings this plugin was given.
        /// </summary>
        public NameValueCollection Settings { get; }

        /// <summary>
        ///     The thread helper for the server.
        /// </summary>
        public DarkRiftThreadHelper ThreadHelper { get; set; }

        /// <summary>
        ///     The server's log manager.
        /// </summary>
        public ILogManager LogManager { get; set; }

        /// <summary>
        ///     The logger this plugin will use.
        /// </summary>
        public Logger Logger { get; set; }

        /// <summary>
        ///     The server we belong to.
        /// </summary>
        internal DarkRiftServer Server { get; set; }

        internal PluginBaseLoadData(string name, DarkRiftServer server, NameValueCollection settings, Logger logger)
            : this(name, settings, server.ServerInfo, server.ThreadHelper)
        {
            Dispatcher = server.Dispatcher;
            LogManager = server.LogManager;
            Logger = logger;
            Server = server;
        }

        /// <summary>
        ///     Creates new load data with the given properties.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="settings">The settings to pass the plugin.</param>
        /// <param name="serverInfo">The runtime details about the server.</param>
        /// <param name="threadHelper">The server's thread helper.</param>
        public PluginBaseLoadData(string name, NameValueCollection settings, DarkRiftInfo serverInfo, DarkRiftThreadHelper threadHelper)
        {
            Name = name;
            Settings = settings;
            ServerInfo = serverInfo;
            ThreadHelper = threadHelper;
        }
    }
}
