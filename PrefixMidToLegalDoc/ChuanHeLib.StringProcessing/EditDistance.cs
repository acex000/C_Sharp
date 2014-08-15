using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuanHeLib.StringProcessing
{
    public class EditDistanceCalculator
    {
        int delCost = 1;    //deletion on pattern
        int insCost = 1;    //insertion into pattern
        int substCost = 1;  //substitution a character in pattern

        /// <summary>
        /// Call recursive method to calculate Levenshtein edit distance between a string and a pattern string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public int EditDistanceRec(string str, string pattern)
        {
            return EditDistanceRec(str, pattern, str.Length, pattern.Length);
        }

        /// <summary>
        /// Recursive method to calculate Levenshtein edit distance between a string and a pattern string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="subsLength"></param>
        /// <param name="subpLength"></param>
        /// <returns></returns>
        private int EditDistanceRec(string str, string pattern, int subsLength, int subpLength)
        {

            if (subsLength == 0 && subpLength == 0)
                return 0;

            if (subsLength == 0 && subpLength >= 1)
                return subpLength * delCost;
            if (subpLength == 0 && subsLength >= 1)
                return subsLength * insCost;

            if (str[subsLength - 1] == pattern[subpLength - 1])
                return EditDistanceRec(str, pattern, subsLength - 1, subpLength - 1);
            else
            {
                return Math.Min(
                                Math.Min(
                                        (EditDistanceRec(str, pattern, subsLength, subpLength - 1) + delCost),
                                        (EditDistanceRec(str, pattern, subsLength - 1, subpLength) + insCost)),
                                        (EditDistanceRec(str, pattern, subsLength - 1, subpLength - 1) + substCost));
            }
        }

        /// <summary>
        /// Wagner-Fisher Algorithm to calculate Levenshtein edit distance between a string and a pattern string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public int EditDistanceWFA(String str, String pattern)
        {

            if (string.IsNullOrEmpty(str))
            {
                if (!string.IsNullOrEmpty(pattern))
                {
                    return pattern.Length;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(pattern))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    return str.Length;
                }
                return 0;
            }


            Int32[,] d = new int[str.Length + 1, pattern.Length + 1];
            Int32 min1;
            Int32 min2;
            Int32 min3;

            for (Int32 i = 0; i <= d.GetUpperBound(0); i += 1)
            {
                d[i, 0] = i;
            }

            for (Int32 i = 0; i <= d.GetUpperBound(1); i += 1)
            {
                d[0, i] = i;
            }

            for (Int32 i = 1; i <= d.GetUpperBound(0); i += 1)
            {
                for (Int32 j = 1; j <= d.GetUpperBound(1); j += 1)
                {
                    if (str[i-1] == pattern[j-1])
                        d[i, j] = d[i - 1, j - 1];
                    else
                    {
                        min1 = d[i - 1, j] + 1;
                        min2 = d[i, j - 1] + 1;
                        min3 = d[i - 1, j - 1] + 1;
                        d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                    }

                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];

        }


       public int Edit_Distance( string s1, string s2, int n, int m )
{
/* Here n = len(s1)
       m = len(s2) */
 
  if(n == 0 && m == 0)   //Base case
     return 0;
  if(n == 0)            //Base case
     return m;
  if( m == 0 )         //Base Case
     return n;
 
/* Recursive Part */
int   a  = Edit_Distance(s1, s2, n-1, m-1) + ((s1[n-1] != s2[m-1])?1:0);
int   b  = Edit_Distance(s1, s2, n-1, m) + 1;                      //Deletion
int   c  = Edit_Distance(s1, s2, n, m-1) + 1;                      //Insertion
 
   return  Math.Min(Math.Min(a, b), c);
}
    }
}
