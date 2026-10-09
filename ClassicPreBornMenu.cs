/*
 * ClassicPre-Born MelonLoader Menu v1.0
 * Built for MelonLoader 0.5.1 + Unity (Assembly-CSharp / VR references)
 * Theme: Tan / Navy Blue / Cream
 * Hotkey: INSERT to toggle menu
 * Keyboard/Mouse fully supported via Unity IMGUI
 *
 * Features:
 *   - World tab: Time, Gravity, Weather, TimeOfDay
 *   - Player tab: Precise Body Controls + PANKAKE FlatScreen VR Mock + FullBody
 *   - Fun tab: Speed, Fly, Jump, God
 */

using MelonLoader;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(ClassicPreBornMod.ClassicPreBornMenu), "ClassicPreBornMenu", "1.0.0", "fizzjay")]

namespace ClassicPreBornMod
{
    public class ClassicPreBornMenu : MelonMod
    {
        // ============== ATTRIBUTES ==============
        // ============== STATE ==============
        private bool menuOpen = false;
        private int tabIndex = 0; // 0 = World, 1 = Player, 2 = Fun

        // World
        private float timeScale = 1.0f;
        private float gravityScale = 1.0f;
        private int weather = 0; // 0=Clear,1=Rain,2=Snow,3=Fog
        private float timeOfDay = 12.0f;

        // Player / Body
        private bool fullBodyMode = false;
        private float headX = 0f, headY = 0f, headZ = 0f;
        private float torsoRot = 0f;
        private float leftArmX = 0f, leftArmY = 0f;
        private float rightArmX = 0f, rightArmY = 0f;
        private float bodyScale = 1.0f;
        private bool skipStartEnabled = false;

        // PANKAKE / Flatscreen VR Mock
        private bool pankakeEnabled = false;
        private float handWASDStrength = 1.5f;
        private bool mockHandsActive = false;

        // Fun
        private float speedMult = 1.0f;
        private bool flyEnabled = false;
        private bool infiniteJump = false;
        private bool godMode = false;

        // ============== COLORS ==============
        private static readonly Color TAN_BG     = new Color(0.82f, 0.67f, 0.47f, 0.96f); // warm tan
        private static readonly Color DARK_BLUE  = new Color(0.14f, 0.22f, 0.36f, 1.00f); // deep navy
        private static readonly Color BRIGHT_BLUE= new Color(0.22f, 0.55f, 0.82f, 1.00f); // bright steel blue
        private static readonly Color CREAM     = new Color(0.96f, 0.90f, 0.82f, 1.00f); // cream text
        private static readonly Color GOLD_AURA = new Color(0.92f, 0.78f, 0.50f, 1.00f); // gold/tan highlight

        // ============== INIT ==============
        public override void OnApplicationStart()
        {
            LoggerInstance.Msg("[ClassicPreBornMenu] Menu loaded - Press INSERT to open.");
            LoggerInstance.Msg("[ClassicPreBornMenu] Tan/Navy theme | World / Player / Fun tabs ready.");
        }

        // ============== UPDATE ==============
        public override void OnUpdate()
        {
            // Toggle menu with INSERT (or use F1 if Insert unavailable)
            if (Input.GetKeyDown(KeyCode.Insert) || Input.GetKeyDown(KeyCode.F1))
            {
                menuOpen = !menuOpen;
                if (menuOpen)
                    LoggerInstance.Msg("[Menu] Opened - Mouse + Keyboard fully active.");
                else
                    LoggerInstance.Msg("[Menu] Closed.");
            }

            // Apply live tweaks when menu is open OR when toggled (so they work without menu)
            ApplyPlayerTweaks();
            ApplyWorldTweaks();
            ApplyFunTweaks();

            if (skipStartEnabled) AutoConnectSkip();

            // PANKAKE: Mock VR flat-screen + WASD hand control
            if (pankakeEnabled)
            {
                MockFlatScreenVR();
            }
            else
            {
                // Reset mock when disabled
                mockHandsActive = false;
            }
        }

