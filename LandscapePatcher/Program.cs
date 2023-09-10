using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using DynamicData;

namespace LandscapePatcher
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {       
            List<Storage> staticItems = new List<Storage>();
            foreach (var staticText in state.LoadOrder.PriorityOrder.TextureSet().WinningOverrides()) { 
                if(staticText != null)
                {
                    if (staticText.EditorID != null) {
                        if (staticText.EditorID.Contains("Static")) {
                                Storage itm = new Storage(staticText.FormKey, staticText.EditorID);
                                staticItems.Add(itm);
                        }
                    }
                    if (staticText.NormalOrGloss != null) {
                        var length = staticText.NormalOrGloss.ToString().Length;
                        if (File.Exists(state.DataFolderPath + "\\textures\\" + staticText.Diffuse?.ToString()) && File.Exists(state.DataFolderPath + "\\textures\\" + staticText.NormalOrGloss?.ToString().Substring(length - 6, length)+".dds") && File.Exists(staticText.NormalOrGloss?.DataRelativePath) && File.Exists(staticText.Height?.DataRelativePath))
                        {
                            Console.WriteLine(staticText.Diffuse?.ToString());
                        }
                    }
                    
                }
            }


            foreach (var patchStatic in state.LoadOrder.PriorityOrder.Static().WinningOverrides(true))
            {
                
                var text = patchStatic.Model?.AlternateTextures;
                if(text != null)
                {
                    List<AlternateTexture> alternateTextures = new List<AlternateTexture>();
                    bool check = false;
                    foreach (var altText in text) {
                        
                        if (altText != null)
                        {
                            AlternateTexture newAltText = new AlternateTexture();
                            var name = altText.NewTexture.TryResolve(state.LinkCache, out var textureSet); 
                            newAltText.Index = altText.Index;
                            newAltText.Name = altText.Name;
                            

                            if (textureSet != null)
                            {
                                newAltText.NewTexture.SetTo(textureSet);
                                if (textureSet.EditorID != null)
                                {
                                    if (textureSet.EditorID.Contains("Landscape")) {
                                        var tempname = "Static" + textureSet.EditorID;
                                        foreach (var item in staticItems) {
                                            if (tempname == item.formValue) {
                                                newAltText.NewTexture.SetTo(item.formKey);
                                                check = true;
                                            }
                                        }
                                    }
                                }
                                alternateTextures.Add(newAltText);
                            }
                            
                        }    
                    }
                    if(check)
                    {
                        var stat = state.PatchMod.Statics.GetOrAddAsOverride(patchStatic);
                        stat.Model?.AlternateTextures?.SetTo(alternateTextures);
                    }
                } 
            }
        }
    }
}
