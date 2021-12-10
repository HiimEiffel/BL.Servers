using BL.Servers.CoC.Logic;

namespace BL.Servers.CoC.Packets.Messages.Server.Errors
{
    internal class Out_Of_Sync : Message
    {
        public Out_Of_Sync(Device Device) : base(Device)
        {
            this.Identifier = 24104;
        }
    }
}
