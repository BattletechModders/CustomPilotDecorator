using BattleTech;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
//using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomPilotDecorator{
  public class CPDSettings {
    public bool debugLog { get; set; } = false;
  }
  public static class Core {
    public static readonly float Epsilon = 0.001f;
    public static string BaseDir { get; private set; }
    public static CPDSettings Settings { get; set; } = new CPDSettings();
    public static void FinishedLoading(List<string> loadOrder, Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources) {
      Log.TWL(0, "FinishedLoading", true);
      try {
        foreach (var customResource in customResources) {
          Log.TWL(0, "customResource:" + customResource.Key);
          if (customResource.Key == "PilotDecorationDef") {
            foreach (var entry in customResource.Value) {
              try {
                Log.WL(1, entry.Value.FilePath);
                PilotDecorationDef def = JsonConvert.DeserializeObject<PilotDecorationDef>(File.ReadAllText(entry.Value.FilePath));
                Log.WL(1, "id:"+def.Description.Id);
                Log.WL(1, JsonConvert.SerializeObject(def, Formatting.Indented));
                def.Register();
              }catch(Exception e) {
                Log.TWL(0, e.ToString(), true);
              }
            }
          }
        }
      } catch (Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
    }
    public static void Init(string directory, string settingsJson) {
      Log.BaseDirectory = directory;
      Log.InitLog();
      Core.BaseDir = directory;
      Core.Settings = JsonConvert.DeserializeObject<CustomPilotDecorator.CPDSettings>(settingsJson);
      Log.TWL(0, "Initing... " + directory + " version: " + Assembly.GetExecutingAssembly().GetName().Version, true);
      //Log.WL(1, "PNG encoder guid:" + ImageFormat.Png.Guid, true);
      //Log.WL(1, "ImageCodecInfo.GetImageEncoders:" + ImageCodecInfo.GetImageEncoders().Length,true);
      //foreach(ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders()) {
      //  Log.WL(2, encoder.FormatID.ToString(), true);
      //}
      try {
        var harmony = new Harmony("io.kmission.custompilotdecorator");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
      } catch (Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
    }
  }
}
