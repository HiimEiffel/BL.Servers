using BL.Servers.CoC.Extensions;
using BL.Servers.CoC.Extensions.Binary;
using BL.Servers.CoC.Logic;
using BL.Servers.CoC.Logic.Enums;
using BL.Servers.CoC.Logic.Structure.Slots.Items;

namespace BL.Servers.CoC.Packets.Commands.Client.Battle
{
    internal class Place_Alliance_Attacker : Command
    {
        internal int GlobalId;
        internal int X;
        internal int Y;
        internal int Tick;
        public Place_Alliance_Attacker(Reader reader, Device client, int id) : base(reader, client, id)
        {
        }

        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();
            this.GlobalId = this.Reader.ReadInt32();
            this.Tick = this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            if (this.Device.State == State.IN_PC_BATTLE)
            {
                System.Console.WriteLine(GlobalId);
                var Battle = Core.Resources.Battles.Get(this.Device.Player.Avatar.Battle_ID, Constants.Database);
                Battle_Command Command = new Battle_Command { Command_Type = this.Identifier, Command_Base = new Command_Base { Base = new Base { Tick = this.Tick }, Data = this.GlobalId, X = this.X, Y = this.Y } };
                Battle.Add_Command(Command);
                Battle.Replay_Info.Stats.Alliance_Used = true;
                Battle.Attacker.Castle_Units = this.Device.Player.Avatar.Castle_Units.Clone();
            }
            this.Device.Player.Avatar.Castle_Units.Clear();
            this.Device.Player.Avatar.Castle_Used = 0;
        }
    }
}
