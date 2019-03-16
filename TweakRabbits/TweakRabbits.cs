using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModSettings;

static class TweakRabbits
{
    static bool killOnHit = false;
    static float stunDuration = 4f;

    internal class TweakRabbitsSettings : ModSettingsBase
    {
        [Name("Kill rabbit on stone hit")]
        public bool killOnHit = false;

        [Name("Stun duration, seconds")]
        [Description("Default value is 4 (as of October 29'th, 2018)")]
        [Slider(1, 300)]
        public int stunDuration = 4;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "killOnHit")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[1], !(bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            TweakRabbits.killOnHit = killOnHit;
            TweakRabbits.stunDuration = stunDuration;

            string settings = FastJson.Serialize(this);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TweakRabbits.json"), settings);
        }
    }

    static public void OnLoad()
    {
        Debug.LogFormat("TweakRabbits: init");

        TweakRabbitsSettings settings = new TweakRabbitsSettings();
        string opts = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TweakRabbits.json"));
        settings = FastJson.Deserialize<TweakRabbitsSettings>(opts);
        settings.AddToModSettings("Tweak Rabbits");

        killOnHit = settings.killOnHit;
        stunDuration = settings.stunDuration;
    }

    [HarmonyPatch(typeof(BaseAi), "EnterStunned")]
    public class TweakRabbitsStunDuration
    {
        public static void Prefix(BaseAi __instance)
        {
            if (__instance.m_AiRabbit != null)
            {
                if (stunDuration >= 0f)
                {
                    Debug.LogFormat("TweakRabbits: stun duration {0}", __instance.m_StunSeconds);
                    __instance.m_StunSeconds = stunDuration;
                }
            }
        }

        public static void Postfix(BaseAi __instance)
        {
            if (__instance.m_AiRabbit != null)
            {
                if (killOnHit)
                {
                    __instance.EnterDead();
                }
            }
        }
    }
}
