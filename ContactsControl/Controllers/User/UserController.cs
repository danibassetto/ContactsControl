﻿using Microsoft.AspNetCore.Mvc;
using ContactsControl.Models;
using ContactsControl.Repositories;
using ContactsControl.Filters;

namespace ContactControl.Controllers
{
    [LoggedUserAdminPage]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IContactRepository _contactRepository;

        public UserController(IUserRepository userRepository, IContactRepository contactRepository)
        {
            _userRepository = userRepository;
            _contactRepository = contactRepository;
        }

        public IActionResult Index()
        {
            List<UserModel> contatos = _userRepository.GetAll();
            return View(contatos);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Update(int id)
        {
            UserModel contato = _userRepository.Get(id);
            return View(contato);
        }

        public IActionResult DeleteConfirmation(int id)
        {
            UserModel contato = _userRepository.Get(id);
            return View(contato);
        }

        public IActionResult Delete(int id)
        {

            try
            {
                bool excluded = _userRepository.Delete(id);

                if (excluded)
                {
                    TempData["SuccessMessage"] = "Usuário excluído com sucesso";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Erro: não foi possível apagar este usuário";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro: {ex.Message}";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public IActionResult Create(UserModel user)
        {
            try
            {
                ModelState.Remove("ListContact");
                if (ModelState.IsValid)
                {
                    _userRepository.Create(user);
                    TempData["SuccessMessage"] = "Usuário cadastrado com sucesso";
                    return RedirectToAction("Index");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Update(UserUpdateModel userUpdate)
        {
            try
            {
                UserModel user = null;

                if (ModelState.IsValid)
                {
                    user = new UserModel()
                    { 
                        Id = userUpdate.Id,
                        Name = userUpdate.Name,
                        Login = userUpdate.Login,
                        TypeUser = userUpdate.TypeUser,
                        Email = userUpdate.Email
                    };

                    user = _userRepository.Update(user);
                    TempData["SuccessMessage"] = "Usuário alterado com sucesso";
                    return RedirectToAction("Index");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult GetByUserId([FromQuery]long userId)
        {
            List<ContactModel> listContact = _contactRepository.GetAll(userId);
            return PartialView("_ContactUser", listContact);
        }
    }
}