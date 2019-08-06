using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokeHack {
	public class BaseStat {

		public int ID {
			get;
			internal set;
		}

		public string Name {
			get;
			internal set;
		}

		public byte BaseHP {
			get;
			internal set;
		}

		public byte FormID {
			get;
			internal set;
		}

		public byte BaseAttack {
			get;
			internal set;
		}

		public byte BaseDefense {
			get;
			internal set;
		}

		public byte BaseSpeed {
			get;
			internal set;
		}

		public byte BaseSpecialAttack {
			get;
			internal set;
		}

		public byte BaseSpecialDefense {
			get;
			internal set;
		}

		public PokeType Type1 {
			get;
			internal set;
		}

		public PokeType Type2 {
			get;
			internal set;
		}

		public byte CatchRate {
			get;
			internal set;
		}

		public byte ExpYield {
			get;
			internal set;
		}

		public uint EffortYield {
			get;
			internal set;
		}

		public uint Item1 {
			get;
			internal set;
		}

		public uint Item2 {
			get;
			internal set;
		}

		public byte GenderValue {
			get;
			internal set;
		}

		public LevelUpType LevelingType {
			get;
			internal set;
		}

		public bool HasAlternate {
			get;
			internal set;
		}


		public override string ToString() {
			return ID + ", " + Name + ", " + FormID;
		}

	}

	public enum PokeType : byte {
		None = 255,
		Normal = 0,
		Fighting,
		Flying,
		Poison,
		Ground,
		Rock,
		Bug,
		Ghost,
		Steel,
		Unknown,
		Fire,
		Water,
		Grass,
		Electric,
		Psychic,
		Ice,
		Dragon,
		Dark,
	}

	public enum LevelUpType : byte {
		MediumFast = 0,
		Erratic,
		Fluctuating,
		MediumSlow,
		Fast,
		Slow,
	}
}
