﻿using BL.Servers.CoC.Logic;
using BL.Servers.CoC.Logic.Structure.Slots.Items;

namespace BL.Servers.CoC.Packets.Messages.Server
{
    internal class Avatar_Stream_Entry : Message
    {
        internal Mail Message = null;

        internal Avatar_Stream_Entry(Device _Device, Mail message) : base(_Device)
        {
            this.Identifier = 24412;
            this.Message = message;
        }

        internal override void Encode()
        {
            this.Data.AddRange(this.Message.ToBytes);
        }
    }
}
