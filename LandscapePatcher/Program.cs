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
                }
            }


                foreach (var patchStatic in state.LoadOrder.PriorityOrder.WinningOverrides<IStaticGetter>())
            {
                //var winningOverride = winningOverrides.Where(x => x.FormKey == book.FormKey).Last();
                //var stat = state.PatchMod.Statics.GetOrAddAsOverride(patchStatic);
                var text = patchStatic.Model?.AlternateTextures;
                if(text != null)
                {
                    List<AlternateTexture> alternateTextures = new List<AlternateTexture>();    
                    AlternateTexture newAltText = new AlternateTexture();
                    bool check = false;
                    foreach (var altText in text) {
                        if (altText != null)
                        {
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
                                            if (tempname == item.formValues) { 
                                                
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }             
            }
        }
    }
}
