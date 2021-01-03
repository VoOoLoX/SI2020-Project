﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared;

namespace API.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class RegisterController : ControllerBase {
		public struct RegisterInfo {
			public string Email { get; set; }
			public string Password { get; set; }
			public string ConfirmPassword { get; set; }
		}

		[AllowAnonymous]
		[HttpPost]
		public IActionResult Register([FromBody] RegisterInfo register_info) {
			return RegisterUser(register_info);
		}

		private IActionResult RegisterUser(RegisterInfo register_info) {
			if (UserBusiness.UserExists(register_info.Email))
				return BadRequest(new { message = "Email already in use." });

			if (register_info.Password.Length < 8)
				return BadRequest(new { message = "Password must be at least 8 characters long." });

			if (register_info.Password != register_info.ConfirmPassword)
				return BadRequest(new { message = "Passwords do not match." });

			var hasher = new PasswordHasher<string>();

			var user = new User {
				Email = register_info.Email,
				Password = hasher.HashPassword(register_info.Email, register_info.Password),
				RoleID = (int)UserRoles.Customer
			};

			if (UserBusiness.Insert(user))
				return Ok(new { message = "Registration successful." });

			return BadRequest();
		}
	}
}
