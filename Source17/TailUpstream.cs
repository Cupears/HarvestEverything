using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using RimWorld;
using Verse;

namespace Harvest_Everything;

public class ThoughtWorker_Precept_KurinWoolApparel : ThoughtWorker
{
	public Type workerClass;

	protected override ThoughtState CurrentStateInternal(Pawn p)
	{
  if(p?.apparel?.WornApparel==null)return ThoughtState.Inactive;
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		List<Apparel> wornApparel = p.apparel.WornApparel;
		bool flag = false;
		try
		{
			if (((Def)((Thing)p).def).defName == "Kurin_Race")
			{
				flag = true;
			}
			if (((Def)((Thing)p).def).defName == "Kurin")
			{
				flag = true;
			}
		}
		catch (Exception)
		{
		}
		if (!flag)
		{
			return ThoughtState.Inactive;
		}
		int num = 0;
		string text = null;
		foreach (Apparel item in wornApparel)
		{
			try
			{
				if (((Def)((Thing)item).Stuff).defName == "WoolKurin")
				{
					num++;
					text = ((text != null) ? (text + "," + ((Def)((Thing)item).def).label) : ((Def)((Thing)item).def).label);
				}
			}
			catch (Exception)
			{
			}
		}
		if (num == 0)
		{
			return ThoughtState.Inactive;
		}
		if (num >= 5)
		{
			return ThoughtState.ActiveAtStage(4, text);
		}
		return ThoughtState.ActiveAtStage(num - 1, text);
	}
}
public class ThoughtWorker_KurinWoolWorn_Disapproved : ThoughtWorker
{
	protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
	{
  if(p?.apparel?.WornApparel==null||otherPawn?.apparel?.WornApparel==null)return ThoughtState.Inactive;
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		if (!p.RaceProps.Humanlike)
		{
			return (ThoughtState)false;
		}
		if (!RelationsUtility.PawnsKnowEachOther(p, otherPawn))
		{
			return (ThoughtState)false;
		}
		bool flag = false;
		bool flag2 = false;
		try
		{
			if (((Def)((Thing)p).def).defName == "Kurin_Race")
			{
				flag = true;
			}
			if (((Def)((Thing)otherPawn).def).defName == "Kurin_Race")
			{
				flag2 = true;
			}
			if (((Def)((Thing)p).def).defName == "Kurin")
			{
				flag = true;
			}
			if (((Def)((Thing)otherPawn).def).defName == "Kurin")
			{
				flag2 = true;
			}
		}
		catch (Exception)
		{
		}
		if (!flag)
		{
			return (ThoughtState)false;
		}
		List<Apparel> wornApparel = otherPawn.apparel.WornApparel;
		int num = 0;
		string text = null;
		foreach (Apparel item in wornApparel)
		{
			try
			{
				if (((Def)((Thing)item).Stuff).defName == "WoolKurin")
				{
					num++;
					text = ((text != null) ? (text + "," + ((Def)((Thing)item).def).label) : ((Def)((Thing)item).def).label);
				}
			}
			catch
			{
			}
		}
		if (num == 0)
		{
			return (ThoughtState)false;
		}
		if (num < 3 && !flag2)
		{
			return ThoughtState.ActiveAtStage(0);
		}
		if (num > 2 && !flag2)
		{
			return ThoughtState.ActiveAtStage(1);
		}
		if ((num < 3) & flag2)
		{
			return ThoughtState.ActiveAtStage(2);
		}
		if ((num > 2) & flag2)
		{
			return ThoughtState.ActiveAtStage(3);
		}
		return (ThoughtState)false;
	}
}
public class ThoughtWorker_Precept_ReviaWoolApparel : ThoughtWorker
{
	public Type workerClass;

	protected override ThoughtState CurrentStateInternal(Pawn p)
	{
  if(p?.apparel?.WornApparel==null)return ThoughtState.Inactive;
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		List<Apparel> wornApparel = p.apparel.WornApparel;
		bool flag = false;
		try
		{
			if (((Def)((Thing)p).def).defName == "ReviaRaceAlien")
			{
				flag = true;
			}
		}
		catch (Exception)
		{
		}
		if (!flag)
		{
			return ThoughtState.Inactive;
		}
		int num = 0;
		string text = null;
		foreach (Apparel item in wornApparel)
		{
			try
			{
				if (((Def)((Thing)item).Stuff).defName == "WoolRevia")
				{
					num++;
					text = ((text != null) ? (text + "," + ((Def)((Thing)item).def).label) : ((Def)((Thing)item).def).label);
				}
			}
			catch (Exception)
			{
			}
		}
		if (num == 0)
		{
			return ThoughtState.Inactive;
		}
		if (num >= 5)
		{
			return ThoughtState.ActiveAtStage(4, text);
		}
		return ThoughtState.ActiveAtStage(num - 1, text);
	}
}
public class ThoughtWorker_ReviaWoolWorn : ThoughtWorker
{
	protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
	{
  if(p?.apparel?.WornApparel==null||otherPawn?.apparel?.WornApparel==null)return ThoughtState.Inactive;
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		if (!p.RaceProps.Humanlike)
		{
			return (ThoughtState)false;
		}
		if (!RelationsUtility.PawnsKnowEachOther(p, otherPawn))
		{
			return (ThoughtState)false;
		}
		bool flag = false;
		bool flag2 = false;
		try
		{
			if (((Def)((Thing)p).def).defName == "ReviaRaceAlien")
			{
				flag = true;
			}
			if (((Def)((Thing)otherPawn).def).defName == "ReviaRaceAlien")
			{
				flag2 = true;
			}
		}
		catch (Exception)
		{
		}
		if (!flag)
		{
			return (ThoughtState)false;
		}
		List<Apparel> wornApparel = otherPawn.apparel.WornApparel;
		int num = 0;
		foreach (Apparel item in wornApparel)
		{
			try
			{
				if (((Def)((Thing)item).Stuff).defName == "WoolRevia")
				{
					num++;
				}
			}
			catch
			{
			}
		}
		List<Apparel> wornApparel2 = p.apparel.WornApparel;
		int num2 = 0;
		foreach (Apparel item2 in wornApparel2)
		{
			try
			{
				if (((Def)((Thing)item2).Stuff).defName == "WoolRevia")
				{
					num2++;
				}
			}
			catch
			{
			}
		}
		if (num == 0 && !flag2)
		{
			return (ThoughtState)false;
		}
		if (num < 3 && !flag2)
		{
			return ThoughtState.ActiveAtStage(0);
		}
		if (num > 2 && !flag2)
		{
			return ThoughtState.ActiveAtStage(1);
		}
		if ((num2 - num == 0) & flag2)
		{
			return (ThoughtState)false;
		}
		if ((num2 - num > 0) & flag2)
		{
			return ThoughtState.ActiveAtStage(2);
		}
		if ((num2 - num < 0) & flag2)
		{
			return ThoughtState.ActiveAtStage(3);
		}
		return (ThoughtState)false;
	}
}
