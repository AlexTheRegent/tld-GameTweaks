using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModSettings;

static class FasterActionRedux
{
    static float containerTime = 0.01f;
    static float actionTime = 0.1f;

    internal class FasterActionReduxSettings : ModSettingsBase
    {
        [Name("Container opening time")]
        [Description("Default value is varies for different actions, seconds")]
        [Slider(0.01f, 1f, 100)]
        public float containerTime = 0.01f;

        [Name("Action time")]
        [Description("Default value is varies for different actions, seconds")]
        [Slider(0.1f, 2f, 200)]
        public float actionTime = 0.1f;

        protected override void OnConfirm()
        {
            FasterActionRedux.containerTime = containerTime;
            FasterActionRedux.actionTime = actionTime;

            string settings = FastJson.Serialize(this);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FasterActionRedux.json"), settings);
        }
    }

    static public void OnLoad()
    {
        Debug.LogFormat("FasterActionRedux: init");

        FasterActionReduxSettings settings = new FasterActionReduxSettings();
        string opts = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FasterActionRedux.json"));
        settings = FastJson.Deserialize<FasterActionReduxSettings>(opts);
        settings.AddToModSettings("Faster Actions Redux");

        containerTime = settings.containerTime;
        actionTime = settings.actionTime;
    }

    // time for these actions is hardcoded or calculated on fly 
    // determine  action type and override time in call to progressbar 
    private enum FasterGenericAction
    {
        NONE,
        FIRE_START,
        FISHING,
        CLEAR_FISHING_HOLE,
        REFUEL,
        PICK_WATER,
        DRINK,
        OPEN_CAN,
        FIRST_AID,
        EAT,
        SMASH,
        PURIFY_WATER
    }

    private static FasterGenericAction lastAction = FasterGenericAction.NONE;

    // Containers 
    [HarmonyPatch(typeof(Container), "BeginContainerInteraction")]
    public class FasterActionContainer
    {
        public static void Prefix(ref float searchTimeSeconds)
        {
            Debug.LogFormat("Container::BeginContainerInteraction");
            searchTimeSeconds = containerTime; 
        }
    }

    // Harvesting dead animal 
    [HarmonyPatch(typeof(Panel_BodyHarvest), "StartHarvest")]
    public class FasterActionHarvest
    {
        public static void Prefix(Panel_BodyHarvest __instance)
        {
            Debug.LogFormat("Panel_BodyHarvest::StartHarvest");
            __instance.m_HarvestTimeSeconds = actionTime; 
        }
    }

    // Quartering
    [HarmonyPatch(typeof(Panel_BodyHarvest), "StartQuarter")]
    public class FasterActionQuarter
    {
        public static void Prefix(Panel_BodyHarvest __instance)
        {
            Debug.LogFormat("Panel_BodyHarvest::StartQuarter");
            __instance.m_HarvestTimeSeconds = actionTime;
        }
    }

    // Breaking down items  
    [HarmonyPatch(typeof(Panel_BreakDown), "OnBreakDown")]
    public class FasterActionBreakDown
    {
        public static void Prefix(Panel_BreakDown __instance)
        {
            Debug.LogFormat("Panel_BreakDown::OnBreakDown");
            __instance.m_SecondsToBreakDown = actionTime; 
        }
    }

    // Mapping 
    [HarmonyPatch(typeof(CharcoalItem), "StartDetailSurvey")]
    public class FasterActionMapping
    {
        public static void Prefix(CharcoalItem __instance)
        {
            Debug.LogFormat("CharcoalItem::StartDetailSurvey");
            __instance.m_SurveyRealSeconds = actionTime; // const value 
        }
    }

    // Crafting  
    [HarmonyPatch(typeof(Panel_Crafting), "StartCrafting")]
    public class FasterActionCrafting
    {
        public static void Prefix(Panel_Crafting __instance)
        {
            Debug.LogFormat("Panel_Crafting::StartCrafting");
            __instance.m_CraftingTimeSeconds = actionTime; 
        }
    }

