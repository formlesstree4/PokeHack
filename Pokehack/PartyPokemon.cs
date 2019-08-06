using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokeHack {
	public class PartyPokemon : Pokemon {
        

		public PartyPokemon(byte[] arr)
			: base(arr) {
			if(arr.Length != 236) {
				throw new ArgumentException("The byte array contains no party data", "arr");
			}
		}
		public PartyPokemon(Pokemon poke)
			: base(poke.GetRawData()) {
			if(poke.GetRawData().Length != 236) {
				throw new ArgumentException("The byte array contains no party data", "poke");
			}
		}

		public static PartyPokemon ConvertPCToParty(Pokemon p) {
			return ConvertPCToParty(p.GetRawData());
		}
		public static PartyPokemon ConvertPCToParty(byte[] arr) {
			Pokemon old = new Pokemon(arr);
			byte[] newpoke = new byte[236];
			Array.Copy(arr, 0, newpoke, 0, arr.Length);
			byte[] blanker = new byte[100];
			Array.Copy(blanker, 0, newpoke, 0x88, 100);
			ManipulatableByteArray manip = new ManipulatableByteArray(newpoke);

			manip.SetByte(0x8C, (byte)old.Level);
			manip.SetUShort(0x8E, (ushort)old.HP);
			manip.SetUShort(0x90, (ushort)old.HP);
			manip.SetUShort(0x92, (ushort)old.Attack);
			manip.SetUShort(0x94, (ushort)old.Defense);
			manip.SetUShort(0x96, (ushort)old.Speed);
			manip.SetUShort(0x98, (ushort)old.SpecialAttack);
			manip.SetUShort(0x9A, (ushort)old.SpecialDefense);

			PartyPokemon p = new PartyPokemon(manip.GetArray(0, manip.Length));
			return p;
		}

		public bool IsAsleep {
			get {
				byte b = rawdata.GetByte(0x88);
				b &= 0x07;
				return b != 0;
			}
			set {
				byte b = rawdata.GetByte(0x88);
				if(value) {
					b &= 0xF8; //remove the sleep bits
					b |= 0x4; // i think? goddamn this bit order
					//b |= 0x1; // this MIGHT be it, needs testing
				}
				else {
					b &= 0xF8; //remove the sleep bits
				}
				rawdata.SetByte(0x88, b);
			}
		}
		public bool IsPoisoned {
			get {
				byte b = rawdata.GetByte(0x88);
				b &= 0x08;
				return b != 0;
			}
			set {
				byte b = rawdata.GetByte(0x88);
				if(value) {
					b |= 0x8; 
				}
				else {
					b &= 0xF7; 
				}
				rawdata.SetByte(0x88, b);
			}
		}
		public bool IsBurned {
			get {
				byte b = rawdata.GetByte(0x88);
				b &= 0x10;
				return b != 0;
			}
			set {
				byte b = rawdata.GetByte(0x88);
				if(value) {
					b |= 0x10;
				}
				else {
					b &= 0xEF;
				}
				rawdata.SetByte(0x88, b);
			}
		}
		public bool IsFrozen {
			get {
				byte b = rawdata.GetByte(0x88);
				b &= 0x20;
				return b != 0;
			}
			set {
				byte b = rawdata.GetByte(0x88);
				if(value) {
					b |= 0x20;
				}
				else {
					b &= 0xDF;
				}
				rawdata.SetByte(0x88, b);
			}
		}
		public bool IsParalyzed {
			get {
				byte b = rawdata.GetByte(0x88);
				b &= 0x40;
				return b != 0;
			}
			set {
				byte b = rawdata.GetByte(0x88);
				if(value) {
					b |= 0x40;
				}
				else {
					b &= 0xBF;
				}
				rawdata.SetByte(0x88, b);
			}
		}
		public bool IsBadlyPoisoned {
			get {
				byte b = rawdata.GetByte(0x88);
				b &= 0x80;
				return b != 0;
			}
			set {
				byte b = rawdata.GetByte(0x88);
				if(value) {
					b |= 0x80;
				}
				else {
					b &= 0x7F;
				}
				rawdata.SetByte(0x88, b);
			}
		}

		public ushort CurrentHP {
			get { return rawdata.GetUShort(0x8E); }
			set {
				if(value > HP) {
					throw new ArgumentException("CurrentHP cannot be higher than HP", "value");
				}
				rawdata.SetUShort(0x8E, value);
			}
		}


	}
}
