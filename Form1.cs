using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows;
using System.Text;

using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using System.Threading.Tasks;
using System.Threading;

using System.Windows.Input;
using System.Media;


namespace HW2
{
    public partial class Form1 : Form
    {
        List<objects.enomy> ens = new List<objects.enomy>();
        List<objects.bullet> bul = new List<objects.bullet>();
        List<objects.enom_tan> etan = new List<objects.enom_tan>();
        List<objects.Beam> list_beam = new List<objects.Beam>();
        List<objects.Beam_root> list_beam_root = new List<objects.Beam_root>();

        List<Bitmap> beam_sprite = new List<Bitmap>();
        List<Bitmap> beam_root_sprite = new List<Bitmap>();

        Bitmap bullet = Properties.Resources.bullet_;

        objects.v1m vm;
        objects.v1h vh;

        int counting = 0;
        int cool_beam = 0;
        int kill = 0;
        bool reset;
        int e_count = 4;

        objects.player pl;
        SoundPlayer sp;

        public Form1()
        {
            InitializeComponent();
            sp = new SoundPlayer(Properties.Resources.bgm);
            sp.PlayLooping();

            Random r = new Random();
            for (int i = 0; i < e_count; i++ )
            {
                int rnd = r.Next(50, 750);
                ens.Add(new objects.enomy() { xPos = rnd, yPos = 15 + i * 35, hp = 60, hp_max = 60, });
            }

            pl = new objects.player(400, 500);

            Bitmap bit_beam = Properties.Resources._beam;
            Bitmap bit_beam_root = Properties.Resources._beam_root;
            for (int i = 0; i < 960; i += 32)
            {
                Rectangle rect_beam = new Rectangle(i, 0, 32, 32);
                Rectangle rect_beam_root = new Rectangle(i, 0, 32, 32);
                beam_sprite.Add(bit_beam.Clone(
                    rect_beam, bit_beam.PixelFormat));
                beam_root_sprite.Add(bit_beam_root.Clone(
                    rect_beam_root, bit_beam_root.PixelFormat));
                beam_sprite[i / 32] = bitmap_resize(beam_sprite[i / 32], 32, 32, 48, 48);
                beam_root_sprite[i / 32] = bitmap_resize(beam_root_sprite[i / 32], 32, 32, 48, 48);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (pl.live)
            {
                switch (e.KeyCode)
                {
                    case Keys.D:
                        pl.xVecInc = true;
                        //pl.xVecDec = false;
                        break;

                    case Keys.A:
                        if (pl.xVecInc == true)
                            pl.stat = true;
                        else pl.stat = false;
                        pl.xVecDec = true;
                        //pl.xVecInc = false;
                        break;

                    case Keys.W:
                        pl.yVecDec = true;
                        //pl.yVecInc = false;
                        break;

                    case Keys.S:
                        pl.yVecInc = true;
                        //pl.yVecDec = false;
                        break;

                    default:
                        break;
                }
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D:
                    pl.xVecInc = false;
                    break;

                case Keys.A:
                    pl.xVecDec = false;
                    pl.stat = false;
                    break;

                case Keys.W:
                    pl.yVecDec = false;
                    break;

                case Keys.S:
                    pl.yVecInc = false;
                    break;

                case Keys.R:
                    pl.live = true;
                    reset = true;
                    for (int i = 0; i < ens.Count(); )
                    {
                        ens.RemoveAt(0);
                    }
                    for (int i = 0; i < etan.Count(); )
                    {
                        etan.RemoveAt(0);
                    }
                    Random r = new Random();
                    for (int i = 0; i < e_count; i++)
                    {
                        int rnd = r.Next(50, 750);
                        ens.Add(new objects.enomy() { xPos = rnd, yPos = 15 + i * 35, hp = 60, hp_max = 60, });
                    }
                    kill = 0;
                    label1.Text = "kill : 0";
                    reset = false;
                    break;

                default:
                    break;
            }
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.D)
            {
                pl.xVecInc = true;
            }
            else if (e.KeyChar == (char)Keys.A)
            {
                pl.xVecDec = true;
            }
            else if (e.KeyChar == (char)Keys.W)
            {
                pl.xVecDec = true;
            }
            else if (e.KeyChar == (char)Keys.S)
            {
                pl.xVecInc = true;
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pl.live)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (!timer2.Enabled)
                    {
                        //마우스 위치로 사격
                        timer2.Interval = 200;
                        timer2.Enabled = true;
                        Fire_tan1();


                        //유도탄
                        /*
                         if (ens.Count > 0)
                            bul.Add(new objects.bullet() { xPos = pl.xPos, yPos = pl.yPos, direction = point_direction(pl.xPos, pl.yPos, ens[0].xPos, ens[0].yPos) });
                        else
                            bul.Add(new objects.bullet() { xPos = pl.xPos, yPos = pl.yPos, direction = 270 });
                         */
                    }
                    pl.isFire = true;
                }
                if (e.Button == MouseButtons.Right)
                {
                    if (cool_beam == 0)
                    {
                        Fire_beam();
                        cool_beam = 45;
                    }
                }
            }
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pl.isFire = false;
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if(pl.live)
            {
                if (pl.bar)
                {
                    Bitmap b = Properties.Resources._barrier;
                    e.Graphics.DrawImage(b, pl.xPos - 32, pl.yPos - 32);
                }
                if (!pl.pow)
                {
                    Bitmap b = Properties.Resources._player;
                    pl.direction = point_direction(pl.xPos, pl.yPos, mouseX(), mouseY());
                    b = rotate(b, pl.direction);
                    e.Graphics.DrawImage(b, pl.xPos - 16, pl.yPos - 16);
                }
                else
                {
                    Bitmap b = Properties.Resources._player_pow;
                    pl.direction = point_direction(pl.xPos, pl.yPos, mouseX(), mouseY());
                    b = rotate(b, pl.direction);
                    e.Graphics.DrawImage(b, pl.xPos - 16, pl.yPos - 16);
                }
            }
            foreach (objects.enomy i in ens)
            {
                Bitmap b;
                if (i.hp > i.hp_max * 3 / 4)
                {
                    b = Properties.Resources.en_1;
                }
                else if (i.hp > i.hp_max * 2 / 4)
                {
                    b = Properties.Resources.en_2;
                }
                else if (i.hp > i.hp_max * 1 / 4)
                {
                    b = Properties.Resources.en_3;
                }
                else
                {
                    b = Properties.Resources.en_4;
                }
                if (i.hp_max == 2000)
                {
                    b = Properties.Resources.en_boss;
                }
                if (i.hp_max == 4000)
                {
                    b = Properties.Resources._v1;
                    e.Graphics.DrawImage(b, i.xPos - 73, i.yPos - 73);
                    Bitmap b1 = Properties.Resources._v1_m;
                    Bitmap b2 = Properties.Resources._v1_h;
                    b1 = rotate(b1, (float)vm.direction);
                    vm.direction += 6;
                    b2 = rotate(b2, (float)vh.direction);
                    vh.direction += 1;
                    e.Graphics.DrawImage(b1, i.xPos - 73, i.yPos - 73);
                    e.Graphics.DrawImage(b2, i.xPos - 50, i.yPos - 50);
                }
                else
                {
                    b = rotate(b, point_direction(i.xPos, i.yPos, pl.xPos, pl.yPos));
                    e.Graphics.DrawImage(b, i.xPos - 16, i.yPos - 16);
                }
            }
            foreach (objects.bullet i in bul)
            {
                Bitmap bb = rotate(bullet, i.direction);
                e.Graphics.DrawImage(bb, i.xPos - 16, i.yPos - 16);
            }
            foreach (objects.enom_tan i in etan)
            {
                Bitmap b = Properties.Resources.en_tan_;
                e.Graphics.DrawImage(b, i.xPos - 16, i.yPos - 16);
            }
            foreach (objects.Beam i in list_beam)
            {
                Bitmap b = rotate(beam_sprite[i.sprite_number], i.direction);
                e.Graphics.DrawImage(b, i.xPos - 24, i.yPos - 24);
            }
            foreach (objects.Beam_root i in list_beam_root)
            {
                Bitmap b = rotate(beam_root_sprite[i.sprite_number], i.direction);
                e.Graphics.DrawImage(b, i.xPos - 24, i.yPos - 24);
            }
        }
        private int rand(int i)
        {
            Random r = new Random();
            return r.Next(0, i + 1);
        }
        private void beam_count(objects.Beam i)
        {
            i.sprite_number++;
            /*
            i.dcount += 10;
            if (i.dcount > i.dspeed)
            {
                i.dcount = Math.Max(0, (i.dcount - i.dspeed));
                i.sprite_number++;
            }
            */
        }
        private void beam_root_count(objects.Beam_root i)
        {
            i.sprite_number++;
        }
        private Bitmap rotate(Bitmap b, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);
            //move rotation point to center of image
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            //rotate
            g.RotateTransform(angle);
            //move image back
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }
        private Bitmap bitmap_resize(Bitmap sourceBMP, int w1, int h1, int w2, int h2)
        {
            Bitmap result = new Bitmap(w2, h2);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, (w2 - w1) / 2, (h2 - h1) / 2, w1, h1);
            return result;
        }
        private float point_direction(int x1, int y1, int x2, int y2)
        {
            double rad = Math.Atan2(y2 - y1, x2 - x1);
            double degree = (rad * 180) / Math.PI;
            return (float)degree;
        }   //x1, y1, x2, y2

        private objects.enomy collision_check(float x, float y)
        {
            foreach (objects.enomy i in ens)
            {
                if (i.hp_max != 4000)
                {
                    if ((i.xPos - x) * (i.xPos - x) + (i.yPos - y) * (i.yPos - y) < 400)
                    {
                        return i;
                    }
                }
                else
                {
                    if ((i.xPos - x) * (i.xPos - x) + (i.yPos - y) * (i.yPos - y) < 2500)
                    {
                        return i;
                    }
                }
            }
            return null;
        }
        private bool player_collision(int x, int y)
        {
            if ((pl.xPos - x) * (pl.xPos - x) + (pl.yPos - y) * (pl.yPos - y) < 100)
            {
                return true;
            }
            return false;
        }
        private bool bullet_hit(objects.enomy enom, float dam)
        {
            if (enom != null){
                enom.hp -= dam;
                if (!reset){
                    if (enom.hp <= 0){
                        enom_die(enom);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private void enom_die(objects.enomy enom)
        {
            ens.Remove(enom);
            kill++;
            label1.Text = "kill : ";
            label1.Text += kill;
            int rnd = rand(700) + 50;
            ens.Add(new objects.enomy() { xPos = rnd, yPos = rnd % 250, hp = 60, hp_max = 60 });
            if (kill % 10 == 0)
            {
                ens.Add(new objects.enomy() { xPos = rnd, yPos = rnd % 200, hp = 300, hp_max = 2000 });
            }
            if (kill == 100)
            {
                reset = true;
                for (int i = Math.Min(4, ens.Count()); i < ens.Count(); )
                {
                    ens.RemoveAt(0);
                }
                for (int i = 0; i < etan.Count(); )
                {
                    etan.RemoveAt(0);
                }
                reset = false;
                ens.Add(new objects.enomy() { xPos = 400, yPos = 250, hp = 2000, hp_max = 4000 });
                vm = new objects.v1m() { xPos = 400, yPos = 250, direction = 0 };
                vh = new objects.v1h() { xPos = 400, yPos = 250, direction = 0 };
            }
        }
        private void Fire_tan1()
        {
            int dir = (int)point_direction(pl.xPos, pl.yPos, mouseX(), mouseY());

            bul.Add(new objects.bullet() {
                xPos = pl.xPos,
                yPos = pl.yPos,
                direction = dir
            });
            bul.Add(new objects.bullet()
            {
                xPos = pl.xPos,
                yPos = pl.yPos,
                direction = dir + 5
            });
            bul.Add(new objects.bullet()
            {
                xPos = pl.xPos,
                yPos = pl.yPos,
                direction = dir - 5
            });
        }
        private void Fire_beam()
        {
            int dir = (int)point_direction(pl.xPos, pl.yPos, mouseX(), mouseY());
            float x = (float)Math.Cos(dir * Math.PI / 180);
            float y = (float)Math.Sin(dir * Math.PI / 180);

            list_beam_root.Add(new objects.Beam_root()
            {
                direction = dir,
                xPos = pl.xPos + 31 * x,
                yPos = pl.yPos + 31 * y
            });

            for (int i = 2; i < 20; i++)
            {
                list_beam.Add(new objects.Beam()
                {
                    direction = dir,
                    xPos = pl.xPos + 31 * i * x,
                    yPos = pl.yPos + 31 * i * y
                });
            }
        }
        private void enom_fire()
        {
            Random r = new Random();
            foreach (objects.enomy i in ens)
            {
                if (i.hp_max != 4000)
                {
                    if (counting % 3 == 0)
                    {
                        int rnd = r.Next(1, 40);
                        if (rnd == 5)
                        {
                            etan.Add(new objects.enom_tan()
                            {
                                xPos = i.xPos,
                                yPos = i.yPos,
                                direction = point_direction(i.xPos, i.yPos, pl.xPos, pl.yPos)
                            });
                        }
                    }
                }
                else
                {
                    if (counting % 3 == 0)
                    {
                        etan.Add(new objects.enom_tan()
                        {
                            xPos = i.xPos,
                            yPos = i.yPos,
                            direction = (float)vm.direction,
                            speed = 2
                        });
                    }
                }
            }
        }

        private bool outofmap(float x, float y)
        {
            if (x < 0 || x > 800 || y < 0 || y > 600)
            {
                return true;
            }
            return false;
        }
        private bool touchofmap(int x, int y)
        {
            if (x < 16 || x > 768 || y < 16 || y > 544)
            {
                return true;
            }
            return false;
        }
        private int mouseX()
        {
            int x = PointToClient(new Point(MousePosition.X, MousePosition.Y)).X;
            return x;
        }
        private int mouseY()
        {
            int y = PointToClient(new Point(MousePosition.Y, MousePosition.Y)).Y;
            return y;
        }
        private void bar_init()
        {
            pl.bar = true;
            pl.bar_charge = 50;
        }
        private void bar_break()
        {
            pl.pow = true;
            pl.bar = false;
            pl.bar_charge = 300;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            counting++;
            enom_fire();
            Invalidate(false);

            if (pl.xVecInc && pl.stat != true)
            {
                if (pl.yVecInc)
                {
                    pl.xPos += 3;
                    pl.yPos += 3;
                }
                else if (pl.yVecDec)
                {
                    pl.xPos += 3;
                    pl.yPos -= 3;
                }
                else
                {
                    pl.xPos += 4;
                }
            }
            else if (pl.xVecDec)
            {
                if (pl.yVecInc)
                {
                    pl.xPos -= 3;
                    pl.yPos += 3;
                }
                else if (pl.yVecDec)
                {
                    pl.xPos -= 3;
                    pl.yPos -= 3;
                }
                else
                {
                    pl.xPos -= 4;
                }
            }
            else
            {
                if (pl.yVecInc)
                {
                    pl.yPos += 4;
                }
                else if (pl.yVecDec)
                {
                    pl.yPos -= 4;
                }
            }

            foreach (objects.bullet i in bul)
            {
                i.xPos = i.xPos + Convert.ToInt32(i.speed * Math.Cos(i.direction * Math.PI / 180));
                i.yPos = i.yPos + Convert.ToInt32(i.speed * Math.Sin(i.direction * Math.PI / 180));
                if (bullet_hit(collision_check(i.xPos, i.yPos), i.dam))
                {
                    bul.Remove(i);
                    break;
                }
                if (outofmap(i.xPos, i.yPos))
                {
                    bul.Remove(i);
                    break;
                }
            }

            foreach (objects.Beam i in list_beam)
            {
                bullet_hit(collision_check((int)i.xPos, (int)i.yPos), i.dam);
                if (outofmap((int)i.xPos, (int)i.yPos))
                {
                    list_beam.Remove(i);
                    break;
                }
            }

            foreach (objects.enomy i in ens)
            {
                if (i.hp_max != 4000)
                {
                    i.xPos = i.xPos + Convert.ToInt32(i.speed * Math.Cos(i.direction * Math.PI / 180));
                    i.yPos = i.yPos + Convert.ToInt32(i.speed * Math.Sin(i.direction * Math.PI / 180));
                    if (touchofmap(i.xPos, i.yPos))
                    {
                        i.direction = point_direction(i.xPos, i.yPos, pl.xPos, pl.yPos);
                    }
                    if (player_collision(i.xPos, i.yPos))
                    {
                        if (!pl.pow)
                        {
                            if (!pl.bar)
                            {
                                pl.live = false;
                                pl.isFire = false;
                                break;
                            }
                            else
                            {
                                bar_break();
                            }
                        }
                    }
                }
            }

            foreach (objects.enom_tan i in etan)
            {
                i.xPos = i.xPos + Convert.ToInt32(6 * Math.Cos(i.direction * Math.PI / 180));
                i.yPos = i.yPos + Convert.ToInt32(6 * Math.Sin(i.direction * Math.PI / 180));
                if (player_collision(i.xPos, i.yPos))
                {
                    if (!pl.pow)
                    {
                        if (!pl.bar)
                        {
                            etan.Remove(i);
                            pl.live = false;
                            pl.isFire = false;
                            break;
                        }
                        else
                        {
                            bar_break();
                        }
                    }
                }
                if (outofmap(i.xPos, i.yPos))
                {
                    etan.Remove(i);
                    break;
                }
            }

            if (!pl.bar)
            {
                pl.bar_charge -= 1;
                if (pl.bar_charge == 0)
                {
                    bar_init();
                }
                else if (pl.bar_charge == 250)
                {
                    pl.pow = false;
                }
            }

            foreach (objects.Beam i in list_beam)
            {
                beam_count(i);
                if (i.sprite_number == i.sprite_end)
                {
                    list_beam.Remove(i);
                    break;
                }
            }
            foreach (objects.Beam_root i in list_beam_root)
            {
                beam_root_count(i);
                if (i.sprite_number == i.sprite_end)
                {
                    list_beam_root.Remove(i);
                    break;
                }
            }

            cool_beam = Math.Max(0, cool_beam - 1);
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (pl.isFire)
            {
                Fire_tan1();
            }
            else
            {
                timer2.Stop();
            }
        }
    }
}