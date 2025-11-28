//NetCoreAPI-main/Services/Servicios/UsuarioServicio.cs

using Core.Entidades;
using Core.Interfaces;
using Core.Interfaces.Servicios;
using Core.Respuestas;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Services.Servicios
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IConfiguration _configuration;

        public UsuarioServicio(IUnidadDeTrabajo unidadDeTrabajo, IConfiguration configuration)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _configuration = configuration;
        }

        public async Task<Respuesta<Usuario>> Actualizar(int entidadParaActualizarId, Usuario nuevosValoresEntidad)
        {
            var validador = new Services.Validadores.UsuarioValidador();
            var resultadoValidacion = await validador.ValidateAsync(nuevosValoresEntidad);

            if (!resultadoValidacion.IsValid)
                throw new ArgumentException(resultadoValidacion.Errors[0].ErrorMessage);

            var usuarioParaActualizar = await _unidadDeTrabajo.UsuarioRepositorio.ObtenerPorIdAsincrono(entidadParaActualizarId);
            if (usuarioParaActualizar == null)
                throw new ArgumentException("Id del usuario a actualizar inválido");

            // Si Password fue cambiado, hashearlo
            if (!string.IsNullOrEmpty(nuevosValoresEntidad.Password))
            {
                usuarioParaActualizar.Password = BCrypt.Net.BCrypt.HashPassword(nuevosValoresEntidad.Password);
            }

            usuarioParaActualizar.Name = nuevosValoresEntidad.Name;
            usuarioParaActualizar.Lastname = nuevosValoresEntidad.Lastname;
            usuarioParaActualizar.Email = nuevosValoresEntidad.Email;
            usuarioParaActualizar.Status = nuevosValoresEntidad.Status;

            await _unidadDeTrabajo.UsuarioRepositorio.Actualizar(usuarioParaActualizar);
            await _unidadDeTrabajo.CommitAsync();

            return new Respuesta<Usuario> { Ok = true, Mensaje = "Usuario actualizado con éxito", Datos = await _unidadDeTrabajo.UsuarioRepositorio.ObtenerPorIdAsincrono(entidadParaActualizarId) };
        }

        public async Task<Respuesta<Usuario>> Agregar(Usuario nuevaEntitidad)
        {
            var validador = new Services.Validadores.UsuarioValidador();
            var resultadoValidacion = await validador.ValidateAsync(nuevaEntitidad);

            if (!resultadoValidacion.IsValid)
                throw new ArgumentException(resultadoValidacion.Errors[0].ErrorMessage);

            // Verificar si email ya existe
            var existente = await _unidadDeTrabajo.UsuarioRepositorio.ConsultarPorEmail(nuevaEntitidad.Email);
            if (existente != null)
                return new Respuesta<Usuario> { Ok = false, Mensaje = "El correo ya está en uso.", Datos = null };

            // Hashear contraseña
            nuevaEntitidad.Password = BCrypt.Net.BCrypt.HashPassword(nuevaEntitidad.Password);

            var entidadAgregada = await _unidadDeTrabajo.UsuarioRepositorio.AgregarAsincrono(nuevaEntitidad);
            await _unidadDeTrabajo.CommitAsync();

            return new Respuesta<Usuario> { Ok = true, Mensaje = "Usuario creado con éxito", Datos = entidadAgregada };
        }

        public async Task<Respuesta<Usuario>> ObternerPorIdAsincrono(int id)
        {
            var obtenido = await _unidadDeTrabajo.UsuarioRepositorio.ObtenerPorIdAsincrono(id);
            if (obtenido == null)
                return new Respuesta<Usuario> { Ok = false, Mensaje = "Usuario no encontrado", Datos = null };

            // No devolver la contraseña en la respuesta
            obtenido.Password = null;
            return new Respuesta<Usuario> { Ok = true, Mensaje = "Usuario obtenido", Datos = obtenido };
        }

        public async Task<Respuesta<IEnumerable<Usuario>>> ObternerTodosAsincrono()
        {
            var list = (await _unidadDeTrabajo.UsuarioRepositorio.ObtenerTodosAsincrono()).ToList();
            // Remover password antes de devolver
            list.ForEach(u => u.Password = null);
            return new Respuesta<IEnumerable<Usuario>> { Ok = true, Mensaje = "Usuarios obtenidos", Datos = list };
        }

        public async Task<Respuesta<Usuario>> Remover(int entidadId)
        {
            var usuario = await _unidadDeTrabajo.UsuarioRepositorio.ObtenerPorIdAsincrono(entidadId);
            if (usuario == null)
                return new Respuesta<Usuario> { Ok = false, Mensaje = "Usuario no encontrado", Datos = null };

            _unidadDeTrabajo.UsuarioRepositorio.Remover(usuario);
            await _unidadDeTrabajo.CommitAsync();
            return new Respuesta<Usuario> { Ok = true, Mensaje = "Usuario eliminado", Datos = null };
        }

        public async Task<Respuesta<RespuestaIniciarSesion>> IniciarSesion(string email, string password)
        {
            var usuario = await _unidadDeTrabajo.UsuarioRepositorio.IniciarSesion(email, password);
            if (usuario == null)
                return new Respuesta<RespuestaIniciarSesion> { Ok = false, Mensaje = "Email y/o contraseña incorrectos.", Datos = null };

            // Verificar contraseña hasheada
            bool verified = BCrypt.Net.BCrypt.Verify(password, usuario.Password);
            if (!verified)
                return new Respuesta<RespuestaIniciarSesion> { Ok = false, Mensaje = "Email y/o contraseña incorrectos.", Datos = null };

            // Crear token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var m) ? m : 60;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim("id", usuario.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string usuarioToken = tokenHandler.WriteToken(token);

            return new Respuesta<RespuestaIniciarSesion>
            {
                Ok = true,
                Mensaje = "Inicio de sesión correcto.",
                Datos = new RespuestaIniciarSesion { jwt = usuarioToken, idusuariosesion = usuario.Id }
            };
        }

        public async Task<Respuesta<ModeloRegistrarse>> Registrarse(ModeloRegistrarse modeloRegistrarse)
        {
            // Crear usuario y delegar a Agregar
            var usuario = new Usuario
            {
                Name = modeloRegistrarse.Name,
                Lastname = modeloRegistrarse.Lastname,
                Email = modeloRegistrarse.Email,
                Password = modeloRegistrarse.Password,
                Status = "Active"
            };

            var result = await Agregar(usuario);
            if (!result.Ok)
                return new Respuesta<ModeloRegistrarse> { Ok = false, Mensaje = result.Mensaje, Datos = null };

            return new Respuesta<ModeloRegistrarse> { Ok = true, Mensaje = "Usuario registrado correctamente.", Datos = modeloRegistrarse };
        }
    }
}
