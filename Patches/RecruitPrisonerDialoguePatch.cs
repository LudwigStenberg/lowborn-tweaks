using HarmonyLib;
using Lowborn.Features.PrisonerPurchase;
using Lowborn.Features.PrisonerPurchase.Dialogues;
using Lowborn.Features.PrisonerPurchase.Domain;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace LowbornTweaks.Patches
{
    [HarmonyPatch(typeof(RecruitPrisonerDialogue), "AddPrisonerPurchaseDialogue")]
    public static class RecruitPrisonerDialoguePatch
    {
        static bool Prefix(RecruitPrisonerDialogue __instance, CampaignGameStarter starter)
        {
            var prisonerService = AccessTools.FieldRefAccess<RecruitPrisonerDialogue, PrisonerPurchaseService>(__instance, "_prisonerPurchaseService");

            starter.AddPlayerLine("ransom_broker_prisoner", "ransom_broker_talk", "ransom_broker_prisoner_buy", "{=lowborn.str_player_ask_buy_prisoners}I would like to buy some prisoners", () => true, null, 100, null, null);
            starter.AddDialogLine("ransom_broker_prisoner_a", "ransom_broker_prisoner_buy", "ransom_broker_prisoner_offer", "{=lowborn.str_rb_say_prisoners_avaliable}I have {AVAILABLE_PRISONERS} prisoners available to negotiate ({PRICE_TOTAL_PRISONERS}{GOLD_ICON})", delegate ()
            {
                RansomBrokerState state = prisonerService.GetBrokerState(Hero.MainHero.CurrentSettlement);
                MBTextManager.SetTextVariable("AVAILABLE_PRISONERS", state.AvailablePrisoners);
                MBTextManager.SetTextVariable("PRICE_TOTAL_PRISONERS", state.AvailablePrisoners * 50);
                return state.AvailablePrisoners >= 2;
            }, null, 100, null);
            starter.AddPlayerLine("ransom_broker_prisoner_c", "ransom_broker_prisoner_offer", "ransom_broker_prisoner_thanks", "{=lowborn.str_player_buy_2_prisoners}I would like to buy 2 ({PRICE_2_PRISONERS}{GOLD_ICON})", delegate
            {
                RansomBrokerState state = prisonerService.GetBrokerState(Hero.MainHero.CurrentSettlement);
                MBTextManager.SetTextVariable("PRICE_2_PRISONERS", 100);
                return Hero.MainHero.Gold >= 100 && state.AvailablePrisoners > 2;
            }, delegate
            {
                prisonerService.BuyPrisoners(2);
            }, 100, null, null);
            starter.AddPlayerLine("ransom_broker_prisoner_d", "ransom_broker_prisoner_offer", "ransom_broker_prisoner_thanks", "{=lowborn.str_player_buy_all_prisoners}I would like to buy them all ({PRICE_TOTAL_PRISONERS}{GOLD_ICON})", delegate
            {
                RansomBrokerState state = prisonerService.GetBrokerState(Hero.MainHero.CurrentSettlement);
                return state.AvailablePrisoners > 0 && Hero.MainHero.Gold >= state.AvailablePrisoners * 50;
            }, delegate
            {
                RansomBrokerState state = prisonerService.GetBrokerState(Hero.MainHero.CurrentSettlement);
                prisonerService.BuyPrisoners(state.AvailablePrisoners);
                state.NextRestockTime = CampaignTime.Now + CampaignTime.Weeks(1f);
            }, 100, null, null);
            starter.AddPlayerLine("ransom_broker_prisoner_e", "ransom_broker_prisoner_offer", "ransom_broker_prisoner_refusal", "{=lowborn.str_player_refuse_prisoners}Thank you, I won't want them", () => true, null, 100, null, null);
            starter.AddDialogLine("ransom_broker_prisoner_f", "ransom_broker_prisoner_thanks", "ransom_broker_talk", "{=lowborn.str_rb_refuse_prisoners}We will deliver them to your men at the city gates", () => MobileParty.MainParty.MemberRoster.TotalRegulars + MobileParty.MainParty.MemberRoster.TotalHeroes > 1, null, 100, null);
            starter.AddDialogLine("ransom_broker_prisoner_f", "ransom_broker_prisoner_thanks", "ransom_broker_talk", "{=lowborn.str_rb_refuse_prisoners_solo}They will be entrusted to you at the city limits. If you have other prisoners, I'll buy them too", () => MobileParty.MainParty.MemberRoster.TotalRegulars + MobileParty.MainParty.MemberRoster.TotalHeroes <= 1, null, 100, null);
            starter.AddDialogLine("ransom_broker_prisoner_f", "ransom_broker_prisoner_refusal", "ransom_broker_talk", "{=lowborn.str_rb_refuse_final_prisoners}Okay, maybe I won't have them next time, I have to take them out of Calradia soon", () => true, null, 100, null);
            starter.AddDialogLine("ransom_broker_prisoner_g", "ransom_broker_prisoner_buy", "ransom_broker_talk", "{=lowborn.str_rb_no_prisoners}Sorry, I don't have any prisoners for you at the moment, but If you have other prisoners, I'll buy them too", delegate ()
            {
                RansomBrokerState state = prisonerService.GetBrokerState(Hero.MainHero.CurrentSettlement);
                return state.AvailablePrisoners < 2;
            }, null, 100, null);
            return false;
        }
    }
}