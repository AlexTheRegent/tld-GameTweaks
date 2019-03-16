using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModSettings;

static class MapTweaks
{
    static bool overrideDrawingRange = true;
    static float drawingRange = 150f;
    static bool autodrawEnabled = true;
    static float autodrawDelay = 5f;

    internal class MapTweakSettings : ModSettingsBase
    {
        [Name("Override drawing range")]
        public bool overrideDrawingRange = true;

        [Name("Drawing range, game units")]
        [Description("Default value is 150 (as of October 29'th, 2018)")]
        [Slider(1, 10000)]
        public int drawingRange = 150;

        // There's an issue with the auto-mapping mod. 
        // Basically, game doesn't save the texture of the uncovered map, 
        // but rather a list of all positions you've mapped in, and then 
        // recreates the map texture every time you load a level. 
        // If you have the mod installed for longer periods of time, your 
        // save file gets huge and the game slows down to a crawl every time 
        // you do a scene transition
        // by zeobviouslyfakeacc
        [Name("Enable autodraw (read description)")]
        [Description("WARNING! This feature temporarily disabled due to technical issues. Details in mod's source code")]
        public bool autodrawEnabled = false;

        [Name("Autodraw delay, seconds")]
        [Description("There is no default value")]
        [Slider(1, 300)]
        public int autodrawDelay = 20;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "overrideDrawingRange")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[1], (bool)newValue);
            }
            else if (field.Name == "autodrawEnabled")
            {
                FieldInfo[] fields = GetType().GetFields();
                // this.SetFieldVisible(fields[3], (bool)newValue);
                this.SetFieldVisible(fields[3], false);
            }
        }

        protected override void OnConfirm()
        {
            MapTweaks.overrideDrawingRange = overrideDrawingRange;
            MapTweaks.drawingRange = drawingRange;

            // force disable 
            MapTweaks.autodrawEnabled = false;
            MapTweaks.autodrawDelay = autodrawDelay;

            string settings = FastJson.Serialize(this);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MapTweaks.json"), settings);
        }
    }

    static public void OnLoad()
    {
        Debug.LogFormat("MapTweaks: init");

        MapTweakSettings settings = new MapTweakSettings();
        string opts = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MapTweaks.json"));
        settings = FastJson.Deserialize<MapTweakSettings>(opts);
        settings.AddToModSettings("Map Tweaks");

        overrideDrawingRange = settings.overrideDrawingRange;
        drawingRange = settings.drawingRange;
        autodrawEnabled = settings.autodrawEnabled;
        autodrawDelay = settings.autodrawDelay;
    }
    
    [HarmonyPatch(typeof(Panel_Map), "DoNearbyDetailsCheck")]
    public class MapTweaksDrawing
    {
        public static void Prefix(ref float radius)
        {
            if (overrideDrawingRange && drawingRange >= 0f)
            {
                // Debug.LogFormat("MapTweaks: DoNearbyDetailsCheck()");
                radius = drawingRange;
            }
        }
    }

    static float lastDrawTime = 0f;

    [HarmonyPatch(typeof(HUDManager), "UpdateCrosshair")]
    public class MapTweaksAutoDraw
    {
        public static void Postfix()
        {
            if (autodrawEnabled && autodrawDelay >= 0f)
            {
                lastDrawTime += Time.deltaTime;
                if (lastDrawTime >= autodrawDelay)
                {
                    InterfaceManager.m_Panel_Map.DoNearbyDetailsCheck(InterfaceManager.m_Panel_Map.m_DetailSurveyRadiusMeters);
                    lastDrawTime = 0f;
                }
            }
        }
    }
}