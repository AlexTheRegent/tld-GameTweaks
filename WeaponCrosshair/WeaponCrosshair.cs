using System.Xml;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;

static class WeaponCrosshair
{
    static string configFileName = "WeaponCrosshair.xml";
    static bool stoneCrosshair = false;
    static bool rifleCrosshair = false;
    static bool bowCrosshair = false;

    static public void OnLoad()
    {
        Debug.LogFormat("WeaponCrosshair: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        if (!GetNodeBool(xml.SelectSingleNode("/config/stone_crosshair"), out stoneCrosshair))
        {
            Debug.LogFormat("WeaponCrosshair: missing/invalid 'stone_crosshair' entry");
            stoneCrosshair = false;
        }
        if (!GetNodeBool(xml.SelectSingleNode("/config/rifle_crosshair"), out rifleCrosshair))
        {
            Debug.LogFormat("WeaponCrosshair: missing/invalid 'rifle_crosshair' entry");
            rifleCrosshair = false;
        }
        if (!GetNodeBool(xml.SelectSingleNode("/config/bow_crosshair"), out bowCrosshair))
        {
            Debug.LogFormat("WeaponCrosshair: missing/invalid 'bow_crosshair' entry");
            bowCrosshair = false;
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

    [HarmonyPatch(typeof(HUDManager), "UpdateCrosshair")]
    public class WeaponCrosshairUpdate
    {
        public static void Postfix(HUDManager __instance)
        {
            if (GameManager.GetPlayerManagerComponent().PlayerIsZooming())
            {
                GearItem itemInHands = GameManager.GetPlayerManagerComponent().m_ItemInHands;
                if ((stoneCrosshair && itemInHands.m_StoneItem) || (rifleCrosshair && itemInHands.m_GunItem) || (bowCrosshair && itemInHands.m_BowItem))
                {
                    Utils.SetActive(InterfaceManager.m_Panel_HUD.m_Sprite_Crosshair.gameObject, true);
                    InterfaceManager.m_Panel_HUD.m_Sprite_Crosshair.alpha = 1f;
                }
            }
        }
    }
}
