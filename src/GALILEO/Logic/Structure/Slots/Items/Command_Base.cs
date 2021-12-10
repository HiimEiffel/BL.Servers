﻿using Newtonsoft.Json;

namespace BL.Servers.CoC.Logic.Structure.Slots.Items
{
    internal class Command_Base
    {
        [JsonProperty("base", DefaultValueHandling = DefaultValueHandling.Include)] internal Base Base = new Base();

        [JsonProperty("d")] internal int Data;

        [JsonProperty("x")] internal int X;

        [JsonProperty("y")] internal int Y;
    }
}
