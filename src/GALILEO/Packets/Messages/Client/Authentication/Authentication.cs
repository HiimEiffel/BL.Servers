using System;
using System.Linq;
using System.Text;
using BL.Servers.CoC.Core;
using BL.Servers.CoC.Core.Networking;
using BL.Servers.CoC.Extensions;
using BL.Servers.CoC.Extensions.Binary;
using BL.Servers.CoC.External.Blake2B;
using BL.Servers.CoC.External.Sodium;
using BL.Servers.CoC.Files;
using BL.Servers.CoC.Logic;
using BL.Servers.CoC.Logic.Enums;
using BL.Servers.CoC.Packets.Messages.Server.Authentication;
using BL.Servers.CoC.Packets.Messages.Server;
using BL.Servers.CoC.Packets.Messages.Server.Clans;
using BL.Servers.CoC.Packets.Messages.Server.Clans.War;
using BL.Servers.CoC.Packets.Cryptography;

namespace BL.Servers.CoC.Packets.Messages.Client.Authentication
{
    internal class Authentification : Message
    {
        public Authentification(Device device, Reader reader) : base(device, reader)
        {
            this.Device.State = State.LOGIN;
        }

        internal StringBuilder Reason;
        internal long UserId;

        internal string Token, MasterHash, Language, UDID;

        internal int Major, Minor, Revision;

        internal string[] ClientVersion;


        internal override void DecryptPepper()
        {
            byte[] Buffer = this.Reader.ReadBytes(this.Length);
            Console.WriteLine($"Raw {BitConverter.ToString(Buffer.ToArray()).Replace("-", "")}");
            Console.WriteLine($"Buffer Lenght {Buffer.ToArray().Length}");
            this.Device.Keys.PublicKey = Buffer.Take(32).ToArray();

            Console.WriteLine($"Public Key {BitConverter.ToString(this.Device.Keys.PublicKey).Replace("-", "")}");

            Blake2BHasher Blake = new Blake2BHasher();

            Blake.Update(this.Device.Keys.PublicKey);
            Blake.Update(Key.PublicKey);

            this.Device.Keys.RNonce = Blake.Finish();
            
            Buffer = Sodium.Decrypt(Buffer.Skip(68).ToArray(), this.Device.Keys.RNonce, Key.PrivateKey, this.Device.Keys.PublicKey);

            this.Device.Keys.SNonce = Buffer.Skip(24).Take(24).ToArray();
            this.Reader = new Reader(Buffer.Skip(48).ToArray());

            Console.WriteLine($"Decrypted {BitConverter.ToString(Buffer.Skip(48).ToArray()).Replace("-", "")}");
            this.Length = (ushort)Buffer.Length;
            Console.WriteLine(this.Length);

        }

        internal override void Decode()
        { 
            this.UserId = this.Reader.ReadInt64();

            this.Token = this.Reader.ReadString();

            this.Major = this.Reader.ReadInt32();
            this.Minor = this.Reader.ReadInt32();
            this.Revision = this.Reader.ReadInt32();

            this.MasterHash = this.Reader.ReadString();

            this.Reader.ReadString();

            this.Device.AndroidID = this.Reader.ReadString();
            this.Device.MACAddress = this.Reader.ReadString();
            this.Device.Model = this.Reader.ReadString();

            this.Reader.Seek(4);    // 2000001

            this.Language = this.Reader.ReadString();
            this.Device.OpenUDID = this.Reader.ReadString();
            this.Device.OSVersion = this.Reader.ReadString();

            this.Device.Android = this.Reader.ReadBoolean();

            this.Reader.ReadString();

            this.Device.AndroidID = this.Reader.ReadString();

            this.Reader.ReadString();

            this.Device.Advertising = this.Reader.ReadBoolean();

            this.Reader.ReadString();

            this.Device.ClientSeed = this.Reader.ReadUInt32();

            this.Reader.ReadByte();
            this.Reader.ReadString();
            this.Reader.ReadString();

            this.ClientVersion = this.Reader.ReadString().Split('.');
        }

