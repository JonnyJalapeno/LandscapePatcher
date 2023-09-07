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
            /*var bookCovers = state.LoadOrder.GetModByFileName("Book Covers Skyrim Updated.esp");
            if (bookCovers == null)
            {
                System.Console.WriteLine("Book Covers Skyrim Updated.esp not found");
                return;
            }
            var books = bookCovers.Books.Select(x => x.FormKey).ToList();
            var books2 = bookCovers.Books;*/
            //var winningOverrides = state.LoadOrder.PriorityOrder.WinningOverrides<IBookGetter>().Where(x => books.Contains(x.FormKey)).ToList();

            foreach (var patchStatic in state.LoadOrder.PriorityOrder.WinningOverrides<IStaticGetter>())
            {
                //var winningOverride = winningOverrides.Where(x => x.FormKey == book.FormKey).Last();
                var stat = state.PatchMod.Statics.GetOrAddAsOverride(patchStatic);
                var text = patchStatic.Model?.AlternateTextures;
                if(text != null)
                {
                    foreach (var altText in text) {
                        if (altText != null)
                        {
                            var name = altText.NewTexture.TryResolve(state.LinkCache, out var textureSet);
                            //Console.WriteLine(textureSet?.EditorID);
                            if (textureSet != null)
                            {
                                if (textureSet.EditorID != null)
                                {
                                    if (textureSet.EditorID.Contains("Landscape")) {
                                        text.
                                    }
                                }
                            }
                        }
                    }

                }
                

                //var key = book.FormKey;
                //var grab = books2.TryGetValue(key, out var value) ? value : null;
                /*if (grab != null)
                {

                    ObjectBounds bounds = new ObjectBounds();

                    var x = grab.ObjectBounds.First.X;
                    var y = grab.ObjectBounds.First.Y;
                    var z = grab.ObjectBounds.First.Z;
                    var x1 = grab.ObjectBounds.Second.X;
                    var y1 = grab.ObjectBounds.Second.Y;
                    var z1 = grab.ObjectBounds.Second.Z;

                    bounds.First = new P3Int16(x, y, z);
                    bounds.Second = new P3Int16(x1, y1, z1);

                    patchBook.ObjectBounds = bounds;



                    if (grab.Model != null)
                    {
                        Model mod = new Model();
                        string dd = grab.Model.File.DataRelativePath;
                        string ff = dd.Substring(7);
                        mod.File = ff;

                        if (grab.Model.AlternateTextures != null)
                        {
                            var text = grab.Model.AlternateTextures.ToArray();
                            ExtendedList<AlternateTexture> arrtex = new ExtendedList<AlternateTexture>();
                            foreach (var texture in text)
                            {
                                var newtext = new AlternateTexture();
                                newtext.Name = texture.Name;
                                newtext.NewTexture.FormKey = texture.NewTexture.FormKey;
                                newtext.Index = texture.Index;
                                if (newtext != null)
                                {
                                    arrtex.Add(newtext);
                                }
                            }
                            mod.AlternateTextures = arrtex;
                        }
                        patchBook.Model = mod;
                    }
                    patchBook.InventoryArt.FormKey = grab.InventoryArt.FormKey;
                }*/

            }
        }
    }
}
