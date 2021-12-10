﻿using System;
using BL.Servers.CoC.Core.Networking;

namespace BL.Servers.CoC.Core
{
    internal class Resources
    {
        internal static Devices Devices;
        internal static Players Players;
        internal static Clans Clans;
        internal static Battles Battles;
        internal static Random Random;
        internal static Classes Classes;
        internal static Global_Chat GChat;
        internal static Region Region;
        internal static Player_Region PRegion;
        internal static Gateway Gateway;

        internal static void Initialize()
        {
            Resources.Classes = new Classes();
            Resources.Devices = new Devices();
            Resources.Players = new Players();
            Resources.Clans = new Clans();
            Resources.Battles = new Battles();
            Resources.Random = new Random(DateTime.Now.ToString().GetHashCode());
            Resources.GChat = new Global_Chat();
            Resources.Region = new Region();
            Resources.PRegion = new Player_Region();
            Resources.Gateway = new Gateway();
        }
    }
}
