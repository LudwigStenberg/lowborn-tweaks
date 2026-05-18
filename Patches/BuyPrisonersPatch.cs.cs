using HarmonyLib;
using Lowborn.Features.PrisonerPurchase;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace LowbornTweaks.Patches
{
    [HarmonyPatch(typeof(PrisonerPurchaseService), "BuyPrisoners")]
    public static class BuyPrisonersPatch
    {
        static bool Prefix(PrisonerPurchaseService __instance, int amount)
        {
            Settlement settlement = Hero.MainHero.CurrentSettlement;
            var state = __instance.GetBrokerState(settlement);
            int price = amount * 50; // Increased from 20 to 50
            Hero.MainHero.ChangeHeroGold(-price);
            CharacterObject looter = CharacterObject.Find("looter");
            MobileParty.MainParty.AddPrisoner(looter, amount);
            state.AvailablePrisoners -= amount;

            bool flag = state.AvailablePrisoners < 2;
            if (flag)
            {
                state.NextRestockTime = CampaignTime.Now + CampaignTime.Weeks(1f);
            }
            return false; // Skip original method
        }
    }
}