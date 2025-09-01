using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMD
{
    public class Constants
    {
        public static readonly double EPSILON = 1e-15;

        public static readonly short[][] DIRECTIONS_WITHOUT_DIAGONAL = new short[4][]{
            new short[]{0, 1},
            new short[]{1, 0},
            new short[]{0, -1},
            new short[] { -1, 0 }};

        public static readonly short[][] DIRECTIONS_WITH_DIAGONAL = new short[8][]{
            new short[]{-1, -1},
            new short[] { -1, 0 },
            new short[] { -1, 1 },
            new short[] { 0, -1 },
            new short[] { 0, 1 },
            new short[] { 1, -1 },
            new short[] { 1, 0 },
            new short[] { 1, 1 }};
    }
}
