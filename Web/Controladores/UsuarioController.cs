//NetCoreAPI-main/Web/Controladores/UsuarioController.cs

using Core.Entidades;
using Core.Interfaces.Servicios;
using Core.Respuestas;
using Microsoft.AspNetCore.Mvc;
using Web.Auxiliar;


namespace Web.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioServicio _servicio;

        public UsuarioController(IUsuarioServicio servicio)
        {
            _servicio = servicio;
        }

        // GET: api/usuario
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Respuesta<IEnumerable<Usuario>>>> Get()
        {
            try
            {
                var respuesta = await _servicio.ObternerTodosAsincrono();
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/usuario/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Respuesta<Usuario>>> Get(int id)
        {
            try
            {
                // Sólo el propio usuario puede leer su registro (seguridad básica)
                var claimId = HttpContext.Items["User"];
                if (claimId == null || (int)claimId != id)
                    return Forbid();

                var respuesta = await _servicio.ObternerPorIdAsincrono(id);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/usuario/registrarse
        [HttpPost("registrarse")]
        [AllowAnonymous]
        public async Task<IActionResult> Registrarse([FromBody] ModeloRegistrarse modeloRegistrarse)
        {
            try
            {
                var respuesta = await _servicio.Registrarse(modeloRegistrarse);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/usuario/iniciosesion
        [HttpPost("iniciosesion")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Usuario usuario)
        {
            try
            {
                var respuesta = await _servicio.IniciarSesion(usuario.Email, usuario.Password);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/usuario/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Respuesta<Usuario>>> Put(int id, [FromBody] Usuario usuario)
        {
            try
            {
                var claimId = HttpContext.Items["User"];
                if (claimId == null || (int)claimId != id)
                    return Forbid();

                var respuesta = await _servicio.Actualizar(id, usuario);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/usuario/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Respuesta<Usuario>>> Delete(int id)
        {
            try
            {
                var claimId = HttpContext.Items["User"];
                if (claimId == null || (int)claimId != id)
                    return Forbid();

                var respuesta = await _servicio.Remover(id);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
