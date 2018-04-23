using System.Xml;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;

static class TweakRabbits
{
    static string configFileName = "TweakRabbits.xml";
    static float stunDuration = -1f;
    static bool killOnHit = true;

    static public void OnLoad()
    {
        Debug.LogFormat("TweakRabbits: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        if (!GetNodeFloat(xml.SelectSingleNode("/config/stun_duration"), out stunDuration))
        {
            Debug.LogFormat("TweakRabbits: missing/invalid 'stun_duration' entry");
            stunDuration = -1f;
        }

        if (!GetNodeBool(xml.SelectSingleNode("/config/kill_on_hit"), out killOnHit))
        {
            Debug.LogFormat("TweakRabbits: missing/invalid 'kill_on_hit' entry");
            killOnHit = true;
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

    [HarmonyPatch(typeof(BaseAi), "EnterStunned")]
    public class TweakRabbitsStunDuration
    {
        public static void Prefix(BaseAi __instance)
        {
            if (stunDuration >= 0f)
                __instance.m_StunSeconds = stunDuration;
        }

        public static void Postfix(BaseAi __instance)
        {
            if (killOnHit)
                __instance.EnterDead();
        }
    }
}
