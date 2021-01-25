using System;
using System.Text;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using SharpDX;
using Point = GameOverlay.Drawing.Point;
using Color = GameOverlay.Drawing.Color;
using Rectangle = GameOverlay.Drawing.Rectangle;
using RawVector2 = SharpDX.Mathematics.Interop.RawVector2;
using ShpVector3 = SharpDX.Vector3;
using ShpVector2 = SharpDX.Vector2;
using System.Linq;
using JOY;

namespace PUBGMESP
{
    public interface IESPForm
    {
        void Initialize();
        void Update();
    }

    public class ESPForm : IESPForm
    {
        public readonly OverlayWindow _window;
        private readonly Graphics _graphics;
        private readonly GameMemSearch _ueSearch;

        private Font _font;
        private Font _font2;
        private Font _font3;
        private Font _font4;
        private Font _infoFont;
        private Font _bigfont;
        private SolidBrush _black;
        private SolidBrush _red;
        private SolidBrush _green;
        private SolidBrush _pink;
        private SolidBrush _blue;
        private SolidBrush _orange;
        private SolidBrush _purple;
        private SolidBrush _yellow;
        private SolidBrush _LIME;
        private SolidBrush _back;
        private SolidBrush _white;
        private SolidBrush _aqua;
        private SolidBrush _transparent;
        private SolidBrush _txtBrush;
        private SolidBrush[] _randomBrush;
        private SolidBrush _boxBrush;
        private SolidBrush _awm;


        private DisplayData _data;
        private int playerCount;

        // offset
        private int actorOffset, boneOffset, tmpOffset;
        private float fClosestDist;
        private long BestTargetUniqID = -1;
        private static float deltamult = 8f;
        private static float offset_check_result = 0f;
        private int h;

        public ESPForm(RECT rect, GameMemSearch ueSearch)
        {
            this._ueSearch = ueSearch;

            _window = new OverlayWindow(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top)
            {
                IsTopmost = true,
                IsVisible = true
            };

            _window.SizeChanged += _window_SizeChanged;

            _graphics = new Graphics
            {
                MeasureFPS = true,
                Height = _window.Height,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = true,
                Width = _window.Width,
                WindowHandle = IntPtr.Zero
            };

            // offset
            actorOffset = 320;
            boneOffset = 1408;
            tmpOffset = 776;
        }

        ~ESPForm()
        {
            _graphics.Dispose();
            _window.Dispose();
        }

        public void Initialize()
        {
            _window.CreateWindow();

            _graphics.WindowHandle = _window.Handle;
            _graphics.Setup();

            _font = _graphics.CreateFont("Arial", 12);
            _font2 = _graphics.CreateFont("Ink Free", 15);
            _font3 = _graphics.CreateFont("Bahnschrift", 15);
            _font4 = _graphics.CreateFont("Impact", 15);
            _infoFont = _graphics.CreateFont("Arial", 14);
            _bigfont = _graphics.CreateFont("Arial", 20, true);

            _black = _graphics.CreateSolidBrush(0, 0, 0);
            _pink = _graphics.CreateSolidBrush(255, 20, 147);
            _red = _graphics.CreateSolidBrush(255, 0, 0);
            _green = _graphics.CreateSolidBrush(Color.Green);
            _blue = _graphics.CreateSolidBrush(0, 0, 255);
            _orange = _graphics.CreateSolidBrush(255, 69, 0);
            _purple = _graphics.CreateSolidBrush(138, 43, 226);
            _yellow = _graphics.CreateSolidBrush(255, 215, 0);
            _white = _graphics.CreateSolidBrush(255, 255, 255);
            _LIME = _graphics.CreateSolidBrush(0, 255, 0);
            _aqua = _graphics.CreateSolidBrush(0, 255, 255);
            _back = _graphics.CreateSolidBrush(40, 40, 40, 160);
            _transparent = _graphics.CreateSolidBrush(0, 0, 0, 0);
            _awm = _graphics.CreateSolidBrush(0, 100, 0);
            _randomBrush = new SolidBrush[]
            {
                _orange,_red,_green,_blue,_yellow,_white,
                _purple,_graphics.CreateSolidBrush(255,160,122),
                _graphics.CreateSolidBrush(0,255,0),
                _graphics.CreateSolidBrush(255,0,0),
                _graphics.CreateSolidBrush(135,206,235),
                _graphics.CreateSolidBrush(153,50,204),
                _graphics.CreateSolidBrush(250,235,215),
                _graphics.CreateSolidBrush(165,42,42),
                _graphics.CreateSolidBrush(127,255,0),
                _graphics.CreateSolidBrush(210,105,30),
                _graphics.CreateSolidBrush(184,134,11),
                _graphics.CreateSolidBrush(169,169,169),
                _graphics.CreateSolidBrush(139,0,139),
                _graphics.CreateSolidBrush(255,140,0),
                _graphics.CreateSolidBrush(152,251,152),
                _graphics.CreateSolidBrush(255,239,213),
                _graphics.CreateSolidBrush(205,133,63),
                _graphics.CreateSolidBrush(238,130,238),
                _graphics.CreateSolidBrush(144,238,144),
                _graphics.CreateSolidBrush(240,230,140),
                _graphics.CreateSolidBrush(178,34,34),
                _graphics.CreateSolidBrush(107,142,45),
                _graphics.CreateSolidBrush(255,69,0),
                _graphics.CreateSolidBrush(255,160,122),
                _graphics.CreateSolidBrush(255,192,203),
                _graphics.CreateSolidBrush(221,160,221),
                _graphics.CreateSolidBrush(176,224,230),
                _graphics.CreateSolidBrush(139,69,19),
                _graphics.CreateSolidBrush(160,82,45),
                _graphics.CreateSolidBrush(192,192,192),
                _graphics.CreateSolidBrush(154,205,50),
                _graphics.CreateSolidBrush(173,216,230),
                _graphics.CreateSolidBrush(255,160,122),


        };
            _txtBrush = _graphics.CreateSolidBrush(0, 0, 0, 0.5f);
        }

