using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModSettings;

static class StruggleTweaks
{
    static float chanceToKillWolfAfterStruggle = 100f;
    static float wolfBleedoutMinutes = 1f;
    static float tapIncrement = 1f;

    internal class StruggleTweaksSettings : ModSettingsBase
    {
        [Name("Change to instantly kill wolf after struggle")]
        [Description("Default value is 0")]
        [Slider(0f, 100f, 101)]
        public float chanceToKillWolfAfterStruggle = 100f;

        [Name("Wolf bleedout time, in-game minutes")]
        [Description("0 = default bleadout time")]
        [Slider(0f, 500f, 501)]
        public float wolfBleedoutMinutes = 30f;

        [Name("Tap increment")]
        [Description("Default value varies from weapon to weapon, multiplier (0 = disabled, 1 = 100%, 2 = 200%)")]
        [Slider(0f, 20f, 201)]
        public float tapIncrement = 1f;

        protected override void OnConfirm()
        {
            StruggleTweaks.chanceToKillWolfAfterStruggle = chanceToKillWolfAfterStruggle;
            StruggleTweaks.wolfBleedoutMinutes = wolfBleedoutMinutes;
            StruggleTweaks.tapIncrement = tapIncrement;

            string settings = FastJson.Serialize(this);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "StruggleTweaks.json"), settings);
        }
    }

    static public void OnLoad()
    {
        Debug.LogFormat("StruggleTweaks: init");

        StruggleTweaksSettings settings = new StruggleTweaksSettings();
        string opts = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "StruggleTweaks.json"));
        settings = FastJson.Deserialize<StruggleTweaksSettings>(opts);
        settings.AddToModSettings("Struggle Tweaks");

        chanceToKillWolfAfterStruggle = settings.chanceToKillWolfAfterStruggle;
        wolfBleedoutMinutes = settings.wolfBleedoutMinutes;
        tapIncrement = settings.tapIncrement;
    }


    [HarmonyPatch(typeof(PlayerStruggle), "WolfTap")]
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
