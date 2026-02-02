using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using movie_mvc.Data;
using movie_mvc.Models;

namespace movie_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieDbContext _context;
        private const int PageSize = 8;

        public HomeController(ILogger<HomeController> logger, MovieDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int pagina = 1, string txtBusqueda = "", int generoId = 0)
        {
            //validación para que pagina no tenga valores negativos o cero
            if (pagina < 1)
                pagina = 1;

            //Filtrado por texto de búsqueda
            var consulta = _context.Peliculas.AsQueryable();
            if(!string.IsNullOrEmpty(txtBusqueda))
            {
                consulta = consulta.Where(p => p.Titulo.Contains(txtBusqueda));
            }

            //Filtrado por género
            if(generoId > 0)
            {
                consulta = consulta.Where(p => p.GeneroId == generoId);
            }

            var totalPeliculas = await consulta.CountAsync();

            //Divicimos totalPaginas entre PageSize (8)
            var totalPaginas = (int)Math.Ceiling(totalPeliculas / (double)PageSize);

            //Validación para que pagina no sea mayor que totalPaginas
            //si se comple, te llevaría a la ultima página disponible
            if (pagina > totalPaginas && totalPaginas > 0)
                pagina = totalPaginas;

            var peliculas = await consulta
                .Skip((pagina - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalPeliculas = totalPeliculas;
            ViewBag.TxtBusqueda = txtBusqueda;

            var generos = await _context.Generos.OrderBy(g => g.Descripcion).ToListAsync();
            generos.Insert(0, new Genero { Id = 0, Descripcion = "Géneros" });

            ViewBag.GeneroId = new SelectList(
                generos, 
                "Id",
                "Descripcion", 
                generoId
            );

            return View(peliculas);
        }

        public async Task<IActionResult> Details(int Id) 
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .FirstOrDefaultAsync(p => p.Id == Id);

            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
