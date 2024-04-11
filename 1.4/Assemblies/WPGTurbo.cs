using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

[StaticConstructorOnStartup]
public class CompPowerPlantWPGTWater : CompPowerPlant
{
	private float spinPosition;
	private bool cacheDirty = true;
	private bool waterUsable;
	private bool waterDoubleUsed;
	private float spinRate = 1f;
	private const float PowerFactorIfWaterDoubleUsed = 0.3f;
	private const float SpinRateFactor = 1f / 100f;
	private const float BladeOffset = 1f;
	private const int BladeCount = 12;
	public static readonly Material BladesMat = MaterialPool.MatFrom("Things/Building/Power/WaterPowerGenerator/HydroelectricPowerStationBlades");
 
	protected override float DesiredPowerOutput
	{
		get
		{
			if (cacheDirty)
			{
				RebuildCache();
			}
			if (!waterUsable)
			{
				return 0f;
			}
			if (waterDoubleUsed)
			{
				return base.DesiredPowerOutput * 0.3f;
			}
			return base.DesiredPowerOutput;
		}
	}

 public static class ThingDefOf
	{ public static ThingDef WWPGTHydroelectricPowerStation; }

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		base.PostSpawnSetup(respawningAfterLoad);
		spinPosition = Rand.Range(0f, 5f);
		RebuildCache();
		ForceOthersToRebuildCache(parent.Map);
	}

	public override void PostDeSpawn(Map map)
	{
		base.PostDeSpawn(map);
		ForceOthersToRebuildCache(map);
	}

	private void ClearCache()
	{
		cacheDirty = true;
	}

 	private void RebuildCache()
	{
		waterUsable = true;
		foreach (IntVec3 item in WaterCells())
		{
			if (item.InBounds(parent.Map) && !parent.Map.terrainGrid.TerrainAt(item).affordances.Contains(TerrainAffordanceDefOf.MovingFluid))
			{
				waterUsable = false;
				break;
			}
		}
		waterDoubleUsed = false;
		IEnumerable<Building> enumerable = parent.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.WWPGTHydroelectricPowerStation);
		foreach (IntVec3 item2 in WaterUseCells())
		{
			if (!item2.InBounds(parent.Map))
			{
				continue;
			}
			foreach (Building item3 in enumerable list)
			{
				if (item3 != parent && item3.GetComp<CompPowerPlantWPGTWater>().WaterUseRect().Contains(item2))
				{
					waterDoubleUsed = true;
					break;
				}
			}
		}
		if (!waterUsable)
		{
			spinRate = 0f;
			return;
		}
		Vector3 zero = Vector3.zero;
		foreach (IntVec3 item4 in WaterCells())
		{
			zero += parent.Map.waterInfo.GetWaterMovement(item4.ToVector3Shifted());
		}
		spinRate = Mathf.Sign(Vector3.Dot(zero, parent.Rotation.Rotated(RotationDirection.Clockwise).FacingCell.ToVector3()));
		spinRate *= Rand.RangeSeeded(2.4f, 2.6f, parent.thingIDNumber * 60509 + 33151);
		if (waterDoubleUsed)
		{
			spinRate *= 0.5f;
		}
		cacheDirty = false;
	}
