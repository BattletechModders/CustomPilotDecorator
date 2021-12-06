using BattleTech;
using BattleTech.Data;
using HBS.Collections;
using Newtonsoft.Json;
using SVGImporter;
using System.Collections.Generic;
using UnityEngine;
using static BattleTech.Data.DataManager;

namespace CustomPilotDecorator {
  public static class PilotDecorationDefHelper {
    private static Dictionary<string, PilotDecorationDef> PilotDecorationDefs = new Dictionary<string, PilotDecorationDef>();
    public static void Register(this PilotDecorationDef def) {
      if (PilotDecorationDefs.ContainsKey(def.Description.Id)) {
        PilotDecorationDefs[def.Description.Id] = def;
      } else {
        PilotDecorationDefs.Add(def.Description.Id, def);
      }
    }
    public static HashSet<PilotDecorationDef> getDecorations(this PilotDef def) {
      HashSet<PilotDecorationDef> result = new HashSet<PilotDecorationDef>();
      Log.TWL(0, "PilotDecorationDefHelper.getDecorations " + def.Description.Id + ":" + def.Description.Callsign );
      Log.W(1, "tags:"); foreach (string tag in def.PilotTags) { Log.W(1, tag); }; Log.WL(0, "");
      foreach (var decoration in PilotDecorationDefs) {
        if (decoration.Value.shouldHaveTags.Count > 0) {
          if (def.PilotTags.ContainsAll(decoration.Value.shouldHaveTags) == false) {
            continue;
          }
        }
        if (decoration.Value.shouldNotHaveTags.Count > 0) {
          if (def.PilotTags.ContainsAny(decoration.Value.shouldNotHaveTags)) { continue; }
        }
        Log.WL(1, decoration.Key);
        result.Add(decoration.Value);
      }
      return result;
    }
    public static HashSet<PilotDecorationDef> getDecorations(this PilotDef def, DecorationType type) {
      Log.TWL(0, "PilotDecorationDefHelper.getDecorations "+def.Description.Id+":"+def.Description.Callsign+" type:"+type);
      Log.W(1, "tags:"); foreach (string tag in def.PilotTags) { Log.W(1, tag); }; Log.WL(0, "");
      HashSet<PilotDecorationDef> result = new HashSet<PilotDecorationDef>();
      foreach (var decoration in PilotDecorationDefs) {
        if (decoration.Value.Type != type) { continue; }
        if (decoration.Value.shouldHaveTags.Count > 0) {
          if (def.PilotTags.ContainsAll(decoration.Value.shouldHaveTags) == false) { continue; }
        }
        if (decoration.Value.shouldNotHaveTags.Count > 0) {
          if (def.PilotTags.ContainsAny(decoration.Value.shouldNotHaveTags)) { continue; }
        }
        Log.WL(1,decoration.Key);
        result.Add(decoration.Value);
      }
      return result;
    }
  }
  public enum DecorationType { Ability, Shevron };
  public enum DecorationSource { Ronin, Veteran };
  public class PilotDecorationDescriptionDef {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public string Icon { get; set; }
  }
  public class PilotDecorationDef: ILoadDependencies {
    public PilotDecorationDescriptionDef Description { get; set; } = new PilotDecorationDescriptionDef();
    [JsonIgnore]
    private BaseDescriptionDef f_description = null;
    [JsonIgnore]
    public BaseDescriptionDef description {
      get {
        if (f_description == null) { f_description = new BaseDescriptionDef(Description.Id, Description.Name, Description.Details, Description.Icon); }
        return f_description;
      }
    }
    public DecorationType Type { get; set; }
    public bool IconColorOverride { get; private set; } = false;
    [JsonIgnore]
    public Color iconColor { get; set; } = Color.white;
    public DecorationSource ShevronDecorationSource { get; set; } = DecorationSource.Ronin;
    public string IconColor {
      set {
        if(ColorUtility.TryParseHtmlString(value,out Color color)) {
          IconColorOverride = true;
          iconColor = color;
        } else {
          Log.TWL(0, value + " is not valid HTML color");
        }
      }
    }
    public TagSet shouldHaveTags { get; private set; } = new TagSet();
    public TagSet shouldNotHaveTags { get; private set; } = new TagSet();
    public List<string> ShouldHaveTags { set { shouldHaveTags = new TagSet(value); } }
    public List<string> ShouldNotHaveTags { set { shouldNotHaveTags = new TagSet(value); } }
    [JsonIgnore]
    public SVGAsset Icon => !string.IsNullOrEmpty(this.Description.Icon) && this.DataManager.Exists(BattleTechResourceType.SVGAsset,this.Description.Icon) ? this.DataManager.GetObjectOfType<SVGAsset>(this.Description.Icon, BattleTechResourceType.SVGAsset) : (SVGAsset)null;
    [JsonIgnore]
    private DataManager dataManager;
    [JsonIgnore]
    public DataManager DataManager {
      get {
        if (this.dataManager == null) { this.dataManager = UnityGameInstance.BattleTechGame.DataManager; }
        return this.dataManager;
      }
      set => this.dataManager = value;
    }
    public bool DependenciesLoaded(uint loadWeight) {
      if (this.dataManager == null) { return false; }
      if(string.IsNullOrEmpty(this.Description.Icon) == false) {
        if (this.dataManager.Exists(BattleTechResourceType.SVGAsset, this.Description.Icon) == false) { return false; }
      }
      return true;
    }
    public void GatherDependencies(DataManager dataManager, DependencyLoadRequest dependencyLoad, uint activeRequestWeight) {
      this.dataManager = dataManager;
      if (string.IsNullOrEmpty(this.Description.Icon) == false) {
        dependencyLoad.RequestResource(BattleTechResourceType.SVGAsset, this.Description.Icon);
      }
    }
  }
}