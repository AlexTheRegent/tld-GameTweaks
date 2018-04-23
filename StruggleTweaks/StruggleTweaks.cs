using System.Xml;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;

static class StruggleTweaks
{
    static string configFileName = "StruggleTweaks.xml";
    static float chanceToKillWolfAfterStruggle = 100f;
    static float wolfBleedoutMinutes = 1f;
    static float tapIncrement = 1f;

    static public void OnLoad()
    {
        Debug.LogFormat("StruggleTweaks: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        if (!GetNodeFloat(xml.SelectSingleNode("/config/chance_to_kill_wolf_after_struggle"), out chanceToKillWolfAfterStruggle))
        {
            Debug.LogFormat("StruggleTweaks: missing/invalid 'chance_to_kill_wolf_after_struggle' entry");
            chanceToKillWolfAfterStruggle = 100f;
        }
        if (!GetNodeFloat(xml.SelectSingleNode("/config/wolf_bleedout_minutes"), out wolfBleedoutMinutes))
        {
            Debug.LogFormat("StruggleTweaks: missing/invalid 'wolf_bleedout_minutes' entry");
            wolfBleedoutMinutes = 1f;
        }
        if (!GetNodeFloat(xml.SelectSingleNode("/config/tap_increment"), out tapIncrement))
        {
            Debug.LogFormat("StruggleTweaks: missing/invalid 'tap_increment' entry");
            tapIncrement = 1f;
        }
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

    [HarmonyPatch(typeof(PlayerStruggle), "Tap")]
    public class StruggleTweaksTap
    {
        public static void Postfix(PlayerStruggle __instance)
        {
            if (wolfBleedoutMinutes >= 0f)
            {
                __instance.m_WolfBleedoutMinutes = wolfBleedoutMinutes;
            }
            
            if (tapIncrement >= 0f)
            {
                __instance.m_TapIncrement = tapIncrement;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerStruggle), "MakePartnerFlee")]
    public class StruggleTweaksKillOnFlee
    {
        public static void Postfix(PlayerStruggle __instance)
        {
            if (__instance.m_PartnerBaseAi.m_AiWolf)
            {
                if (Utils.RollChance(chanceToKillWolfAfterStruggle))
                {
                    __instance.m_PartnerBaseAi.EnterDead();
                }
            }
        }
    }
}
