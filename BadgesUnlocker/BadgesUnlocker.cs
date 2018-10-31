using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModSettings;
using System;
using System.Collections;

static class BadgesUnlocker
{
    internal class BadgesUnlockerSettings : ModSettingsBase
    {
        [Section("Feats")]

        [Name("Feat \"Book Smart\"")]
        [Description("Locking/unlocking feat WILL RESET YOUR CURRENT FEAT PROGRESS! BE CAREFULLY!")]
        [Choice("Do not change", "Edit progress")]
        public int feat_book_smart = 0;

        [Name("\"Book Smart\" progress")]
        [Description("0 = 0%, 100 = 100% (unlock)")]
        [Slider(0, 100)]
        public int feat_book_smart_progress = 100;

        [Name("Feat \"Cold Fusion\"")]
        [Description("Locking/unlocking feat WILL RESET YOUR CURRENT FEAT PROGRESS! BE CAREFULLY!")]
        [Choice("Do not change", "Edit progress")]
        public int feat_cold_fusion = 0;

        [Name("\"Cold Fusion\" progress")]
        [Description("0 = 0%, 100 = 100% (unlock)")]
        [Slider(0, 100)]
        public int feat_cold_fusion_progress = 100;

        [Name("Feat \"Efficient Machine\"")]
        [Description("Locking/unlocking feat WILL RESET YOUR CURRENT FEAT PROGRESS! BE CAREFULLY!")]
        [Choice("Do not change", "Edit progress")]
        public int feat_efficient_machine = 0;

        [Name("\"Efficient Machine\" progress")]
        [Description("0 = 0%, 100 = 100% (unlock)")]
        [Slider(0, 100)]
        public int feat_efficient_machine_progress = 100;

        [Name("Feat \"Fire Master\"")]
        [Description("Locking/unlocking feat WILL RESET YOUR CURRENT FEAT PROGRESS! BE CAREFULLY!")]
        [Choice("Do not change", "Edit progress")]
        public int feat_fire_master = 0;

        [Name("\"Fire Master\" progress")]
        [Description("0 = 0%, 100 = 100% (unlock)")]
        [Slider(0, 100)]
        public int feat_fire_master_progress = 100;

        [Name("Feat \"Free Runner\"")]
        [Description("Locking/unlocking feat WILL RESET YOUR CURRENT FEAT PROGRESS! BE CAREFULLY!")]
        [Choice("Do not change", "Edit progress")]
        public int feat_free_runner = 0;

        [Name("\"Free Runner\" progress")]
        [Description("0 = 0%, 100 = 100% (unlock)")]
        [Slider(0, 100)]
        public int feat_free_runner_progress = 100;

        [Name("Feat \"Snow Walker\"")]
        [Description("Locking/unlocking feat WILL RESET YOUR CURRENT FEAT PROGRESS! BE CAREFULLY!")]
        [Choice("Do not change", "Edit progress")]
        public int feat_snow_walker = 0;

        [Name("\"Snow Walker\" progress")]
        [Description("0 = 0%, 100 = 100% (unlock)")]
        [Slider(0, 100)]
        public int feat_snow_walker_progress = 100;

        [Section("Badges")]

        [Name("Unlock badge \"Hopeless Rescue\"")]
        [Description("Locking/unlocking badge WILL RESET YOUR BEST TIME FOR THIS CHALLENGE! BE CAREFULLY!")]
        [Choice("Do not change", "UNLOCK", "LOCK")]
        public int badge_rescue = 0;

        [Name("Unlock badge \"The Hunted, Part One\"")]
        [Description("Locking/unlocking badge WILL RESET YOUR BEST TIME FOR THIS CHALLENGE! BE CAREFULLY!")]
        [Choice("Do not change", "UNLOCK", "LOCK")]
        public int badge_hunted_one = 0;

        [Name("Unlock badge \"The Hunted, Part Two\"")]
        [Description("Locking/unlocking badge WILL RESET YOUR BEST TIME FOR THIS CHALLENGE! BE CAREFULLY!")]
        [Choice("Do not change", "UNLOCK", "LOCK")]
        public int badge_hunted_two = 0;

        [Name("Unlock badge \"Whiteout\"")]
        [Description("Locking/unlocking badge WILL RESET YOUR BEST TIME FOR THIS CHALLENGE! BE CAREFULLY!")]
        [Choice("Do not change", "UNLOCK", "LOCK")]
        public int badge_whiteout = 0;

        [Name("Unlock badge \"Nomad\"")]
        [Description("Locking/unlocking badge WILL RESET YOUR BEST TIME FOR THIS CHALLENGE! BE CAREFULLY!")]
        [Choice("Do not change", "UNLOCK", "LOCK")]
        public int badge_nomad = 0;