    // Cooking    
    [HarmonyPatch(typeof(Panel_FeedFire), "StartCooking")]
    public class FasterActionCooking
    {
        public static void Prefix(Panel_FeedFire __instance)
        {
            Debug.LogFormat("Panel_FeedFire::StartCooking");
            __instance.m_CookProgressBarSeconds = actionTime; 
        }
    }

    // Water boiling  
    [HarmonyPatch(typeof(Panel_FeedFire), "StartBoil")]
    public class FasterActionWaterBoiling
    {
        public static void Prefix(Panel_FeedFire __instance)
        {
            Debug.LogFormat("Panel_FeedFire::StartBoil");
            __instance.m_BoilProgressBarSeconds = actionTime; 
        }
    }

    // Fire starting    
    [HarmonyPatch(typeof(Fire), "PlayerBeginCreate")]
    public class FasterActionFireStarting
    {
        public static void Prefix()
        {
            Debug.LogFormat("Fire::PlayerBeginCreate");
            lastAction = FasterGenericAction.FIRE_START;
        }
    }

    // Fishing  
    [HarmonyPatch(typeof(Panel_IceFishing), "OnFish")]
    public class FasterActionFishing
    {
        public static void Prefix(Panel_IceFishing __instance)
        {
            Debug.LogFormat("Panel_IceFishing::OnFish");
            lastAction = FasterGenericAction.FISHING;
        }
    }

    // Ice hole cleaning 
    [HarmonyPatch(typeof(Panel_IceFishingHoleClear), "UseTool")]
    public class FasterActionIceHoleCleaning
    {
        public static void Prefix(Panel_IceFishingHoleClear __instance)
        {
            Debug.LogFormat("Panel_IceFishingHoleClear::UseTool");
            lastAction = FasterGenericAction.CLEAR_FISHING_HOLE;
        }
    }

    // Refuel 
    [HarmonyPatch(typeof(Panel_Inventory_Examine), "OnRefuel")]
    public class FasterActionRefuel
    {
        public static void Prefix(Panel_Inventory_Examine __instance)
        {
            Debug.LogFormat("Panel_Inventory_Examine::OnRefuel");
            lastAction = FasterGenericAction.REFUEL;
        }
    }
    
    // Water pickup 
    [HarmonyPatch(typeof(Panel_PickWater), "TakeWater")]
    public class FasterActionWaterPickup
    {
        public static void Prefix(Panel_PickWater __instance)
        {
            Debug.LogFormat("Panel_PickWater::TakeWater");
            lastAction = FasterGenericAction.PICK_WATER;
        }
    }

    // Drinking
    [HarmonyPatch(typeof(PlayerManager), "DrinkFromWaterSupply")]
    public class FasterActionDrinking
    {
        public static void Prefix(ref WaterSupply ws)
        {
            Debug.LogFormat("PlayerManager::DrinkFromWaterSupply");
            ws.m_TimeToDrinkSeconds = actionTime;
            lastAction = FasterGenericAction.DRINK;
        }
    }

    // Can opening
    [HarmonyPatch(typeof(PlayerManager), "OpenAndUseFoodInventoryItem")]
    public class FasterActionCanOpening
    {
        public static void Prefix()
        {
            Debug.LogFormat("PlayerManager::OpenAndUseFoodInventoryItem");
            lastAction = FasterGenericAction.OPEN_CAN;
        }
    }

    // First Aid        
    [HarmonyPatch(typeof(PlayerManager), "TreatAfflictionWithFirstAid")]
    public class FasterActionFirstAid
    {
        public static void Prefix(ref FirstAidItem firstAidItem)
        {
            Debug.LogFormat("PlayerManager::TreatAfflictionWithFirstAid");
            lastAction = FasterGenericAction.FIRST_AID;
        }
    }

    // Eating food 
    [HarmonyPatch(typeof(PlayerManager), "UseFoodInventoryItem")]
    public class FasterActionEating
    {
        public static void Prefix(ref GearItem gi)
        {
            Debug.LogFormat("PlayerManager::UseFoodInventoryItem");
            gi.m_FoodItem.m_TimeToEatSeconds = actionTime;
            lastAction = FasterGenericAction.EAT;
        }
    }

