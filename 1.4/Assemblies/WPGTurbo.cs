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
