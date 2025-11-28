//NetCoreAPI-main/Web/Auxiliar/JwtMiddleware.cs

using Core.Interfaces.Servicios;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Web.Auxiliar
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IUsuarioServicio usuarioServicio)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var idsesion = context.Request.Query["idusuariosesion"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                if (!string.IsNullOrEmpty(idsesion) && int.TryParse(idsesion, out var idParsed))
                {
                    await AttachUserToContextConId(context, usuarioServicio, token, idParsed);
                }
                else
                {
                    await AttachUserToContext(context, usuarioServicio, token);
                }
            }

            await _next(context);
        }

        private async Task AttachUserToContextConId(HttpContext context, IUsuarioServicio usuarioServicio, string token, int idsesion)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var skey = _configuration["Jwt:Key"];
                var key = Encoding.ASCII.GetBytes(skey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var usuarioId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                var usuarioResponse = await usuarioServicio.ObternerPorIdAsincrono(usuarioId);
                if (usuarioResponse != null && usuarioResponse.Ok && usuarioResponse.Datos != null && usuarioId == idsesion)
                {
                    context.Items["User"] = usuarioId;
                }
                else
                {
                    context.Items["User"] = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "JWT validation failed (con id).");
            }
        }

        private async Task AttachUserToContext(HttpContext context, IUsuarioServicio usuarioServicio, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var skey = _configuration["Jwt:Key"];
                var key = Encoding.ASCII.GetBytes(skey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var usuarioId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                var usuarioResponse = await usuarioServicio.ObternerPorIdAsincrono(usuarioId);
                if (usuarioResponse != null && usuarioResponse.Ok && usuarioResponse.Datos != null)
                {
                    context.Items["User"] = usuarioId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "JWT validation failed.");
            }
        }
    }
}
