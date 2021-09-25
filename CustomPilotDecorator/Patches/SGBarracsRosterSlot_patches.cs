using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using Harmony;
using SVGImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomPilotDecorator {
  public class CallsignAutoSizer : MonoBehaviour {
    public LocalizableText text { get; set; } = null;
    public void Awake() {
      text = this.gameObject.GetComponent<LocalizableText>();
    }
    public void Update() {
      if (text == null) { return; }
      Vector3 size = text.GetPreferredValues(float.PositiveInfinity,float.PositiveInfinity);
      text.rectTransform.sizeDelta = size;
    }
  }
  [HarmonyPatch(typeof(SGBarracksRosterSlot))]
  [HarmonyPatch("Refresh")]
  [HarmonyPatch(MethodType.Normal)]
  [HarmonyPatch(new Type[] { })]
  public static class SGBarracksRosterSlot_Refresh {
    public static Transform FindRecursive(this Transform parent, string name) {
      Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
      foreach (Transform tr in trs) { if (tr.name == name) { return tr; } }
      return null;
    }
    public static void Prefix(SGBarracksRosterSlot __instance, ref Pilot ___pilot, ref GameObject ___AbilitiesObject, ref List<SVGImage> ___abilities, ref List<GameObject> ___activeAbilityObjs, ref List<GameObject> ___emptyAbilityObjs, ref List<HBSTooltip> ___AbilityReferences, ref SVGImage ___roninIcon) {
      if (___pilot == null) { return; }
      try {
        {
          OrderedGridLayoutGroup orderLayout = ___AbilitiesObject.GetComponent<OrderedGridLayoutGroup>();
          if (orderLayout == null) {
            GridLayoutGroup old_gridLayout = ___AbilitiesObject.GetComponent<GridLayoutGroup>();
            RectOffset padding = old_gridLayout.padding;
            Vector2 cellSize = old_gridLayout.cellSize;
            TextAnchor childAlignment = old_gridLayout.childAlignment;
            GridLayoutGroup.Constraint constraint = old_gridLayout.constraint;
            HideFlags hideFlags = old_gridLayout.hideFlags;
            int constraintCount = old_gridLayout.constraintCount;
            Vector2 spacing = old_gridLayout.spacing;
            GridLayoutGroup.Axis startAxis = old_gridLayout.startAxis;
            GridLayoutGroup.Corner startCorner = old_gridLayout.startCorner;
            GameObject.DestroyImmediate(old_gridLayout);
            orderLayout = ___AbilitiesObject.AddComponent<OrderedGridLayoutGroup>();
            orderLayout.cellSize = cellSize;
            orderLayout.childAlignment = childAlignment;
            orderLayout.hideFlags = hideFlags;
            orderLayout.spacing = spacing;
            orderLayout.startAxis = startAxis;
            orderLayout.startCorner = startCorner;
            orderLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            orderLayout.constraintCount = 3;
          }
          List<AbilityDef> primaryPilotAbilities = SimGameState.GetPrimaryPilotAbilities(___pilot.pilotDef);
          HashSet<PilotDecorationDef> decorations = ___pilot.pilotDef.getDecorations(DecorationType.Ability);
          int abilities_count = primaryPilotAbilities.Count + decorations.Count;
          if (abilities_count < 3) { abilities_count = 3; }
          orderLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
          orderLayout.constraintCount = abilities_count;
          for (int t = ___AbilitiesObject.transform.childCount; t < abilities_count; ++t) {
            RectTransform baseAbility = ___AbilitiesObject.transform.GetChild(0) as RectTransform;
            GameObject newAbility = GameObject.Instantiate(baseAbility.gameObject);
            newAbility.transform.SetParent(baseAbility.parent);
            newAbility.transform.localPosition = baseAbility.localPosition;
            newAbility.transform.localRotation = baseAbility.localRotation;
            newAbility.transform.localScale = baseAbility.localScale;
            Transform empty_ab1 = newAbility.transform.FindRecursive("empty-ab1");
            Transform unlocked_ab1 = newAbility.transform.FindRecursive("unlocked-ab1");
            ___activeAbilityObjs.Add(unlocked_ab1.gameObject);
            ___emptyAbilityObjs.Add(empty_ab1.gameObject);
            Transform ability_icon1 = newAbility.transform.FindRecursive("ability_icon1");
            ___AbilityReferences.Add(ability_icon1.gameObject.GetComponent<HBSTooltip>());
            ___abilities.Add(ability_icon1.gameObject.GetComponent<SVGImage>());
          }
          for (int t = 0; t < ___AbilitiesObject.transform.childCount; ++t) {
            RectTransform child = ___AbilitiesObject.transform.GetChild(t) as RectTransform;
            if (child != null) {
              ShuffleGridLayoutElement el = child.gameObject.GetComponent<ShuffleGridLayoutElement>();
              if (el == null) { el = child.gameObject.AddComponent<ShuffleGridLayoutElement>(); }
              el.weight = t;
            }
          }
          ShuffleGridLayoutGroup shuffle = ___AbilitiesObject.GetComponent<ShuffleGridLayoutGroup>();
          if (shuffle == null) { shuffle = ___AbilitiesObject.AddComponent<ShuffleGridLayoutGroup>(); } else { shuffle.Refresh(); }
        }
        Transform mw_Callsign = ___roninIcon.gameObject.transform.parent;
        HashSet<PilotDecorationDef> sdecorations = ___pilot.pilotDef.getDecorations(DecorationType.Shevron);
        for (int t = mw_Callsign.childCount - 4; t < sdecorations.Count; ++t) {
          GameObject shevron = GameObject.Instantiate(___roninIcon.gameObject);
          shevron.transform.SetParent(___roninIcon.gameObject.transform.parent);
          shevron.transform.localPosition = ___roninIcon.gameObject.transform.localPosition;
          shevron.transform.localRotation = ___roninIcon.gameObject.transform.localRotation;
          shevron.transform.localScale = ___roninIcon.gameObject.transform.localScale;
        }
      } catch(Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
    }
    public static void Postfix(SGBarracksRosterSlot __instance, ref Pilot ___pilot, ref GameObject ___AbilitiesObject, ref List<SVGImage> ___abilities, ref List<GameObject> ___activeAbilityObjs, ref List<GameObject> ___emptyAbilityObjs, ref List<HBSTooltip> ___AbilityReferences, ref SVGImage ___roninIcon,ref LocalizableText ___callsign) {
      try { 
        int abilities_count = 3;
        int real_abilies_count = 0;
        List<PilotDecorationDef> decorations = ___pilot == null?new List<PilotDecorationDef>():___pilot.pilotDef.getDecorations(DecorationType.Ability).ToList();
        if (___pilot != null) {
          List<AbilityDef> primaryPilotAbilities = SimGameState.GetPrimaryPilotAbilities(___pilot.pilotDef);
          abilities_count = primaryPilotAbilities.Count + decorations.Count;
          real_abilies_count = primaryPilotAbilities.Count;
        }
        for (int t = 0; t < ___AbilitiesObject.transform.childCount; ++t) {
          RectTransform child = ___AbilitiesObject.transform.GetChild(t) as RectTransform;
          if (child != null) {
            if (t < 3) { child.gameObject.SetActive(true); } else { child.gameObject.SetActive(t < abilities_count); }
          }
        }
        for(int t = real_abilies_count; t < abilities_count; ++t) {
          PilotDecorationDef decoration = decorations[t - real_abilies_count];
          ___activeAbilityObjs[t].SetActive(true);
          ___emptyAbilityObjs[t].SetActive(false);
          ___abilities[t].vectorGraphics = decoration.Icon;
          if (decoration.IconColorOverride) { ___abilities[t].color = decoration.iconColor; }
          ___AbilityReferences[t].SetDefaultStateData(TooltipUtilities.GetStateDataFromObject(decoration.description));
        }
        decorations = ___pilot == null ? new List<PilotDecorationDef>() : ___pilot.pilotDef.getDecorations(DecorationType.Shevron).ToList();
        for(int t = 4; t < ___roninIcon.gameObject.transform.parent.childCount; ++t) {
          Transform tr = ___roninIcon.gameObject.transform.parent.GetChild(t);
          if ((t - 4) > decorations.Count) { tr.gameObject.SetActive(false); };
          tr.gameObject.SetActive(true);
          PilotDecorationDef decoration = decorations[t - 4];
          SVGImage img = tr.gameObject.GetComponent<SVGImage>();
          if (img != null) {
            img.vectorGraphics = decoration.Icon;
            if (decoration.IconColorOverride) { ___abilities[t].color = decoration.iconColor; }
          }
          HBSTooltip tooltip = tr.gameObject.GetComponent<HBSTooltip>();
          if (tooltip != null) {
            tooltip.SetDefaultStateData(TooltipUtilities.GetStateDataFromObject(decoration.description));
          }
        }
        //Vector2 size = ___callsign.GetPreferredValues(float.PositiveInfinity, float.PositiveInfinity);
        CallsignAutoSizer autoSizer = ___callsign.gameObject.GetComponent<CallsignAutoSizer>();
        if (autoSizer == null) { autoSizer = ___callsign.gameObject.AddComponent<CallsignAutoSizer>(); }
      } catch(Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
}
  }
}