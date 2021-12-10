using BL.Servers.CoC.Logic;

namespace BL.Servers.CoC.Packets.Messages.Server.API
{
    internal class Facebook_Connect_OK : Message
    {
        public Facebook_Connect_OK(Device Device) : base(Device)
        {
            this.Identifier = 24201;
        }
        internal override void Encode()
        {
            this.Data.Add(1);
        }
    }
}
