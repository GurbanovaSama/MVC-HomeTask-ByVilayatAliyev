﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurpleBuzzPr.DAL;
using PurpleBuzzPr.DTOs.UserDTOs;
using PurpleBuzzPr.Models;

namespace PurpleBuzzPr.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountsController(AppDbContext appDbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateUserDto createUserDto)
        {
            if(!ModelState.IsValid)
            {
                return View(createUserDto); 
            }
            AppUser user = new AppUser();   
            user.FirstName = createUserDto.FirstName;       
            user.LastName = createUserDto.LastName;
            user.Email = createUserDto.Email;
            user.UserName = createUserDto.Username;
            var result = await _userManager.CreateAsync(user, createUserDto.Password);
        
            if(!result.Succeeded)
            {
             
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }
                return View(createUserDto);    
            }
            return RedirectToAction(nameof(Index), "Home");     

        }

        public IActionResult Login()
        {
            return View();  
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            AppUser? user = await _userManager.FindByNameAsync(loginUserDto.EmailOrUsername);    
            if ( user == null)
            {
               user = await _userManager.FindByEmailAsync(loginUserDto.EmailOrUsername);  
                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "Username or    password is wrong");
                    return View();
                }
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginUserDto.Password, loginUserDto.IsPersistant, true);
             if (!result.Succeeded)
             {
                 ModelState.AddModelError(string.Empty, "Username or    password is wrong");
                 return View();
             }
         
             return RedirectToAction(nameof(Index), "Home");  
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index) , "Home");  
        }

    }
}
