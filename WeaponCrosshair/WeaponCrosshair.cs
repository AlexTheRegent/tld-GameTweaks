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

    static WeaponCrosshair()
    {
        Debug.LogFormat("WeaponCrosshair: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        if (!bool.TryParse(xml.SelectSingleNode("/config/stone_crosshair").Attributes["value"].Value, out stoneCrosshair))
        {
            Debug.LogFormat("TweakRabbits: missing 'stone_crosshair' entry");
        }
        if (!bool.TryParse(xml.SelectSingleNode("/config/rifle_crosshair").Attributes["value"].Value, out rifleCrosshair))
        {
            Debug.LogFormat("TweakRabbits: missing 'rifle_crosshair' entry");
        }
        if (!bool.TryParse(xml.SelectSingleNode("/config/bow_crosshair").Attributes["value"].Value, out bowCrosshair))
        {
            Debug.LogFormat("TweakRabbits: missing 'bow_crosshair' entry");
        }
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
