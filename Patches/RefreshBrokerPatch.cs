using HarmonyLib;
using Lowborn.Features.PrisonerPurchase;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace LowbornTweaks.Patches
{
    [HarmonyPatch(typeof(PrisonerPurchaseService), "RefreshBrokerIfNeeded")]
    public static class RefreshBrokerPatch
    {
        static bool Prefix(PrisonerPurchaseService __instance, Settlement settlement)
        {
            var state = __instance.GetBrokerState(settlement);
            bool flag = CampaignTime.Now >= state.NextRestockTime && state.AvailablePrisoners < 2;
            if (flag)
            {
                state.AvailablePrisoners = MBRandom.RandomInt(2, 15); // Increased from 2, 9
            }
            return false; // Skip original method
        }
    }
}