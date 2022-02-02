using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

using Shiftbid.Models;

namespace Shiftbid.Models.ViewModels
{
    public class ResponsesViewModel
    {
        public string EmailAddress { get; set; }
        public IEnumerable<SelectListItem> Shifts { get; set; }
    }
}
