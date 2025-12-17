using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdminWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace AdminWeb.Controllers;

[Authorize]
public class HomeController : Controller
{

}
