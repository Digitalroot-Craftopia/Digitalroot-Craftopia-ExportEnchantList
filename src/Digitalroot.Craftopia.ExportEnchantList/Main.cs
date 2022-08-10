using BepInEx;
using BepInEx.Configuration;
using Digitalroot.Valheim.Common;
using HarmonyLib;
using JetBrains.Annotations;
using Oc;
using Oc.Item;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Digitalroot.Craftopia.ExportEnchantList
{
  [BepInPlugin(Guid, Name, Version)]
  public partial class Main : BaseUnityPlugin, ITraceableLogging
  {
    private Harmony _harmony;

    [UsedImplicitly]
    public static ConfigEntry<int> NexusId;

    public static Main Instance;
    private const string CacheName = "chainloader";

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion

    public Main()
    {
      try
      {
        #if DEBUG
        EnableTrace = true;
        #else
        EnableTrace = false;
        #endif
        Instance = this;
        NexusId = Config.Bind("General", "NexusID", 0000, new ConfigDescription("Nexus mod ID for updates", null, new ConfigurationManagerAttributes { Browsable = false, ReadOnly = true }));
        Log.RegisterSource(Instance);
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    [UsedImplicitly]
    private void Awake()
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
        _harmony = Harmony.CreateAndPatchAll(typeof(Main).Assembly, Guid);
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
        _harmony?.UnpatchSelf();
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    public void OnOcUI_HomeSceneStart()
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
        ExportEnchantList();
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private void ExportEnchantList()
    {
      try
      {
        Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}()");
        Log.Trace(Instance, $"[{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}] ---------------------------------------------------------------");
        Log.Trace(Instance, $"[{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}](OcResidentData.EnchantDataList.all.Length : {OcResidentData.EnchantDataList.all.Length})");

        var csvFileInfo = new FileInfo(Path.Combine(Paths.BepInExRootPath ?? AssemblyDirectory.FullName, "exports", $"Craftopia.EnchantList.csv"));
        if (csvFileInfo.DirectoryName != null) Directory.CreateDirectory(csvFileInfo.DirectoryName);
        if (csvFileInfo.Exists)
        {
          csvFileInfo.Delete();
          csvFileInfo.Refresh();
        }

        Log.Info(Instance, $"[{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}] Creating Export at {csvFileInfo.FullName}.");

        var enchantPropList = new List<string>
        {
          "DisplayName"
          , "ID"
          , "Rarity"
          , "IsTreasureDropped"
          , "Only_Accessory"
          , "Only_WeaponSlot"
          , "Only_Shield"
          , "modify_Atk"
          , "modify_AtkRate"
          , "modify_Def"
          , "modify_DefRate"
          , "modify_MAtk"
          , "modify_MAtkRate"
          , "modify_MaxHp"
          , "modify_MaxHpRate"
          , "modify_MaxMp"
          , "modify_MaxMpRate"
          , "modify_MaxSp"
          , "modify_MaxSpRate"
          , "modify_MaxSt"
          , "modify_MaxStRate"
          , "modify_CriticalDmgRate_Physical"
          , "modify_CriticalDmgRate_Magical"
          , "modify_SpConsumeRate"
          , "modify_StConsumeRate"
          , "modify_ManaConsumeRate"
          , "modify_StRegenerateRate"
          , "modify_ManaRegenerateRate"
          , "modify_SkillCoolDownRate"
          , "modify_ItemCoolDownRate"
          , "modify_CriticalProb_Physical"
          , "modify_CriticalProb_Magical"
          // , "modify_ItemDropProb"
          // , "modify_PoisonProb"
          // , "modify_FireProb"
          , "modify_MovementSpeedRate"
          , "modify_MovementSpeedRate_Air"
          , "modify_MotionSpeedRate"
          // , "modify_JumpSpeedRate"
          // , "modify_AtkUndead"
          // , "modify_DefUndead"
          // , "modify_AtkIce"
          // , "modify_DefIce"
          // , "modify_AtkFire"
          // , "modify_DefFire"
          // , "modify_AtkBoss"
          // , "modify_MAtkBoss"
          // , "modify_DefBoss"
          // , "modify_AtkAnimal"
          // , "modify_DamageCut"
          // , "modify_JumpCount"
          , "modify_IncreaseAtkByDefRate"
          , "modify_IncreaseMAtkByDefRate"
          , "modify_IncreaseAtkByHpRate"
          , "modify_IncreaseMAtkByHpRate"
          // , "modify_IncreaseAtkByMpRate"
          , "modify_IncreaseMAtkByMpRate"
          , "modify_HealthSkillHealRate"
          , "modify_FinalDamageRate"
          , "modify_FinalPhysicsDamageRate"
          // , "modify_FinalMagicDamageRate"
          , "modify_FinalArrowDamageRate"
          , "modify_FinalUnarmedDamgeRate"
          , "modify_FinalSkillDamageRate"
          , "modify_FinalMeleeSkillDamageRate"
          , "modify_PetAtkRate"
          , "modify_PetDefRate"
          , "modify_PetCriticalRate"
          , "modify_PetCriticalDamageRate"
          , "modify_PetSpeedRate"
          , "EquipmentPassiveSkillId"
          , "EquipmentPassiveSkillLevel"
          , "PriceModify"
          , "DurabilityModify"
        };

        var sb = new StringBuilder();
        foreach (var key in enchantPropList)
        {
          sb.Append(key).Append(",");
        }

        File.AppendAllText(csvFileInfo.FullName, $"{sb.ToString().TrimEnd(',')}{Environment.NewLine}", Encoding.UTF8);
        // Log.Trace(Instance, $"[{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}]{sb.ToString().TrimEnd(',')}");

        foreach (SoEnchantment soEnchantment in OcResidentData.EnchantDataList.all)
        {
          if (soEnchantment.IsEnabled == false) continue;

          sb.Clear();
          sb
            .Append(soEnchantment.DisplayName).Append(",")
            .Append(soEnchantment.ID).Append(",")
            .Append(soEnchantment.Rarity).Append(",")
            .Append(soEnchantment.IsTreasureDropped == 1).Append(",")
            .Append(soEnchantment.Only_Accessory).Append(",")
            .Append(soEnchantment.Only_WeaponSlot).Append(",")
            .Append(soEnchantment.Only_Shield).Append(",")
            .Append(soEnchantment.modify_Atk).Append(",")
            .Append(soEnchantment.modify_AtkRate).Append(",")
            .Append(soEnchantment.modify_Def).Append(",")
            .Append(soEnchantment.modify_DefRate).Append(",")
            .Append(soEnchantment.modify_MAtk).Append(",")
            .Append(soEnchantment.modify_MAtkRate).Append(",")
            .Append(soEnchantment.modify_MaxHp).Append(",")
            .Append(soEnchantment.modify_MaxHpRate).Append(",")
            .Append(soEnchantment.modify_MaxMp).Append(",")
            .Append(soEnchantment.modify_MaxMpRate).Append(",")
            .Append(soEnchantment.modify_MaxSp).Append(",")
            .Append(soEnchantment.modify_MaxSpRate).Append(",")
            .Append(soEnchantment.modify_MaxSt).Append(",")
            .Append(soEnchantment.modify_MaxStRate).Append(",")
            .Append(soEnchantment.modify_CriticalDmgRate_Physical).Append(",")
            .Append(soEnchantment.modify_CriticalDmgRate_Magical).Append(",")
            .Append(soEnchantment.modify_SpConsumeRate).Append(",")
            .Append(soEnchantment.modify_StConsumeRate).Append(",")
            .Append(soEnchantment.modify_ManaConsumeRate).Append(",")
            .Append(soEnchantment.modify_StRegenerateRate).Append(",")
            .Append(soEnchantment.modify_ManaRegenerateRate).Append(",")
            .Append(soEnchantment.modify_SkillCoolDownRate).Append(",")
            .Append(soEnchantment.modify_ItemCoolDownRate).Append(",")
            .Append(soEnchantment.modify_CriticalProb_Physical).Append(",")
            .Append(soEnchantment.modify_CriticalProb_Magical).Append(",")
            // .Append(soEnchantment.modify_ItemDropProb).Append(",")
            // .Append(soEnchantment.modify_PoisonProb).Append(",")
            // .Append(soEnchantment.modify_FireProb).Append(",")
            .Append(soEnchantment.modify_MovementSpeedRate).Append(",")
            .Append(soEnchantment.modify_MovementSpeedRate_Air).Append(",")
            .Append(soEnchantment.modify_MotionSpeedRate).Append(",")
            // .Append(soEnchantment.modify_JumpSpeedRate).Append(",")
            // .Append(soEnchantment.modify_AtkUndead).Append(",")
            // .Append(soEnchantment.modify_DefUndead).Append(",")
            // .Append(soEnchantment.modify_AtkIce).Append(",")
            // .Append(soEnchantment.modify_DefIce).Append(",")
            // .Append(soEnchantment.modify_AtkFire).Append(",")
            // .Append(soEnchantment.modify_DefFire).Append(",")
            // .Append(soEnchantment.modify_AtkBoss).Append(",")
            // .Append(soEnchantment.modify_MAtkBoss).Append(",")
            // .Append(soEnchantment.modify_DefBoss).Append(",")
            // .Append(soEnchantment.modify_AtkAnimal).Append(",")
            // .Append(soEnchantment.modify_DamageCut).Append(",")
            // .Append(soEnchantment.modify_JumpCount).Append(",")
            .Append(soEnchantment.modify_IncreaseAtkByDefRate).Append(",")
            .Append(soEnchantment.modify_IncreaseMAtkByDefRate).Append(",")
            .Append(soEnchantment.modify_IncreaseAtkByHpRate).Append(",")
            .Append(soEnchantment.modify_IncreaseMAtkByHpRate).Append(",")
            // .Append(soEnchantment.modify_IncreaseAtkByMpRate).Append(",")
            .Append(soEnchantment.modify_IncreaseMAtkByMpRate).Append(",")
            .Append(soEnchantment.modify_HealthSkillHealRate).Append(",")
            .Append(soEnchantment.modify_FinalDamageRate).Append(",")
            .Append(soEnchantment.modify_FinalPhysicsDamageRate).Append(",")
            // .Append(soEnchantment.modify_FinalMagicDamageRate).Append(",")
            .Append(soEnchantment.modify_FinalArrowDamageRate).Append(",")
            .Append(soEnchantment.modify_FinalUnarmedDamgeRate).Append(",")
            .Append(soEnchantment.modify_FinalSkillDamageRate).Append(",")
            .Append(soEnchantment.modify_FinalMeleeSkillDamageRate).Append(",")
            .Append(soEnchantment.modify_PetAtkRate).Append(",")
            .Append(soEnchantment.modify_PetDefRate).Append(",")
            .Append(soEnchantment.modify_PetCriticalRate).Append(",")
            .Append(soEnchantment.modify_PetCriticalDamageRate).Append(",")
            .Append(soEnchantment.modify_PetSpeedRate).Append(",")
            .Append(soEnchantment.EquipmentPassiveSkillId).Append(",")
            .Append(soEnchantment.EquipmentPassiveSkillLevel).Append(",")
            .Append(soEnchantment.PriceModify).Append(",")
            .Append(soEnchantment.DurabilityModify).Append(",")
            ;

          // Log.Trace(Instance, $"[{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}]{sb.ToString().TrimEnd(',')}");
          File.AppendAllText(csvFileInfo.FullName, $"{sb.ToString().TrimEnd(',')}{Environment.NewLine}", Encoding.UTF8);
        }
        Log.Info(Instance, $"[{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}] Export complete. {csvFileInfo.FullName}.");
        Log.Trace(Instance, $"[{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}] ---------------------------------------------------------------");
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private DirectoryInfo AssemblyDirectory
    {
      get
      {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new(codeBase);
        var fileInfo = new FileInfo(Uri.UnescapeDataString(uri.Path));
        return fileInfo.Directory;
      }
    }
  }
}
