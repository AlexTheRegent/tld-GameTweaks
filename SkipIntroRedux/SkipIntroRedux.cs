using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModSettings;

static class SkipIntroRedux
{
    static bool skipDisclaimer = true;
    static bool skipIntro = true;
    static bool skipFade = true;
    static bool fakeInput = false;

    internal class SkipIntroReduxSettings : ModSettingsBase
    {
        [Name("Skip disclaimer")]
        public bool skipDisclaimer = true;

        [Name("Skip intro")]
        public bool skipIntro = true;

        [Name("Skip main menu fade")]
        public bool skipFade = true;

        protected override void OnConfirm()
        {
            SkipIntroRedux.skipDisclaimer = skipDisclaimer;
            SkipIntroRedux.skipIntro = skipIntro;
            SkipIntroRedux.skipFade = skipFade;

            string settings = FastJson.Serialize(this);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SkipIntroRedux.json"), settings);
        }
    }

    static public void OnLoad()
    {
        Debug.LogFormat("SkipIntroRedux: init");

        SkipIntroReduxSettings settings = new SkipIntroReduxSettings();
        string opts = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SkipIntroRedux.json"));
        settings = FastJson.Deserialize<SkipIntroReduxSettings>(opts);
        settings.AddToModSettings("Skip Intro Redux", MenuType.MainMenuOnly);

        skipDisclaimer = settings.skipDisclaimer;
        skipIntro = settings.skipIntro;
        skipFade = settings.skipFade;
    }

    // Skip Disclaimer 
    // Seems like they obfuscate call to GAMEPLAY_Disclaimer 
    // Anyway this will work but disclaimer will be seen (until all resources is loaded)
    // So no clicks/keypress required 
    [HarmonyPatch(typeof(BootUpdate), "Start")]
    public class SkipIntroReduxSkipDisclaimer
    {
        public static void Prefix()
        {
            if (skipDisclaimer)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
    }

    // Skip Intro 
    [HarmonyPatch(typeof(Panel_MainMenu), "Enable")]
    public class SkipIntroReduxSkipIntro
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
    public class SkipIntroReduxSkipFade
    {
        public static void Prefix(Panel_MainMenu __instance)
        {
            fakeInput = true;
            __instance.m_FadeInTimeSeconds = 0.00001f;
            __instance.m_InitialScreenFadeInDuration = 0f;
        }
    }

    [HarmonyPatch(typeof(InputManager), "AnyInput")]
    public class SkipIntroReduxFakeInput
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