        internal override void Process()
        {
            if (Constants.RC4)
                new Session_Key(this.Device).Send();
            if (string.Equals(this.MasterHash, Fingerprint.Sha))
            {
                if (this.UserId == 0)
                {
                    this.Device.Player = Resources.Players.New(0, Constants.Database);

                    if (this.Device.Player != null)
                    {
                        if (this.Device.Player.Avatar.Locked)
                        {
                            new Authentication_Failed(this.Device, Logic.Enums.Reason.Locked).Send();
                        }
                        else
                        {
                            this.Login();
                        }
                    }
                    else
                    {
                        new Authentication_Failed(this.Device, Logic.Enums.Reason.Pause).Send();
                    }
                }
                else if (this.UserId > 0)
                {
                    this.Device.Player = Resources.Players.Get(this.UserId, Constants.Database);
                    if (this.Device.Player != null)
                    {
                        if (string.Equals(this.Token, this.Device.Player.Avatar.Token))
                        {
                            if (this.Device.Player.Avatar.Locked)
                            {
                                new Authentication_Failed(this.Device, Logic.Enums.Reason.Locked).Send();
                            }
                            else if (this.Device.Player.Avatar.Banned)
                            {
                                this.Reason = new StringBuilder();
                                this.Reason.AppendLine(
                                    "Your account has been banned from our servers, please contact one of the server staff members with these following information:");
                                this.Reason.AppendLine();
                                this.Reason.AppendLine("Your Account Name: " + this.Device.Player.Avatar.Name);
                                this.Reason.AppendLine("Your Account ID: " + this.Device.Player.Avatar.UserId);
                                this.Reason.AppendLine("Your Account Tag: " +
                                                       GameUtils.GetHashtag(this.Device.Player.Avatar.UserId));
                                this.Reason.AppendLine("Your Account Ban Duration: " +
                                                       Math.Round(
                                                           (this.Device.Player.Avatar.BanTime.AddDays(3) -
                                                            DateTime.UtcNow).TotalDays, 3) + " Day");
                                this.Reason.AppendLine("Your Account Unlock Date : " +
                                                       this.Device.Player.Avatar.BanTime.AddDays(3));
                                this.Reason.AppendLine();

                                new Authentication_Failed(this.Device, Logic.Enums.Reason.Banned)
                                {
                                    Message = Reason.ToString()
                                }.Send();
                            }
                            else
                            {
                                this.Login();
                            }
                        }
                        else
                        {
                            new Authentication_Failed(this.Device, Logic.Enums.Reason.Locked).Send();
                        }
                    }
                    else
                    {
                        this.Reason = new StringBuilder();
                        this.Reason.AppendLine(
                            "Your device have been block from accessing our servers due to invalid id, please clear your game data or contact one of the staff members with these following information if you are not able to clear you game data:");
                        this.Reason.AppendLine("Your Device IP: " + this.Device.IPAddress + ".");
                        this.Reason.AppendLine("Your Requested ID: " + this.UserId + ".");
                        this.Reason.AppendLine();

                        new Authentication_Failed(this.Device, (Reason) 443)
                        {
                            Message = Reason.ToString()
                        }.Send();
                    }
                }
            }
            else
            {
                new Authentication_Failed(this.Device, Logic.Enums.Reason.Patch).Send();
            }
        }

        internal void Login()
        {
            this.Device.Player.Client = this.Device;
            this.Device.Player.Avatar.Region = Resources.Region.GetIpCountry(this.Device.Player.Avatar.IpAddress = this.Device.IPAddress);

            Resources.GChat.Add(this.Device);
            Resources.PRegion.Add(this.Device.Player);

            //new Authentication_Failed(this.Device,(Reason)19).Send();
            new Authentication_OK(this.Device).Send();
            new Own_Home_Data(this.Device).Send();

            new Server.Avatar_Stream(this.Device).Send();

            if (this.Device.Player.Avatar.ClanId > 0)
            {
                Clan Alliance = Resources.Clans.Get(this.Device.Player.Avatar.ClanId, Constants.Database);

                if (Alliance != null)
                {
                    this.Device.Player.Avatar.Alliance_Level = Alliance.Level;
                    new Alliance_Full_Entry(this.Device) {Clan = Alliance}.Send();
                    new War_Map(this.Device).Send();

                    if (Alliance.Chats != null)
                    {
                        new Alliance_All_Stream_Entry(this.Device, Alliance).Send();
                    }
                }
                else
                {
                    this.Device.Player.Avatar.ClanId = 0;
                }
            }
        }
    }
}
