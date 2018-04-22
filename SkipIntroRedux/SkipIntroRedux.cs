using System.Xml;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;

static class SkipIntroRedux
{
    static string configFileName = "SkipIntroRedux.xml";
    static bool skipDisclaimer = true;
    static bool skipIntro = true;
    static bool skipFade = true;

    static SkipIntroRedux()
    {
        Debug.LogFormat("SkipIntroRedux: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        if (!bool.TryParse(xml.SelectSingleNode("/config/skip_disclaimer").Attributes["value"].Value, out skipDisclaimer))
        {
            Debug.LogFormat("FasterActionRedux: missing 'skip_disclaimer' entry");
        }

        if (!bool.TryParse(xml.SelectSingleNode("/config/skip_intro").Attributes["value"].Value, out skipIntro))
        {
            Debug.LogFormat("FasterActionRedux: missing 'skip_intro' entry");
        }

        if (!bool.TryParse(xml.SelectSingleNode("/config/skip_fade").Attributes["value"].Value, out skipFade))
        {
            Debug.LogFormat("FasterActionRedux: missing 'skip_fade' entry");
        }
    }

    // Skip Disclaimer 
    // Seems like they obfuscate call to GAMEPLAY_Disclaimer 
    // Anyway this will work but disclaimer will be seen (until all resources is loaded)
    // So no clicks/keypress required 
    [HarmonyPatch(typeof(BootUpdate), "Start")]
    public class FasterActionSkipDisclaimer
    {
        public static void Prefix()
        {
            if (skipDisclaimer)
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    // Skip Intro 
    [HarmonyPatch(typeof(Panel_MainMenu), "Enable")]
    public class FasterActionSkipIntro
    {
        public static void Prefix(Panel_MainMenu __instance)
        {
            if (skipIntro)
            {
                MoviePlayer.m_HasIntroPlayedForMainMenu = true;
            }
        }
    }

    [HarmonyPatch(typeof(Panel_MainMenu), "UpdateFading")]
    public class FasterActionSkipFade
    {
        public static void Postfix(Panel_MainMenu __instance)
        {
            if (skipFade)
                __instance.m_InitialScreenFadeInDuration = 0f;
        }
    }

    [HarmonyPatch(typeof(Panel_MainMenu), "FadeOutPanel")]
    public class FasterActionSkipFade2
    {
        public static bool Prefix(Panel_MainMenu __instance)
        {
            return false;
        }
    }
}
