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
    static bool fakeInput = false;

    static public void OnLoad()
    {
        Debug.LogFormat("SkipIntroRedux: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        if (!GetNodeBool(xml.SelectSingleNode("/config/skip_disclaimer"), out skipDisclaimer))
        {
            Debug.LogFormat("SkipIntroRedux: missing/invalid 'skip_disclaimer' entry");
            skipDisclaimer = true;
        }

        if (!GetNodeBool(xml.SelectSingleNode("/config/skip_intro"), out skipIntro))
        {
            Debug.LogFormat("SkipIntroRedux: missing/invalid 'skip_intro' entry");
            skipIntro = true;
        }

        if (!GetNodeBool(xml.SelectSingleNode("/config/skip_fade"), out skipFade))
        {
            Debug.LogFormat("SkipIntroRedux: missing/invalid 'skip_fade' entry");
            skipFade = true;
        }
    }

    static private bool GetNodeBool(XmlNode node, out bool value)
    {
        if (node==null || node.Attributes["value"] == null || !bool.TryParse(node.Attributes["value"].Value, out value))
        {
            value = false;
            return false;
        }

        return true;
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
            if (skipFade)
            {
                __instance.m_StartFadedOut = true;
            }
        }
    }
    
    [HarmonyPatch(typeof(Panel_MainMenu), "UpdateFading")]
    public class FasterActionSkipFade
    {
        public static void Prefix(Panel_MainMenu __instance)
        {
            fakeInput = true;
            __instance.m_FadeInTimeSeconds = 0.00001f;
            __instance.m_InitialScreenFadeInDuration = 0f;
        }
    }

    [HarmonyPatch(typeof(InputManager), "AnyInput")]
    public class FasterActionFakeInput
    {
        public static void Postfix(ref bool __result)
        {
            if (fakeInput)
            {
                fakeInput = false;
                __result = true;
            }
        }
    }
}
