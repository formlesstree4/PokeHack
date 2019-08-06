using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokeHack {
	public class PIDGenerator {

		public class PokePRNG {

			public PokePRNG() {
				m_seed = 0u;
			}

			public PokePRNG(uint _SEED) {
				m_seed = _SEED;
			}

			private uint m_seed;

			public uint Seed {
				get { return m_seed; }
				set { m_seed = value; }
			}

			public uint Previous() {
				return 0xeeb9eb65 * m_seed + 0xa3561a1;
			}

			public uint PreviousNum() {
				m_seed = Previous();
				return m_seed;
			}

			public uint Next() {
				return (0x41c64e6d * m_seed) + 0x6073;
			}

			public uint NextNum() {
				m_seed = Next();
				return m_seed;
			}

			public uint NextInBounds(uint low, uint high) {
				return (high - 1) - (NextNum() % (high - low + 1));
			}

		}

		public static List<uint> GeneratePIDFromIV(ushort hp, ushort att, ushort def, ushort spatt, ushort spdef, ushort spe) {
			do {
				PokePRNG rand;
				uint ivs = (uint)(hp + (att << 5) + (def << 10) + (spe << 15) + (spatt << 20) + (spdef << 25));
				uint iv1 = (ivs & 0xFFFF) % 0x8000;
				uint iv2 = (ivs >> 0xF);
				
				uint check = (iv2 << 16);
				List<uint> pids = new List<uint>();
				for(uint i = 0; i < 65535; i++) {
					uint X4 = check + i;
					rand = new PokePRNG(X4);
					uint X3 = rand.PreviousNum();
					if((X3 >> 16) == iv1) {
						uint X2 = rand.PreviousNum();
						uint X1 = rand.PreviousNum();
						pids.Add((X1 >> 16) + (X2 & 0xFFFF0000));
					}
				}

				return pids;

			} while(true); 

		}

		public static uint GeneratePID(int dex, Nature n, Gender g, bool shiny, bool secondability, ushort trainerID, ushort secretID) {
			byte genderval = Pokemon.GetBaseStats(dex, 0).GenderValue;

			if(genderval == 255 && g != Gender.Genderless) {
				return 0;
			}
			if(genderval == 254 && g != Gender.Female) {
				return 0;
			}
			if(genderval == 0 && g != Gender.Male) {
				return 0;
			}

			lollabel:

			PokePRNG rand = new PokePRNG((uint)DateTime.Now.Ticks);


			ushort E = (ushort)(trainerID ^ secretID);
			ushort EF;
			if(shiny) {
				EF = (ushort)(rand.NextNum() & 0x7);
			}
			else {
				EF = (ushort)(rand.NextNum());
			}
			ushort F = (ushort)(EF ^ E);

			ushort p2 = 0;
			ushort p1 = 0;
			if(genderval == 255) { //genderless, randomize last byte
				while(true) {
					byte last = (byte)rand.NextNum();
					if((secondability && last % 2 != 0) || (!secondability && last % 2 == 0)) {
						p2 = (ushort)(last + (ushort)((byte)rand.NextNum() << 8));
						break;
					}
				}
			}
			else { // gender
				if(g == Gender.Male) {
					while(true) {
						byte last = (byte)rand.NextInBounds(genderval, 256);
						if((secondability && last % 2 != 0) || (!secondability && last % 2 == 0)) {
							p2 = (ushort)(last + (ushort)((byte)rand.NextNum() << 8));
							break;
						}
					}
				}
				else if(g == Gender.Female) {
					while(true) {
						byte last = (byte)rand.NextInBounds(1, genderval);
						if((secondability && last % 2 != 0) || (!secondability && last % 2 == 0)) {
							p2 = (ushort)(last + (ushort)((byte)rand.NextNum() << 8));
							break;
						}
					}
				}
			}

			p1 = (ushort)(F ^ p2);

			int pid = (p1 << 16) + p2;

			if(pid % 25 != (int)n)
				goto lollabel;     // http://xkcd.com/292/
								   // yes, I am lazy

			return (uint)pid;
		}



	}
}