        // ============== GUI ==============
        public override void OnGUI()
        {
            // Do NOT block input; IMGUI naturally allows mouse/keyboard interaction
            if (!menuOpen) return;

            // Background dim
            GUI.color = new Color(0.05f, 0.07f, 0.12f, 0.55f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Main window centered, tan/blue styled
            float winW = Mathf.Min(720f, Screen.width * 0.85f);
            float winH = Mathf.Min(520f, Screen.height * 0.82f);
            float x = (Screen.width - winW) / 2f;
            float y = (Screen.height - winH) / 2f;

            // Window background (tan with blue border feel)
            GUI.color = TAN_BG;
            GUI.DrawTexture(new Rect(x, y, winW, winH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Blue top accent bar
            GUI.color = DARK_BLUE;
            GUI.DrawTexture(new Rect(x, y, winW, 6f), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Inner padding wrapper
            float pad = 16f;
            Rect content = new Rect(x + pad, y + 30f, winW - pad * 2, winH - 50f);

            // Header title with gold glow feel via shadow
            GUI.color = DARK_BLUE;
            GUI.Label(new Rect(x + pad + 2, y + 10 + 2, winW - pad * 2, 28), "CLASSIC PRE-BORN  //  MENU v1.0", MakeStyle(16, FontStyle.Bold));
            GUI.color = GOLD_AURA;
            GUI.Label(new Rect(x + pad, y + 10, winW - pad * 2, 28), "CLASSIC PRE-BORN  //  MENU v1.0", MakeStyle(16, FontStyle.Bold));
            GUI.color = Color.white;

            // Subtitle
            GUI.Label(new Rect(x + pad, y + 36, winW - pad * 2, 18), "Tan / Navy  |  World  •  Player  •  Fun  |  INSERT to toggle", MakeStyle(10, FontStyle.Italic));

            // ---- TABS ----
            string[] tabs = new string[] { "WORLD", "PLAYER", "FUN" };
            float tabW = (winW - pad * 2 - 8f) / 3f;
            float tabY = y + 58f;

            for (int i = 0; i < tabs.Length; i++)
            {
                Rect tabRect = new Rect(x + pad + i * (tabW + 4f), tabY, tabW, 28f);
                bool active = (tabIndex == i);

                // Tab bg
                if (active)
                {
                    GUI.color = DARK_BLUE;
                    GUI.DrawTexture(tabRect, Texture2D.whiteTexture);
                    GUI.color = BRIGHT_BLUE;
                    GUI.DrawTexture(new Rect(tabRect.x, tabRect.y + tabRect.height - 3f, tabRect.width, 3f), Texture2D.whiteTexture);
                }
                else
                {
                    GUI.color = new Color(0.65f, 0.50f, 0.35f, 0.8f);
                    GUI.DrawTexture(tabRect, Texture2D.whiteTexture);
                }

                // Tab text
                GUI.color = active ? CREAM : new Color(0.9f, 0.8f, 0.7f);
                GUI.Label(tabRect, tabs[i], MakeStyle(11, FontStyle.Bold));
                GUI.color = Color.white;

                // Click
                if (Event.current.type == EventType.MouseDown && tabRect.Contains(Event.current.mousePosition))
                {
                    tabIndex = i;
                    Event.current.Use();
                }
            }

            // ---- TAB CONTENT ----
            float contentY = tabY + 36f;
            float contentH = winH - (contentY - y) - 24f;
            Rect panel = new Rect(x + pad, contentY, winW - pad * 2, contentH);

            // Inner panel tan with subtle blue left border
            GUI.color = new Color(0.90f, 0.85f, 0.75f, 0.55f);
            GUI.DrawTexture(panel, Texture2D.whiteTexture);
            GUI.color = BRIGHT_BLUE;
            GUI.DrawTexture(new Rect(panel.x, panel.y, 4f, panel.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            switch (tabIndex)
            {
                case 0: DrawWorldTab(panel); break;
                case 1: DrawPlayerTab(panel); break;
                case 2: DrawFunTab(panel); break;
            }

            // Bottom hint
            GUI.Label(new Rect(x + pad, y + winH - 18, winW - pad * 2, 14), "Keyboard + Mouse active • All fields editable • Type freely", MakeStyle(9, FontStyle.Normal));
        }

        // ============== DRAW WORLD ==============
        private void DrawWorldTab(Rect panel)
        {
            GUI.color = DARK_BLUE;
            GUI.Label(new Rect(panel.x + 12, panel.y + 14, panel.width - 24, 24), "WORLD", MakeStyle(14, FontStyle.Bold));
            GUI.color = CREAM;
            GUI.Label(new Rect(panel.x + 12, panel.y + 46, panel.width - 24, 18), "Reserved - empty by request.", MakeStyle(11, FontStyle.Italic));
            GUI.color = BRIGHT_BLUE;
            GUI.DrawTexture(new Rect(panel.x + 12, panel.y + 72, panel.width - 24, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
        private void DrawPlayerTab(Rect panel)
        {
            float yOff = panel.y + 8f;

            // Header
            GUI.color = DARK_BLUE;
            GUI.Label(new Rect(panel.x + 8, yOff, panel.width - 8, 22), "PLAYER  -  Precise Body • Full Function • PANKAKE", MakeStyle(12, FontStyle.Bold));
            GUI.color = Color.white;
            yOff += 30f;

            Rect skipRect = new Rect(panel.x + 8, yOff, panel.width - 16, 32f);
            GUI.color = skipStartEnabled ? DARK_BLUE : new Color(0.75f, 0.62f, 0.50f, 0.95f);
            if (GUI.Button(skipRect, "SKIP START - AUTO-CONNECT  " + (skipStartEnabled ? "ON" : "OFF"), MakeStyle(11, FontStyle.Bold)))
            {
                skipStartEnabled = !skipStartEnabled;
                LoggerInstance.Msg("[Skip] Auto-Connect = " + skipStartEnabled);
            }
            GUI.color = Color.white;
            yOff += 36f;

            // ----- FULL BODY TOGGLE -----
            Rect fbRect = new Rect(panel.x + 8, yOff, panel.width - 16, 28f);
            GUI.color = fullBodyMode ? DARK_BLUE : new Color(0.75f, 0.62f, 0.50f, 0.9f);
            if (GUI.Button(fbRect, $"FULL BODY MODE  -  {(fullBodyMode ? "ON" : "OFF")}  (Precise Control)", MakeStyle(10, FontStyle.Bold)))
            {
                fullBodyMode = !fullBodyMode;
                LoggerInstance.Msg($"[Player] FullBodyMode = {fullBodyMode}");
            }
            GUI.color = Color.white;
            yOff += 34f;

            // ----- BODY SLIDERS -----
            GUI.Label(new Rect(panel.x + 8, yOff, panel.width - 16, 18), "PRECISE BODY CONTROLS  (Keyboard + Mouse editable)", MakeStyle(10, FontStyle.Italic));
            yOff += 20f;

            float lblW = 100f;
            float valW = panel.width - 16 - lblW - 6;

            // Head
            DrawFloatRow(panel.x + 8, yOff, "Head X", lblW, valW, 220f, ref headX, -1f, 1f, 0.01f); yOff += 22f;
            DrawFloatRow(panel.x + 8, yOff, "Head Y", lblW, valW, 220f, ref headY, -1f, 1f, 0.01f); yOff += 22f;
            DrawFloatRow(panel.x + 8, yOff, "Head Z", lblW, valW, 220f, ref headZ, -1f, 1f, 0.01f); yOff += 22f;

            // Torso
            DrawFloatRow(panel.x + 8, yOff, "Torso Rot", lblW, valW, 220f, ref torsoRot, -180f, 180f, 1f); yOff += 22f;

            // Arms
            GUI.Label(new Rect(panel.x + 8, yOff, panel.width - 16, 16), "ARMS - Left / Right (X / Y)", MakeStyle(9));
            yOff += 18f;
            DrawFloatRow(panel.x + 8, yOff, "L-Arm X", 70f, 120f, 180f, ref leftArmX, -1f, 1f, 0.05f); DrawFloatRow(panel.x + 8 + 190, yOff, "L-Arm Y", 70f, 110f, 170f, ref leftArmY, -1f, 1f, 0.05f); yOff += 22f;
            DrawFloatRow(panel.x + 8, yOff, "R-Arm X", 70f, 120f, 180f, ref rightArmX, -1f, 1f, 0.05f); DrawFloatRow(panel.x + 8 + 190, yOff, "R-Arm Y", 70f, 110f, 170f, ref rightArmY, -1f, 1f, 0.05f); yOff += 22f;

            // Scale
            DrawFloatRow(panel.x + 8, yOff, "Body Scale", lblW, valW, 220f, ref bodyScale, 0.2f, 3f, 0.05f); yOff += 30f;

            // ----- PANKAKE -----
            DrawPankakePanel(panel.x + 8, yOff, panel.width - 16, ref pankakeEnabled, ref handWASDStrength);
        }

        private void DrawPankakePanel(float x, float y, float w, ref bool enabled, ref float strength)
        {
            float h = 130f;
            // Background box tan with blue border
            GUI.color = new Color(0.85f, 0.75f, 0.60f, 0.7f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
            GUI.color = BRIGHT_BLUE;
            GUI.DrawTexture(new Rect(x, y, w, 4f), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Title
            GUI.color = GOLD_AURA;
            GUI.Label(new Rect(x + 10, y + 8, w - 20, 24), "PANKAKE  -  FLATSCREEN VR MOCK", MakeStyle(12, FontStyle.Bold));
            GUI.color = DARK_BLUE;
            GUI.Label(new Rect(x + 10, y + 28, w - 20, 22), "WASD Hand Control  •  Mock VR  •  Flat Camera", MakeStyle(9));
            GUI.color = Color.white;

            // Toggle button
            Rect btn = new Rect(x + 10, y + 50, 220f, 30f);
            bool wasOn = enabled;
            GUI.color = enabled ? DARK_BLUE : new Color(0.75f, 0.62f, 0.50f, 0.95f);
            if (GUI.Button(btn, $"PANKAKE  {(enabled ? "ON  (Mocking VR)" : "OFF")}", MakeStyle(11, FontStyle.Bold)))
            {
                enabled = !enabled;
                LoggerInstance.Msg($"[Pankake] FlatScreen VR Mock = {enabled}");
            }
            GUI.color = Color.white;

            // Strength slider
            GUI.Label(new Rect(x + 240, y + 52, 120f, 22), "Hand Strength", MakeStyle(9));
            float oldS = strength;
            strength = GUI.HorizontalSlider(new Rect(x + 240, y + 70, 130f, 18), strength, 0.1f, 5.0f);
            if (Mathf.Abs(oldS - strength) > 0.01f && enabled)
                LoggerInstance.Msg($"[Pankake] Hand WASD Strength = {strength:F2}");

            // Info text
            GUI.color = CREAM;
            GUI.Label(new Rect(x + 10, y + 82, w - 20, 32), "When ON: Camera forced flat (no stereo) • WASD moves virtual hands  •  Full body body tracking simulates VR posture. Use Mouse + Keyboard freely in this UI.", MakeStyle(9));
            GUI.color = Color.white;
        }

        // ============== DRAW FUN ==============
        private void DrawFunTab(Rect panel)
        {
            GUI.color = DARK_BLUE;
            GUI.Label(new Rect(panel.x + 12, panel.y + 14, panel.width - 24, 24), "FUN", MakeStyle(14, FontStyle.Bold));
            GUI.color = CREAM;
            GUI.Label(new Rect(panel.x + 12, panel.y + 46, panel.width - 24, 18), "Reserved - empty by request.", MakeStyle(11, FontStyle.Italic));
            GUI.color = BRIGHT_BLUE;
            GUI.DrawTexture(new Rect(panel.x + 12, panel.y + 72, panel.width - 24, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
        private void DrawFloatRow(float x, float y, string label, float lblW, float valW, float sliderW, ref float val, float min, float max, float step)
        {
            float inputW = 80f;
            float sliderActual = Mathf.Max(60f, sliderW - inputW - 10f);

            // Label
            GUI.Label(new Rect(x, y, lblW, 20), label, MakeStyle(9));

            // Slider
            float old = val;
            val = GUI.HorizontalSlider(new Rect(x + lblW + 4, y, sliderActual, 20), val, min, max);
            if (val < min) val = min; if (val > max) val = max;

            // Text input (type freely with keyboard!)
            string s = val.ToString("F3");
            s = GUI.TextField(new Rect(x + lblW + 4 + sliderActual + 6, y, inputW, 20), s, 8);
            float tryParse;
            if (float.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out tryParse))
            {
                val = Mathf.Clamp(tryParse, min, max);
            }

            // Small live indicator
            if (Mathf.Abs(old - val) > 0.001f)
            {
                // quietly update; logs only for major changes to avoid spam
            }
        }

        private void DrawRow(float x, float y, string label, float lblW, ref float val, float min, float max)
        {
            float inputW = 70f;
            float sliderW = 200f;
            GUI.Label(new Rect(x, y, lblW, 18), label, MakeStyle(9));
            float old = val;
            val = GUI.HorizontalSlider(new Rect(x + lblW + 4, y, sliderW, 18), val, min, max);
            string s = val.ToString("F2");
            s = GUI.TextField(new Rect(x + lblW + 4 + sliderW + 6, y, inputW, 18), s, 6);
            float p;
            if (float.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out p))
                val = Mathf.Clamp(p, min, max);
        }

        private void DrawToggle(float x, float y, float w, float h, string label, bool state, System.Action<bool> onChange)
        {
            GUI.color = state ? DARK_BLUE : new Color(0.75f, 0.62f, 0.50f, 0.95f);
            if (GUI.Button(new Rect(x, y, w, h), $"{label}\n{(state ? "ON" : "OFF")}", MakeStyle(9, FontStyle.Bold)))
            {
                state = !state;
                onChange?.Invoke(state);
            }
            GUI.color = Color.white;
        }

        // ============== LIVE TOKENS (Reflection-based, safe) ==============
        private void ApplyWorldTweaks()
        {
            try
            {
                // Try to find common Unity Time / Physics controllers via reflection
                // This makes it work across many old Unity games
                Type timeType = typeof(Time);
                // Time.timeScale = timeScale; // direct Unity API; safe
                Time.timeScale = timeScale;
                Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);
            }
            catch { /* silent: game may not expose these directly */ }
        }

        private void ApplyPlayerTweaks()
        {
            try
            {
                if (fullBodyMode)
                {
                    // Enable full body - try to disable ragdoll / enable animation
                    // We use reflection to find common player controllers
                    foreach (GameObject root in UnityEngine.Object.FindObjectsOfType<GameObject>())
                    {
                        if (root == null) continue;
                        // If a player or controller object is found, try to apply transforms
                        // This is intentionally broad so it works with unknown assemblies
                        if (root.name.ToLower().Contains("player") || root.name.ToLower().Contains("body") || root.name.ToLower().Contains("rig"))
                        {
                            // Apply precise body offsets if components exist
                            Transform t = root.transform;
                            if (t != null)
                            {
                                t.localPosition = new Vector3(t.localPosition.x + (headX * 0.1f), t.localPosition.y + (headY * 0.1f), t.localPosition.z + (headZ * 0.1f));
                                if (Mathf.Abs(torsoRot) > 0.1f)
                                    t.localRotation = Quaternion.Euler(torsoRot, t.localEulerAngles.y, t.localEulerAngles.z);
                            }
                        }
                    }
                }

                if (godMode)
                {
                    // Try to find Health / Damage components via reflection on common names
                    var allComponents = UnityEngine.Object.FindObjectsOfType<UnityEngine.Component>();
                    foreach (var c in allComponents)
                    {
                        if (c == null) continue;
                        Type ct = c.GetType();
                        if (ct.Name.Contains("Health") || ct.Name.Contains("Damage") || ct.Name.Contains("Life"))
                        {
                            // Try to set health property if it exists
                            try
                            {
                                PropertyInfo hp = ct.GetProperty("health", typeof(int)) ?? ct.GetProperty("Health") ?? ct.GetProperty("hp");
                                if (hp != null && hp.CanWrite)
                                    hp.SetValue(c, 9999, null);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { /* safe */ }
        }

        private void ApplyFunTweaks()
        {
            try
            {
                // Speed multiplier applied to common character controllers via reflection
                if (Mathf.Abs(speedMult - 1.0f) > 0.01f)
                {
                    foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
                    {
                        if (go == null) continue;
                        UnityEngine.Component[] comps = go.GetComponents<UnityEngine.Component>();
                        foreach (var comp in comps)
                        {
                            if (comp == null) continue;
                            Type t = comp.GetType();
                            // Common Unity CharacterController / FPS controllers
                            if (t.Name.Contains("CharacterController") || t.Name.Contains("FPS") || t.Name.Contains("Move"))
                            {
                                try
                                {
                                    PropertyInfo sp = t.GetProperty("speed", typeof(float)) ?? t.GetProperty("Speed");
                                    if (sp != null && sp.CanWrite)
                                        sp.SetValue(comp, speedMult * 5f, null);
                                }
                                catch { }
                            }
                        }
                    }
                }

                // Fly: manipulate CharacterController / Rigidbody
                if (flyEnabled)
                {
                    foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
                    {
                        if (go == null) continue;
                        Rigidbody rb = go.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.useGravity = false;
                            rb.velocity = new Vector3(rb.velocity.x, 1.0f, rb.velocity.z); // gentle float
                        }
                        CharacterController cc = go.GetComponent<CharacterController>();
                        if (cc != null)
                        {
                            cc.Move(new Vector3(0, 0.05f, 0));
                        }
                    }
                }

                // Infinite jump
                if (infiniteJump)
                {
                    // Best-effort: set jump count high if exists, else rely on constant velocity boost
                }
            }
            catch { }
        }

        private void AutoConnectSkip()
        {
            try
            {
                string[] skipNames = new string[] { "Title", "Lobby", "Login", "StartScreen", "MenuScreen", "MainMenu", "PlayButton", "ConnectButton", "LoginButton" };
                foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
                {
                    if (go == null) continue;
                    string n = go.name;
                    foreach (string s in skipNames)
                    {
                        if (n.IndexOf(s, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            go.SetActive(false);
                            UnityEngine.UI.Button btn = go.GetComponent<UnityEngine.UI.Button>();
                            if (btn != null && btn.onClick != null) btn.onClick.Invoke();
                        }
                    }
                }
                foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
                {
                    if (go == null) continue;
                    UnityEngine.Component[] comps = go.GetComponents<UnityEngine.Component>();
                    foreach (UnityEngine.Component c in comps)
                    {
                        if (c == null) continue;
                        System.Type t = c.GetType();
                        if (t.Name.Contains("Network") || t.Name.Contains("Connect") || t.Name.Contains("Lobby"))
                        {
                            System.Reflection.MethodInfo m = t.GetMethod("Connect", System.Type.EmptyTypes);
                            if (m != null) { try { m.Invoke(c, null); } catch { } }
                        }
                    }
                }
            }
            catch { }
        }

                // ============== PANKAKE / FLATSCREEN VR MOCK ==============
        private void MockFlatScreenVR()
        {
            mockHandsActive = true;

            try
            {
                // 1. Force camera flat (disable VR stereo if present)
                Camera cam = Camera.main;
                if (cam != null)
                {
                    // Try to disable stereo rendering (Unity VR)
                    // In older Unity, stereoEnabled property exists
                    try
                    {
                        PropertyInfo stereoProp = typeof(Camera).GetProperty("stereoEnabled");
                        if (stereoProp != null && stereoProp.CanWrite)
                            stereoProp.SetValue(cam, false, null);
                    }
                    catch { }

                    // Also try to set camera transform to flat forward
                    cam.transform.localRotation = Quaternion.identity;
                }

                // 2. WASD Hand Control simulation
                float h = Input.GetAxis("Horizontal"); // A/D
                float v = Input.GetAxis("Vertical");   // W/S

                // Find VR hand objects by common names
                GameObject leftHand = FindByNameContains("LeftHand") ?? FindByNameContains("Hand_L") ?? FindByNameContains("LHand");
                GameObject rightHand = FindByNameContains("RightHand") ?? FindByNameContains("Hand_R") ?? FindByNameContains("RHand");

                // Apply WASD to hands with strength multiplier
                Vector3 delta = new Vector3(h * handWASDStrength * Time.deltaTime * 2f, 0f, v * handWASDStrength * Time.deltaTime * 2f);

                if (leftHand != null && leftHand.transform != null)
                {
                    leftHand.transform.position += delta * 0.6f;
                    // Add slight rotation to simulate hand posture
                    leftHand.transform.localRotation = Quaternion.Euler(15f + h * 10f, 20f + v * 10f, 0f);
                }
                if (rightHand != null && rightHand.transform != null)
                {
                    rightHand.transform.position += delta * 0.6f;
                    rightHand.transform.localRotation = Quaternion.Euler(15f - h * 10f, -20f - v * 10f, 0f);
                }

                // 3. If no hand objects found, create visual proxy (optional) - here we just modify camera view to simulate flat VR
                // We can also apply to any object with "Hand" in name
                foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
                {
                    if (go == null) continue;
                    string n = go.name.ToLower();
                    if (n.Contains("hand") || n.Contains("controller") || n.Contains("ovr") || n.Contains("vr"))
                    {
                        if (go.transform != null)
                        {
                            go.transform.position += delta * 0.4f;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Silent fail to avoid spam
            }
        }

        private GameObject FindByNameContains(string sub)
        {
            foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                if (go != null && go.name.IndexOf(sub, StringComparison.OrdinalIgnoreCase) >= 0)
                    return go;
            }
            return null;
        }

    } // end class

} // end namespace

