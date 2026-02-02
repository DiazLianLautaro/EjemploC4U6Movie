using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace movie_mvc.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public string ImagenUrlPerfil { get; set; }
        public List<Favorito>? PeliculasFavoritas { get; set; }
        public List<Review> ReviewsUsuario { get; set; }
    }

    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El Apellido es obligatorio.")]
        [StringLength(50)]
        public string Apellido { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="La contraseña es obligatoria.")]
        public string Clave { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Debes confirmar la clave.")]
        [Compare("Clave", ErrorMessage = "La clave y la confirmación no coinciden.")]
        public string ConfirmarClave { get; set; }
    }

    public class LoginViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="La contraseña es obligatoria.")]
        public string Clave { get; set; }
        public bool Recordarme { get; set; }
    }   
}
