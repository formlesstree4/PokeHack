using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokeHack {
	public class ManipulatableByteArray {

		byte[] underlying;

		public ManipulatableByteArray(byte[] arr) {
			underlying = arr;
		}

		public int Length {
			get { return underlying.Length; }
		}

		public byte this[int index] {
			get { return underlying[index]; }
			set { underlying[index] = value; }
		}

		public void SetByte(int i, byte val) {
			this[i] = val;
		}

		public byte GetByte(int i) {
			return this[i];
		}

		public void SetUShort(int i, ushort val) {
            byte[] bits = BitConverter.GetBytes(val);
			underlying[i] = bits[0];
			underlying[i + 1] = bits[1];
		}
		public void SetShort(int i, short val) {
            byte[] bits = BitConverter.GetBytes(val);
            underlying[i] = bits[0];
            underlying[i + 1] = bits[1];
		}

		public ushort GetUShort(int i) {
			ushort s = 0;
			s |= underlying[i + 1];
			s <<= 8;
			s |= underlying[i];
			return s;
		}
		public short GetShort(int i) {
			short s = 0;
			s |= underlying[i + 1];
			s <<= 8;
			s |= underlying[i];
			return s;
		}

		public void SetUInt(int i, uint val) {
            byte[] bits = BitConverter.GetBytes(val);
			underlying[i] = bits[0];
			underlying[i + 1] = bits[1];
			underlying[i + 2] = bits[2];
			underlying[i + 3] = bits[3];
		}
		public void SetInt(int i, int val) {
            byte[] bits = BitConverter.GetBytes(val);
			underlying[i] = bits[0];
			underlying[i + 1] = bits[1];
			underlying[i + 2] = bits[2];
			underlying[i + 3] = bits[3];
		}

		public uint GetUInt(int i) {
			uint x = 0;
			x |= underlying[i + 3];
			x <<= 8;
			x |= underlying[i + 2];
			x <<= 8;
			x |= underlying[i + 1];
			x <<= 8;
			x |= underlying[i];
			return x;
		}
		public int GetInt(int i) {
			int x = 0;
			x |= underlying[i + 3];
			x <<= 8;
			x |= underlying[i + 2];
			x <<= 8;
			x |= underlying[i + 1];
			x <<= 8;
			x |= underlying[i];
			return x;
		}

		public void SetArray(int i, byte[] b) {
			Array.Copy(b, 0, underlying, i, b.Length);
		}
		public byte[] GetArray(int i, int len) {
			byte[] b = new byte[len];
			Array.Copy(underlying, i, b, 0, len);
			return b;
		}


	}
}
