using _0_3a_diegopozzi.Hubs;
using _0_3a_diegopozzi.models;
using _0_3a_diegopozzi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace _0_3a_diegopozzi.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<LoginHub> _hub;
        private readonly EmailService _email;

        // email → Usuario
        private static readonly Dictionary<string, Usuario> _usuarios = new();

        public HomeController(IHubContext<LoginHub> hub, EmailService email)
        {
            _hub = hub;
            _email = email;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (_usuarios.TryGetValue(email, out var usuario))
            {
                if (usuario.Verificado)
                {
                    if (usuario.Password == password)
                        return RedirectToAction("Index");
                    else
                    {
                        ViewBag.Mensaje = "Contraseña incorrecta.";
                        return View();
                    }
                }

                // Si no verificó todavía, regenero código
                usuario.CodigoVerificacion = Guid.NewGuid().ToString("N");

                var url = Url.Action("Verificar", "Home", new { codigo = usuario.CodigoVerificacion }, Request.Scheme);
                // Enviar el correo en segundo plano
                Task.Run(() => _email.SendVerificationEmailAsync(email, url));

                ViewBag.Mensaje = "Ya estabas registrado, te reenviamos el correo de verificación.";
                return View();
            }
            else
            {
               
                var nuevoUsuario = new Usuario
                {
                    Email = email,
                    Password = password,
                    CodigoVerificacion = Guid.NewGuid().ToString("N"),
                    Verificado = false
                };

                _usuarios[email] = nuevoUsuario;

                var url = Url.Action("Verificar", "Home", new { codigo = nuevoUsuario.CodigoVerificacion }, Request.Scheme);
            
                Task.Run(() => _email.SendVerificationEmailAsync(email, url));

                ViewBag.Mensaje = "Te registramos y enviamos un correo de verificación.";
                return View();
            }
        }

        [HttpGet("/verificar/{codigo}")]
        public async Task<IActionResult> Verificar(string codigo)
        {
            var usuario = _usuarios.Values.FirstOrDefault(u => u.CodigoVerificacion == codigo);
            if (usuario == null)
                return NotFound("Código inválido.");

            usuario.Verificado = true;
            usuario.CodigoVerificacion = null;
            if (LoginHub.UsuariosConectados.TryGetValue(usuario.Email, out var connId))
            {
                await _hub.Clients.Client(connId).SendAsync("EmailVerificado");
            }

            return Content($" Email {usuario.Email} verificado correctamente. Ya podés entrar.");
        }

        public IActionResult Index() => View();
    }
}
