﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Servers.CoC.Core.Networking;
using BL.Servers.CoC.Extensions;
using BL.Servers.CoC.Extensions.List;
using BL.Servers.CoC.Logic.Structure.Slots.Items;
using BL.Servers.CoC.Packets.Messages.Server;
using Newtonsoft.Json;

namespace BL.Servers.CoC.Logic.Structure.Slots
{
    internal class Inbox 
    {
        internal Player Player;
        [JsonProperty("seed")] internal int Seed;
        [JsonProperty("slots")] internal List<Mail> Slots;

        internal object Gate = new object();

        internal Inbox()
        {
            this.Slots = new List<Mail>(100);
        }

        internal Inbox(Player Player, int Limit = 100)
        {
            this.Player = Player;
            this.Slots = new List<Mail>(Limit);
        }

        internal void Add(Mail Message)
        {
            lock (this.Gate)
            {
                Message.Message_LowID = Seed++;

                if (this.Slots.Count < this.Slots.Capacity)
                {
                    this.Slots.Add(Message);
                }
                else
                {
                    this.Slots.RemoveAt(0);
                    this.Slots.Add(Message);
                }
            }

            var Avatar = Core.Resources.Players.Get(Player.UserId, Constants.Database, false);
            if (Avatar?.Client != null)
            {
                new Avatar_Stream_Entry(Avatar.Client, Message).Send();
            }
        }

        internal byte[] ToBytes
        {
            get
            {
                List<byte> Packet = new List<byte>();

                Packet.AddInt(this.Slots.Count);

                foreach (Mail Message in this.Slots)
                {
                    Packet.AddRange(Message.ToBytes);
                }

                return Packet.ToArray();
            }
        }

        internal void Update()
        {
            foreach (Mail Entry in this.Slots)
            {
                /* if (Entry.Outdated)
                {
                    this.Remove(Entry);
                } */
            }
        }

    }
}
