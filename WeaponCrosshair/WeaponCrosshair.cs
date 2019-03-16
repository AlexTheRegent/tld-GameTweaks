using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModSettings;

static class WeaponCrosshair
{
    static bool stoneCrosshair = false;
    static bool rifleCrosshair = false;
    static bool bowCrosshair = false;

    internal class WeaponCrosshairSettings : ModSettingsBase
    {
        [Name("Stone crosshair")]
        [Description("Default value is false")]
        public bool stoneCrosshair = true;

        [Name("Rifle crosshair")]
        [Description("Default value is false")]
        public bool rifleCrosshair = false;

        [Name("Bow crosshair")]
        [Description("Default value is false")]
        public bool bowCrosshair = false;

        protected override void OnConfirm()
        {
            WeaponCrosshair.stoneCrosshair = stoneCrosshair;
            WeaponCrosshair.rifleCrosshair = rifleCrosshair;
            WeaponCrosshair.bowCrosshair = bowCrosshair;

            string settings = FastJson.Serialize(this);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "WeaponCrosshair.json"), settings);
        }
    }

    static public void OnLoad()
    {
        Debug.LogFormat("WeaponCrosshair: init");

        WeaponCrosshairSettings settings = new WeaponCrosshairSettings();
        string opts = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "WeaponCrosshair.json"));
        settings = FastJson.Deserialize<WeaponCrosshairSettings>(opts);
        settings.AddToModSettings("Weapon Crosshair");

        stoneCrosshair = settings.stoneCrosshair;
        rifleCrosshair = settings.rifleCrosshair;
        bowCrosshair = settings.bowCrosshair;
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
