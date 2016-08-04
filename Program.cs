using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Sion_Shooting
{
    static class Program
    {
        private const double witTime = 1000.0f / 60.0f;
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            double tgtTime;
            Form1 mainForm = new Form1();
            int ECCount = 0;
            int ESCount = 0;
            int ESSCount = 0;
            mainForm.StartPosition = FormStartPosition.CenterParent;
            mainForm.Show();

            tgtTime = (double)System.Environment.TickCount;
            tgtTime += witTime;
            while (mainForm.Created)
            {
                if ((double)System.Environment.TickCount >= tgtTime)
                {
                    //メインの処理
                    mainForm.RenderFps();
                    if(ECCount == 0)
                    {
                        mainForm.crtEnemy();
                        ECCount = 200;
                    }
                    if(ESCount == 0)
                    {
                        mainForm.crtEBullet();
                        ESCount = 20;
                    }
                    mainForm.crtBullet();
                    mainForm.emyBMove();
                    mainForm.chrMove();
                    mainForm.brtMove();
                    mainForm.emyDamage();
                    mainForm.chrDamage();

                    tgtTime += witTime;
                    ECCount--;
                    ESCount--;
                }
                //                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            }
        }

        public partial class Form1 : Form
        {
            private int fps;
            private int oldTime;
            private int chrHealth;
            private int itvSwi;
            private int itvTime;
            private int itvCount;
            private Label fpsLabel = new Label();
            private Rectangle dspSize = Screen.PrimaryScreen.Bounds;
            private Rectangle crtBounds;
            private Rectangle mainRange;
            public static Rectangle chrBounds;
            private Image tmpImage;
            private Image bckImage;
            private Image chrImage;
            private Image emyImage;
            private bool[] WASDKey = { false, false, false, false };
            private bool spcKey = false;
            private bool tabKey = false;
            private ArrayList bulletX;
            private ArrayList bulletY;
            private ArrayList enemyX;
            private ArrayList enemyY;
            private ArrayList emyWidth;
            private ArrayList emyHeight;
            private ArrayList emyHealth;
            private ArrayList emyType;
            private ArrayList emyBX;
            private ArrayList emyBY;
            private ArrayList EBWidth;
            private ArrayList EBHeight;
            private ArrayList EBSpeed;
            private ArrayList EBXMove;
            private ArrayList EBYMove;
            private Random rnd = new Random();
            private Brush brush;

            public Form1()
            {
                itvTime = 0;
                itvCount = 2;
                chrHealth = 3;
                bulletX = new ArrayList();
                bulletY = new ArrayList();
                enemyX = new ArrayList();
                enemyY = new ArrayList();
                emyWidth = new ArrayList();
                emyHeight = new ArrayList();
                emyHealth = new ArrayList();
                emyType = new ArrayList();
                emyBX = new ArrayList();
                emyBY = new ArrayList();
                EBWidth = new ArrayList();
                EBHeight = new ArrayList();
                EBSpeed = new ArrayList();
                EBXMove = new ArrayList();
                EBYMove = new ArrayList();
                SetStyle(
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint, true
                );
                crtBounds.Width = dspSize.Width * 80 / 100;
                crtBounds.Height = dspSize.Height * 80 / 100;
                /*
                crtBounds.Width = dspSize.Width;
                crtBounds.Height = dspSize.Height;
                */
                tmpImage = new Bitmap(crtBounds.Width, crtBounds.Height);
                Graphics g = Graphics.FromImage(tmpImage);
                Brush brush = new SolidBrush(Color.White);
                g.FillRectangle(brush, 0, 0, tmpImage.Width, tmpImage.Height);
                /*
                crtBounds.X = dspSize.Width / 2 - crtBounds.Width / 2;
                crtBounds.Y = dspSize.Height / 2 - crtBounds.Height / 2;
                */
                this.Width = crtBounds.Width;
                this.Height = crtBounds.Height;
                mainRange.Height = crtBounds.Height;
                mainRange.Width = mainRange.Height / 16 * 9;
                mainRange.X = crtBounds.Width / 2 - mainRange.Width / 2;
                mainRange.Y = 0;
                chrBounds.Width = 50;
                chrBounds.Height = 50;
                chrBounds.X = crtBounds.Width / 2 - chrBounds.Width / 2;
                chrBounds.Y = mainRange.Height * 9 / 10 - chrBounds.Height / 2;
                fps = 0;
                fpsLabel.Size = new Size(150, 10);
                bckImage = getImage(bckImage, "/Image/backImage.png", mainRange);
                chrImage = getImage(chrImage, "/Image/charactor.png", chrBounds);
                emyImage = getImage(emyImage, "/Image/enemy.png", chrBounds);
                oldTime = System.Environment.TickCount;
                KeyDown += new KeyEventHandler(Form_KeyDown);
                KeyUp += new KeyEventHandler(Form_KeyUp);
            }

            public void RenderFps()
            {
                fps++;
                if (System.Environment.TickCount >= oldTime + 1000)
                {
                    oldTime = System.Environment.TickCount;
                    fpsLabel.Text = fps.ToString() + ":" + emyType.Count + ":" + tabKey + ";" + chrHealth;
                    this.Controls.Add(fpsLabel);
                    fps = 0;
                }
            }

            public Image getImage(Image tmpImage, String Path, Rectangle tmpRec)
            {
                try
                {
                    tmpImage = Image.FromFile(System.IO.Directory.GetCurrentDirectory() + Path);
                }
                catch (OutOfMemoryException)
                {
                    MessageBox.Show("OutOfMemoryException");
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("FileNotFoundException");
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("ArgumentException");
                }
                return tmpImage;
            }

            private void Form_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyData == Keys.W) WASDKey[0] = true;
                if (e.KeyData == Keys.A) WASDKey[1] = true;
                if (e.KeyData == Keys.S) WASDKey[2] = true;
                if (e.KeyData == Keys.D) WASDKey[3] = true;
                if (e.KeyData == Keys.Space) spcKey = true;
            }

            private void Form_KeyUp(object sender, KeyEventArgs e)
            {
                if (e.KeyData == Keys.W) WASDKey[0] = false;
                if (e.KeyData == Keys.A) WASDKey[1] = false;
                if (e.KeyData == Keys.S) WASDKey[2] = false;
                if (e.KeyData == Keys.D) WASDKey[3] = false;
                if (e.KeyData == Keys.Space) spcKey = false;
                if (e.KeyData == Keys.Tab && tabKey == true)
                {
                    tabKey = false;
                }
                else if (e.KeyData == Keys.Tab)
                {
                    tabKey = true;
                }
            }

            public void crtEnemy()
            {
                if (tabKey)
                {
                    emyType.Add(0);
                    emyHealth.Add(30);
                    emyWidth.Add(50);
                    emyHeight.Add(50);
                    enemyX.Add(mainRange.X + rnd.Next(mainRange.X) - 50);
                    enemyY.Add(rnd.Next(500) - 50);
                }
            }

            public void crtEBullet()
            {
                for (int i = 0; i < emyType.Count; i++)
                {
                    //3Way
                    if ((int)emyType[i] == 0)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            EBWidth.Add(15);
                            EBHeight.Add(15);
                            emyBX.Add((int)enemyX[i] + (int)emyWidth[i] / 2 - (int)EBWidth[EBWidth.Count - 1] / 2);
                            emyBY.Add((int)enemyY[i] + (int)emyHeight[i] - (int)EBHeight[EBHeight.Count - 1] / 2);
                            EBSpeed.Add(5);
                            if (j == 0)
                            {
                                EBXMove.Add(-1);
                                EBYMove.Add(0);
                            }else if(j == 1)
                            {
                                EBXMove.Add(0);
                                EBYMove.Add(0);
                            }
                            else if (j == 2)
                            {
                                EBXMove.Add(1);
                                EBYMove.Add(0);
                            }
                        }
                    }
                    if((int)emyType[i] == 1)
                    {

                    }
                }
            }

            public void crtBullet()
            {
                if (spcKey)
                {
                    bulletX.Add(chrBounds.X + chrBounds.Width / 2);
                    bulletY.Add(chrBounds.Y);
                }
            }

            public void emyBMove()
            {
                for (int i = 0; i < emyBY.Count; i++)
                {
                    if((int)EBYMove[i] == 0)
                    {
                        emyBY[i] = (int)emyBY[i] + 5;
                    }
                    emyBX[i] = (int)emyBX[i] + (int)EBXMove[i];
                }
            }

            public void chrMove()
            {
                if (WASDKey[0] && chrBounds.Y > mainRange.Y)
                {
                    chrBounds.Y -= 5;
                    if (WASDKey[0] && chrBounds.Y < mainRange.Y) chrBounds.Y = mainRange.Y;
                }
                if (WASDKey[1] && chrBounds.X > mainRange.X)
                {
                    chrBounds.X -= 5;
                    if (WASDKey[1] && chrBounds.X < mainRange.X) chrBounds.X = mainRange.X;
                }
                if (WASDKey[2] && mainRange.Y + mainRange.Height - 41 > chrBounds.Y + chrBounds.Height)
                {
                    chrBounds.Y += 5;
                }
                if (WASDKey[3] && mainRange.X + mainRange.Width - chrBounds.Width > chrBounds.X)
                {
                    chrBounds.X += 5;
                    if (WASDKey[3] && mainRange.X + mainRange.Width - chrBounds.Width < chrBounds.X) chrBounds.X = mainRange.X + mainRange.Width - chrBounds.Width;
                }
                Invalidate();
            }

            public void brtMove()
            {
                for (int i = 0; i < bulletY.Count; i++)
                {
                    bulletY[i] = (int)bulletY[i] - 10;
                    if ((int)bulletY[i] < 0)
                    {
                        bulletX.RemoveAt(i);
                        bulletY.RemoveAt(i);
                    }
                }
            }

            public void emyDamage()
            {
                for (int i = 0; i < bulletX.Count; i++)
                {
                    for (int j = 0; j < enemyX.Count; j++)
                    {
                        if (((int)bulletX[i] >= (int)enemyX[j] && (int)bulletX[i] + 1 <= (int)enemyX[j] + (int)emyWidth[j]) &&
                           ((int)bulletY[i] >= (int)enemyY[j] && (int)bulletY[i] + 2 <= (int)enemyY[j] + (int)emyHeight[j]))
                        {
                            bulletX[i] = -999;
                            bulletY[i] = -999;
                            emyHealth[j] = (int)emyHealth[j] - 1;
                            if ((int)emyHealth[j] == 0)
                            {
                                emyType[j] = -999;
                                emyHealth[j] = -999;
                                emyWidth[j] = -999;
                                emyHeight[j] = -999;
                                enemyX[j] = -999;
                                enemyY[j] = -999;
                            }
                        }
                    }
                }
            }
            /*
                                if ((int)enemyX[i] == -999)
                                {
                                    emyType.RemoveAt(i);
                                    emyHealth.RemoveAt(i);
                                    emyWidth.RemoveAt(i);
                                    emyHeight.RemoveAt(i);
                                    enemyX.RemoveAt(i);
                                    enemyY.RemoveAt(i);
                                }
            */

            public void chrDamage()
            {
                itvSwi = 0;
                for (int j = 0; j < enemyX.Count; j++)
                {
                    for (int k = chrBounds.X; k < chrBounds.X + chrBounds.Width; k++)
                    {
                        for (int l = chrBounds.Y; l < chrBounds.Y + chrBounds.Height; l++)
                        {
                            if (IsRange(k, (int)enemyX[j], (int)enemyX[j] + (int)emyWidth[j]) &&
                                IsRange(l, (int)enemyY[j], (int)enemyY[j] + (int)emyHeight[j]) &&
                                itvTime < System.Environment.TickCount)
                            {
                                itvSwi = 1;
                                itvCount = 0;
                                itvTime = System.Environment.TickCount + 3000;
                                chrHealth -= 1;
                                if (chrHealth == 0)
                                {
                                    MessageBox.Show("GameOver");
                                    this.Close();
                                }
                                break;
                            }
                        }
                        if (itvSwi == 1) break;
                    }
                    if (itvSwi == 1) break;
                }
            }

            public static bool IsRange(int a, int from, int to)
            {
                return (from <= a && a <= to);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Graphics g = Graphics.FromImage(tmpImage);
                Pen p = new Pen(Color.Yellow);
                brush = new SolidBrush(Color.White);
                g.FillRectangle(brush, crtBounds.X, crtBounds.Y, crtBounds.Width, crtBounds.Height);
                g.DrawImage(bckImage, mainRange.X, mainRange.Y, mainRange.Width, mainRange.Height);
                brush = new SolidBrush(Color.Red);
                if (itvSwi != 1) g.DrawImage(chrImage, chrBounds.X, chrBounds.Y, chrBounds.Width, chrBounds.Height);
                else
                {
                    g.FillRectangle(brush, chrBounds.X, chrBounds.Y, chrBounds.Width, chrBounds.Height);
                }
                for (int i = 0; i < bulletY.Count; i++) g.DrawRectangle(p, (int)bulletX[i], (int)bulletY[i], 1, 2);
                brush = new SolidBrush(Color.Blue);
                for (int i = 0; i < enemyY.Count; i++)
                {
                    g.FillRectangle(brush, (int)enemyX[i], (int)enemyY[i], (int)emyWidth[i], (int)emyHeight[i]);
                }
                brush = new SolidBrush(Color.Purple);
                for (int i = 0; i < emyBY.Count; i++) g.FillRectangle(brush, (int)emyBX[i], (int)emyBY[i], (int)EBWidth[i], (int)EBHeight[i]);
                brush = new SolidBrush(Color.White);
                g.FillRectangle(brush, crtBounds.X, crtBounds.Y, mainRange.X, crtBounds.Height);
                g.FillRectangle(brush, mainRange.X + mainRange.Width - 3, crtBounds.Y, crtBounds.Width - (mainRange.X + mainRange.Width), crtBounds.Height);
                e.Graphics.DrawImage(tmpImage, 0, 0);
            }
        }
    }
}