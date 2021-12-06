using BattleTech;
using BattleTech.Data;
using BattleTech.Portraits;
using BattleTech.UI;
using Gif.Components;
using Harmony;
using IRBTModUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CustomPilotDecorator {
  public class GifSpriteAnimator: MonoBehaviour {
    public UniGif.GifSprites gif { get; set; }
    private float t = 0f;
    public Image portrait { get; set; } = null;
    private int index = 0;
    public void Reset() {
      index = 0;
      t = 0f;
    }
    public void LateUpdate() {
      if (portrait == null) { return; }
      if (gif == null) { return; }
      if (gif.frames.Count == 0) { return; }
      t += Time.deltaTime;
      if (t > gif.frames[index].m_delaySec) {
        this.index = (this.index + 1) % gif.frames.Count;
        t = 0f;
        portrait.sprite = gif.frames[this.index].m_sprite;
      }
    }
  }
  public class GifImageAnimator : MonoBehaviour {
    public UniGif.GifImage gif { get; set; }
    private float t = 0f;
    public RawImage portrait { get; set; } = null;
    private int index = 0;
    public void Reset() {
      index = 0;
      t = 0f;
    }
    public void LateUpdate() {
      if (portrait == null) { return; }
      if (gif == null) { return; }
      if (gif.frames.Count == 0) { return; }
      t += Time.deltaTime;
      if (t > gif.frames[index].m_delaySec) {
        this.index = (this.index + 1) % gif.frames.Count;
        t = 0f;
        portrait.texture = gif.frames[this.index].m_texture2d;
      }
    }
  }
  [HarmonyPatch(typeof(PortraitRenderPanel))]
  [HarmonyPatch("Render")]
  [HarmonyPatch(MethodType.Normal)]
  public static class PortraitRenderPanel_Render {
    public static void Postfix(PortraitRenderPanel __instance, PortraitSettings portraitSettings) {
      try {
        Log.TWL(0, "PortraitRenderPanel.Render "+ portraitSettings.Description.Icon);
        if (__instance.portraitPreviewImage != null) {
          GifImageAnimator gifImageAnimator = __instance.portraitPreviewImage.gameObject.GetComponent<GifImageAnimator>();
          if (gifImageAnimator == null) {
            gifImageAnimator = __instance.portraitPreviewImage.gameObject.AddComponent<GifImageAnimator>();
            gifImageAnimator.portrait = __instance.portraitPreviewImage;
          }
          gifImageAnimator.gif = null;
          if (portraitSettings != null) {
            if (portraitSettings.Description != null) {
              gifImageAnimator.gif = portraitSettings.Description.GetDescrGifImage();
            }
          }
          gifImageAnimator.Reset();
          Log.WL(1, "GifImageAnimator.gif: " + (gifImageAnimator.gif == null ? "null" : "not null"));
        }
      } catch (Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
    }
  }
  [HarmonyPatch(typeof(SGBarracksDossierPanel))]
  [HarmonyPatch("SetPilot")]
  [HarmonyPatch(MethodType.Normal)]
  public static class SGBarracksDossierPanel_SetPilot_Gif {
    public static void Postfix(SGBarracksDossierPanel __instance, Pilot p, SGBarracksMWDetailPanel details, bool isDissmissable, bool isThumb) {
      try {
        Log.TWL(0, "SGBarracksDossierPanel.SetPilot");
        if(__instance.portrait != null) {
          GifSpriteAnimator gifSpriteAnimator = __instance.portrait.gameObject.GetComponent<GifSpriteAnimator>();
          if (gifSpriteAnimator == null) {
            gifSpriteAnimator = __instance.portrait.gameObject.AddComponent<GifSpriteAnimator>();
            gifSpriteAnimator.portrait = __instance.portrait;
          }
          gifSpriteAnimator.gif = null;
          if(p != null) {
            gifSpriteAnimator.gif = p.GetPortraitGifSprite();
          }
          gifSpriteAnimator.Reset();
          Log.WL(1, "gifSpriteAnimator.gif: "+(gifSpriteAnimator.gif==null?"null":"not null"));
        }
      } catch (Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
    }
  }
  [HarmonyPatch(typeof(SG_HiringHall_DetailPanel))]
  [HarmonyPatch("DisplayPilot")]
  [HarmonyPatch(MethodType.Normal)]
  public static class SG_HiringHall_DetailPanel_DisplayPilot {
    public static void Postfix(SG_HiringHall_DetailPanel __instance, Pilot p) {
      try {
        Log.TWL(0, "SG_HiringHall_DetailPanel.DisplayPilot");
        if (__instance.Portrait != null) {
          GifSpriteAnimator gifSpriteAnimator = __instance.Portrait.gameObject.GetComponent<GifSpriteAnimator>();
          if (gifSpriteAnimator == null) {
            gifSpriteAnimator = __instance.Portrait.gameObject.AddComponent<GifSpriteAnimator>();
            gifSpriteAnimator.portrait = __instance.Portrait;
          }
          gifSpriteAnimator.gif = null;
          if (p != null) {
            gifSpriteAnimator.gif = p.GetPortraitGifSprite();
          }
          gifSpriteAnimator.Reset();
          Log.WL(1, "gifSpriteAnimator.gif: " + (gifSpriteAnimator.gif == null ? "null" : "not null"));
        }
      } catch (Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
    }
  }
  [HarmonyPatch(typeof(CombatHUDPortrait))]
  [HarmonyPatch("ResetDisplayedActor")]
  [HarmonyPatch(MethodType.Normal)]
  public static class CombatHUDPortrait_ResetDisplayedActor {
    public static void Postfix(CombatHUDPortrait __instance) {
      try {
        Log.TWL(0, "CombatHUDPortrait.ResetDisplayedActor");
        Pilot pilot = __instance.DisplayedActor.GetPilot();
        if (pilot == null) { return; };
        if (__instance.Portrait != null) {
          GifSpriteAnimator gifSpriteAnimator = __instance.Portrait.gameObject.GetComponent<GifSpriteAnimator>();
          if (gifSpriteAnimator == null) {
            gifSpriteAnimator = __instance.Portrait.gameObject.AddComponent<GifSpriteAnimator>();
            gifSpriteAnimator.portrait = __instance.Portrait;
          }
          gifSpriteAnimator.gif = null;
          if (pilot != null) {
            gifSpriteAnimator.gif = pilot.GetPortraitGifSprite();
          }
          gifSpriteAnimator.Reset();
          Log.WL(1, "gifSpriteAnimator.gif: " + (gifSpriteAnimator.gif == null ? "null" : "not null"));
        }
      } catch (Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
    }
  }
  //[HarmonyPatch(typeof(PilotDef))]
  //[HarmonyPatch("DownsampleSprite")]
  //[HarmonyPatch(MethodType.Normal)]
  //[HarmonyPatch(new Type[] { typeof(Sprite) })]
  //public static class PilotDef_DownsampleSprite {
  //  public static void Prefix(PilotDef __instance, Sprite oldSprite, ref Sprite __result) {
  //    try {
  //      Log.TWL(0, "PilotDef.DownsampleSprite " + __instance.Description.Callsign+" "+__instance.Description.Icon+" oldSprite:"+(oldSprite==null?"null":"not null"));
  //    } catch (Exception e) {
  //      Log.TWL(0, e.ToString(), true);
  //    }
  //  }
  //}
  //[HarmonyPatch(typeof(SGBarracksRosterList))]
  //[HarmonyPatch("PopulateRosterAsync")]
  //[HarmonyPatch(MethodType.Normal)]
  //[HarmonyPatch(new Type[] { typeof(List<Pilot>), typeof(UnityAction<SGBarracksRosterSlot>), typeof(Action), typeof(bool) })]
  //public static class SGBarracksRosterList_PopulateRosterAsync {
  //  private static IEnumerator WaitForPortraitRenderInt(this SGBarracksRosterList __instance, List<Pilot> pilotList, Action complete) {
  //    int requested = 0;
  //    int rendered = 0;
  //    try {
  //      SimGameState sim = __instance.GetSim();
  //      for (int index = 0; index < pilotList.Count; ++index) {
  //        PilotDef pilotDef = pilotList[index].pilotDef;
  //        if (pilotDef.PortraitSettings != null) {
  //          ++requested;
  //          pilotDef.PortraitSettings.RenderPortrait(sim.DataManager, (Action)(() => ++rendered), PortraitManager.PortraitSizes.VERYSMALL, PortraitManager.PortraitSizes.MEDIUM);
  //        }
  //      }
  //    }catch(Exception e) {
  //      Log.TWL(0,e.ToString(),true);
  //    }
  //    do {
  //      yield return (object)null;
  //    }
  //    while (rendered < requested);
  //    yield return (object)null;
  //    complete();
  //  }

  //  public static bool Prefix(SGBarracksRosterList __instance, List<Pilot> pilotList, UnityAction<SGBarracksRosterSlot> pilotSelectedOnClick, Action populateComplete, bool isInHireHall) {
  //    try {
  //      if ((UnityEngine.Object)__instance.coroutineRunner == (UnityEngine.Object)null)
  //        __instance.coroutineRunner = CoroutineRunner.GetNewCoroutineRunner(nameof(SGBarracksRosterList), true);
  //      __instance.populating = true;
  //      __instance.coroutineRunner.StartCoroutine(__instance.WaitForPortraitRenderInt(pilotList, (Action)(() =>
  //      {
  //        try {
  //          __instance.populating = false;
  //          foreach (Pilot pilot in pilotList) {
  //            __instance.AddPilot(pilot, pilotSelectedOnClick, isInHireHall);
  //          }
  //          __instance.SetSorting(0);
  //          __instance.OnSlotSelected(__instance.currentRoster[pilotList[0].GUID]);
  //          __instance.ForceRefreshImmediate();
  //          if (populateComplete == null) { return; }
  //          populateComplete();
  //        }catch(Exception e) {
  //          Log.TWL(0,e.ToString(),true);
  //        }
  //      })));
  //    } catch (Exception e) {
  //      Log.TWL(0, e.ToString(), true);
  //    }
  //    return false;
  //  }
  //}

  [HarmonyPatch(typeof(DataManager.SpriteLoadRequest))]
  [HarmonyPatch("SpriteFromDisk")]
  [HarmonyPatch(MethodType.Normal)]
  [HarmonyPatch(new Type[] { typeof(string) })]
  public static class SpriteLoadRequest_SpriteFromDisk {
    public static bool Prefix(DataManager.SpriteLoadRequest __instance, string assetPath, ref Sprite __result) {
      try {
        if (!File.Exists(assetPath)) { __result = null; return false; }
        try {
          byte[] numArray = File.ReadAllBytes(assetPath);
          Log.TWL(0, "DataManager.SpriteLoadRequest.SpriteFromDisk:" + __instance.resourceId + ":" + assetPath + " isGIF:"+ numArray.IsGIF(),true);
          Texture2D texture2D = null; 
          if (numArray.IsGIF()) {
            try {
              UniGif.GifImage gif = UniGif.GetTexturesList(numArray);
              Log.WL(1, "frames:" + gif.frames.Count + " loop:" + gif.loopCount + " size:" + gif.width + "x" + gif.height, true);
              gif.Register(__instance.resourceId);
              UniGif.GifSprites gifSprites = new UniGif.GifSprites(gif);
              gifSprites.Register(__instance.resourceId);
              texture2D = gif.frames.Count > 0 ? gif.frames[0].m_texture2d:new Texture2D(1,1);
            } catch (Exception e) {
              Log.TWL(0, assetPath);
              Log.WL(0, e.ToString());
              __result = null; return false;
            }
          } else {
            if (TextureManager.IsDDS(numArray))
              texture2D = TextureManager.LoadTextureDXT(numArray);
            else if (TextureManager.IsPNG(numArray) || TextureManager.IsJPG(numArray)) {
              texture2D = new Texture2D(2, 2, TextureFormat.DXT5, false);
              if (!texture2D.LoadImage(numArray)) {
                __result = null; return false;
              }
            } else {
              Log.TWL(0, string.Format("Unable to load unknown file type from disk (not DDS, PNG, or JPG) at: {0}", (object)assetPath),true);
              __result = null; return false;
            }
          }         
          __result = Sprite.Create(texture2D, new UnityEngine.Rect(0.0f, 0.0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0U, SpriteMeshType.FullRect, Vector4.zero);
          return false;
        } catch (Exception ex) {
          Log.TWL(0,string.Format("Unable to load image at: {0}\nExceptionMessage:\n{1}", (object)assetPath, (object)ex.Message),true);
          __result = null; return false;
        }
      }catch(Exception e) {
        Log.TWL(0, e.ToString(), true);
        return true;
      }
    }
  }

  [HarmonyPatch(typeof(TextureManager))]
  [HarmonyPatch("ProcessCompletedRequest")]
  [HarmonyPatch(MethodType.Normal)]
  [HarmonyPatch(new Type[] { typeof(TextureManager.TextureLoadRequest) })]
  public static class TextureManager_ProcessCompletedRequest {
    public static bool Prefix(TextureManager __instance, TextureManager.TextureLoadRequest completed) {
      try {
        Log.TWL(0, "TextureManager.ProcessCompletedRequest:" + completed.loadRequest.Id + ":" + completed.loadRequest.Path);
        string message = "";
        if (completed.loadRequest.IsError(out message)) {
          string error = string.Format("Failed to load texture {0} with error {1}", (object)completed.loadRequest.Id, (object)message);
          Log.TWL(0,error);
          if (completed.error == null) { return false; }
          completed.error(error);
        } else {
          Texture2D texture;
          if (__instance.loadedTextures.ContainsKey(completed.loadRequest.Id)) {
            Log.WL(1,"cached");
            texture = __instance.loadedTextures[completed.loadRequest.Id];
          } else {
            Log.WL(1, "non cached");
            byte[] data = completed.loadRequest.GetBytes();
            Log.WL(1, "is GIF:" + data.IsGIF());
            if (data.IsGIF()) {
              try {
                UniGif.GifImage gif = UniGif.GetTexturesList(data);
                Log.WL(1, "frames:" + gif.frames.Count + " loop:" + gif.loopCount + " size:" + gif.width + "x" + gif.height, true);
                gif.Register(completed.loadRequest.Id);
                texture = gif.frames.Count > 0 ? gif.frames[0].m_texture2d : new Texture2D(1, 1);
              } catch (Exception e) {
                Log.TWL(0, completed.loadRequest.Id+":"+ completed.loadRequest.Path);
                Log.WL(0, e.ToString());
                texture = TextureManager.TextureFromBytes(data);
              }
            } else {
              texture = TextureManager.TextureFromBytes(data);
            }
            __instance.loadedTextures.Add(completed.loadRequest.Id, texture);
          }
          if (completed.callback == null) { return false; }
          Log.WL(1, "completed.callback");
          completed.callback(texture);
        }
        return false;
      } catch (Exception e) {
        Log.TWL(0, e.ToString(), true);
        return true;
      }
    }
  }
  //[HarmonyPatch(typeof(TextureManager))]
  //[HarmonyPatch("TextureFromBytes")]
  //[HarmonyPatch(MethodType.Normal)]
  //[HarmonyPatch(new Type[] { typeof(byte[]) })]
  //public static class TextureManager_TextureFromBytes {
  //  private static Dictionary<string, GifDecoder> gifImages = new Dictionary<string, GifDecoder>();
  //  public static bool IsGIF(byte[] data) {
  //    return (data[0] == 0x47) && (data[0] == 0x49) && (data[0] == 0x46);
  //  }
  //  public static bool Prefix(byte[] data,ref Texture2D __result) {
  //    try {
  //      if (TextureManager.IsDDS(data)) {
  //        __result = TextureManager.LoadTextureDXT(data);
  //        return false;
  //      }
  //      if (TextureManager_TextureFromBytes.IsGIF(data)) {
  //        MemoryStream stream = new MemoryStream(data);
  //        stream.Position = 0;
  //        GifDecoder gif = new GifDecoder();
  //        gif.Read(stream);
  //        __result = gif.GetTexture(0);
  //      } else { 
  //        Texture2D texture2D = new Texture2D(2, 2);
  //        texture2D.mipMapBias = 0.0f;
  //        texture2D.anisoLevel = 2;
  //        Texture2D tex = texture2D;
  //        if (TextureManager.IsPNG(data) || TextureManager.IsJPG(data)) {
  //          tex.LoadImage(data);
  //        } else {
  //          tex.LoadRawTextureData(data);
  //        }
  //        tex.Apply();
  //        __result = tex;
  //      }
  //      return false;
  //    } catch (Exception e) {
  //      Log.TWL(0, e.ToString(), true);
  //      return true;
  //    }
  //  }
  //}
}