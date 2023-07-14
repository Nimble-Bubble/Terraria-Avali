using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using MrPlagueRaces.Common.Races;
using MrPlagueRaces;
using AvaliMod.Sounds;

namespace AvaliMod
{
	public class Avali : Race
	{
		/*
		//display name, used to override the race's displayed name. By default, a race will use its class name
		public override string RaceDisplayName => "Avali";
		//decides if the race has a custom hurt sound (prevents default hurt sound from playing)
		public override bool UsesCustomHurtSound => true;
		//decides if the race has a custom death sound (prevents default death sound from playing)
		public override bool UsesCustomDeathSound => true;
		//decides if the race has a custom female hurt sound (by default, the race will play the male/default hurt sound for both genders)
		public override bool HasFemaleHurtSound => true;

		//textures for the race's display background in the UI
		public override string RaceEnvironmentIcon => ($"MrPlagueRaces/Common/UI/RaceDisplay/Environment/Environment_Tundra");
		public override string RaceEnvironmentOverlay1Icon => ($"MrPlagueRaces/Common/UI/RaceDisplay/Environment/EnvironmentOverlay_SolarEclipse");
		public override string RaceEnvironmentOverlay2Icon => ($"MrPlagueRaces/Common/UI/RaceDisplay/Environment/EnvironmentOverlay_Blizzard");

		//information for the race's textures and lore in the UI
		public override string RaceSelectIcon => ($"AvaliMod/Common/UI/RaceDisplay/AvaliSelect");
		public override string RaceDisplayMaleIcon => ($"AvaliMod/Common/UI/RaceDisplay/AvaliDisplayMale");
		public override string RaceDisplayFemaleIcon => ($"AvaliMod/Common/UI/RaceDisplay/AvaliDisplayFemale");
		public override string RaceLore1 => "The Avali are \ndescribed as 'fluffy \nspace raptors' by \nmany.";
		public override string RaceLore2 => "The Avali come from the \nfaraway planet of Avalon.";

		
		//"\n" is normally used to move to the next line, but it conflicts with colored text so I split the ability and additional notes into several lines
		public override string RaceAbilityName => $"{(ModContent.GetInstance<AvaliModConfig>().HasGlidingAbility?"Glide":"")}";
		public override string RaceAbilityDescription1 => $"{(ModContent.GetInstance<AvaliModConfig>().HasGlidingAbility ? "Press [c/34EB93:Racial Ability Hotkey] to glide. \nGliding negates fall damage and makes you fall slower" : "")}";
		public override string RaceAdditionalNotesDescription1 => $"{(ModContent.GetInstance<AvaliModConfig>().DarknessEnabled ? "Can't see in the dark.\n" : "")}{(ModContent.GetInstance<AvaliModConfig>().CanHearInDark ? "Can locate other creatures using echolocation." : "")}";
		//makes the race's display background in the UI appear darker, can be used to make it look like it is night
		public override bool DarkenEnvironment => true;
		*/
		private bool IsGliding = false;

        //custom hurt sounds would normally be put in PreHurt, but they conflict with Godmode in other mods so I made a custom system to avoid the confliction
        public override void Load()
        {
            Description = "Hailing from the cold planet of Avalon, the Avali are described as 'fluffy space raptors' by many.";
			AbilitiesDescription = $"{(ModContent.GetInstance<AvaliModConfig>().HasGlidingAbility ? "Press [c/34EB93:Primary Racial Ability] to glide. \nGliding negates fall damage and makes you fall slower" : "")}";
		}
        /* public override bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
		{
			return true;
		} */
		public override bool PreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			//death sound
			var AvaliMod = ModLoader.GetMod("AvaliMod");
			SoundEngine.PlaySound(new SoundStyle("Sounds/Avali_Killed"));
			return true;
		}
		public override void ResetEffects(Player player)
		{
			var modPlayer = player.GetModPlayer<MrPlagueRaces.MrPlagueRacesPlayer>();
			if (modPlayer.statsEnabled)
			{
				AvaliModConfig config = ModContent.GetInstance<AvaliModConfig>();
				if (config.AvaliHaveModifiedStats)
				{
					player.statLifeMax2 -= player.statLifeMax2 / 4; //I apologize for changing stat changes, but halving max HP seemed like a bit too much.
					player.statDefense -= (player.statDefense / 4);

					player.GetAttackSpeed(DamageClass.Melee) *= 1.1f;
					player.moveSpeed += player.moveSpeed / 25;

					player.GetDamage(DamageClass.Melee) *= 1.25f;

				}
			}
		}
		public override void ProcessTriggers(Player player, Mod mod)
		{
			//custom hotkey stuff goes here
			var modPlayer = player.GetModPlayer<MrPlagueRaces.MrPlagueRacesPlayer>();
			if (modPlayer.statsEnabled)
			{
				AvaliModConfig config = ModContent.GetInstance<AvaliModConfig>();
				if(MrPlagueRaces.MrPlagueRaces.RaceAbilityKeybind1.Current && !player.dead && config.HasGlidingAbility)
                {
					if (config.RacialKeyIsToggle)
					{
						IsGliding = !IsGliding;
                    }
                    else
                    {
						player.AddBuff(BuffID.Featherfall, 1);
					}
                }
				if (IsGliding && config.RacialKeyIsToggle)
				{
					player.AddBuff(BuffID.Featherfall, 1);
				}
			}
		}
	}
}

