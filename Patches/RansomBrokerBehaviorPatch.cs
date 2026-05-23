using HarmonyLib;
using Lowborn.Features.PrisonerPurchase.Behaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace LowbornTweaks.Patches
{
    [HarmonyPatch(typeof(RansomBrokerPrisonerSellerBehavior), "OnAgentJoinedConversation")]
    public static class RansomBrokerBehaviorPatch
    {
        static bool Prefix(IAgent agent)
        {
            if (agent?.Character == null) return false;

            CharacterObject character = (CharacterObject)agent.Character;
            if (character.Occupation == Occupation.RansomBroker)
            {
                var service = Campaign.Current.GetCampaignBehavior<RansomBrokerPrisonerSellerBehavior>();
                // Use AccessTools to get the private service field
                var prisonerService = AccessTools.FieldRefAccess<RansomBrokerPrisonerSellerBehavior,
                    Lowborn.Features.PrisonerPurchase.PrisonerPurchaseService>(service, "_prisonerPurchaseService");
                prisonerService.RefreshBrokerIfNeeded(Hero.MainHero.CurrentSettlement);
            }
            return false;
        }
    }
}