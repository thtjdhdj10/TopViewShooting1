using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

namespace HW2
{
    public class objects
    {
        public class player
        {
            public int xPos;
            public int yPos;
            public float direction;
            public bool xVecInc;
            public bool xVecDec;
            public bool yVecInc;
            public bool yVecDec;
            public bool isFire;
            public bool stat;
            public bool live = true;
            public bool pow;
            public player(int xPos, int yPos)
            {
                this.xPos = xPos;
                this.yPos = yPos;
            }
        }
        public class enomy
        {
            public int xPos;
            public int yPos;
            public float hp;
            public float hp_max;
            public float direction;
            public int speed = 4;
            public int rnd;

            public int dcount;
            public int dspeed;
            public int sprite_number;
            public int sprite_end;
        }
        public class enom_tan
        {
            public int xPos;
            public int yPos;
            public float direction;

            public int dcount;
            public int dspeed;
            public int sprite_number;
            public int sprite_end;
        }
        public class bullet
        {
            public int xPos;
            public int yPos;
            public int speed = 12;
            public float direction;

            public int dcount;
            public int dspeed;
            public int sprite_number;
            public int sprite_end;
            public float dam = 6;
        }
        public class Beam
        {
            public float xPos;
            public float yPos;
            public int direction;

            public int dcount = 0;
            public int dspeed = 10;
            public int sprite_number = 0;
            public int sprite_end = 29;
            public float dam = 1;
        }
        public class Beam_root
        {
            public float xPos;
            public float yPos;
            public int direction;

            public int sprite_number = 0;
            public int sprite_end = 29;
        }
    }
}
