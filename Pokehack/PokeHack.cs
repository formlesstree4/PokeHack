using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokeHack {
	public class PokeHack {

		public static byte[] Encrypt(Pokemon poke) {
			HyperGTS.Pokemon.InitializeDictionaries();
			byte[] pkmdata = poke.GetRawData();
			return HyperGTS.Pokemon.EncryptPokemon(pkmdata);
		}

		public static Pokemon Decrypt(byte[] bin) {
			HyperGTS.Pokemon.InitializeDictionaries();
			return new Pokemon(HyperGTS.Pokemon.DecryptPokemon(bin));
		}

		public static bool IsEncrypted(Pokemon p) {
			ushort realcheck = p.Checksum;
			ushort calccheck = p.GenerateChecksum;
			if(realcheck != calccheck)
				return true;
			return false;
		}

	}
}
