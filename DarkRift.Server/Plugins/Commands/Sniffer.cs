/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace DarkRift.Server.Plugins.Commands
{
    /// <summary>
    ///     Packet sniffer plugin.
    /// </summary>
    internal class Sniffer : Plugin
    {
        public override Version Version => new Version(1, 0, 0);

        public override Command[] Commands => new Command[]
        {
            new Command("sniffer", "Configures the message sniffer.", "sniffer add|remove (-a|-all) (-t|-tag=<tag>) (-id=<id>) (-ip=<ip>) (-h)\nsniffer clear\nsniffer list", SniffCommandHandler)
        };

        public override bool ThreadSafe => true;

        internal override bool Hidden => true;

        /// <summary>
        ///     The rules we are following
        /// </summary>
        private HashSet<RuleGroup> rules = new HashSet<RuleGroup>();

        public Sniffer(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
        }

        private void ClientManager_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Client.MessageReceived += Client_MessageReceived;
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (var message = e.GetMessage())
            {
                lock (rules)
                {
                    foreach (var group in rules)
                    {
                        if (group.Accepts(message, (Client)sender))
                        {
                            var builder = new StringBuilder();
                            builder.Append('[');
                            builder.Append(e.Client.ID);
                            builder.Append("]");

                            if (group.OutputData)
                            {
                                using (var reader = message.GetReader())
                                {
                                    for (var i = 0; i < reader.Length; i++)
                                    {
                                        builder.Append(" ");
                                        builder.Append(reader.ReadByte().ToString("X2"));

                                        if (i % 4 == 3)
                                        {
                                            builder.Append(" ");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                builder.Append(" ");
                                builder.Append(message);
                            }

                            Logger.Info(builder.ToString());
                            return;
                        }
                    }
                }
            }
        }

        private void SniffCommandHandler(object sender, CommandEventArgs e)
        {
            //Check args length
            if (e.Arguments.Length != 1)
            {
                throw new CommandSyntaxException($"Expected 1 argument but found {e.Arguments.Length}.");
            }

            var group = BuildRuleGroup(e.Flags);

            lock (rules)
            {
                switch (e.Arguments[0])
                {
                    //Add rule groups
                    case "add":
                        if (group.Count == 0)
                        {
                            throw new CommandSyntaxException("Found no conditions to define rule. Use -a to sniff all messages.");
                        }

                        var added = rules.Add(group);

                        if (added)
                        {
                            //Subscribe to events if we're the first rule
                            if (rules.Count == 1)
                            {
                                ClientManager.ClientConnected += ClientManager_ClientConnected;

                                foreach (Client client in ClientManager.GetAllClients())
                                {
                                    client.MessageReceived += Client_MessageReceived;
                                }
                            }

                            Logger.Info("Now sniffing " + group.ToString());
                        }
                        else
                        {
                            Logger.Info("Already sniffing " + group.ToString());
                        }

                        break;

                    //Remove rule groups
                    case "remove":
                        if (group.Count == 0)
                        {
                            throw new CommandSyntaxException("Found not conditions to define rule. Use -a to remove a sniff for all messages or 'sniffer clear' to remove all.");
                        }

                        var removed = rules.Remove(group);

                        if (removed)
                        {
                            //Unsubscribe to events if there's no more rules
                            if (rules.Count == 0)
                            {
                                ClientManager.ClientConnected -= ClientManager_ClientConnected;

                                foreach (Client client in ClientManager.GetAllClients())
                                {
                                    client.MessageReceived -= Client_MessageReceived;
                                }
                            }

                            Logger.Info("No longer sniffing " + group.ToString());
                        }
                        else
                        {
                            Logger.Info("Not sniffing " + group.ToString());
                        }

                        break;

                    //Clear all rule groups
                    case "clear":
                        if (group.Count != 0)
                        {
                            throw new CommandSyntaxException();
                        }

                        rules.Clear();

                        ClientManager.ClientConnected -= ClientManager_ClientConnected;

                        foreach (Client client in ClientManager.GetAllClients())
                        {
                            client.MessageReceived -= Client_MessageReceived;
                        }

                        Logger.Info("All sniffing rules cleared.");

                        break;

                    //List sniffing rules
                    case "list":
                        var builder = new StringBuilder();
                        builder.Append("Found " + rules.Count + " rules defined.");
                        var i = 1;
                        foreach (var rule in rules)
                        {
                            builder.Append("\n" + i++ + "\t" + rule.ToString());
                        }

                        Logger.Info(builder.ToString());

                        break;

                    //Complain
                    default:
                        throw new CommandSyntaxException($"Unknown operation '{e.Arguments[0]}'.");
                }
            }
        }

        private RuleGroup BuildRuleGroup(NameValueCollection flags)
        {
            //Should we output data?
            var outputData = flags.AllKeys.Contains("h");

            var group = new RuleGroup(outputData);

            //Load all rules
            if (flags.AllKeys.Contains("a") || flags.AllKeys.Contains("all"))
            {
                group.Add(new ALLTHERULES());

                return group;
            }

            //Load tag rules
            try
            {
                if (flags.AllKeys.Contains("t"))
                {
                    group.Add(new TagRule(ushort.Parse(flags["t"])));
                }
                else if (flags.AllKeys.Contains("tag"))
                {
                    group.Add(new TagRule(ushort.Parse(flags["tag"])));
                }
            }
            catch (FormatException)
            {
                throw new CommandSyntaxException("Unable to parse the tag condition.");
            }

            //Load ID rules
            try
            {
                if (flags.AllKeys.Contains("id"))
                {
                    group.Add(new IDRule(uint.Parse(flags["id"])));
                }
            }
            catch (FormatException)
            {
                throw new CommandSyntaxException("Unable to parse the ID condition.");
            }

            //Load IP rules
            try
            {
                if (flags.AllKeys.Contains("ip"))
                {
                    group.Add(new IPRule(IPAddress.Parse(flags["ip"])));
                }
            }
            catch (FormatException)
            {
                throw new CommandSyntaxException("Unable to parse the IP condition.");
            }

            return group;
        }
    }
}
