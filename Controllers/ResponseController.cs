using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

using Shiftbid.Models;
using Shiftbid.Data;

namespace Shiftbid.Controllers
{
    public class ResponseController : Controller
    {
        private readonly ApplicationDbContext context;
        public ResponseController(ApplicationDbContext ctx)
        {
            context = ctx;
        }



    }
}