﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// A game is played with three piles of stones and two players.
    /// At her turn, a player removes one or more stones from the piles. However, if
    /// she takes stones from more than one pile, she must remove the same number of
    /// stones from each of the selected piles.
    ///
    /// In other words, the player chooses some N>0 and removes:
    ///
    /// N stones from any single pile; or
    /// N stones from each of any two piles (2N total); or
    /// N stones from each of the three piles (3N total).
    /// The player taking the last stone(s) wins the game.
    /// A winning configuration is one where the first player can force a win.
    /// For example, (0,0,13), (0,11,11) and (5,5,5) are winning configurations because
    /// the first player can immediately remove all stones.
    ///
    /// A losing configuration is one where the second player can force a win, no
    /// matter what the first player does.
    /// For example, (0,1,2) and (1,3,3) are losing configurations: any legal move
    /// leaves a winning configuration for the second player.
    ///
    /// Consider all losing configurations (xi,yi,zi) where xi <= yi <= zi <= 100.
    /// We can verify that sum(xi+yi+zi) = 173895 for these.
    ///
    /// Find sum(xi+yi+zi) where (xi,yi,zi) ranges over the losing configurations with
    /// xi <= yi <= zi <= 1000.
    /// </summary>
    internal class Problem260 : Problem
    {
        private const int upper = 1000;

        public Problem260() : base(260) { }

        private int GetKey(int x, int y, int z)
        {
            return (x << 20) + (y << 10) + z;
        }

        private bool GetResult(BitVector wins, BitVector loses, int x, int y, int z)
        {
            int key = GetKey(x, y, z);
            bool ret = true;

            if (!wins[key] && !loses[key])
            {
                for (int n = 1; n <= z; n++)
                {
                    if (n <= x)
                    {
                        // Take n from x
                        ret = GetResult(wins, loses, x - n, y, z);
                        if (!ret)
                            break;
                        // Take n from x, y
                        ret = GetResult(wins, loses, x - n, y - n, z);
                        if (!ret)
                            break;
                        // Take n from x, z
                        if (z - n < y)
                            ret = GetResult(wins, loses, x - n, z - n, y);
                        else
                            ret = GetResult(wins, loses, x - n, y, z - n);
                        if (!ret)
                            break;
                        // Take n from x, y, z
                        ret = GetResult(wins, loses, x - n, y - n, z - n);
                        if (!ret)
                            break;
                    }

                    if (n <= y)
                    {
                        // Take n from y
                        if (y - n < x)
                            ret = GetResult(wins, loses, y - n, x, z);
                        else
                            ret = GetResult(wins, loses, x, y - n, z);
                        if (!ret)
                            break;
                        // Take n from y, z
                        if (z - n < x)
                            ret = GetResult(wins, loses, y - n, z - n, x);
                        else if (y - n < x)
                            ret = GetResult(wins, loses, y - n, x, z - n);
                        else
                            ret = GetResult(wins, loses, x, y - n, z - n);
                        if (!ret)
                            break;
                    }

                    // Take n from z
                    if (z - n < x)
                        ret = GetResult(wins, loses, z - n, x, y);
                    else if (z - n < y)
                        ret = GetResult(wins, loses, x, z - n, y);
                    else
                        ret = GetResult(wins, loses, x, y, z - n);
                    if (!ret)
                        break;
                }

                if (ret)
                    loses.Set(key);
                else
                    wins.Set(key);

                // if this is a losing configuration, add winning configuration
                if (ret)
                {
                    for (int n = 1; n <= upper - x; n++)
                    {
                        if (z + n <= upper)
                        {
                            // Add n for z
                            wins.Set(GetKey(x, y, z + n));
                            // Add n for y, z
                            wins.Set(GetKey(x, y + n, z + n));
                            // Add n for x, y, z
                            wins.Set(GetKey(x + n, y + n, z + n));
                        }
                        if (y + n <= upper)
                        {
                            // Add n for y
                            if (y + n > z)
                                wins.Set(GetKey(x, z, y + n));
                            else
                                wins.Set(GetKey(x, y + n, z));
                            // Add n for x, y
                            if (x + n > z)
                                wins.Set(GetKey(z, x + n, y + n));
                            else if (y + n > z)
                                wins.Set(GetKey(x + n, z, y + n));
                            else
                                wins.Set(GetKey(x + n, y + n, z));
                        }
                        // Add n for x
                        if (x + n > z)
                            wins.Set(GetKey(y, z, x + n));
                        else if (x + n > y)
                            wins.Set(GetKey(y, x + n, z));
                        else
                            wins.Set(GetKey(x + n, y, z));
                    }
                }
            }

            if (wins[key])
                return true;
            else
                return false;
        }

        protected override string Action()
        {
            BitVector wins = new BitVector(1 << 30), loses = new BitVector(1 << 30);
            long sum = 0;

            for (int z = 0; z <= upper; z++)
            {
                for (int y = 0; y <= z; y++)
                {
                    for (int x = 0; x <= y; x++)
                    {
                        if (!GetResult(wins, loses, x, y, z))
                            sum += (x + y + z);
                    }
                }
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Let us call a positive integer k a square-pivot, if there is a pair of integers
    /// m > 0 and n >= k, such that the sum of the (m+1) consecutive squares up to k
    /// equals the sum of the m consecutive squares from (n+1) on:
    ///
    /// (k-m)^2 + ... + k^2 = (n+1)^2 + ... + (n+m)^2.
    /// Some small square-pivots are
    ///
    /// 4: 3^2 + 4^2 = 5^2
    /// 21: 20^2 + 21^2 = 29^2
    /// 24: 21^2 + 22^2 + 23^2 + 24^2 = 25^2 + 26^2 + 27^2
    /// 110: 108^2 + 109^2 + 110^2 = 133^2 + 134^2
    /// Find the sum of all distinct square-pivots <= 10^10.
    /// </summary>
    internal class Problem261 : Problem
    {
        private const long upper = 10000000000;

        public Problem261() : base(261) { }

        private bool Func(ref UInt64 sum, HashSet<UInt64> s, UInt64 m, UInt64 a, UInt64 b, UInt64 c)
        {
            UInt64 k = (4 * m + 2) * a - 2 * m * m - b, nextm = a + c - 1;

            if (k > upper)
                return a > m;
            if (s.Add(k))
                sum += k;
            c += k + a - m;
            if (a > m)
                Func(ref sum, s, nextm, c - 1, a, k + 1);

            return Func(ref sum, s, m, k, a, c);
        }

        protected override string Action()
        {
            /*
            Binary tree like structure from xsd's post
            + m=1 -    4,    5
            |         21,   29 => m= 8*-    (28,  22)
            |                               820, 862 => m=49#-   (861,    821)
            |                                                  165648, 167281
            |                                                      ......
            |                             27724,29398
            |                             ......
            |        120,  169 => m=49#-  (168,   121)
            |                            28441, 28681
            |                               ......
            |        697,  985 =>m=288%-  (984,    698)
            |                            969528, 970922
            |                               ......
            |         .....
            + m=2 -   12,   13
            |        110,  133 => m=24*-  (132,    111)
            |                             11772,  11991
            |       1080, 1321
            |         .....
            + m=3 -   .....
            */
            HashSet<UInt64> s = new HashSet<UInt64>();
            UInt64 sum = 0, m = 1;

            while (Func(ref sum, s, m, m, 0, 1))
                m++;

            return sum.ToString();
        }
    }

    internal class Problem262 : Problem
    {
        public Problem262() : base(262) { }

        protected override string Action()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Consider the number 6. The divisors of 6 are: 1,2,3 and 6.
    /// Every number from 1 up to and including 6 can be written as a sum of distinct
    /// divisors of 6:
    /// 1=1, 2=2, 3=1+2, 4=1+3, 5=2+3, 6=6.
    /// A number n is called a practical number if every number from 1 up to and
    /// including n can be expressed as a sum of distinct divisors of n.
    ///
    /// A pair of consecutive prime numbers with a difference of six is called a sexy
    /// pair (since "sex" is the Latin word for "six"). The first sexy pair is (23,
    /// 29).
    ///
    /// We may occasionally find a triple-pair, which means three consecutive sexy
    /// prime pairs, such that the second member of each pair is the first member of
    /// the next pair.
    ///
    /// We shall call a number n such that :
    ///
    /// (n-9, n-3), (n-3,n+3), (n+3, n+9) form a triple-pair, and
    /// the numbers n-8, n-4, n, n+4 and n+8 are all practical,
    /// an engineers’ paradise.
    /// Find the sum of the first four engineers’ paradises.
    /// </summary>
    internal class Problem263 : Problem
    {
        private const int upper = 100000000;
        private const int required = 4;

        public Problem263() : base(263) { }

        private bool IsParadise(Prime p, long num)
        {
            return p.IsPrime(num - 9) && !p.IsPrime(num - 7) && !p.IsPrime(num - 5) && p.IsPrime(num - 3) && !p.IsPrime(num - 1)
                && !p.IsPrime(num + 1) && p.IsPrime(num + 3) && !p.IsPrime(num + 5) && !p.IsPrime(num + 7) && p.IsPrime(num + 9)
                && Factor.IsPracticalNumber(p, num - 8) && Factor.IsPracticalNumber(p, num - 4) && Factor.IsPracticalNumber(p, num)
                && Factor.IsPracticalNumber(p, num + 4) && Factor.IsPracticalNumber(p, num + 8);
        }

        protected override string Action()
        {
            var prime = new Prime(upper);
            long sum = 0;
            int ncounter = 0;

            /**
             * http://en.wikipedia.org/wiki/Practical_number
             * If n is a "paradise" number, n-4 and n+4 cannot be divisible by 5 or 7
             * (because n-9, n-3, n+3, and n+9 are prime).  Thus, to be practical,
             * n-4 and n+4 must be disible by 8.  n cannot be divisible by 3; it is
             * divisible by 4 (but not 8) and by 5.  So n = 20 + 40j
             *
             * n-8 and n+8 are also divisible by 4 but not 8; they must be divisible
             * either by 3 or by 7.  This leaves the following two possibilities:
             * the remainders when n is divided by
             * 3 and 7 are
             * 1 and 1
             * 2 and 6
             * respectively.
             * Combining that with n = 20 + 40j we get
             *
             * n = 20 + 1680k or n = -20 + 1680k
             */
            prime.GenerateAll();
            for (long num = 1680; ; num += 1680)
            {
                if (IsParadise(prime, num - 20))
                {
                    sum += num - 20;
                    if (++ncounter == required)
                        break;
                }
                if (IsParadise(prime, num + 20))
                {
                    sum += num + 20;
                    if (++ncounter == required)
                        break;
                }
            }

            return sum.ToString();
        }
    }

    internal class Problem264 : Problem
    {
        public Problem264() : base(264) { }

        protected override string Action()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 2^N binary digits can be placed in a circle so that all the N-digit clockwise
    /// subsequences are distinct.
    ///
    /// For N=3, two such circular arrangements are possible, ignoring rotations:
    ///
    /// 00010111    00011101
    ///
    /// For the first arrangement, the 3-digit subsequences, in clockwise order, are:
    /// 000, 001, 010, 101, 011, 111, 110 and 100.
    ///
    /// Each circular arrangement can be encoded as a number by concatenating the
    /// binary digits starting with the subsequence of all zeros as the most
    /// significant bits and proceeding clockwise. The two arrangements for N=3 are
    /// thus represented as 23 and 29:
    ///
    /// 00010111 = 23
    /// 00011101 = 29
    /// Calling S(N) the sum of the unique numeric representations, we can see that
    /// S(3) = 23 + 29 = 52.
    ///
    /// Find S(5).
    /// </summary>
    internal class Problem265 : Problem
    {
        private const int nDigits = 5;
        private static int length = (int)Misc.Pow(2, nDigits);

        public Problem265() : base(265) { }

        private bool IsBinaryCircle(int[] numbers)
        {
            bool[] flags = new bool[length];
            int n = 0;

            for (int i = 0; i < nDigits; i++)
            {
                n <<= 1;
                n += numbers[i];
            }
            for (int i = nDigits; i < length; i++)
            {
                n <<= 1;
                n += numbers[i];
                n &= (length - 1);
                if (flags[n])
                    return false;
                flags[n] = true;
            }

            return true;
        }

        private int GetNumber(int[] numbers)
        {
            int ret = 0;

            foreach (var digit in numbers.Take(length))
            {
                ret <<= 1;
                ret += digit;
            }

            return ret;
        }

        protected override string Action()
        {
            int[] digits = new int[length + nDigits];
            int freeSlot = length - nDigits * 2;
            var numbers = new HashSet<int>();
            long sum = 0;

            /**
             * There must be a sequence of 1s and 0s of length 5, assuming there is
             * five leading 0s, loop through the position of five 1s.
             */
            foreach (var posArray in Itertools.Combinations(Itertools.Range(nDigits, nDigits + freeSlot - 1), freeSlot / 2))
            {
                for (int pos = nDigits; pos <= length - nDigits + 1; pos++)
                {
                    for (int i = nDigits; i < length; i++)
                        digits[i] = 1;
                    foreach (var pos1 in posArray)
                    {
                        if (pos1 < pos)
                            digits[pos1] = 0;
                        else
                            digits[pos1 + nDigits] = 0;
                    }
                    if (IsBinaryCircle(digits))
                        numbers.Add(GetNumber(digits));
                }
            }
            foreach (var num in numbers)
                sum += num;

            return sum.ToString();
        }
    }
}