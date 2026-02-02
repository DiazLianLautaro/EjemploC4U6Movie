using maxi_movie_mvc.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using movie_mvc.Data;
using movie_mvc.Models;

namespace movie_mvc.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly MovieDbContext _context;
        private readonly ImagenStorage _imagenStorage;
        public UsuarioController(UserManager<Usuario> userManager, MovieDbContext context, SignInManager<Usuario> signInManager, ImagenStorage imagenStorage)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _imagenStorage = imagenStorage;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Clave, usuario.Recordarme, lockoutOnFailure: false);
                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                }
            }
            return View(usuario);
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var nuevoUsuario = new Usuario
                {
                    UserName = usuario.Email,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    ImagenUrlPerfil = "~/default.png"
                };

                var resultado = await _userManager.CreateAsync(nuevoUsuario, usuario.Clave);
                if (resultado.Succeeded)
                {
                    await _signInManager.SignInAsync(nuevoUsuario, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(usuario);
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MiPerfil()
        {
            var usuarioActual = await _userManager.GetUserAsync(User);

            var usuarioVM = new MiPerfilViewModel
            {
                Nombre = usuarioActual.Nombre,
                Apellido = usuarioActual.Apellido,
                Email = usuarioActual.Email,
                ImagenUrlPerfil = usuarioActual.ImagenUrlPerfil
            };
            return View(usuarioVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MiPerfil(MiPerfilViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioActual = await _userManager.GetUserAsync(User);

                try
                {
                    if(usuario.ImagenPerfil is not null && usuario.ImagenPerfil.Length > 0)
                    {
                        // opcional: Borrar la anterior (si no es placeholder)
                        if(!string.IsNullOrWhiteSpace(usuarioActual.ImagenUrlPerfil))
                            await _imagenStorage.DeleteAsync(usuarioActual.ImagenUrlPerfil);

                        var nuevaRuta = await _imagenStorage.SaveAsync(usuarioActual.Id, usuario.ImagenPerfil);
                        usuarioActual.ImagenUrlPerfil = nuevaRuta;
                        usuario.ImagenUrlPerfil = nuevaRuta;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el perfil: " + ex.Message);
                    return View(usuario);
                }

                usuarioActual.Nombre = usuario.Nombre;
                usuarioActual.Apellido = usuario.Apellido;

                var resultado = await _userManager.UpdateAsync(usuarioActual);

                if(resultado.Succeeded)
                {
                    ViewBag.Message = "Perfil actualizado con éxito.";
                    return View(usuario);
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(usuario);
        }
    }
}