        [Name("Unlock badges \"4DON\"")]
        [Description("Locking/unlocking badge WILL CHANGE YOUR SURVIVED DAYS IN THIS CHALLANGE! BE CAREFULLY!")]
        [Choice("Do not change", "HALF", "FULL", "LOCK")]
        public int badge_4DON = 0;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "feat_book_smart")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[1], (int)newValue == 1);
            }
            else if (field.Name == "feat_cold_fusion")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[3], (int)newValue == 1);
            }
            else if (field.Name == "feat_efficient_machine")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[5], (int)newValue == 1);
            }
            else if (field.Name == "feat_fire_master")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[7], (int)newValue == 1);
            }
            else if (field.Name == "feat_free_runner")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[9], (int)newValue == 1);
            }
            else if (field.Name == "feat_snow_walker")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[11], (int)newValue == 1);
            }
        }

        protected override void OnConfirm()
        {
            switch (feat_book_smart)
            {
                case 1:
                    {
                        GameManager.GetFeatsManager().GetFeat(FeatType.BookSmarts).SetNormalizedProgress(feat_book_smart_progress / 100f);
                        break;
                    }
            }
            switch (feat_cold_fusion)
            {
                case 1:
                    {
                        GameManager.GetFeatsManager().GetFeat(FeatType.ColdFusion).SetNormalizedProgress(feat_cold_fusion_progress / 100f);
                        break;
                    }
            }
            switch (feat_efficient_machine)
            {
                case 1:
                    {
                        GameManager.GetFeatsManager().GetFeat(FeatType.EfficientMachine).SetNormalizedProgress(feat_efficient_machine_progress / 100f);
                        break;
                    }
            }
            switch (feat_fire_master)
            {
                case 1:
                    {
                        GameManager.GetFeatsManager().GetFeat(FeatType.FireMaster).SetNormalizedProgress(feat_fire_master_progress / 100f);
                        break;
                    }
            }
            switch (feat_free_runner)
            {
                case 1:
                    {
                        GameManager.GetFeatsManager().GetFeat(FeatType.FreeRunner).SetNormalizedProgress(feat_free_runner_progress / 100f);
                        break;
                    }
            }
            switch (feat_snow_walker)
            {
                case 1:
                    {
                        GameManager.GetFeatsManager().GetFeat(FeatType.SnowWalker).SetNormalizedProgress(feat_snow_walker_progress / 100f);
                        break;
                    }
            }

            switch (badge_rescue)
            {
                case 1:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeRescue = 10000f;
                        break;
                    }
                case 2:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeRescue = float.PositiveInfinity;
                        break;
                    }
            }
            switch (badge_hunted_one)
            {
                case 1:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeHunted = 10000f;
                        break;
                    }
                case 2:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeHunted = float.PositiveInfinity;
                        break;
                    }
            }
            switch (badge_hunted_two)
            {
                case 1:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeHunted2 = 10000f;
                        break;
                    }
                case 2:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeHunted2 = float.PositiveInfinity;
                        break;
                    }
            }
            switch (badge_whiteout)
            {
                case 1:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeWhiteout = 10000f;
                        break;
                    }
                case 2:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeWhiteout = float.PositiveInfinity;
                        break;
                    }
            }
            switch (badge_nomad)
            {
                case 1:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeNomad = 10000f;
                        break;
                    }
                case 2:
                    {
                        InterfaceManager.m_Panel_OptionsMenu.m_State.m_BestTimeNomad = float.PositiveInfinity;
                        break;
                    }
            }
            switch (badge_4DON)
            {
                case 1:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            InterfaceManager.m_Panel_OptionsMenu.m_State.m_DaysCompleted4DON[i] = true;
                        }
                        break;
                    }
                case 2:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            InterfaceManager.m_Panel_OptionsMenu.m_State.m_DaysCompleted4DON[i] = true;
                        }
                        break;
                    }
                case 3:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            InterfaceManager.m_Panel_OptionsMenu.m_State.m_DaysCompleted4DON[i] = false;
                        }
                        break;
                    }
            }

            SaveGameSystem.SaveProfile(ProfileSerializationOptions.All);
        }
    }

    static public void OnLoad()
    {
        BadgesUnlockerSettings settings = new BadgesUnlockerSettings();
        settings.AddToModSettings("Badges Unlocker");

        FieldInfo[] fields = settings.GetType().GetFields();
        settings.SetFieldVisible(fields[1], false);
        settings.SetFieldVisible(fields[3], false);
        settings.SetFieldVisible(fields[5], false);
        settings.SetFieldVisible(fields[7], false);
        settings.SetFieldVisible(fields[9], false);
        settings.SetFieldVisible(fields[11], false);
    }
}