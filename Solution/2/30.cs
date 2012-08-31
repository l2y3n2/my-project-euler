﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// For any two strings of digits, A and B, we define F(A,B) to be the sequence
    /// (A,B,AB,BAB,ABBAB,...) in which each term is the concatenation of the previous
    /// two.
    ///
    /// Further, we define D(A,B)(n) to be the nth digit in the first term of F(A,B) that
    /// contains at least n digits.
    ///
    /// Example:
    ///
    /// Let A=1415926535, B=8979323846. We wish to find D(A,B)(35), say.
    ///
    /// The first few terms of F(A,B) are:
    /// 1415926535
    /// 8979323846
    /// 14159265358979323846
    /// 897932384614159265358979323846
    /// 14159265358979323846897932384614159265358979323846
    ///
    /// Then D(A,B)(35) is the 35th digit in the fifth term, which is 9.
    ///
    /// Now we use for A the first 100 digits of π behind the decimal point:
    ///
    /// 14159265358979323846264338327950288419716939937510
    /// 58209749445923078164062862089986280348253421170679
    ///
    /// and for B the next hundred digits:
    ///
    /// 82148086513282306647093844609550582231725359408128
    /// 48111745028410270193852110555964462294895493038196.
    ///
    /// Find sum(10^n * D(A,B)((127+19n)*7^n) n = 0, 1, ..., 17
    /// </summary>
    internal class Problem230 : Problem
    {
        private abstract class FibonacciNode
        {
            public long Length { get; protected set; }

            public abstract char GetIthChar(long idx);
        }

        private class NodeA : FibonacciNode
        {
            private const string A =
                "1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679";

            public NodeA() { Length = A.Length; }

            public override char GetIthChar(long idx) { return A[(int)idx]; }
        }

        private class NodeB : FibonacciNode
        {
            private const string B =
                "8214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196";

            public NodeB() { Length = B.Length; }

            public override char GetIthChar(long idx) { return B[(int)idx]; }
        }

        private class CommonNode : FibonacciNode
        {
            private FibonacciNode Left;
            private FibonacciNode Right;

            public CommonNode(FibonacciNode left, FibonacciNode right)
            {
                Left = left;
                Right = right;
                Length = left.Length + right.Length;
            }

            public override char GetIthChar(long idx)
            {
                if (idx < Left.Length)
                    return Left.GetIthChar(idx);
                else
                    return Right.GetIthChar(idx - Left.Length);
            }
        }

        public Problem230() : base(230) { }

        private char GetChar(List<FibonacciNode> list, long idx)
        {
            while (list[list.Count - 1].Length < idx)
                list.Add(new CommonNode(list[list.Count - 2], list[list.Count - 1]));

            return list[list.Count - 1].GetIthChar(idx - 1);
        }

        protected override string Action()
        {
            var list = new List<FibonacciNode>();
            long sum = 0, p10 = 1, p7 = 1;

            list.Add(new NodeA());
            list.Add(new NodeB());

            for (int n = 0; n <= 17; n++)
            {
                sum += p10 * (GetChar(list, (127 + 19 * n) * p7) - '0');
                p10 *= 10;
                p7 *= 7;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// The binomial coefficient C(10, 3) = 120.
    /// 120 = 2^3 * 3 * 5 = 2 * 2 * 2 * 3 * 5, and 2 + 2 + 2 + 3 + 5 = 14.
    /// So the sum of the terms in the prime factorisation of C(10, 3) is 14.
    ///
    /// Find the sum of the terms in the prime factorisation of C(20000000, 15000000).
    /// </summary>
    internal class Problem231 : Problem
    {
        private const int N = 20000000;
        private const int C = 15000000;

        public Problem231() : base(231) { }

        private long GetSum(Prime prime, int l)
        {
            long sum = 0, factor;

            foreach (var p in prime)
            {
                if (p > l)
                    break;

                factor = p;
                while (factor <= l)
                {
                    sum += p * (l / factor);
                    factor *= p;
                }
            }

            return sum;
        }

        protected override string Action()
        {
            var p = new Prime(N);

            p.GenerateAll();

            return (GetSum(p, N) - GetSum(p, N - C) - GetSum(p, C)).ToString();
        }
    }

    /// <summary>
    /// Two players share an unbiased coin and take it in turns to play "The Race". On
    /// Player 1's turn, he tosses the coin once: if it comes up Heads, he scores one
    /// point; if it comes up Tails, he scores nothing. On Player 2's turn, she chooses
    /// a positive integer T and tosses the coin T times: if it comes up all Heads, she
    /// scores 2^(T-1) points; otherwise, she scores nothing. Player 1 goes first. The
    /// winner is the first to 100 or more points.
    ///
    /// On each turn Player 2 selects the number, T, of coin tosses that maximises the
    /// probability of her winning.
    ///
    /// What is the probability that Player 2 wins?
    ///
    /// Give your answer rounded to eight decimal places in the form 0.abcdefgh .
    /// </summary>
    internal class Problem232 : Problem
    {
        private const int scores = 100;

        public Problem232() : base(232) { }

        protected override string Action()
        {
            double[,] array = new double[scores + 1, scores + 1];

            /**
             * Array[a,b] stores the probability for Player 2 to win before Player 2 tosses
             * when Player 1 has a points left and Player 2 has b points left
             *
             * when player 2 choose t' coins to toss to get t points with the probability p
             *
             * if b>t:
             * array[a,b] = p * (array[a,b-t] / 2 + array[a-1,b-t]/2) + (1-p) * (array[a-1,b]/2 + array[a,b]/2)
             * array[a,b] * (1+p) = array[a,b-t] * p + array[a-1,b-t] * p + array[a-1,b] * (1-p)
             * else:
             * array[a,b] = p + (1-p) * (array[a-1,b]/2 + array[a,b]/2)
             * array[a,b] * (1+p) = 2p + array[a-1,b]*(1-p)
             */
            for (int a = 1; a <= scores; a++)
                array[a, 0] = 1;
            for (int b = 1; b <= scores; b++)
                array[0, b] = 0;

            for (int b = 1; b <= scores; b++)
            {
                for (int a = 1; a <= scores; a++)
                {
                    double p = 0.5, tmp;

                    array[a, b] = 0;
                    for (int t = 1; t < b; t *= 2)
                    {
                        tmp = (array[a, b - t] * p + array[a - 1, b - t] * p + array[a - 1, b] * (1 - p)) / (1 + p);
                        if (tmp > array[a, b])
                            array[a, b] = tmp;
                        p /= 2;
                    }
                    tmp = (2 * p + array[a - 1, b] * (1 - p)) / (1 + p);
                    if (tmp > array[a, b])
                        array[a, b] = tmp;
                }
            }

            return Math.Round(array[scores - 1, scores] / 2 + array[scores, scores] / 2, 8).ToString("F8");
        }
    }

    /// <summary>
    /// Let f(N) be the number of points with integer coordinates that are on a circle
    /// passing through (0,0), (N,0), (0,N), and (N,N).
    ///
    /// It can be shown that f(10000) = 36.
    ///
    /// What is the sum of all positive integers N <= 10^11 such that f(N) = 420 ?
    /// </summary>
    internal class Problem233 : Problem
    {
        private const long upper = 100000000000;

        public Problem233() : base(233) { }

        private List<long> GenerateNumbers(List<int> p)
        {
            var list = new List<long>();
            long n1, n2, n3;

            // case 1
            foreach (var p1 in p)
            {
                n1 = Misc.Pow(p1, 3);
                if (n1 > upper)
                    break;

                foreach (var p2 in p)
                {
                    if (p2 == p1)
                        continue;
                    n2 = n1 * p2 * p2;
                    if (n2 > upper)
                        break;

                    foreach (var p3 in p)
                    {
                        if (p3 == p1 || p3 == p2)
                            continue;
                        n3 = n2 * p3;
                        if (n3 > upper)
                            break;
                        list.Add(n3);
                    }
                }
            }

            // case 2
            foreach (var p1 in p)
            {
                n1 = Misc.Pow(p1, 7);
                if (n1 > upper)
                    break;

                foreach (var p2 in p)
                {
                    if (p2 == p1)
                        continue;
                    n2 = n1 * Misc.Pow(p2, 3);
                    if (n2 > upper)
                        break;
                    list.Add(n2);
                }
            }

            // case 3
            foreach (var p1 in p)
            {
                n1 = Misc.Pow(p1, 10);
                if (n1 > upper)
                    break;

                foreach (var p2 in p)
                {
                    if (p2 == p1)
                        continue;
                    n2 = n1 * p2 * p2;
                    if (n2 > upper)
                        break;
                    list.Add(n2);
                }
            }

            return list;
        }

        private List<long> GenerateCounter(List<int> p, int length)
        {
            var list = new List<long>();
            var flags = new bool[length + 1];

            foreach (var f in p)
            {
                for (int i = f; i <= length; i += f)
                    flags[i] = true;
            }

            list.Add(0);
            for (int i = 1; i <= length; i++)
            {
                if (flags[i])
                    list.Add(list[i - 1]);
                else
                    list.Add(list[i - 1] + i);
            }

            return list;
        }

        protected override string Action()
        {
            /**
             * Center of the circle is (N/2, N/2)
             * Think about a lattice point on the left side of the arc between the top corners of the square.
             * By symmetry there is another such point directly to the right of it and a third directly below.
             * The hypotenuse The triangle formed by these three points is the diameter of the circle
             * N^2+N^2 = 2N^2 => 4 points
             * Finding ways to represent 2N^2 = a^2+b^2
             *
             * http://mathworld.wolfram.com/SchinzelsTheorem.html
             * assume N = p1^a1 * p2^a2 * ... * pn^an = p11^a11 * ... * p1n^a1n * p31^a31 *... * p3n^a3n * pi^ai * ...
             * f(N) = 4((a1*2+1)*(a2*2+1)*...*(an*2+1)) where pn mod 4 = 1
             *
             * f(N) = 420 => 4 * 3*5*7
             * case 1: N = p1 * p2^2 * p3^3
             * case 2: N = p1^3 * p2^7
             * case 3: N = p1^2 * p2^10
             * case 4: N = p1 * p2^17, impossible
             * case 5: N = p1^52, impossible
             */
            var p = new Prime((int)(upper / 5 / 5 / 5 / 13 / 13));
            var nums = new List<long>();
            long sum = 0;

            p.GenerateAll();

            var p5 = p.Nums.Where(it => it % 4 == 1).ToList();
            var sums = GenerateCounter(p5, (int)(upper / 5 / 5 / 5 / 13 / 13 / 17));
            var list = GenerateNumbers(p5);

            foreach (var n in list)
                sum += n * sums[(int)(upper / n)];

            return sum.ToString();
        }
    }
}