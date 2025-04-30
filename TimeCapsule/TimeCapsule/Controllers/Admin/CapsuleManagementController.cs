using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeCapsule.Controllers.Admin
{
    public class CapsuleManagementController : TimeCapsuleBaseController
    {
        [Authorize(Roles = "Admin")]
        [Route("AdminPanel/Capsules")]
       
        public IActionResult GetCapsules()
        {
            return View();
        }
    }
}
