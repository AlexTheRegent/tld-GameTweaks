using System.Xml;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;

static class MapTweaks
{
    static string configFileName = "MapTweaks.xml";
    static float drawingRange = 1000f;
    static float autodrawDelay = 5f;
    static float lastDrawTime = 0f;

    static public void OnLoad()
    {
        Debug.LogFormat("MapTweaks: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        if (!GetNodeFloat(xml.SelectSingleNode("/config/drawing_range"), out drawingRange))
        {
            Debug.LogFormat("MapTweaks: missing/invalid 'drawing_range' entry");
            drawingRange = 1000f;
        }

        if (!GetNodeFloat(xml.SelectSingleNode("/config/autodraw_delay"), out autodrawDelay))
        {
            Debug.LogFormat("MapTweaks: missing/invalid 'autodraw_delay' entry");
            autodrawDelay = 5f;
        }
    }

    static private bool GetNodeBool(XmlNode node, out bool value)
    {
        if (node == null || node.Attributes["value"] == null || !bool.TryParse(node.Attributes["value"].Value, out value))
        {
            value = false;
            return false;
        }

        return true;
    }

    static private bool GetNodeFloat(XmlNode node, out float value)
    {
        if (node == null || node.Attributes["value"] == null || !float.TryParse(node.Attributes["value"].Value, out value))
        {
            value = -1f;
            return false;
        }

        return true;
    }
    
    [HarmonyPatch(typeof(Panel_Map), "DoNearbyDetailsCheck")]
    public class MapTweaksDrawing
    {
        public static void Prefix(ref float radius)
        {
            if (drawingRange >= 0f)
            {
                Debug.LogFormat("MapTweaks: DoNearbyDetailsCheck()");
                radius = drawingRange;
            }
        }
    }

    [HarmonyPatch(typeof(HUDManager), "UpdateCrosshair")]
    public class MapTweaksAutoDraw
    {
        public static void Postfix()
        {
            if (autodrawDelay >= 0f)
            {
                lastDrawTime += Time.deltaTime;
                if (lastDrawTime >= autodrawDelay)
                {
                    Debug.LogFormat("MapTweaks: UpdateCrosshair");
                    InterfaceManager.m_Panel_Map.DoNearbyDetailsCheck(InterfaceManager.m_Panel_Map.m_DetailSurveyRadiusMeters);
                    lastDrawTime = 0f;
                }
            }
        }
    }
}