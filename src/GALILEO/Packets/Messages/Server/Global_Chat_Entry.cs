﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Servers.CoC.Core;
using BL.Servers.CoC.Core.Database;
using BL.Servers.CoC.Extensions;
using BL.Servers.CoC.Extensions.List;
using BL.Servers.CoC.Logic;

namespace BL.Servers.CoC.Packets.Messages.Server
{
    internal class Global_Chat_Entry : Message
    {
        internal string Message = string.Empty;
        internal Logic.Player Message_Sender = null;
        internal bool Sender = false;

        public Global_Chat_Entry(Device Device) : base(Device)
        {
            this.Identifier = 24715;
        }

        internal override void Encode()
        {
            this.Data.AddString(this.Message);
            this.Data.AddString(Sender ? "You" : this.Message_Sender.Name);
            this.Data.AddInt(0); // Unknown
            this.Data.AddInt(this.Message_Sender.League);

            this.Data.AddLong(this.Message_Sender.UserId);
            this.Data.AddLong(this.Message_Sender.UserId);

            this.Data.AddBool(this.Message_Sender.ClanId > 0);
            if (this.Message_Sender.ClanId > 0)
            {
                Logic.Clan _Clan = Resources.Clans.Get(this.Message_Sender.ClanId, Constants.Database, false);

                this.Data.AddLong(_Clan.Clan_ID);
                this.Data.AddString(_Clan.Name);
                this.Data.AddInt(_Clan.Badge);
            }
        }
    }
}
