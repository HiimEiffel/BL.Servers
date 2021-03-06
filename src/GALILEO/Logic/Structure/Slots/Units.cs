using System;
using System.Collections.Generic;
using BL.Servers.CoC.Logic.Structure.Slots.Items;

namespace BL.Servers.CoC.Logic.Structure.Slots
{
    internal class Units : List<Slot>, ICloneable
    {
        internal Player Player;

        internal Units()
        {
            // Units.
        }

        internal Units(Player _Player)
        {
            this.Player = _Player;
        }

        internal Units Clone()
        {
            return this.MemberwiseClone() as Units;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}