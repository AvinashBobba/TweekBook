using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweekBook.Contracts.V1.Requests;

namespace TweekBook.Controllers.V1
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost("role")]
        public async Task<IActionResult> Create([FromBody]CreateRoleRequest createRoleRequest )
        {
            IdentityRole identityRole = new IdentityRole(roleName:createRoleRequest.Role);
            var result = await _roleManager.CreateAsync(identityRole);
            return Ok(result);
        }
    } 
}
