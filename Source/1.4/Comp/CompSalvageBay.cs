﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using SaveOurShip2;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompShipSalvageBay : ThingComp
    {
        public static int salvageCapacity = 5000;
        public CompProperties_SalvageBay Props
        {
            get
            {
                return (CompProperties_SalvageBay)props;
            }
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
			var mapComp = parent.Map.GetComponent<ShipHeatMapComp>();

			foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
            if (parent.Faction == Faction.OfPlayer && mapComp.IsPlayerShipMap || (Prefs.DevMode && ShipInteriorMod2.HasSoS2CK))
			{
                if (Props.archo && (parent.TryGetComp<CompPowerTrader>()?.PowerOn ?? false))
                {
                    //td
                }
				foreach (Map map in Find.Maps.Where(m => m.GetComponent<ShipHeatMapComp>().IsGraveyard))
				{
                    Command_VerbTargetWreckMap retrieveShipEnemy = new Command_VerbTargetWreckMap
                    {
                        salvageBay = (Building)parent,
                        sourceMap = parent.Map,
                        targetMap = map,
						icon = ContentFinder<Texture2D>.Get("UI/SalvageShip"),
						defaultLabel = TranslatorFormattedStringExtensions.Translate("ShipSalvageCommand", map.Parent.Label),
						defaultDesc = TranslatorFormattedStringExtensions.Translate("ShipSalvageCommandDesc", map.Parent.Label)
					};
                    /*Command_Action stablizeShipEnemy = new Command_Action
                    {
                        action = delegate
                        {
                            var targetMapComp = map.GetComponent<ShipHeatMapComp>();
                            int bCount = map.listerBuildings.allBuildingsNonColonist.Count + map.listerBuildings.allBuildingsColonist.Count;
                            int bMax = mapComp.SalvBayCount * salvageCapacity;
                            if (bCount > bMax)
                            {
                                Messages.Message(TranslatorFormattedStringExtensions.Translate("ShipSalvageCount", bCount, bMax), MessageTypeDefOf.NeutralEvent);
                            }
                            else
                            {
                                float req = bCount * 0.01f;
                                float fuel = 0;
                                foreach (SoShipCache ship in targetMapComp.ShipsOnMapNew.Values.Where(s => !s.IsStuck))
                                {
                                    //td calc fuel
                                }
                                if (req > fuel)
                                {
                                    Messages.Message(TranslatorFormattedStringExtensions.Translate("ShipSalvageStablizeFuel", req), MessageTypeDefOf.NeutralEvent);
                                }
                                else
                                {
                                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(TranslatorFormattedStringExtensions.Translate("ShipSalvageStablizeConfirm", map.Parent.Label), delegate
                                    {
                                        //td consume req
                                        map.Parent.GetComponent<TimedForcedExitShip>().ticksLeftToForceExitAndRemoveMap += 60000;
                                        float adj = Rand.Range(0.025f, 0.075f);
                                        ((WorldObjectOrbitingShip)map.Parent).theta = ((WorldObjectOrbitingShip)parent.Map.Parent).theta + adj;
                                    }));
                                }
                            }
                        },
                        defaultLabel = TranslatorFormattedStringExtensions.Translate("ShipSalvageStablize", map.Parent.Label),
                        defaultDesc = TranslatorFormattedStringExtensions.Translate("ShipSalvageStablizeDesc", map.Parent.Label),
                        icon = ContentFinder<Texture2D>.Get("UI/StabilizeShip")
                    };*/
                    if (mapComp.InCombat)
                    {
                        retrieveShipEnemy.Disable(TranslatorFormattedStringExtensions.Translate("ShipSalvageDisabled"));
                        //stablizeShipEnemy.Disable(TranslatorFormattedStringExtensions.Translate("ShipSalvageDisabled"));
                    }
					yield return retrieveShipEnemy;
                    //yield return stablizeShipEnemy;
                }
                Command_VerbTargetWreckMap moveWreck = new Command_VerbTargetWreckMap
                {
                    salvageBay = (Building)this.parent,
                    sourceMap = this.parent.Map,
                    targetMap = this.parent.Map,
                    icon = ContentFinder<Texture2D>.Get("UI/SalvageMove"),
                    defaultLabel = TranslatorFormattedStringExtensions.Translate("ShipMoveWreckCommand"),
                    defaultDesc = TranslatorFormattedStringExtensions.Translate("ShipMoveWreckCommandDesc")
                };
                Command_VerbTargetWreckMap moveWreckFlip = new Command_VerbTargetWreckMap
                {
                    salvageBay = (Building)this.parent,
                    sourceMap = this.parent.Map,
                    targetMap = this.parent.Map,
                    rotb = 2,
                    icon = ContentFinder<Texture2D>.Get("UI/SalvageMoveFlip"),
                    defaultLabel = TranslatorFormattedStringExtensions.Translate("ShipMoveWreckFlipCommand"),
                    defaultDesc = TranslatorFormattedStringExtensions.Translate("ShipMoveWreckFlipCommandDesc")
                };
                Command_VerbTargetWreckMap moveWreckRot = new Command_VerbTargetWreckMap
                {
                    salvageBay = (Building)this.parent,
                    sourceMap = this.parent.Map,
                    targetMap = this.parent.Map,
                    rotb = 1,
                    icon = ContentFinder<Texture2D>.Get("UI/SalvageMoveRot"),
                    defaultLabel = TranslatorFormattedStringExtensions.Translate("ShipMoveWreckRotCommand"),
                    defaultDesc = TranslatorFormattedStringExtensions.Translate("ShipMoveWreckRotCommandDesc")
                };
                Command_Action claim = new Command_Action
                {
                    action = delegate
                    {
                        Claim();
                    },
                    defaultLabel = TranslatorFormattedStringExtensions.Translate("ShipClaimWrecksCommand"),
                    defaultDesc = TranslatorFormattedStringExtensions.Translate("ShipClaimWrecksCommandDesc"),
                    icon = ContentFinder<Texture2D>.Get("UI/SalvageClaim")
                };
                Command_VerbTargetWreck removeTargetWreck = new Command_VerbTargetWreck
                {
                    //abandon target wreck (rem rock floor)
                    targetMap = this.parent.Map,
                    position = this.parent.Position,
                    defaultLabel = TranslatorFormattedStringExtensions.Translate("ShipRemoveWrecksCommand"),
                    defaultDesc = TranslatorFormattedStringExtensions.Translate("ShipRemoveWrecksCommandDesc"),
                    icon = ContentFinder<Texture2D>.Get("UI/SalvageCancel")
                };
                if (mapComp.InCombat || GenHostility.AnyHostileActiveThreatToPlayer(parent.Map))
                {
                    moveWreck.Disable(TranslatorFormattedStringExtensions.Translate("ShipSalvageDisabled"));
                    moveWreckFlip.Disable(TranslatorFormattedStringExtensions.Translate("ShipSalvageDisabled"));
                    moveWreckRot.Disable(TranslatorFormattedStringExtensions.Translate("ShipSalvageDisabled"));
                    claim.Disable(TranslatorFormattedStringExtensions.Translate("ShipClaimWrecksDisabled"));
                    removeTargetWreck.Disable(TranslatorFormattedStringExtensions.Translate("ShipClaimWrecksDisabled"));
                }
                yield return moveWreck;
                yield return moveWreckFlip;
                yield return moveWreckRot;
                yield return claim;
                yield return removeTargetWreck;
            }
		}
        private void Claim()
        {
            List<Building> buildings = new List<Building>();
            List<Thing> things = new List<Thing>();
            foreach (Thing t in this.parent.Map.listerThings.AllThings)
            {
                if (t is Building b && b.def.CanHaveFaction && b.Faction != Faction.OfPlayer)
                {
                    buildings.Add(b);
                }
                else if (t is DetachedShipPart)
                    things.Add(t);
            }
            if (buildings.Any())
            {
                foreach (Building b in buildings)
                {
                    if (b is Building_Storage s)
                        s.settings.filter.SetDisallowAll();
                    b.SetFaction(Faction.OfPlayer);
                }
                Messages.Message(TranslatorFormattedStringExtensions.Translate("ShipClaimWrecksSuccess", buildings.Count), parent, MessageTypeDefOf.PositiveEvent);
            }
            //remove floating tiles
            foreach (Thing t in things)
            {
                t.Destroy();
            }
        }
        public override void CompTickRare()
        {
            base.CompTickRare();
        }
        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("ShipSalvageBase".Translate());
            return stringBuilder.ToString();
            //return base.CompInspectStringExtra();
        }
    }
}