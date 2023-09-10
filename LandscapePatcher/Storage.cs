using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapePatcher
{
    public class Storage
    {
        public FormKey formKey;
        public String formValue;
        public Storage(FormKey fK, String fV) {
            formKey = fK;
            formValue = fV;
        }
    }
}