        public void UpdateData(DisplayData data)
        {
            _data = data;
        }


        public void Update()
        {
            
            var gfx = _graphics;
             gfx.BeginScene();
            gfx.ClearScene(_transparent);
            // Draw FPS
            //gfx.DrawTextWithBackground(_font, _red, _black, 10, 10, "FPS: " + gfx.FPS);
            // Draw Menu
            if (Settings.ShowMenu)
            {
                //gfx.FillRectangle(_menuBrush, 10f, _window.Height / 2 -185, 150, _window.Height / 2 + 50);
                //gfx.FillRectangle(_menuBrush, 10f, _window.Height / 2 -185, 150, _window.Height / 2 + 50);


                //gfx.FillRectangle(this._back, 9f, (float)(this._window.Height / 2 - 76), 141f, (float)(this._window.Height / 2 + 150));
                //gfx.FillRectangle(_LIME, 10f, _window.Height / 2 - 75, 140, _window.Height / 2 +149);


                gfx.OutlineFillRectangle(_LIME, _back, 10f, _window.Height / 2 - 76, 170, _window.Height / 2 + 180, h + 1);
                //gfx.OutlineFillRectangle(_fr, _b123, 10f, _window.Height / 2 + 70, 170, _window.Height / 2 + 125, h + 2);
            //    gfx.DrawCrosshair(_white, new Point sc, 40, 70, CrosshairStyle.Plus);
                DrawShadowText(gfx, _font2, 35, _LIME, new Point(20f, _window.Height / 2 - 78), " JOY -  X");
                //DrawShadowText(gfx, _font, _green, new Point(20f, _window.Height / 2 - 36), "JOY 2000% SAFE");
                //awShadowText(gfx, _font, _green, new Point(20f, _window.Height / 2 - 36), "                              ♦");
                DrawShadowText(gfx, _font2, 12, _white, new Point(20f, _window.Height / 2 + 148), "    HOME       -      HIDE");
            //  DrawShadowText(gfx, _font, _red, new Point(20f, _window.Height / 2 + 112), "                              ♦");
                DrawShadowText(gfx, _font2, 12, _white, new Point(20f, _window.Height / 2 + 162), "    EXIT         -      END");
              //DrawShadowText(gfx, _font, _red, new Point(20f, _window.Height / 2 + 130), "                              ♦");
                DrawShadowText(gfx, _font, 11, _white, new Point(20f, _window.Height / 2 - 36), "Refesh speed  - " + gfx.FPS);
                if (Settings.PlayerESP)
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 - 18), "        ESP  ✔");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _red, new Point(20f, _window.Height / 2 - 18), "        ESP");
                }
                 if (Settings.PlayerESP)
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 - 18), "F2");
                 }
                 else
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 - 18), "F2");
                 }

                if (Settings.Headmode)
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 + 0), "        SPEED FIRE  ✔");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _red, new Point(20f, _window.Height / 2 + 0), "        SPEED FIRE");
                }
                if (Settings.Headmode)
                {
                    DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 0), "F3");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 0), "F3");
                }
                if (Settings.aimEnabled)
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 + 18), "        AIMBOT  ✔");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _red, new Point(20f, _window.Height / 2 + 18), "        AIMBOT");
                }
                if (Settings.aimEnabled)
                {
                    DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 18), "F4");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 18), "F4");
                }
                if (Settings.Waterbox)
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 + 36), "        WATER BOX  ✔");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _red, new Point(20f, _window.Height / 2 + 36), "        WATER BOX");
                }
                 if (Settings.Waterbox)
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 36), "F5");
                 }
                 else
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 36), "F5");
                 }
                if (Settings.SpeedCar)
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 + 108), "        FAST CAR  ✔");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _red, new Point(20f, _window.Height / 2 + 108), "        FAST CAR");
                }
                 if (Settings.SpeedCar)
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 108), "F9");
                 }
                 else
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 108), "F9");
                 }
                if (Settings.FastLanding)
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 + 72), "        FAST LAND  ✔");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _red, new Point(20f, _window.Height / 2 + 72), "        FAST LAND");
                }
                 if (Settings.FastLanding)
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 72), "F7");
                 }
                 else
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 72), "F7");
                     //DrawShadowText(gfx, _font, _red, new Point(20f, _window.Height / 2 + 36), "                              █");
                 }
                if (Settings.NoRecoil)
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 + 90), "        NO RECOIL  ✔");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _red, new Point(20f, _window.Height / 2 + 90), "        NO RECOIL");
                }
                 if (Settings.NoRecoil)
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 90), "F8");
                 }
                 else
                 {
                     DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 90), "F8");
                 }

                if (Settings.INSTANTHIT)
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 + 54), "        POWER HIT  ✔");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _red, new Point(20f, _window.Height / 2 + 54), "        POWER HIT");
                }
                if (Settings.INSTANTHIT)
                {
                    DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 54), "F6");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 54), "F6");
                }
                gfx.DrawTextWithBackground(_font, 5, _white, _transparent, new Point(10f, _window.Height / 2 + 138), "_________________________________________________________");
                gfx.DrawTextWithBackground(_font, 5, _white, _transparent, new Point(10f, _window.Height / 2 - 45), "_________________________________________________________");
                //gfx.DrawTextWithBackground(_font, 5, _white, _transparent, new Point(10f, _window.Height / 2 + 122), "_______________________________________________");
                float ScreenCenterxX = (float)_window.Width / 2f, ScreenCenteryY = (float)_window.Height / 2f;
                if (Settings.smallpointer)
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 + 126), "        CROSSHAIR  ✔");
                    gfx.DrawCrosshair(_white, ScreenCenterxX, ScreenCenteryY, 22, 2, CrosshairStyle.Plus);
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _green, new Point(20f, _window.Height / 2 + 126), "        CROSSHAIR");
                }
                if (Settings.smallpointer)
                {
                    DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 126), "F10");
                }
                else
                {
                    DrawShadowText(gfx, _font, 13, _yellow, new Point(20f, _window.Height / 2 + 126), "F10");
                }


            }

            if (_data.Players.Length > 0)
            {
                playerCount = _data.Players.Length;
                //Dgfx.FillRectangle(_txtBrush, 10f, _window.Height / 2 - 75, 140, _window.Height / 2 + 220);
                //gfx.OutlineFillRectangle(_LIME, _back, 10f, _window.Height / 2 - 76, 170, _window.Width / 2 - 70f, h + 1);
                  
                gfx.DrawTextWithBackground(_font, 16, _white, _purple, new Point(_window.Width / 2 - 50f, 120f), "FUCKERS : " + playerCount);
                
                //gfx.FillRectangle(_, 15, _red, _transparent, new Point(_window.Width / 2 - 70f, 80f), "Enemies: " + playerCount);
                //gfx.DrawTextWithBackground(_font, _red, _black, 0, 0, "JOY !!! ENEMY NEAR YOU :  " + playerCount);
            }

            // Read View Matrix
            long viewMatrixAddr = Mem.ReadMemory<int>(Mem.ReadMemory<int>(_data.ViewMatrixBase) + 32) + 512;
            D3DMatrix viewMatrix = Algorithms.ReadViewMatrix(viewMatrixAddr);
            float fClosestDist = -1;
            // Draw Player ESP
            if (Settings.PlayerESP)
            {
                foreach (var player in _data.Players)
                {

                    //if (player.Health <= 0) continue;
                    if (Algorithms.WorldToScreenPlayer(viewMatrix, player.Position, out ShpVector3 playerScreen, out int distance, _window.Width, _window.Height))
                    {
                        // Too Far not render
                        if (distance > 400) continue;
                        float x = playerScreen.X;
                        float y = playerScreen.Y;
                        float h = playerScreen.Z;
                        float w = playerScreen.Z / 2;

                        try
                        {
                            _boxBrush = _randomBrush[player.TeamID % 7];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            _boxBrush = _LIME;
                        }
                        //DrawShadowText(gfx,_font, _green, new Point((x - playerScreen.Z / 4) - 3, y - 15), player.Pose.ToString());

                        // Adjust Box
                        if (player.Pose == 1114636288)
                        {
                            y = playerScreen.Y + playerScreen.Z / 5;
                            h -= playerScreen.Z / 5;
                        }
                        if (player.Pose == 1112014848 || player.Status == 7)
                        {
                            y = playerScreen.Y + playerScreen.Z / 4;
                            h -= playerScreen.Z / 4;
                        }

                        if (Settings.PlayerLines)
                        {
                            // Draw Line
                            gfx.DrawLine(_red, new Line(_window.Width / 2, 0, x, y - 10), 2);
                            //gfx.DrawArrowLine(_boxBrush, new Line(_window.Width / 5, 2, x, y), 1);
                        }

                        if (Settings.playername)
                        {
                            
                            StringBuilder sb = new StringBuilder("");
                            if (player.IsRobot)
                                sb.Append("【BOT】");
                            else
                                sb.Append("【Player】");
                            
                            sb = new StringBuilder("");
                            sb.Append(distance).Append("");

                            //  gfx.FillRectangle(_transparent, Rectangle.Create(x - 73, y - 56, 148, 30));
                            gfx.FillRectangle(_red, Rectangle.Create(x - 73, y - 25, 15, 15));
                            gfx.FillRectangle(_black, Rectangle.Create(x + 55, y - 25, 20, 15));
                            gfx.FillRectangle(_txtBrush, Rectangle.Create(x - 59, y - 25, 113, 15));

                            //gfx.DrawRectangle(_black, Rectangle.Create(x - 73, y - 56, 148, 35), 1);
                            gfx.DrawRectangle(_black, Rectangle.Create(x - 73, y - 25, 148, 15), 1);
                            gfx.DrawRectangle(_black, Rectangle.Create(x - 73, y - 25, 15, 15), 1);
                            gfx.DrawRectangle(_black, Rectangle.Create(x + 55, y - 25, 20, 15), 1);
                           // gfx.DrawRectangle(_black, Rectangle.Create(x - 15, y - 20, 30, 15), 1);

                            //DrawShadowText(gfx, _font, _white, new Point(x - 73, y - 56), " " + player.weaponType);
                            DrawShadowText(gfx, _font, _white, new Point(x - 73, y - 25),
                                (player.IsRobot) ? " B" : " P");
                            DrawShadowText(gfx, _font, _white, new Point(x - 43, y - 25), player.Name);
                           // DrawShadowText(gfx, _font, _white, new Point(x + 55, y - 38), " " + player.TeamID);
                            DrawShadowText(gfx, _font, _white, new Point(x + 55, y - 25), " " + sb.ToString());
                        }
                        if (Settings.PlayerHealth)
                        {
                            // Draw Health
                            DrawPlayerBlood(x - 73, y - 10, 3, 148, player.Health);
                        }
                        if (Settings.PlayerBox)
                        {
                            // Draw Box
                           // gfx.DrawRectangle(_boxBrush, Rectangle.Create(x - playerScreen.Z / 4 - 3, y - 5, w + 3, h + 5), 1);
                            gfx.DrawRectangleEdges(_LIME, Rectangle.Create(x - playerScreen.Z / 4 - 3, y - 5, w + 3, h + 5), 1);
                            // _graphics.FillRectangle(_txtBrush, Rectangle.Create(x - playerScreen.Z / 4 - 3, y - 5, w + 3, h + 5));
                        }
                        if (Settings.Waterbox)
                        {
                            // Draw Box

                            _graphics.FillRectangle(_txtBrush, Rectangle.Create(x - playerScreen.Z / 4 - 3, y - 5, w + 3, h + 5));
                        }
                        if (Settings.Player3dBox)
                        {
                            Draw3DBox(viewMatrix, player, playerScreen, _window.Width, _window.Height, 180f);
                            // Draw Box
                            //gfx.DrawRectangle(_boxBrush, Rectangle.Create(x - playerScreen.Z / 4 - 3, y - 5, w + 3, h + 5), 1);
                        }
                        if (Settings.PlayerBone)
                        {
                            // Draw Bone
                            long tmpAddv = Mem.ReadMemory<int>(player.Address + tmpOffset);
                            long bodyAddv = tmpAddv + actorOffset;
                            long boneAddv = Mem.ReadMemory<int>(tmpAddv + boneOffset) + 48;
                            DrawPlayerBone(bodyAddv, boneAddv, w, viewMatrix, _window.Width, _window.Height);
                        }


                        float ScreenCenterX = (float)_window.Width / 2f, ScreenCenterY = (float)_window.Height / 2f;

                        if (Settings.aimEnabled)
                        {
                            long tmpAddv = Mem.ReadMemory<int>(player.Address + tmpOffset);
                            long bodyAddv = tmpAddv + actorOffset;
                            long boneAddv = Mem.ReadMemory<int>(tmpAddv + boneOffset) + 48;
                            ShpVector3 headPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 5 * 48);

                            headPos.Z += 7;

                            var clampPos = headPos - player.Position;
                            bool w2sHead = Algorithms.WorldToScreen3DBox(viewMatrix, new ShpVector3(headPos.X, headPos.Y - (Settings.bPredict * 2), headPos.Z - (Settings.bYAxis * 8)), out ShpVector2 HeadPosition, _window.Width, _window.Height);

                            player.Screen2D = HeadPosition;
                            player.CrosshairDistance = ShpVector2.Distance(HeadPosition, new ShpVector2(ScreenCenterX, ScreenCenterY));


                            if (Algorithms.isInside(ScreenCenterX, ScreenCenterY, Settings.bFovArray[Settings.bFovInt], player.Screen2D.X, player.Screen2D.Y))
                            {
                                fClosestDist = player.CrosshairDistance;
                                BestTargetUniqID = player.Address;
                                player.IsIn = true;
                            }


                        }
                        if (Settings.bDrawFow)
                        {

                            gfx.DrawCircle(_white, ScreenCenterX, ScreenCenterY, Settings.bFovArray[Settings.bFovInt], 2);
                            


                        }
                    }
                }
            }
            if (MainForm.GetAsyncKeyState(Settings.bAimKeys[Settings.bAimKeyINT]))
            {
                if (BestTargetUniqID != -1)
                {

                    var aimTarget = (from x in _data.Players.ToList()
                                     where x != null
                                     where x.IsIn
                                     orderby x.CrosshairDistance
                                     select x).FirstOrDefault();
                    if (aimTarget != null)
                    {
                        BestTargetUniqID = aimTarget.Address;
                    }

                    var best = FindAimTargetByUniqueID(_data.Players, BestTargetUniqID);

                    if (best != null)
                    {
                        {
                            var roundPos = new ShpVector2(best.Screen2D.X, best.Screen2D.Y);
                            AimAtPosV2(roundPos.X, roundPos.Y);
                            //long bodyAddv, long boneAddv, float w, D3DMatrix viewMatrix, int winWidth, int winHeight

                        }
                    }
                }
            }
            else
            {
                BestTargetUniqID = -1;
            }
            // Draw Item ESP
            if (Settings.ItemESP)
            {
                foreach (var item in _data.Items)
                {
                    if (Algorithms.WorldToScreenItem(viewMatrix, item.Position, out ShpVector2 itemScreen, out int distance, _window.Width, _window.Height))
                    {
                        // Too Far not render
                        if (distance > 300) continue;
                        // Draw Item
                        string disStr = string.Format(" {0} m", distance);
                        // DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X, itemScreen.Y), item.Name);
                        // DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X + 40, itemScreen.Y), disStr);

                        if (item.Name == "AWM")
                        {
                            DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X + 40, itemScreen.Y), disStr);

                        }
                        if (item.Name == "300 Magnum Ammo")
                        {
                            DrawShadowText(gfx, _font, _purple, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _purple, new Point(itemScreen.X + 110, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Enegy Drink")
                        {
                            DrawShadowText(gfx, _font, _blue, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _blue, new Point(itemScreen.X + 70, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Epinephrine")
                        {
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X + 70, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Pain Killer")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 68, itemScreen.Y), disStr);
                        }
                        if (item.Name == "First Aid Kit")
                        {
                            DrawShadowText(gfx, _font, _orange, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _orange, new Point(itemScreen.X + 80, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Lv.3 Bag")
                        {
                            DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X + 65, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Lv.2 Bag")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 60, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Lv.2 Armor")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 60, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Lv.3 Armor")
                        {
                            DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X + 75, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Lv.3 Helmet")
                        {
                            DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, 15, _purple, new Point(itemScreen.X + 85, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Lv.2 Helmet")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 65, itemScreen.Y), disStr);
                        }
                        if (item.Name == "SCAR-L")
                        {
                            DrawShadowText(gfx, _font, _LIME, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _LIME, new Point(itemScreen.X + 48, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Kar-98")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 49, itemScreen.Y), disStr);
                        }
                        if (item.Name == "M762")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 41, itemScreen.Y), disStr);
                        }
                        if (item.Name == "DP-28")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 52, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Groza")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 48, itemScreen.Y), disStr);
                        }
                        if (item.Name == "AKM")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 39, itemScreen.Y), disStr);
                        }
                        if (item.Name == "AUG")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 39, itemScreen.Y), disStr);
                        }
                        if (item.Name == "QBZ")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 39, itemScreen.Y), disStr);
                        }
                        if (item.Name == "M249")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 41, itemScreen.Y), disStr);
                        }
                        if (item.Name == "M416")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 40, itemScreen.Y), disStr);
                        }
                        if (item.Name == "7.62 Ammo")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 65, itemScreen.Y), disStr);
                        }
                        if (item.Name == "5.56 Ammo")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 65, itemScreen.Y), disStr);
                        }
                        if (item.Name == "4x Scope")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 60, itemScreen.Y), disStr);
                        }
                        if (item.Name == "6x Scope")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 60, itemScreen.Y), disStr);
                        }
                        if (item.Name == "8x Scope")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 60, itemScreen.Y), disStr);
                        }
                        if (item.Name == "AR SUPPRESSOR")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 110, itemScreen.Y), disStr);
                        }
                        if (item.Name == "SNIPER SUPPRESSOR")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 135, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Grenade")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 60, itemScreen.Y), disStr);
                        }
                        if (item.Name == "AR Quick Extended Mag")
                        {
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X + 135, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Ghillie Suit")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y + 14), disStr);
                        }
                        if (item.Name == "Flare Gun")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _yellow, new Point(itemScreen.X, itemScreen.Y + 14), disStr);
                        }
                        if (item.Name == "M24")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 39, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Mk14")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 40, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Mk47")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 40, itemScreen.Y), disStr);
                        }
                        if (item.Name == "SKS")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 40, itemScreen.Y), disStr);
                        }
                        if (item.Name == "SLR")
                        {
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _red, new Point(itemScreen.X + 40, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Mini14")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 48, itemScreen.Y), disStr);
                        }
                        if (item.Name == "QBU")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 39, itemScreen.Y), disStr);
                        }
                        if (item.Name == "G36")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 39, itemScreen.Y), disStr);
                        }
                        if (item.Name == "M16A4")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 42, itemScreen.Y), disStr);
                        }
                        if (item.Name == "AUG")
                        {
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _green, new Point(itemScreen.X + 39, itemScreen.Y), disStr);
                        }
                        if (item.Name == "S12K")
                        {
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X + 41, itemScreen.Y), disStr);
                        }
                        if (item.Name == "SawedOff")
                        {
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X + 60, itemScreen.Y), disStr);
                        }
                        if (item.Name == "S686")
                        {
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X + 41, itemScreen.Y), disStr);
                        }
                        if (item.Name == "S1897")
                        {
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X + 43, itemScreen.Y), disStr);
                        }
                        if (item.Name == "UZI")
                        {
                            DrawShadowText(gfx, _font, _pink, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _pink, new Point(itemScreen.X + 39, itemScreen.Y), disStr);
                        }
                        if (item.Name == "VSS")
                        {
                            DrawShadowText(gfx, _font, _pink, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _pink, new Point(itemScreen.X + 39, itemScreen.Y), disStr);
                        }
                        if (item.Name == "Vector")
                        {
                            DrawShadowText(gfx, _font, _pink, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _pink, new Point(itemScreen.X + 48, itemScreen.Y), disStr);
                        }
                        if (item.Name == "TommyGun")
                        {
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X + 65, itemScreen.Y), disStr);
                        }
                        if (item.Name == "UMP45")
                        {
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X, itemScreen.Y), item.Name);
                            DrawShadowText(gfx, _font, _white, new Point(itemScreen.X + 50, itemScreen.Y), disStr);
                        }

                    }
                }
                foreach (var box in _data.Boxes)
                {
                    if (Algorithms.WorldToScreenItem(viewMatrix, box.Position, out ShpVector2 itemScreen, out int distance, _window.Width, _window.Height))
                    {
                        // Too Far not render
                        if (distance > 250) continue;
                        gfx.DrawTextWithBackground(_font, _aqua, _back, new Point(itemScreen.X, itemScreen.Y), "SUPPLY BOX [" + distance.ToString() + "M]");
                        //DrawShadowText(gfx, _font, _white, new Point(itemScreen.X, itemScreen.Y), "BOX [" + distance.ToString() + "M]");
                        if (Settings.ShowLootItem && box.Items != null)
                        {

                            for (int j = 0; j < box.Items.Length; j++)
                            {
                                DrawShadowText(gfx, _font, _aqua, new Point(itemScreen.X, itemScreen.Y - 15 * (j + 1)), box.Items[j]);
                              
                            }
                        }
                    }
                }
            }

            // Draw Vehicle ESP
            if (Settings.VehicleESP)
            {
                foreach (var car in _data.Vehicles)
                {
                    if (Algorithms.WorldToScreenItem(viewMatrix, car.Position, out ShpVector2 carScreen, out int distance, _window.Width, _window.Height))
                    {
                        // Too Far not render
                        if (distance > 5000) continue;
                        string disStr = string.Format(" {0}m", distance);
                        if (car.HP < 30)
                            //   gfx.DrawTextWithBackground(_font, _red, _black, new Point(carScreen.X, carScreen.Y), "HP:" + car.HP);
                            //  gfx.DrawRectangle(_black, Rectangle.Create(carScreen.X - 15, carScreen.Y - 20, 30, 15), 1);
                            //gfx.FillRectangle(_graphics.CreateSolidBrush(0, 0, 0, 2), 10f, _window.Height / 2 - 10, 140, _window.Height / 2 + 80);
                            DrawShadowText(gfx, _font, 10, _green, new Point(carScreen.X, carScreen.Y + 15), "HP:" + car.HP);
                        else
                            DrawShadowText(gfx, _font, 10, _green, new Point(carScreen.X, carScreen.Y + 15), "HP:" + car.HP);
                        // gfx.DrawTextWithBackground(_font, _graphics.CreateSolidBrush(102, 252, 3, 255), _black, new Point(carScreen.X, carScreen.Y), "HP:" + car.HP);
                        if (car.Fuel < 30)
                            DrawShadowText(gfx, _font, 10, _red, new Point(carScreen.X + 35, carScreen.Y + 15), "F:" + car.Fuel);
                        //  gfx.DrawTextWithBackground(_font, _red, _black, new Point(carScreen.X + 100, carScreen.Y), "GAS:" + car.Fuel);
                        else
                            DrawShadowText(gfx, _font, 10, _red, new Point(carScreen.X + 35, carScreen.Y + 15), "F:" + car.Fuel);
                        //  gfx.DrawTextWithBackground(_font, _graphics.CreateSolidBrush(254, 155, 1, 255), _black, new Point(carScreen.X + 100, carScreen.Y), "GAS:" + car.Fuel);
                        //DrawShadowText(gfx, _font, _yellow, new Point(carScreen.X, carScreen.Y), car.Name);

                        //DrawShadowText(gfx, _font, _white, new Point(carScreen.X, carScreen.Y + 15), disStr);
                        // Draw Car
                        //gfx.DrawTextWithBackground(_font, _black, _purple, new Point(carScreen.X + 10, carScreen.Y), disStr);
                        //gfx.DrawTextWithBackground(_font, _black, _purple, new Point(carScreen.X, carScreen.Y), car.Name);

                        //DrawShadowText(gfx, _font, _blue, new Point(carScreen.X, carScreen.Y), car.Name);
                        //DrawShadowText(gfx, _font, _red, new Point(carScreen.X, carScreen.Y + 10), disStr);

                        if (car.Name == "JEEP")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 23, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "CAR")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 23, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "BRDM")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 32, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "BOAT")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 28, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "TRUCK")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 35, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "Scooter")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 38, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "Motorcycle")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 50, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "Snowmobile")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 60, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "BOAT 2 SEAT")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 60, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "Tuk")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 18, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "Buggy")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 32, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "PickUp")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 39, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "Sports Car")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 50, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }
                        if (car.Name == "MiniBus")
                        {
                            gfx.DrawTextWithBackground(_font, 10, _black, _green, new Point(carScreen.X + 40, carScreen.Y), disStr);
                            gfx.DrawTextWithBackground(_font, 10, _green, _black, new Point(carScreen.X, carScreen.Y), car.Name);
                        }

                    }
                }
            }
            // Grenade alert
            foreach (var gre in _data.Grenades)
            {
                if (Algorithms.WorldToScreenItem(viewMatrix, gre.Position, out ShpVector2 greScreen, out int distance, _window.Width, _window.Height))
                {
                    //gfx.DrawTextWithBackground(_font, _boxBrush, _black, new Point
                    //DrawShadowText(gfx, _font, 15, _red, new Point(greScreen.X, greScreen.Y), string.Format("!!! {0} !!! [{1}M]", gre.Type.GetDescription(), distance));
                    gfx.DrawTextWithBackground(_font, 15, _white, _back, new Point(greScreen.X, greScreen.Y), string.Format(" {0}  {1} m", gre.Type.GetDescription(), distance));
                }
            }
            gfx.EndScene();
        }
        private static PlayerData FindAimTargetByUniqueID(PlayerData[] array, long address)
        {
            var entityList = array;
            for (int i = 0; i < entityList.Length; i++)
            {
                var current = entityList[i];
                if (current == null)
                    continue;

                if (current.Address == address)
                    return current;
            }
            return null;
        }

        private void AimAtPosV2(float x, float y)

            
        //long bodyAddv, long boneAddv, float w, D3DMatrix viewMatrix, int winWidth, int winHeight
        {
            
            int num = _window.Width / 2;
            int num2 = _window.Height / 2;
            float num3 = (float)Settings.bSmooth + 1f;
            float num4 = 0f;
            float num5 = 0f;
            _graphics.DrawCircle(_red, x, y, 4f, 4f);
            {
            }




            if (x != 0f)
            {
                if (x > (float)num)
                {
                    num4 = -((float)num - x);
                    num4 /= num3;
                    if (num4 + (float)num > (float)(num * 2))
                    {
                        num4 = 0f;
                    }
                }
                if (x < (float)num)
                {
                    num4 = x - (float)num;
                    num4 /= num3;
                    if (num4 + (float)num < 0f)
                    {
                        num4 = 0f;
                    }
                }
            }
            if (y != 0f)
            {
                if (y > (float)num2)
                {
                    num5 = -((float)num2 - y);
                    num5 /= num3;
                    if (num5 + (float)num2 > (float)(num2 * 2))
                    {
                        num5 = 0f;
                    }
                }
                if (y < (float)num2)
                {
                    num5 = y - (float)num2;
                    num5 /= num3;
                    if (num5 + (float)num2 < 0f)
                    {
                        num5 = 0f;
                    }
                }
            }
            if (Math.Abs(num4) < 1f)
            {
                if (num4 > 0f)
                {
                    deltamult = offset_check_result * 100f;
                }
                if (num4 < 0f)
                {
                    deltamult = offset_check_result * 10f;
                }
            }
            if (Math.Abs(num5) < 1f)
            {
                if (num5 > 0f)
                {
                    deltamult = offset_check_result * 10f;
                }
                if (num5 < 0f)
                {
                    deltamult = offset_check_result * 10f;
                }
            }

            MainForm.mouse_event(1U, (int)num4, (int)num5, 0U, UIntPtr.Zero);
        }
        private void Draw3DBox(D3DMatrix viewMatrix, PlayerData player, ShpVector3 playersc, int winWidth, int winHeight, float hei= 180f)
        {
            float num = 70f;
            float num2 = 60f;
            float num3 = 50f;
            float num4 = 85f;
            hei = 180f;
            ShpVector3 vector = new ShpVector3(num3, -num2 / 2f, 0f);
            ShpVector3 vector2 = new ShpVector3(num3, num2 / 2f, 0f);
            ShpVector3 vector3 = new ShpVector3(num3 - num, num2 / 2f, 0f);
            ShpVector3 vector4 = new ShpVector3(num3 - num, -num2 / 2f, 0f);

            Matrix matrix = Matrix.RotationZ((6.28318548f * (player.Position.Y ) / 180f / 2f));
            ShpVector3 vector5 = new ShpVector3(player.Position.X, player.Position.Y, player.Position.Z - num4);
            vector = ShpVector3.TransformCoordinate(vector, matrix) + vector5;
            vector2 = ShpVector3.TransformCoordinate(vector2, matrix) + vector5;
            vector3 = ShpVector3.TransformCoordinate(vector3, matrix) + vector5;
            vector4 = ShpVector3.TransformCoordinate(vector4, matrix) + vector5;
            ShpVector2 vector6;

            if (!Algorithms.WorldToScreen3DBox(viewMatrix, vector, out vector6,  winWidth, winHeight))
                return;
            ShpVector2 vector7;
            if (!Algorithms.WorldToScreen3DBox(viewMatrix, vector2, out vector7,  winWidth, winHeight))
                return;
            ShpVector2 vector8;
            if (!Algorithms.WorldToScreen3DBox(viewMatrix, vector3, out vector8,  winWidth, winHeight))
                return;
            ShpVector2 vector9;
            if (!Algorithms.WorldToScreen3DBox(viewMatrix, vector4, out vector9,  winWidth, winHeight))
                return;
            
            RawVector2[] array = new RawVector2[]
            {
                vector6,
                vector7,
                vector8,
                vector9,
                vector6
            };
            DrawLines(array, _boxBrush);
            vector.Z += hei;
            
            bool arg_240_0 = Algorithms.WorldToScreen3DBox(viewMatrix, vector, out vector6,  winWidth, winHeight);
            vector2.Z += hei;
            bool flag = Algorithms.WorldToScreen3DBox(viewMatrix, vector2, out vector7, winWidth, winHeight);
            vector3.Z += hei;
            bool flag2 = Algorithms.WorldToScreen3DBox(viewMatrix, vector3, out vector8,  winWidth, winHeight);
            vector4.Z += hei;
            bool flag3 = Algorithms.WorldToScreen3DBox(viewMatrix, vector4, out vector9,  winWidth, winHeight);
            if (!arg_240_0 || !flag || !flag2 || !flag3)
            {
                return;
            }
            RawVector2[] array2 = new RawVector2[]
            {
                vector6,
                vector7,
                vector8,
                vector9,
                vector6
            };
            DrawLines(array2, _boxBrush);
            DrawLine(new RawVector2(array[0].X, array[0].Y), new RawVector2(array2[0].X, array2[0].Y), _boxBrush);
            DrawLine(new RawVector2(array[1].X, array[1].Y), new RawVector2(array2[1].X, array2[1].Y), _boxBrush);
            DrawLine(new RawVector2(array[2].X, array[2].Y), new RawVector2(array2[2].X, array2[2].Y), _boxBrush);
            DrawLine(new RawVector2(array[3].X, array[3].Y), new RawVector2(array2[3].X, array2[3].Y), _boxBrush);
        }
        public void DrawLines( RawVector2[] point0, IBrush gxx)
        {
            if (point0.Length < 2)
            {
                return;
            }
            for (int i = 0; i < point0.Length - 1; i++)
            {
                DrawLine(point0[i], point0[i + 1], _boxBrush);
            }
        }
        public void DrawLine(RawVector2 a, RawVector2 b, IBrush gcolr)
        {
            _graphics.DrawLine(_boxBrush, a.X, a.Y, b.X, b.Y, 1f);
        }
        private void DrawPlayerBone(long bodyAddv, long boneAddv, float w, D3DMatrix viewMatrix, int winWidth, int winHeight)
        {

            float sightX = winWidth / 2, sightY = winHeight / 2;

            ShpVector3 headPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 5 * 48);
            headPos.Z += 7;
            ShpVector2 head;
            Algorithms.WorldToScreenBone(viewMatrix, headPos, out head, out int distance, winWidth, winHeight);
            ShpVector2 neck = head;
            ShpVector2 chest;
            ShpVector3 chestPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 4 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, chestPos, out chest, out distance, winWidth, winHeight);
            ShpVector2 pelvis;
            ShpVector3 pelvisPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 1 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, pelvisPos, out pelvis, out distance, winWidth, winHeight);
            ShpVector2 lSholder;
            ShpVector3 lSholderPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 14 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, lSholderPos, out lSholder, out distance, winWidth, winHeight);
            ShpVector2 rSholder;
            ShpVector3 rSholderPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 35 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, rSholderPos, out rSholder, out distance, winWidth, winHeight);
            ShpVector2 lElbow;
            ShpVector3 lElbowPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 15 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, lElbowPos, out lElbow, out distance, winWidth, winHeight);
            ShpVector2 rElbow;
            ShpVector3 rElbowPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 36 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, rElbowPos, out rElbow, out distance, winWidth, winHeight);
            ShpVector2 lWrist;
            ShpVector3 lWristPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 17 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, lWristPos, out lWrist, out distance, winWidth, winHeight);
            ShpVector2 rWrist;
            ShpVector3 rWristPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 39 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, rWristPos, out rWrist, out distance, winWidth, winHeight);
            ShpVector2 lThigh;
            ShpVector3 lThighPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 55 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, lThighPos, out lThigh, out distance, winWidth, winHeight);
            ShpVector2 rThigh;
            ShpVector3 rThighPos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 59 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, rThighPos, out rThigh, out distance, winWidth, winHeight);
            ShpVector2 lKnee;
            ShpVector3 lKneePos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 56 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, lKneePos, out lKnee, out distance, winWidth, winHeight);
            ShpVector2 rKnee;
            ShpVector3 rKneePos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 60 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, rKneePos, out rKnee, out distance, winWidth, winHeight);
            ShpVector2 lAnkle;
            ShpVector3 lAnklePos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 57 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, lAnklePos, out lAnkle, out distance, winWidth, winHeight);
            ShpVector2 rAnkle;
            ShpVector3 rAnklePos = Algorithms.GetBoneWorldPosition(bodyAddv, boneAddv + 61 * 48);
            Algorithms.WorldToScreenBone(viewMatrix, rAnklePos, out rAnkle, out distance, winWidth, winHeight);

            if (head != null && chest != null && pelvis != null && lSholder != null
                && rSholder != null && lElbow != null && rElbow != null && lWrist != null
               // && rWrist != null && lThigh != null && rThigh != null && lKnee != null
                && rKnee != null && lAnkle != null && rAnkle != null)
            {

                _graphics.DrawCircle(_white, new Circle(head.X, head.Y, w / 7), 1);
                
                _graphics.DrawLine(_white, new Line(neck.X, neck.Y, chest.X, chest.Y),1);
                _graphics.DrawLine(_white, new Line(chest.X, chest.Y, pelvis.X, pelvis.Y), 1);

                _graphics.DrawLine(_white, new Line(chest.X, chest.Y, lSholder.X, lSholder.Y), 1);
                _graphics.DrawLine(_white, new Line(chest.X, chest.Y, rSholder.X, rSholder.Y), 1);

                _graphics.DrawLine(_white, new Line(lSholder.X, lSholder.Y, lElbow.X, lElbow.Y), 1);
                _graphics.DrawLine(_white, new Line(rSholder.X, rSholder.Y, rElbow.X, rElbow.Y), 1);

                _graphics.DrawLine(_white, new Line(lElbow.X, lElbow.Y, lWrist.X, lWrist.Y), 1);
                _graphics.DrawLine(_white, new Line(rElbow.X, rElbow.Y, rWrist.X, rWrist.Y), 1);

                _graphics.DrawLine(_white, new Line(pelvis.X, pelvis.Y, lThigh.X, lThigh.Y), 1);
                _graphics.DrawLine(_white, new Line(pelvis.X, pelvis.Y, rThigh.X, rThigh.Y), 1);

                _graphics.DrawLine(_white, new Line(lThigh.X, lThigh.Y, lKnee.X, lKnee.Y), 1);
                _graphics.DrawLine(_white, new Line(rThigh.X, rThigh.Y, rKnee.X, rKnee.Y), 1);

                _graphics.DrawLine(_white, new Line(lKnee.X, lKnee.Y, lAnkle.X, lAnkle.Y), 1);
                _graphics.DrawLine(_white, new Line(rKnee.X, rKnee.Y, rAnkle.X, rAnkle.Y), 1);

            }
        }
        


        private void DrawShadowText(Graphics gfx, Font font, IBrush brush, Point pt, string txt)
        {
            var bpPt = new Point(pt.X - 1, pt.Y + 1);
            //var bpPt2 = new Point(pt.X + 1, pt.Y - 1);
            gfx.DrawText(font, _txtBrush, bpPt, txt);
            //gfx.DrawText(font, _txtBrush, bpPt2, txt);
            gfx.DrawText(font, brush, pt, txt);

        }
        private void DrawShadowText(Graphics gfx, Font font, float fontSize, IBrush brush, Point pt, string txt)
        {
            var bpPt = new Point();
            bpPt.X = pt.X - 1;
            bpPt.Y = pt.Y + 1;
            gfx.DrawText(font, fontSize, _txtBrush, bpPt, txt);
            gfx.DrawText(font, fontSize, brush, pt, txt);
        }

        private void DrawPlayerBlood(float x, float y, float h, float w, float fBlood)
        {
            if (fBlood > 70.0)
            {
                //FillRGB(x, y, 5, h, TextBlack);
                //FillRGB(x, y, 5, h * fBlood / 100.0, TextGreen);
                _graphics.FillRectangle(_black, Rectangle.Create(x, y, w, h));
                _graphics.FillRectangle(_green, Rectangle.Create(x, y, w * fBlood / 100, h));
            }
            if (fBlood > 30.0 && fBlood <= 70.0)
            {
                //FillRGB(x, y, 5, h, TextBlack);
                //FillRGB(x, y, 5, h * fBlood / 100.0, TextYellow);
                _graphics.FillRectangle(_black, Rectangle.Create(x, y, w, h));
                _graphics.FillRectangle(_yellow, Rectangle.Create(x, y, w * fBlood / 100, h));
            }
            if (fBlood > 0.0 && fBlood <= 30.0)
            {
                //FillRGB(x, y, 5, h, TextBlack);
                //FillRGB(x, y, 5, h * fBlood / 100.0, TextRed);
                _graphics.FillRectangle(_black, Rectangle.Create(x, y, w, h));
                _graphics.FillRectangle(_red, Rectangle.Create(x, y, w * fBlood / 100, h));
            }
        }

        private void _window_SizeChanged(object sender, OverlaySizeEventArgs e)
        {
            if (_graphics == null) return;

            if (_graphics.IsInitialized)
            {
                _graphics.Resize(e.Width, e.Height);
            }
            else
            {
                _graphics.Width = e.Width;
                _graphics.Height = e.Height;
            }
        }
    }
}
