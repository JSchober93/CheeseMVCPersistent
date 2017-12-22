﻿using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller

    {
        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {

            List<Menu> menus = context.Menus.ToList();
            return View(menus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();

            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu
                {
                    Name = addMenuViewModel.Name
                };

                context.Menus.Add(newMenu);
                context.SaveChanges();
                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }

            return View(addMenuViewModel);
        }

        public IActionResult ViewMenu(int id)
        {
            List<CheeseMenu> items = context.CheeseMenus.Include(item => item.Cheese).Where(cm => cm.MenuID == id).ToList();
            Menu currentMenu = context.Menus.Single(c => c.ID == id);


            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel();

            viewMenuViewModel.Items = items;
            viewMenuViewModel.Menu = currentMenu;

            return View(viewMenuViewModel);
        }


        public IActionResult AddItem(int id)
        {
            Menu currentMenu = context.Menus.Single(c => c.ID == id);
            IEnumerable<Cheese> cheeses = context.Cheeses.ToList();

            AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(currentMenu, cheeses);

            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            int cheeseID = addMenuItemViewModel.cheeseID;
            int menuID = addMenuItemViewModel.menuID;

            if (ModelState.IsValid)
            {
                IList<CheeseMenu> existingItems = context.CheeseMenus.Where(cm => cm.CheeseID == cheeseID).Where(cm => cm.MenuID == menuID).ToList();
                if (existingItems.Count == 0)
                {
                    CheeseMenu newCheeseMenu = new CheeseMenu
                    {
                        CheeseID = cheeseID,
                        MenuID = menuID
                    };
                    context.CheeseMenus.Add(newCheeseMenu);
                    context.SaveChanges();
                    return Redirect("/Menu/ViewMenu/" + menuID);
                }
            }

            return View(addMenuItemViewModel);
        }

    }
}
