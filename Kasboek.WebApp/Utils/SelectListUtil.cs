using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Kasboek.WebApp.Utils
{
    public static class SelectListUtil
    {
        public static SelectList GetSelectList(IList<KeyValuePair<int, string>> items, int? selectedValue = null)
        {
            return new SelectList(items, "Key", "Value", selectedValue);
        }
    }
}
