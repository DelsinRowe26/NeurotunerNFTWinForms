﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeurotunerNFTWinForms
{
    public class TembroClass
    {
        private static int[] minfreq = new int[96000];
        private static int[] maxfreq = new int[96000];
        private static int[] coef = new int[96000];
        public static int[] kt = new int[96000]; 
        private static StreamReader fileName = new StreamReader("Wide_voiceTurbo.txt", System.Text.Encoding.Default);
        private static int Nlines = File.ReadAllLines("Wide_voiceTurbo.txt").Length;
        private static string[] txt = fileName.ReadToEnd().Split(new char[] { ' ', '.' }, StringSplitOptions.None);
        private static int Nvalues = txt.Length;

        public void Tembro(int sampleRate)
        {
            for (int k = 0; k < 96000; k++)
            {
                kt[k] = 1;
            }

            for (int p = 0, q = 0, r = 1, e = 2; p < Nlines && q < Nvalues && r < Nvalues && e < Nvalues; p++, q += 3, r += 3, e += 3)
            {
                minfreq[p] = int.Parse(txt[q]);
                maxfreq[p] = int.Parse(txt[r]);
                coef[p] = int.Parse(txt[e]);
            }

            for (int t = 0; t < Nlines; t++)
            {
                for (int k = minfreq[t]; k < maxfreq[t]; k++)
                {
                    kt[k] *= coef[t];
                    kt[sampleRate - k] *= coef[t];
                }
            }
        }
    }
}