    // Smashing cans 
    [HarmonyPatch(typeof(PlayerManager), "UseSmashableItem")]
    public class FasterActionSmashing
    {
        public static void Prefix(ref GearItem gi)
        {
            Debug.LogFormat("PlayerManager::UseSmashableItem");
            lastAction = FasterGenericAction.SMASH;
        }
    }

    // Purifying water       
    [HarmonyPatch(typeof(PlayerManager), "UseWaterPurificationItem")]
    public class FasterActionPurifyingWater
    {
        public static void Prefix(ref GearItem gi)
        {
            Debug.LogFormat("PlayerManager::UseWaterPurificationItem");
            lastAction = FasterGenericAction.PURIFY_WATER;
        }
    }
    
    // Harvesting (what?)
    [HarmonyPatch(typeof(Panel_Harvest), "StartHarvest")]
    public class FasterActionHarvesting
    {
        public static void Prefix(Panel_Harvest __instance)
        {
            Debug.LogFormat("Panel_Harvest::StartHarvest");
            __instance.m_HarvestTimeSeconds = actionTime;
        }
    }

    // Harvesting plants, etc
    [HarmonyPatch(typeof(Harvestable), "DoHarvest")]
    public class FasterActionHarvestingPlant
    {
        public static void Prefix(Harvestable __instance)
        {
            Debug.LogFormat("Harvestable::DoHarvest");
            __instance.m_SecondsToHarvest = actionTime;
        }

    }

    // fix plant harvesting by TheWyrdsmith
    // https://github.com/AlexTheRegent/tld-GameTweaks/issues/5
    // remove item text otherwise it will be stuck on screen 
    // [HarmonyPatch(typeof(Harvestable), "CompletedHarvest")]
    // public class FasterActionHarvestingPlantFix
    // { 
        // public static void Postfix(Harvestable __instance)
        // {
            // Debug.LogFormat("Harvestable::CompletedHarvest");
            // InterfaceManager.m_Panel_HUD.SetHoverText(string.Empty, null, false);
        // }
    // }
    
    // Weapon cleaning  
    [HarmonyPatch(typeof(Panel_Inventory_Examine), "StartClean")]
    public class FasterActionWeaponCleaning
    {
        public static void Prefix(Panel_Inventory_Examine __instance)
        {
            Debug.LogFormat("Panel_Inventory_Examine::StartClean");
            __instance.m_CleanTimeSeconds = actionTime;
        }
    }
    
    // Scrapping items in inventory 
    [HarmonyPatch(typeof(Panel_Inventory_Examine), "StartHarvest")]
    public class FasterActionSrapping
    {
        public static void Prefix(Panel_Inventory_Examine __instance)
        {
            Debug.LogFormat("Panel_Inventory_Examine::StartHarvest");
            __instance.m_HarvestTimeSeconds = actionTime;
        }
    }
    
    // Reading books
    [HarmonyPatch(typeof(Panel_Inventory_Examine), "StartRead")]
    public class FasterActionReading
    {
        public static void Prefix(Panel_Inventory_Examine __instance)
        {
            Debug.LogFormat("Panel_Inventory_Examine::StartRead");
            __instance.m_ReadTimeSeconds = actionTime;
        }
    }
    
    // Repairing (probably for items in inventory)
    [HarmonyPatch(typeof(Panel_Inventory_Examine), "StartRepair")]
    public class FasterActionRepairing
    {
        public static void Prefix(Panel_Inventory_Examine __instance)
        {
            Debug.LogFormat("Panel_Inventory_Examine::StartRepair");
            __instance.m_RepairTimeSeconds = actionTime;
        }
    }
    
    // Sharpening     
    [HarmonyPatch(typeof(Panel_Inventory_Examine), "StartSharpen")]
    public class FasterActionSharpening
    {
        public static void Prefix(Panel_Inventory_Examine __instance)
        {
            Debug.LogFormat("Panel_Inventory_Examine::StartSharpen");
            __instance.m_SharpenTimeSeconds = actionTime;
        }
    }
    
