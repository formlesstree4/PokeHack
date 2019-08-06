using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PokeHack {
    [Serializable()]
	public class Pokemon {

		internal static List<BaseStat> basestats;
		internal static Dictionary<int, string> movenames;
		internal static Dictionary<ushort, char> chartable;
        internal static Dictionary<int, string> items;
		internal static Dictionary<int, string> abilities;
        internal static Dictionary<int, string> growth;
		private static bool hasLoaded = false;

		public static void LoadDataFiles() {
			basestats = new List<BaseStat>();
			movenames = new Dictionary<int, string>();
			chartable = new Dictionary<ushort, char>();
            items = new Dictionary<int, string>();
			abilities = new Dictionary<int, string>();
            growth = new Dictionary<int, string>();
            Exp.LoadDataFiles();
			string[] strSplit = new string[1];
			strSplit[0] = "\r\n";

            
			//load moves first
			string[] moves = Resources.moves.Split(strSplit, StringSplitOptions.RemoveEmptyEntries);
			for(int i = 0; i < moves.Length; i++) {
				movenames.Add(i + 1, moves[i]);
			}

            //load items
			string[] item = Resources.items.Split(strSplit, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < item.Length; i++){
                char[] charstotrim = { ' ' };
                items.Add(i + 1, item[i].TrimEnd(charstotrim));
			}

            //load growth
            string[] growthrate = Resources.exptable.Split(strSplit, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < growthrate.Length; i++)
            {
                growth.Add(i + 1, growthrate[i]);
            }

			//load abilities
			//inconsistent file endings is a bit of a no-no, but I don't really care, since it works.
			string[] ability = Resources.abilities.Split(strSplit, StringSplitOptions.RemoveEmptyEntries);
			for(int i = 0; i < ability.Length; i++) {
				string[] spl = ability[i].Split(',');
				abilities.Add(int.Parse(spl[0]), spl[1]);
			}

			string[] pokedata = Resources.pokedata.Split(strSplit, StringSplitOptions.RemoveEmptyEntries);
			for(int i = 0; i < pokedata.Length; i++) {
				string[] data = pokedata[i].Split(',');
				BaseStat bs = new BaseStat() {
					ID = int.Parse(data[0]),
					Name = data[1],
					BaseHP = byte.Parse(data[2]),
					BaseAttack = byte.Parse(data[3]),
					BaseDefense = byte.Parse(data[4]),
					BaseSpecialAttack = byte.Parse(data[5]),
					BaseSpecialDefense = byte.Parse(data[6]),
					BaseSpeed = byte.Parse(data[7]),
					LevelingType = (LevelUpType)int.Parse(data[8]),
					FormID = byte.Parse(data[9]),

					Type1 = PokeType.None,
					Type2 = PokeType.None,
					CatchRate = 0,
					EffortYield = 0,
					GenderValue = byte.Parse(data[10]),
					ExpYield = 0
				};
				basestats.Add(bs);
			}

			//    just data dump code, leave it in
			//FileStream str = File.Create("pokedata.txt");
			//StreamWriter w = new StreamWriter(str);
			//foreach(BaseStat s in basestats) {
			//    w.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", s.ID, s.Name, s.BaseHP, s.BaseAttack, s.BaseDefense, s.BaseSpecialAttack, s.BaseSpecialDefense, s.BaseSpeed, (int)s.LevelingType, s.FormID, s.GenderValue);
			//}
			//w.Flush();

			#region old pokedata reader
			//string[] pokenames = File.ReadAllLines(directory + "pokenames.txt");
			//string[] stats = File.ReadAllLines(directory + "basestats.txt");
			//string[] levelup = File.ReadAllLines(directory + "levelup.txt");
			//int pokeid = 1;
			//int pni = 0;
			//int si = 0;
			//int lui = 0;

			//for(; pokeid <= 493;) {
			//        string[] stat = stats[si].Split(',');
			//        byte[] intstat = new byte[stat.Length];
			//        for(int i = 0; i < intstat.Length; i++) {
			//            intstat[i] = byte.Parse(stat[i]);
			//        }

			//        if(intstat.Length < 6) {
			//            Console.WriteLine("Error on line " + (si + 1));
			//        }

			//        BaseStat bs = new BaseStat() {
			//            ID = pokeid,
			//            Name = pokenames[pni].Substring(0,pokenames[pni].Length - 1),
			//            BaseHP = intstat[0],
			//            BaseAttack = intstat[1],
			//            BaseDefense = intstat[2],
			//            BaseSpecialAttack = intstat[3],
			//            BaseSpecialDefense = intstat[4],
			//            BaseSpeed = intstat[5],
			//            LevelingType = (LevelUpType)byte.Parse(levelup[lui]),
			//            FormID = 0,
			//            HasAlternate = false,

			//            Type1 = PokeType.None, //gonna change this stuff later I suppose
			//            Type2 = PokeType.None,
			//            Item1 = 0,
			//            Item2 = 0,
			//            GenderValue = 0,
			//            ExpYield = 0,
			//            EffortYield = 0,
			//            CatchRate = 0,
			//        };
			//        if(intstat.Length == 7) {
			//            bs.FormID = intstat[6];
			//            bs.HasAlternate = true;
			//        }

			//        basestats.Add(bs);

			//        if(intstat.Length == 6 || (si + 1 < stats.Length && stats[si + 1].Split(',').Length == 6)) {
			//            pokeid++;
			//            pni++;
			//            si++;
			//            lui++;
			//        }
			//        else if(intstat.Length == 7) {
			//            if (si == 428){
			//                pokeid++;
			//                pni++;
			//                lui++;
			//            }
			//            si++;
			//        }

			//}
			#endregion

			//character table
			chartable.Add(0, (char)0);
			string[] chartbl = Resources.chartable.Split(strSplit, StringSplitOptions.RemoveEmptyEntries);
			for(int i = 0; i < chartbl.Length; i++) {
				ushort pokechar = ushort.Parse(chartbl[i].Split('=')[0], System.Globalization.NumberStyles.HexNumber);
				char unicode = (char)int.Parse(chartbl[i].Split('=')[1], System.Globalization.NumberStyles.HexNumber);
				chartable.Add(pokechar, unicode);
			}

			hasLoaded = true;
		}

		public Pokemon(byte[] data) {
			if(!hasLoaded) {
				LoadDataFiles();
			}
			rawdata = new ManipulatableByteArray(data);
		}

        public Pokemon()
        {
        }



		//private string pokename;

		protected ManipulatableByteArray rawdata;
		public byte[] GetRawData() {
			return rawdata.GetArray(0, rawdata.Length);
		}


		//private uint rawperson;

		public uint Personality {
			get { return rawdata.GetUInt(0x00); }
            set { rawdata.SetUInt(0x00, value); }
		}

		public ushort Checksum {
			get { return rawdata.GetUShort(0x06); }
			set { rawdata.SetUShort(0x06, value); }
		}
        public ushort GenerateChecksum {
            get {
                int temp = 0;
                for (int i = 0x8; i < 0x87; i += 2) {
                    temp = temp + rawdata.GetUShort(i);
                }
                byte[] tempbyte = BitConverter.GetBytes(temp);
                ushort s = 0;
                s |= tempbyte[1];
                s <<= 8;
                s |= tempbyte[0];
                return s;
            }
        }
		public Gender Gender {
			get {

				byte b = rawdata.GetByte(0x40);
				if((((b >> 2) & 0x1) != 0)) {
					return Gender.Genderless;
				}
				else {
					if((((b >> 1) & 0x1) != 0)) {
						return Gender.Female;
					}
					else {
						return Gender.Male;
					}
				}

				//uint genval = basestats[pokename].GenderValue;
				//uint pergen = (uint)(rawperson & 0xFF);

				//if(pergen >= genval) {
				//    return Gender.Male;
				//}
				//else {
				//    return Gender.Female;
				//}
			}
			set {
				byte genderval = GetBaseStats(Species, AlternateForms).GenderValue;
				if(genderval == 255 && value != Gender.Genderless) {
					throw new ArgumentException("Value must be Genderless for genderless Pokemon", "value");
				}
				if(genderval == 254 && value != Gender.Female) {
					throw new ArgumentException("Value must be Female for all-female Pokemon", "value");
				}
				if(genderval == 0 && value != Gender.Male) {
					throw new ArgumentException("Value must be Male for all-male Pokemon", "value");
				}
				// TODO must be fixed! There is no way to know if the ability is the first or the second!!!!
				uint newpid = PIDGenerator.GeneratePID(Species, Nature, value, IsShiny, false, OriginalTrainer, SecretID);
				Personality = newpid;
				byte genbyte = rawdata.GetByte(0x40);
				genbyte &= 0x9F;
				if(value == Gender.Genderless) {
					genbyte |= 0x20;
				}
				else if(value == Gender.Male) {
					genbyte |= 0x0;
				}
				else if(value == Gender.Female) {
					genbyte |= 0x40;
				}
				rawdata.SetByte(0x40, genbyte);
			}
		}

		public Nature Nature {
			get { return (Nature)(rawdata.GetByte(0x41)); }
			set { rawdata.SetByte(0x41,(byte)value);}
		}

        public Nature Gen4Nature
        {
            get { return (Nature)(Personality % 25); }
        }

		public ushort Species {
			get { return rawdata.GetUShort(0x08); }
			set {
				rawdata.SetUShort(0x08, value);
			}
		}

		public string Name {
			get { return GetBaseStats(Species, AlternateForms).Name; }
		}
        
		public ushort HeldItem {
			get { return rawdata.GetUShort(0x0A); }
			set { rawdata.SetUShort(0x0A, value); }
		}

        public string HeldItemStr {
            get { return ConvertItemIDToString(HeldItem); }
			set {
				rawdata.SetUShort(0x0A, ConvertStringToItemID(value));
			}
        }

        public static string ConvertItemIDToString(int id)
        {
            if (id != 0)
            {
                return items[id];
            }
            else
            {
                return "";
            }
        }
		public static ushort ConvertStringToItemID(string item) {
			if(items.ContainsValue(item)) {
				for(int i = 0; i < items.Count; i++) {
					string[] arr = items.Values.ToArray();
					if(arr[i] == item) {
						return (ushort)(i + 1);
					}
				}
			}
			return 0;
		}
		public static ushort ConvertStringToMoveID(string move) {
            if (movenames.ContainsValue(move))
            {
                for (int i = 0; i < movenames.Count; i++)
                {
                    string[] arr = movenames.Values.ToArray();
                    if (arr[i] == move)
                    {
                        return (ushort)(i + 1);
                    }
                }
            }
            return 0;
        }
		public static string ConvertAbilityIDToString(int id) {
			if(id != 0) {
				return abilities[id];
			}
			else {
				return "";
			}
		}
         public static byte ConvertStringToAbilityID(string ability)
        {
            for (int i = 0; i < abilities.Count; i++) {
                string[] arr = abilities.Values.ToArray();
                if (arr[i] == ability) { return (byte)(i + 1); }
            }
            return 0;
         }
		public ushort OriginalTrainer {
			get { return rawdata.GetUShort(0x0C); }
			set {
				// TODO must be fixed! There is no way to know if the ability is the first or the second!!!!
				uint newpid = PIDGenerator.GeneratePID(Species, Nature, Gender, IsShiny, false, value, SecretID);
				Personality = newpid;
				rawdata.SetUShort(0x0C, value);
			}
		}

		public string OTName {
			get {
				byte[] ba = rawdata.GetArray(0x68, 16);
				return GetPokemonString(ba);
			}
		}

        public string Gen4OTName
        {
            get
            {
                byte[] ba = rawdata.GetArray(0x68, 16);
                return GetGen4PokemonString(ba);
            }
        }

		public string Nickname {
			get {
				byte[] ba = rawdata.GetArray(0x48, 22);
				return GetPokemonString(ba);
			}
		}
        public string Gen4Nickname
        {
            get
            {
                byte[] ba = rawdata.GetArray(0x48, 22);
                return GetGen4PokemonString(ba);
            }
        }
		public ushort SecretID {
			get { return rawdata.GetUShort(0x0E); }
			set {
				// TODO must be fixed! There is no way to know if the ability is the first or the second!!!!
				uint newpid = PIDGenerator.GeneratePID(Species, Nature, Gender, IsShiny, false, OriginalTrainer, value);
				Personality = newpid;
				rawdata.SetUShort(0x0E, value);
			}
		}

		public bool IsShiny {
			get {
				ushort p1 = rawdata.GetUShort(0x00);
				ushort p2 = rawdata.GetUShort(0x02);
				ushort tid = OriginalTrainer;
				ushort sid = SecretID;

				ushort E = (ushort)(tid ^ sid);
				ushort F = (ushort)(p1 ^ p2);
				if((E ^ F) < 8) {
					return true;
				}

				return false;
			}
			set {
				// TODO must be fixed! There is no way to know if the ability is the first or the second!!!!
				uint newpid = PIDGenerator.GeneratePID(Species, Nature, Gender, value, false, OriginalTrainer, SecretID);
				Personality = newpid;
			}
		}
		public bool IsFatefulEncounter {
			get {
				byte b = rawdata.GetByte(0x40);
				if(((b >> 0) & 0x1) != 0) {
					return true;
				}
				return false;
			}
			set {
				byte b = rawdata.GetByte(0x40);
				b &= 0xFE;
				if(value)
					b |= 0x1;
				rawdata.SetByte(0x40, b);
			}
		}
		public bool IsEgg {
			get {
				uint i = rawdata.GetUInt(0x38);
				i >>= 30;
				if((i & 0x1) != 0) {
					return true;
				}
				return false;
			}
			set {
				uint i = rawdata.GetUInt(0x38);
				i &= 0xBFFFFFFF;
				if(value) {
					i |= 0x40000000;
				}
				rawdata.SetUInt(0x38, i);
			}
		}
		public bool IsNicknamed {
			get {
				uint i = rawdata.GetUInt(0x38);
				i >>= 31;
				if((i & 0x1) != 0) {
					return true;
				}
				return false;
			}
		}

		public uint Experience {
			get {
				return rawdata.GetUInt(0x10);
			}
			set {
				rawdata.SetUInt(0x10,value);
			}
		}

		public uint Level {
			get 
            {
                uint level = 0;
                for (uint i = 0; i < 100; i++)
                {
                    if (Experience >= Exp.exparray[(int)GetBaseStats((int)Species, AlternateForms).LevelingType, i])
                    {
                        level = i + 1;
                    }
                }
                return level; 
            }
			set 
            { 
                Experience = (uint)Exp.exparray[(int)GetBaseStats((int)Species, AlternateForms).LevelingType, value - 1]; 
            }
		}

		public byte HappinessEggSteps {
			get { return rawdata.GetByte(0x14); }
			set {
				rawdata.SetByte(0x14, value);
			}
		}

		public byte Ability {
			get { return rawdata.GetByte(0x15); }
			set { rawdata.SetByte(0x15, value); }
		}
		public string AbilityStr {
			get {
				return ConvertAbilityIDToString(Ability);
			}
		}

		public Marking Markings {
			get { return (Marking)rawdata.GetByte(0x16); }
			//set;
		}

		public OriginalLanguage Origin {
			get { return (OriginalLanguage)rawdata.GetByte(0x17); }
			//set;
		}

		public ushort AlternateForms {
			get {
				ushort b = rawdata.GetByte(0x40);
				b &= 0xFFF8;
				return b;
			}
			set {
				ushort b = rawdata.GetByte(0x40);
				b &= 0xFFF8;
				b |= value;
				rawdata.SetByte(0x40, (byte)b);
			}
		}

		public ushort HPIV {
			get {
				uint i = rawdata.GetUInt(0x38);
				i >>= 0;
				i &= 0x1F;
				return (ushort)i;
			}
            set
            {
                uint i = rawdata.GetUInt(0x38);
				i &= 0xFFFFFFE0;
				uint val = (uint)(value << 0);
				i |= val;
				rawdata.SetUInt(0x38, i);
				//uint input = value;
				//uint temp = i;
				//input &= 0x1F;
				//temp >>= 0;
				//temp &= 0x1F;
				//input <<= 0;
				//temp <<= 0;
				//i ^= temp;
				//i |= input;
				//rawdata.SetUInt(0x38, i);
            }
		}
		public ushort AttIV {
			get {
				uint i = rawdata.GetUInt(0x38);
				i >>= 5;
				i &= 0x1F;
				return (ushort)i;
			}
            set 
            {
				uint i = rawdata.GetUInt(0x38);
				i &= 0xFFFFFC1F;
				uint val = (uint)(value << 5);
				i |= val;
				rawdata.SetUInt(0x38, i);
				//uint i = rawdata.GetUInt(0x38);
				//uint input = value;
				//uint temp = i;
				//input &= 0x1F;
				//temp >>= 5;
				//temp &= 0x1F;
				//input <<= 5;
				//temp <<= 5;
				//i ^= temp;
				//i |= input;
				//rawdata.SetUInt(0x38, i);
            }
		}
		public ushort DefIV {
			get {
				uint i = rawdata.GetUInt(0x38);
				i >>= 10;
				i &= 0x1F;
				return (ushort)i;
			}
            set
            {
				uint i = rawdata.GetUInt(0x38);
				i &= 0xFFFF83FF;
				uint val = (uint)(value << 10);
				i |= val;
				rawdata.SetUInt(0x38, i);
				//uint i = rawdata.GetUInt(0x38);
				//uint input = value;
				//uint temp = i;
				//input &= 0x1F;
				//temp >>= 10;
				//temp &= 0x1F;
				//input <<= 10;
				//temp <<= 10;
				//i ^= temp;
				//i |= input;
				//rawdata.SetUInt(0x38, i);
            }
		}
		public ushort SpeIV {
			get {
				uint i = rawdata.GetUInt(0x38);
				i >>= 15;
				i &= 0x1F;
				return (ushort)i;
			}
            set
            {
				uint i = rawdata.GetUInt(0x38);
				i &= 0xFFF07FFF;
				uint val = (uint)(value << 15);
				i |= val;
				rawdata.SetUInt(0x38, i);
				//uint i = rawdata.GetUInt(0x38);
				//uint input = value;
				//uint temp = i;
				//input &= 0x1F;
				//temp >>= 15;
				//temp &= 0x1F;
				//input <<= 15;
				//temp <<= 15;
				//i ^= temp;
				//i |= input;
				//rawdata.SetUInt(0x38, i);
            }
		}
		public ushort SPAttIV {
			get {
				uint i = rawdata.GetUInt(0x38);
				i >>= 20;
				i &= 0x1F;
				return (ushort)i;
			}
            set
            {
				uint i = rawdata.GetUInt(0x38);
				i &= 0xFE0FFFFF;
				uint val = (uint)(value << 20);
				i |= val;
				rawdata.SetUInt(0x38, i);
				//uint i = rawdata.GetUInt(0x38);
				//uint input = value;
				//uint temp = i;
				//input &= 0x1F;
				//temp >>= 20;
				//temp &= 0x1F;
				//input <<= 20;
				//temp <<= 20;
				//i ^= temp;
				//i |= input;
				//rawdata.SetUInt(0x38, i);
            }
		}
		public ushort SPDefIV {
			get {
				uint i = rawdata.GetUInt(0x38);
				i >>= 25;
				i &= 0x1F;
				return (ushort)i;
			}
            set
            {
				uint i = rawdata.GetUInt(0x38);
				i &= 0xC1FFFFFF;
				uint val = (uint)(value << 25);
				i |= val;
				rawdata.SetUInt(0x38, i);
				//uint i = rawdata.GetUInt(0x38);
				//uint input = value;
				//uint temp = i;
				//input &= 0x1F;
				//temp >>= 25;
				//temp &= 0x1F;
				//input <<= 25;
				//temp <<= 25;
				//i ^= temp;
				//i |= input;
				//rawdata.SetUInt(0x38, i);
            }
		}

		public byte HPEV {
			get { return rawdata.GetByte(0x18); }
            set { rawdata.SetByte(0x18, value); }
		}
		public byte AttEV {
			get { return rawdata.GetByte(0x19); }
            set { rawdata.SetByte(0x19, value); }
		}
		public byte DefEV {
			get { return rawdata.GetByte(0x1A); }
            set { rawdata.SetByte(0x1A, value); }
		}
		public byte SpeEV {
			get { return rawdata.GetByte(0x1B); }
            set { rawdata.SetByte(0x1B, value); }

		}
		public byte SPAttEV {
			get { return rawdata.GetByte(0x1C); }
            set { rawdata.SetByte(0x1C, value); }
		}
		public byte SPDefEV {
			get { return rawdata.GetByte(0x1D); }
            set { rawdata.SetByte(0x1D, value); }
		}

		public int HP {
			get {
				if(Species == 292)
					return 1;
				double bas = GetBaseStats(Species, AlternateForms).BaseHP;
				double iv = HPIV;
				double ev = HPEV;
				double val = (((iv + (2 * bas) + (ev / 4) + 100) * Level) / 100) + 10;
				return (int)val;
			}
		}
		public int Attack {
			get {
				double bas = GetBaseStats(Species, AlternateForms).BaseAttack;
				double iv = AttIV;
				double ev = AttEV;
				double val = (((iv + (2 * bas) + (ev / 4)) * Level) / 100) + 5;
				val = (int)val;

				switch(Nature) {
					case Nature.Lonely:
					case Nature.Brave:
					case Nature.Adamant:
					case Nature.Naughty:
						val += (val * 0.1);
						break;
					case Nature.Bold:
					case Nature.Timid:
					case Nature.Modest:
					case Nature.Calm:
						val -= (val * 0.1);
						break;
				}
				return (int)val;
			}
		}
		public int Defense {
			get {
				double bas = GetBaseStats(Species, AlternateForms).BaseDefense;
				double iv = DefIV;
				double ev = DefEV;
				double val = (((iv + (2 * bas) + (ev / 4)) * Level) / 100) + 5;
				val = (int)val;

				switch(Nature) {
					case Nature.Bold:
					case Nature.Relaxed:
					case Nature.Impish:
					case Nature.Lax:
						val += (val * 0.1);
						break;
					case Nature.Lonely:
					case Nature.Hasty:
					case Nature.Mild:
					case Nature.Gentle:
						val -= (val * 0.1);
						break;
				}
				return (int)val;
			}
		}
		public int Speed {
			get {
				double bas = GetBaseStats(Species, AlternateForms).BaseSpeed;
				double iv = SpeIV;
				double ev = SpeEV;
				double val = (((iv + (2 * bas) + (ev / 4)) * Level) / 100) + 5;
				val = (int)val;

				switch(Nature) {
					case Nature.Timid:
					case Nature.Hasty:
					case Nature.Jolly:
					case Nature.Naive:
						val += (val * 0.1);
						break;
					case Nature.Brave:
					case Nature.Relaxed:
					case Nature.Quiet:
					case Nature.Sassy:
						val -= (val * 0.1);
						break;
				}
				return (int)val;
			}
		}
		public int SpecialAttack {
			get {
				double bas = GetBaseStats(Species, AlternateForms).BaseSpecialAttack;
				double iv = SPAttIV;
				double ev = SPAttEV;
				double val = (((iv + (2 * bas) + (ev / 4)) * Level) / 100) + 5;
				val = (int)val;

				switch(Nature) {
					case Nature.Modest:
					case Nature.Mild:
					case Nature.Quiet:
					case Nature.Rash:
						val += (val * 0.1);
						break;
					case Nature.Adamant:
					case Nature.Impish:
					case Nature.Jolly:
					case Nature.Careful:
						val -= (val * 0.1);
						break;
				}
				return (int)val;
			}
		}
		public int SpecialDefense {
			get {
				double bas = GetBaseStats(Species, AlternateForms).BaseSpecialDefense;
				double iv = SPDefIV;
				double ev = SPDefEV;
				double val = (((iv + (2 * bas) + (ev / 4)) * Level) / 100) + 5;
				val = (int)val;

				switch(Nature) {
					case Nature.Calm:
					case Nature.Gentle:
					case Nature.Sassy:
					case Nature.Careful:
						val += (val * 0.1);
						break;
					case Nature.Naughty:
					case Nature.Lax:
					case Nature.Naive:
					case Nature.Rash:
						val -= (val * 0.1);
						break;
				}
				return (int)val;
			}
		}

		public byte CoolValue {
			get { return rawdata.GetByte(0x1E); }
		}
		public byte BeautyValue {
			get { return rawdata.GetByte(0x1F); }
		}
		public byte CuteValue {
			get { return rawdata.GetByte(0x20); }
		}
		public byte SmartValue {
			get { return rawdata.GetByte(0x21); }
		}
		public byte ToughValue {
			get { return rawdata.GetByte(0x22); }
		}
		public byte SheenValue {
			get { return rawdata.GetByte(0x23); }
		}

		public SinnohRibbons1_1 SRibbons1_1 {
			get { return (SinnohRibbons1_1)rawdata.GetByte(0x24); }
		}
		public SinnohRibbons1_2 SRibbons1_2 {
			get { return (SinnohRibbons1_2)rawdata.GetByte(0x25); }
		}
		public SinnohRibbons2_1 SRibbons2_1 {
			get { return (SinnohRibbons2_1)rawdata.GetByte(0x26); }
		}
		public SinnohRibbons2_2 SRibbons2_2 {
			get { return (SinnohRibbons2_2)rawdata.GetByte(0x27); }
		}
		public SinnohRibbons3_1 SRibbons3_1 {
			get { return (SinnohRibbons3_1)rawdata.GetByte(0x60); }
		}
		public SinnohRibbons3_2 SRibbons3_2 {
			get { return (SinnohRibbons3_2)rawdata.GetByte(0x61); }
		}
		public SinnohRibbons4 SRibbons4 {
			get { return (SinnohRibbons4)rawdata.GetByte(0x62); }
		}
		public HoennRibbons1_1 HRibbons1_1 {
			get { return (HoennRibbons1_1)rawdata.GetByte(0x3C); }
		}
		public HoennRibbons1_2 HRibbons1_2 {
			get { return (HoennRibbons1_2)rawdata.GetByte(0x3D); }
		}
		public HoennRibbons2_1 HRibbons2_1 {
			get { return (HoennRibbons2_1)rawdata.GetByte(0x3E); }
		}
		public HoennRibbons2_2 HRibbons2_2 {
			get { return (HoennRibbons2_2)rawdata.GetByte(0x3F); }
		}
		

		public ushort Move1 {
			get { return rawdata.GetUShort(0x28); }
            set { rawdata.SetUShort(0x28, value); }
		}
		public ushort Move2 {
			get { return rawdata.GetUShort(0x2A); }
            set { rawdata.SetUShort(0x2A, value); }
		}
		public ushort Move3 {
			get { return rawdata.GetUShort(0x2C); }
            set { rawdata.SetUShort(0x2C, value); }
		}
		public ushort Move4 {
			get { return rawdata.GetUShort(0x2E); }
            set { rawdata.SetUShort(0x2E, value); }
		}

		public string Move1Str {
			get { return ConvertMoveIDToString(Move1); }
			set {
				Move1 = ConvertStringToMoveID(value);
			}
		}
		public string Move2Str {
			get { return ConvertMoveIDToString(Move2); }
			set {
				Move2 = ConvertStringToMoveID(value);
			}
		}
		public string Move3Str {
			get { return ConvertMoveIDToString(Move3); }
			set {
				Move3 = ConvertStringToMoveID(value);
			}
		}
		public string Move4Str {
			get { return ConvertMoveIDToString(Move4); }
			set {
				Move4 = ConvertStringToMoveID(value);
			}
		}


		public List<string> Moves {
			get {
				List<string> m = new List<string>();
				m.Add(Move1Str);
				m.Add(Move2Str);
				m.Add(Move3Str);
				m.Add(Move4Str);
				return m;
			}
		}

		public string ConvertMoveIDToString(int id) {
			if(id != 0) {
				return movenames[id];
			}
			else {
				return "";
			}
		}

		public static BaseStat GetBaseStats(int id, ushort form) {
            try
            {
                return basestats.First(delegate(BaseStat bs)
                {
                    if (bs.ID == id && bs.FormID == form)
                    {
                        return true;
                    }
                    return false;
                });
            }
            catch
            {
                return basestats.First(delegate(BaseStat bs)
                {
                    if (bs.ID == id && bs.FormID == 0)
                    {
                        return true;
                    }
                    return false;
                });
            }
		}

		public static string GetPokemonString(byte[] ba) {
        System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
        byte[] stringb = new byte[1];
        for (int i = 0; i < ba.Length - 1; i += 2)
        {
            if (ba[i] == 0xff && ba[i + 1] == 0xff)
            {
                Array.Resize(ref stringb, i);
                Array.Copy(ba, stringb, i);
                break;
            }
        }
        return enc.GetString(stringb);
		}
        public static string GetGen4PokemonString(byte[] ba)
        {
            int FFloc = 0;
            for (int i = 0; i < ba.Length - 1; i += 2)
            {
                if (ba[i] == 0xFF && ba[i + 1] == 0xFF)
                {
                    FFloc = i;
                    break;
                }
            }

            string str = "";
            for (int i = 0; i < FFloc; i += 2)
            {
                ushort pokechar = 0;
                pokechar |= ba[i + 1];
                pokechar <<= 8;
                pokechar |= ba[i];
                str += chartable[pokechar];
            }

            #region old
            //for(int i = 0; i < FFloc; i+=2) {
            //    if(ba[i + 1] == 0x01) { //0x01 seems to mean character, i dont really know
            //        if(ba[i] >= 0x2b && ba[i] <= 0x44) { // A-Z
            //            str += (char)(ba[i] + 0x16);
            //        }
            //        else if(ba[i] >= 0x45 && ba[i] <= 0x5E) { // a-z
            //            str += (char)(ba[i] + 0x1C);
            //        }
            //        else if(ba[i] >= 0x21 && ba[i] <= 0x2A) { // 0-9
            //            str += (char)(ba[i] + 0x0F);
            //        }
            //        else { // symbols, probably missing a ton, who cares
            //            switch(ba[i]) {
            //                case 0xDE:
            //                    str += ' ';
            //                    break;
            //                case 0xAB:
            //                    str += '!';
            //                    break;
            //                case 0xB5:
            //                    str += '"';
            //                    break;
            //                case 0xC0:
            //                    str += '#';
            //                    break;
            //                case 0xD2:
            //                    str += '%';
            //                    break;
            //                case 0xC2:
            //                    str += '&';
            //                    break;
            //                case 0xB3:
            //                    str += '\'';
            //                    break;
            //                case 0xB9:
            //                    str += '(';
            //                    break;
            //                case 0xBA:
            //                    str += ')';
            //                    break;
            //                case 0xBF:
            //                    str += '*';
            //                    break;
            //                case 0xBD:
            //                    str += '+';
            //                    break;
            //                case 0xAD:
            //                    str += ',';
            //                    break;
            //                case 0xBE:
            //                    str += '-';
            //                    break;
            //                case 0xAE:
            //                    str += '.';
            //                    break;
            //                case 0xB1:
            //                    str += '/';
            //                    break;
            //                case 0xC4:
            //                    str += ':';
            //                    break;
            //                case 0xC5:
            //                    str += ';';
            //                    break;
            //                case 0xC1:
            //                    str += '=';
            //                    break;
            //                case 0xAC:
            //                    str += '?';
            //                    break;
            //                case 0xD0:
            //                    str += '@';
            //                    break;
            //                case 0xC3:
            //                    str += '~';
            //                    break;
            //            }
            //        }


            //    }
            //    // thought it was a second set of chars, seems to map the same hex to several chars, so i have no clue
            //    //else if(ba[i + 1] == 0x00) { // this is ridiculous
            //    //    switch(ba[i]) {
            //    //        case 0xE2:
            //    //            str += '$';
            //    //            break;

            //    //    }
            //    //}
            //}
            #endregion
            return str;
        }
		public static byte[] ConvertPokemonString(string text) {
			List<byte> barr = new List<byte>();

			//lower first, then higher
			for(int i = 0; i < text.Length; i++) {
				char s = text[i];
				char[] list = chartable.Values.ToArray();
				int j = 0;
				for(; j < list.Length; j++) {
					if(list[j] == s) {
						break;
					}
				}
				ushort trans = 0;
				if(j < chartable.Keys.ToArray().Length)
					trans = chartable.Keys.ToArray()[j];
				else
					j = 0;
				barr.Add((byte)(trans & 0xFF));
				barr.Add((byte)(trans >> 8));
			}
			barr.Add(0xFF);
			barr.Add(0xFF);

			return barr.ToArray();
		}

	}

	public enum Gender {
		Male,
		Female,
		Genderless,
	}

	public enum Nature {
		Hardy,
		Lonely,
		Brave,
		Adamant,
		Naughty,
		Bold,
		Docile,
		Relaxed,
		Impish,
		Lax,
		Timid,
		Hasty,
		Serious,
		Jolly,
		Naive,
		Modest,
		Mild,
		Quiet,
		Bashful,
		Rash,
		Calm,
		Gentle,
		Sassy,
		Careful,
		Quirky,
	}

	public enum OriginalLanguage : byte {
		Japanese = 1,
		English = 2,
		French = 3,
		Italian = 4,
		German = 5,
		Spanish = 7,
		SouthKorea = 8,
	}

	[Flags]
	public enum Marking : byte {
		Circle = 0x01,
		Triangle = 0x02,
		Square = 0x04,
		Heart = 0x08,
		Star = 0x10,
		Diamond = 0x20,
	}

	#region fucking ribbons, how do they work

	[Flags]
	public enum SinnohRibbons1_1 : byte {
		SinnohChamp = 0x01,
		Ability = 0x02,
		GreatAbility = 0x04,
		DoubleAbility = 0x08,
		MultiAbility = 0x10,
		PairAbility = 0x20,
		WorldAbility = 0x40,
		Alert = 0x80,
	}
	[Flags]
	public enum SinnohRibbons1_2 : byte {
		Shock = 0x01,
		Downcast = 0x02,
		Careless = 0x04,
		Relax = 0x08,
		Snooze = 0x10,
		Smile = 0x20,
		Gorgeous = 0x40,
		Royal = 0x80,
	}

	[Flags]
	public enum SinnohRibbons2_1 : byte {
		GorgeousRoyal = 0x01,
		Footprint = 0x02,
		Record = 0x04,
		History = 0x08,
		Legend = 0x10,
		Red = 0x20,
		Green = 0x40,
		Blue = 0x80,
	}
	[Flags]
	public enum SinnohRibbons2_2 : byte {
		Festival = 0x01,
		Carnival = 0x02,
		Classic = 0x04,
		Premier = 0x08,
	}

	[Flags]
	public enum SinnohRibbons3_1 : byte {
		Cool = 0x01,
		CoolGreat = 0x02,
		CoolUltra = 0x04,
		CoolMaster = 0x08,
		Beauty = 0x10,
		BeautyGreat = 0x20,
		BeautyUltra = 0x40,
		BeautyMaster = 0x80,
	}
	[Flags]
	public enum SinnohRibbons3_2 : byte {
		Cute = 0x01,
		CuteGreat = 0x02,
		CuteUltra = 0x04,
		CuteMaster = 0x08,
		Smart = 0x10,
		SmartGreat = 0x20,
		SmartUltra = 0x40,
		SmartMaster = 0x80,
	}

	[Flags]
	public enum SinnohRibbons4 : byte {
		Tough = 0x01,
		ToughGreat = 0x02,
		ToughUltra = 0x04,
		ToughMaster = 0x08,
	}

	[Flags]
	public enum HoennRibbons1_1 : byte {
		Cool = 0x01,
		CoolSuper = 0x02,
		CoolHyper = 0x04,
		CoolMaster = 0x08,
		Beauty = 0x10,
		BeautySuper = 0x20,
		BeautyHyper = 0x40,
		BeautyMaster = 0x80,
	}
	[Flags]
	public enum HoennRibbons1_2 : byte {
		Cute = 0x01,
		CuteSuper = 0x02,
		CuteHyper = 0x04,
		CuteMaster = 0x08,
		Smart = 0x10,
		SmartSuper = 0x20,
		SmartHyper = 0x40,
		SmartMaster = 0x80,
	}

	[Flags]
	public enum HoennRibbons2_1 : byte {
		Tough = 0x01,
		ToughSuper = 0x02,
		ToughHyper = 0x04,
		ToughMaster = 0x08,
		Champion = 0x10,
		Winning = 0x20,
		Victory = 0x40,
		Artist = 0x80,
	}
	[Flags]
	public enum HoennRibbons2_2 : byte {
		Effort = 0x01,
		Marine = 0x02,
		Land = 0x04,
		Sky = 0x08,
		Country = 0x10,
		National = 0x20,
		Earth = 0x40,
		World = 0x80,
	}

	#endregion

}
