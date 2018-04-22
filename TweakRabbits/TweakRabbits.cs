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

    static TweakRabbits()
    {
        Debug.LogFormat("TweakRabbits: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        if (!float.TryParse(xml.SelectSingleNode("/config/stun_duration").Attributes["value"].Value, out stunDuration))
        {
            Debug.LogFormat("TweakRabbits: missing 'stun_duration' entry");
            stunDuration = -1f;
        }

        if (!bool.TryParse(xml.SelectSingleNode("/config/kill_on_hit").Attributes["value"].Value, out killOnHit))
        {
            Debug.LogFormat("TweakRabbits: missing 'kill_on_hit' entry");
        }
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