    // Repairing for ?
    [HarmonyPatch(typeof(Panel_Repair), "StartRepair")]
    public class FasterActionRepairing2
    {
        public static void Prefix(Panel_Repair __instance)
        {
            Debug.LogFormat("Panel_Repair::StartRepair");
            __instance.m_RepairTimeSeconds = actionTime;
        }
    }
    
    // Rope deploy and removal       
    [HarmonyPatch(typeof(RopeAnchorPoint), "ActionStarted")]
    public class FasterActionRoping
    {
        public static void Prefix(RopeAnchorPoint __instance)
        {
            Debug.LogFormat("RopeAnchorPoint::ActionStarted");
            __instance.m_ProgressBarDurationSeconds = actionTime;
        }
    }
    
    // Passing time      
    [HarmonyPatch(typeof(PassTime), "Begin")]
    public class FasterActionPassingTime
    {
        public static void Prefix(PassTime __instance)
        {
            Debug.LogFormat("PassTime::Begin");
            __instance.m_PassTimeRealtimeSecondsPerHour = actionTime;
        }
    }

    // Sleeping       
    [HarmonyPatch(typeof(Rest), "Start")]
    public class FasterActionSleeping
    {
        public static void Prefix(Rest __instance)
        {
            Debug.LogFormat("Rest::Start");
            __instance.m_SleepFadeOutSeconds = actionTime;
        }
    }

    // Unlocking containers        
    [HarmonyPatch(typeof(Lock), "StartInteract")]
    public class FasterActionPrybarUnlocking
    {
        public static void Prefix(Lock __instance)
        {
            Debug.LogFormat("Lock::StartInteract");
            __instance.m_ForceLockDurationSecondsMin = actionTime;
            __instance.m_ForceLockDurationSecondsMax = actionTime;
        }
    }

    // safe?
    [HarmonyPatch(typeof(OpenClose), "StartInteract")]
    public class FasterActionDoor1
    {
        public static void Prefix()
        {
            Debug.LogFormat("OpenClose::StartInteract");
            GameManager.GetInterfaceManager().m_DoorInteractSeconds = actionTime;
        }
    }
    // probably vehicle door
    [HarmonyPatch(typeof(VehicleDoor), "StartInteract")]
    public class FasterActionDoor2
    {
        public static void Prefix()
        {
            Debug.LogFormat("VehicleDoor::StartInteract");
            GameManager.GetInterfaceManager().m_DoorInteractSeconds = actionTime;
        }
    }
    // idk what it is 
    [HarmonyPatch(typeof(LoadingZone), "StartInteract")]
    public class FasterActionDoor3
    {
        public static void Prefix()
        {
            Debug.LogFormat("LoadingZone::StartInteract");
            GameManager.GetInterfaceManager().m_SceneTransitionSeconds = actionTime * 2;
            GameManager.m_SceneTransitionFadeOutTime = actionTime;
        }
    }
    // doors/entrance between maps/buildings 
    [HarmonyPatch(typeof(LoadScene), "StartInteract")]
    public class FasterActionDoor4
    {
        public static void Prefix()
        {
            Debug.LogFormat("LoadScene::StartInteract");
            GameManager.GetInterfaceManager().m_SceneTransitionSeconds = actionTime * 2;
            GameManager.m_SceneTransitionFadeOutTime = actionTime;
        }
    }

    // Generic actions (hardcoded mess)
    [HarmonyPatch(typeof(Panel_GenericProgressBar), "Launch")]
    public class FasterActionGeneric
    {
        public static void Prefix(ref float seconds)
        {
            Debug.LogFormat("Panel_GenericProgressBar::Launch");
            // accelerate only known actions 
            if (lastAction != FasterGenericAction.NONE)
            {
                lastAction = FasterGenericAction.NONE;
                seconds = actionTime;
            }
        }
    }

    // Disable fading becaused it wont disappear after action 
    [HarmonyPatch(typeof(TimeOfDay), "Accelerate")]
    public class FasterActionAccelerate
    {
        public static void Prefix(ref float realTimeSeconds, ref float gameTimeHours, ref bool doFadeToBlack)
        {
            Debug.LogFormat("TimeOfDay::Accelerate");
            if (realTimeSeconds < 1.0f)
            {
                doFadeToBlack = false;
            }
        }
    }
}
