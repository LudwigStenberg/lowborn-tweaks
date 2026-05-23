using HarmonyLib;
using Lowborn.Features.PrisonerPurchase;
using Lowborn.Features.PrisonerPurchase.Domain;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace LowbornTweaks.Patches
{
    [HarmonyPatch(typeof(PrisonerPurchaseService), "GetBrokerState")]
    public static class GetBrokerStatePatch
    {
        static bool Prefix(PrisonerPurchaseService __instance, Settlement settlement, ref RansomBrokerState __result)
        {
            RansomBrokerState brokerState;
            bool flag = !__instance.BrokerData.TryGetValue(settlement.StringId, out brokerState);
            if (flag)
            {
                brokerState = new RansomBrokerState
                {
                    AvailablePrisoners = MBRandom.RandomInt(2, 15),
                    NextRestockTime = CampaignTime.Now + CampaignTime.Weeks(1f)
                };
                __instance.BrokerData[settlement.StringId] = brokerState;
            }
            __result = brokerState;
            return false; // Skip original method
        }
    }
}