using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokeHack {
	public static class ExperienceFunctions {

		public static int GetFloorExpFromLevel(int level, LevelUpType ltype) {
			if(level > 100) level = 100;
			if(level < 2) return 2;

			double exp = 0;

			switch(ltype) {
				case LevelUpType.MediumFast:
					exp = Math.Pow(level, 3);
					break;
				case LevelUpType.Erratic:
					if(level > 0 && level <= 50) {
						exp = ((100 - level) / 50.0) * Math.Pow(level, 3);
					}
					else if(level > 50 && level <= 68) {
						exp = ((150 - level) / 100.0) * Math.Pow(level, 3);
					}
					else if(level > 68 && level <= 98) {
						double x = level % 3;
						double p = 0;
						if(x == 0)
							p = 0;
						if(x == 1)
							p = 0.008;
						if(x == 2)
							p = 0.014;
						exp = ((level / 3.0) - p) / 50;
						exp = 1.274 - exp;
						exp = exp * Math.Pow(level, 3);
						// Third function still has issues
						exp = Math.Pow(level, 3) * (1.274 - (1 / 50) * (level / 3) - p);
					}
					else if(level > 98 && level <= 100) {
						exp = ((160 - level) / 100.0) * Math.Pow(level, 3);
					}
					break;
				case LevelUpType.Fluctuating:
					if(level > 0 && level <= 15) {
						// First fluctuation
						exp = (level + 1) / 3.0;
						exp = (exp + 24) / 50;
						exp = exp * Math.Pow(level, 3);
					}
					else if(level > 15 && level <= 35) {
						// Second fluctuation
						exp = (level + 14) / 50.0;
						exp = exp * Math.Pow(level, 3);
					}
					else if(level > 25 && level <= 100) {
						// Third fluctuation
						exp = 32 + (level / 2);
						exp = exp / 50;
						exp = exp * Math.Pow(level, 3);
					}

					break;
				case LevelUpType.MediumSlow:
					exp = ((6 * Math.Pow(level, 3)) / 5) - (15 * Math.Pow(level, 2)) + (100 * level) - 140;
					break;
				case LevelUpType.Fast:
					exp = (4 * Math.Pow(level, 3)) / 5.0;
					break;
				case LevelUpType.Slow:
					exp = (5 * Math.Pow(level, 3)) / 4.0;
					break;
			}
			return (int)(exp);
		}

		public static int GetCeilingExpFromLevel(int level, LevelUpType ltype) {
			return GetFloorExpFromLevel(level + 1, ltype) - 1;
		}

		public static int GetLevelFromExp(int exp, LevelUpType ltype) {
			double level = 0;

			switch(ltype) {
				case LevelUpType.MediumFast:
					level = Math.Pow(exp, (1 / 3.0));
					break;
				case LevelUpType.Erratic:
				case LevelUpType.Fluctuating:
				case LevelUpType.MediumSlow:
					for(level = 1; level < 100; level++) {
						if(GetCeilingExpFromLevel((int)level, ltype) > exp)
							break;
					}

					break;
				case LevelUpType.Fast:
					level = (5 * exp) / 4.0;
					level = Math.Pow(level, 1 / 3.0);
					break;
				case LevelUpType.Slow:
					level = exp * 0.8;
					level = Math.Pow(level, 1 / 3.0);
					break;
			}

			if(level < 1)
				level = 1;
			if(level > 99.99999) {
				level = 100; // yes, it's fucking 100
			}
			return (int)(level);
		}


	}

    public class Exp
    {
        public static int[,] exparray = new int[6,100];
        public static void LoadDataFiles()
        {
            string[] strSplit = new string[1];
            strSplit[0] = "\r\n";
            string[] expdata = Resources.exptable.Split(strSplit, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < expdata.Length; i++)
            {
                int j = 0;
                string[] expline = expdata[i].Split(';');
                foreach (string s in expline)
                {
                    switch (j)
                    {
                        case 0:
                            exparray[1, i] = int.Parse(s);
                            goto default;
                        case 1:
                            exparray[4, i] = int.Parse(s);
                            goto default;
                        case 2:
                            exparray[0, i] = int.Parse(s);
                            goto default;
                        case 3:
                            exparray[3, i] = int.Parse(s);
                            goto default;
                        case 4:
                            exparray[5, i] = int.Parse(s);
                            goto default;
                        case 5:
                            exparray[2, i] = int.Parse(s);
                            goto default;
                        default:
                            break;
                    }
                    j++;
                }
            }
        }
    }
}
