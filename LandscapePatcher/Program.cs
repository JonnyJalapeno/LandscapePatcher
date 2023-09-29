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
            foreach (var staticText in state.LoadOrder.PriorityOrder.TextureSet().WinningOverrides(true)) { 
                if(staticText != null)
                {                   
                    if (staticText.Diffuse != null)
                    {
                        var directory = Path.GetDirectoryName(staticText.Diffuse);
                        var fileName = Path.GetFileName(staticText.Diffuse);
                        var gamepath = state.DataFolderPath+"\\textures\\";
                        switch (fileName)
                        {
                            case "Snow01Landscape.dds":
                                fileName = "Snow01.dds";
                                break;
                            case "Snow02Landscape.dds":
                                fileName = "Snow02.dds";
                                break;
                        }

                        var parallaxPath = directory+"\\parallax\\"+fileName;
                        var alphaDiffusePath = directory+"\\"+fileName;
                        var normalPath = directory+"\\"+ fileName.Insert(fileName.Length-4,"_n");
                        var heightPath = directory + "\\" + fileName.Insert(fileName.Length - 4, "_p");


                        if (File.Exists(gamepath + parallaxPath) && File.Exists(gamepath + alphaDiffusePath) && File.Exists(gamepath + normalPath) && File.Exists(gamepath + heightPath))
                        {
                            var txst = state.PatchMod.TextureSets.AddNew();
                            txst.EditorID = "Static" + staticText.EditorID;
                            txst.Diffuse = alphaDiffusePath;
                            txst.NormalOrGloss = normalPath;
                            txst.Height = heightPath;
                            var stat = state.PatchMod.TextureSets.GetOrAddAsOverride(staticText);
                            stat.Diffuse = parallaxPath;
                        }
                    }
                }
            }

            foreach (var staticText in state.LoadOrder.PriorityOrder.TextureSet().WinningOverrides(true)) {
                if (staticText.EditorID != null)
                {
                    if (staticText.EditorID.Contains("Static"))
                    {
                        Storage itm = new Storage(staticText.FormKey, staticText.EditorID);
                        staticItems.Add(itm);
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
