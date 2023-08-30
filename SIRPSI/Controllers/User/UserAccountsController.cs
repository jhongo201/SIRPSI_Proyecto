using AutoMapper;
using DataAccess.Context;
using DataAccess.Models.Users;
using EmailServices;
using EvertecApi.Log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Validations;
using Org.BouncyCastle.Crypto.Prng;
using SIRPSI.Core.Helper;
using SIRPSI.DTOs.Companies;
using SIRPSI.DTOs.User;
using SIRPSI.DTOs.User.Roles;
using SIRPSI.DTOs.User.Usuario;
using SIRPSI.Helpers.Answers;
using SIRPSI.Settings;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SIRPSI.Controllers.User
{
    [Route("api/user")]
    [ApiController]
    [EnableCors("CorsApi")]
    public class UserAccountsController : ControllerBase
    {
        #region Dependences
        private readonly UserManager<IdentityUser> userManager;
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILoggerManager logger;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;
        private readonly StatusSettings statusSettings;

        //Constructor 
        public UserAccountsController(AppDbContext context,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILoggerManager logger,
            IMapper mapper,
            IEmailSender emailSender,
            StatusSettings statusSettings)
        {
            this.context = context;
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.statusSettings = statusSettings;
        }
        #endregion

        #region RegisterUser
        [HttpPost("RegisterUser")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticationResponse>> Register(UserCredentials userCredentials)
        {
            try
            {
                #region Validaciones

                var existTypeDoc = await context.tiposDocumento.Where(x => x.Id.Equals(userCredentials.IdTypeDocument)).FirstOrDefaultAsync();

                if (existTypeDoc == null)
                {
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "Tipo de documento NO existe."
                    });
                }

                var existRoles = await context.AspNetRoles.Where(x => x.Id.Equals(userCredentials.IdRol)).FirstOrDefaultAsync();

                if (existRoles == null)
                {
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "Estado del usuario NO existe."
                    });
                }

                var existCountry = await context.pais.Where(x => x.Id.Equals(userCredentials.IdCountry)).FirstOrDefaultAsync();

                if (existCountry == null)
                {
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "Pais NO existe."
                    });
                }

                if (!string.IsNullOrEmpty(userCredentials.IdCompany))
                {
                    var existCompany = await context.empresas.Where(x => x.Id.Equals(userCredentials.IdCompany)).FirstOrDefaultAsync();

                    if (existCompany == null)
                    {
                        return BadRequest(new General()
                        {
                            title = "usuario",
                            status = 400,
                            message = "Empresa NO existe."
                        });
                    }
                }

                var existStatus = await context.estados.Where(x => x.Id.Equals(userCredentials.IdEstado)).FirstOrDefaultAsync();

                if (existStatus == null)
                {
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "Estado NO existe."
                    });
                }

                if (userCredentials.Document != null && userCredentials.IdRol != null && userCredentials.IdCompany != null)
                {
                    var userCompanyRole = await context.AspNetUsers.Where(x => x.Document.Equals(userCredentials.Document)
                        && x.IdRol.Equals(userCredentials.IdRol) && x.IdCompany.Equals(userCredentials.IdCompany)).FirstOrDefaultAsync();

                    if (userCompanyRole != null)
                    {
                        return BadRequest(new General()
                        {
                            title = "usuario",
                            status = 400,
                            message = "Ya existe este usuario en la compañia con este mismo role."
                        });
                    }
                }

                var emailValidation = userCredentials.Email.Split('@')[1].Split('.')[0];

                var rolesValidationMinisterio = await context.variables
                    .Where(x => x.Modulo == "Users" && x.Variable1 == "ValidacionMinisterio").ToListAsync();

                var validarionRoleEspecial = rolesValidationMinisterio.Where(x => x.Variable2 == existRoles.Id).FirstOrDefault();

                if (emailValidation != "ministerio" && validarionRoleEspecial != null)
                {
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "El email no tiene el dominio correcto (Ejm: ministerio.xxx.xx)."
                    });
                }
                #endregion

                #region agregar datos al contexto

                var userNet = mapper.Map<AspNetUsers>(userCredentials);

                userNet.Document = userCredentials.Document;
                userNet.IdCompany = userCredentials.IdCompany;
                userNet.IdCountry = userCredentials.IdCountry;
                userNet.TypeDocument = userCredentials.IdTypeDocument;

                if (string.IsNullOrEmpty(userNet.Email))
                {
                    userNet.UserName = userCredentials.Document + "@sirpsi.com";
                    userNet.Email = userCredentials.Document + "@sirpsi.com";
                }
                else
                {
                    userNet.UserName = userCredentials.Email;
                    userNet.Email = userCredentials.Email;
                }
                userNet.Surnames = userCredentials.Surnames;
                userNet.Names = userCredentials.Names;
                userNet.PhoneNumber = userCredentials.PhoneNumber;
                userNet.UserRegistration = userCredentials.Document;
                userNet.RegistrationDate = DateTime.Now.ToDateTimeZone().DateTime;
                userNet.UserModify = null;
                userNet.ModifiedDate = null;
                userNet.Status = userCredentials.IdEstado;
                userNet.Id = Guid.NewGuid().ToString();

                if (emailValidation != "mintrabajo" && validarionRoleEspecial != null)
                {
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "El email no tiene el dominio correcto (Ejm: mintrabajo.gov.co)."
                    });
                }

                var ministerioValidationUsersDisabled = await context.variables
                    .Where(x => x.Variable1 == "ValidationInternalActivation").ToListAsync();

                var valministerioValidationUsersDisabled = ministerioValidationUsersDisabled.Where(x => x.Variable2 == userNet.IdCompany).FirstOrDefault();
                var StatusCodeChange = context.variables.Where(x => x.Nombre == "SendChangeStatusCode" && x.Variable1 == userCredentials.IdEstado).FirstOrDefault();

                if (StatusCodeChange != null && valministerioValidationUsersDisabled == null)
                    userCredentials.CodeActivation = userNet.CodeActivation = new Random().Next(10000, 99999).ToString();

                context.Add(userNet);
                #endregion
                #region Registro de datos
                var result = await userManager.CreateAsync(userNet, userCredentials.Password);
                if (result.Succeeded)
                {
                    logger.LogInformation("Registro de usuario ¡Exitoso!");
                    return await BuildToken(userCredentials);
                }
                else
                {
                    logger.LogError("Registro de usuario ¡fallido!");
                    var errorsDescripcion = "";
                    var index = 0;
                    foreach (var item in result.Errors)
                    {
                        errorsDescripcion += (index != 0 ? "\n\n" : "") + item.Description;
                    }

                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "Registro de usuario ¡fallido!  " + errorsDescripcion
                    });
                }
                #endregion
            }
            catch (Exception ex)
            {
                logger.LogError("usuario " + ex.Message + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Registro de usuario ¡fallido!"
                });
            }
        }

        #endregion

        #region DeleteUser
        [HttpDelete("DeleteUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete([FromBody] DeleteUser deleteUser)
        {
            try
            {
                //Claims de usuario - Enviados por token
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }

                var documento = identity.FindFirst("documento").Value.ToString();
                //Consulta de usuario
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();



                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Usuario",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }

                var usuarioLogin = context.AspNetUsers.Where(u => u.Id.Equals(deleteUser.Id)).FirstOrDefault();

                if (usuarioLogin.Document.Equals(usuario.Document))
                {
                    return NotFound(new General()
                    {
                        title = "Usuario",
                        status = 404,
                        message = "No te puedes eliminar a ti mismo."
                    });
                }

                //Consultar estados
                var estados = await context.estados.ToListAsync();

                if (estados == null)
                {
                    return NotFound(new General()
                    {
                        title = "Usuario",
                        status = 404,
                        message = "Estados no encontrados"
                    });
                }

                //Agregar datos al contexto
                context.AspNetUsers.Where(x => x.Id.Equals(deleteUser.Id)).ToList()
                  .ForEach(r =>
                  {
                      r.Status = estados.Where(x => x.IdConsecutivo.Equals(2)).Select(x => x.Id).First();
                      r.UserModify = usuario.Document;
                      r.ModifiedDate = DateTime.Now.ToDateTimeZone().DateTime;
                  });

                context.AspNetUserRoles.Where(x => x.UserId.Equals(deleteUser.Id)).ToList()
                .ForEach(r =>
                {
                    r.IdEstado = estados.Where(x => x.IdConsecutivo.Equals(2)).Select(x => x.Id).First();
                    r.UsuarioModifico = usuario.Document;
                    r.FechaModifico = DateTime.Now.ToDateTimeZone().DateTime;
                });

                //Se elimina el regitro
                await context.SaveChangesAsync();

                return Ok(new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "usuario",
                    status = 200,
                    message = "Usuario eliminado"
                });
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("usuario " + ex.Message + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }
        #endregion

        #region Actualizar
        //[HttpPut("ActualizarUsuario")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult> Put(ActualizarUsuario actualizarUsuario)
        //{
        //    try
        //    {
        //        //Claims de usuario - Enviados por token
        //        var identity = HttpContext.User.Identity as ClaimsIdentity;

        //        if (identity != null)
        //        {
        //            IEnumerable<Claim> claims = identity.Claims;
        //        }

        //        //Consulta el documento con los claims
        //        var documento = identity.FindFirst("documento").Value.ToString();

        //        //Consulta el rol con los claims
        //        var roles = identity.FindFirst("rol").Value.ToString();

        //        //Consulta de usuario
        //        var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();

        //        if (usuario == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Actualizar tipo documento",
        //                status = 404,
        //                message = "Usuario no encontrado"
        //            });
        //        }

        //        //Obtiene la url del servicio
        //        var countLast = HttpContext.Request.GetDisplayUrl().Split("/").Last().Count();
        //        string Url = HttpContext.Request.GetDisplayUrl();

        //        var getUrl = Url.Remove(Url.Length - (countLast + 1));

        //        //Consulta de roles por id de usuario

        //        var rolesList = new List<string>();

        //        //Verifica los roles
        //        var list = roles.Split(',').ToList();

        //        foreach (var i in list)
        //        {
        //            var result = context.AspNetRoles.Where(r => r.Id.Equals(i)).Select(x => x.Description).FirstOrDefault();

        //            if (result != null)
        //            {
        //                rolesList.Add(result.ToString());
        //            }
        //        }

        //        if (rolesList == null)
        //        {
        //            return NotFound(new General()
        //            {
        //                title = "Actualizar tipo documento",
        //                status = 404,
        //                message = "Roles no encontrados"
        //            });
        //        }

        //        //Revisa los permisos de usuario
        //        var permisos = await context.permisosXUsuario.Where(x => x.Vista.Equals(getUrl) && x.IdUsuario.Equals(usuario.Id)).ToListAsync();

        //        //Consulta si tiene el permiso
        //        var permitido = permisos.Select(x => x.Actualizar.Equals(true)).FirstOrDefault();

        //        //Si es permitido
        //        if (true)
        //        {
        //            //Consulta de empresa del usuario
        //            var existe = await context.AspNetUsers.Where(x => x.Id.Equals(actualizarUsuario.Id)).FirstOrDefaultAsync();

        //            if (existe == null)
        //            {
        //                return NotFound(new
        //                {
        //                    //Visualizacion de mensajes al usuario del aplicativo
        //                    title = "Actualizar tipo documento",
        //                    status = 404,
        //                    message = "Tipo documento no encontrado"
        //                });
        //            }

        //            //Registro de datos

        //            var user = (Student)UserManager.FindById(model.Id);

        //            // Update it with the values from the view model
        //            user.Name = model.Name;
        //            user.Surname = model.Surname;
        //            user.UserName = model.UserName;
        //            user.Email = model.Email;
        //            user.PhoneNumber = model.PhoneNumber;
        //            user.Number = model.Number; //custom property
        //            user.PasswordHash = checkUser.PasswordHash;

        //            // Apply the changes if any to the db
        //            UserManager.Update(user);

        //            //context.AspNetUsers.Where(x => x.Id.Equals(existe.Id)).ToList()
        //            //    .ForEach(r =>
        //            //    {
        //            //        r.TypeDocument = actualizarUsuario.TypeDocument;
        //            //        r.Document = actualizarUsuario.Document;

        //            //        r.UserModify = usuario.Document;
        //            //        r.ModifiedDate = DateTime.Now.ToDateTimeZone().DateTime;
        //            //    });

        //            //Guardado de datos
        //            await context.SaveChangesAsync();

        //            return Ok(new General()
        //            {
        //                //Visualizacion de mensajes al usuario del aplicativo
        //                title = "Actualizar tipo documento",
        //                status = 200,
        //                message = "Tipo documento actualizado"
        //            });
        //        }
        //        else
        //        {
        //            return BadRequest(new General()
        //            {
        //                title = "Actualizar tipo documento",
        //                status = 400,
        //                message = "No tiene permisos para actualizar tipo documento"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError("Actualizar tipo documento" + ex.Message.ToString() + " - " + ex.StackTrace);
        //        return BadRequest(new General()
        //        {
        //            title = "Actualizar tipo documento",
        //            status = 400,
        //            message = ""
        //        });
        //    }
        //}
        #endregion

        #region Login
        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentials userCredentials)
        {
            try
            {
                //Consulta estados
                var estados = await context.estados.Select(x => new { x.Id, x.IdConsecutivo }).Where(x => x.IdConsecutivo.Equals(1) || x.IdConsecutivo.Equals(2)).Select(x => x.Id).ToListAsync();
                //Consulta empresa
                var empresa = await context.empresas.Where(x => x.Documento.Equals(userCredentials.IdCompany) && x.IdEstado == statusSettings.Activo).ToListAsync();
                if (empresa == null)
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "Empresa no encontrado o su estado no es activo."
                    });
                //Cconsulta usuarios
                var existUser = await context.AspNetUsers
                    .Where(x => x.Document.Equals(userCredentials.Document) && estados.Contains(x.Status)).FirstOrDefaultAsync();

                existUser = empresa.Where(x => x.Id == existUser.IdCompany).FirstOrDefault() == null ? null : existUser;
                if (existUser == null || existUser.Status != statusSettings.Activo)
                {
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "Usuario no encontrado o su estado no es activo."
                    });
                }
                var listEmpresasDb = existUser.IdCompany.Split(",").Select(x => x.Trim()).ToList();
                var contextListEmpresas = await context.empresas.Where(x => listEmpresasDb.Contains(x.Id)).ToListAsync();
                var dataEmpresa = await context.empresas.Where(x => listEmpresasDb.Contains(x.Id) && x.Documento.Equals(userCredentials.IdCompany)).Select(x => x.Id).FirstOrDefaultAsync();
                if (dataEmpresa != null)
                {
                    userCredentials.IdCompany = dataEmpresa != null ? dataEmpresa : "";
                }
                else
                {
                    userCredentials.IdCompany = "";
                }
                var email = existUser.Email.Trim() != null ? existUser.Email.Trim() : "";
                var result = await signInManager.PasswordSignInAsync(email,
                userCredentials.Password, isPersistent: false, lockoutOnFailure: false);
                //var userId = userManager.FindByIdAsync();
                userCredentials.IdRol = existUser.IdRol;
                userCredentials.Id = existUser.Id;
                userCredentials.Names = existUser.Names;
                userCredentials.Surnames = existUser.Surnames;
                userCredentials.IdCompany = existUser.IdCompany;
                userCredentials.Role = context.AspNetRoles.Where(x => x.Id == existUser.IdRol).First().Name;
                if (result.Succeeded)
                {
                    logger.LogInformation("Login de usuario ¡exitoso!");
                    return await BuildToken(userCredentials);
                }
                else
                {
                    logger.LogError("Login de usuario ¡fallido!");
                    return BadRequest(new General()
                    {
                        title = "User",
                        status = 400,
                        message = "Login de usuario ¡fallido!"
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("usuario " + ex.Message + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Login de usuario ¡fallido!"
                });
            }
        }

        #endregion

        #region RenewToken
        [HttpGet("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticationResponse>> RenewToken()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }

                var userDocument = identity.FindFirst("documento").Value.ToString();

                var userCredentials = new UserCredentials()
                {
                    Document = userDocument
                };

                return await BuildToken(userCredentials);
            }
            catch (Exception ex)
            {
                logger.LogError("usuario " + ex.Message + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }
        }
        #endregion

        #region SendEmail Password
        [HttpPost("SendEmailChangedPasswword")]
        public async Task<ActionResult<General>> SendEmail([FromBody] ChangedPassword changedPassword)
        {

            try
            {
                var empresa = await context.empresas.Where(x => x.Documento.Equals(changedPassword.Empresa) && x.IdEstado == statusSettings.Activo).FirstOrDefaultAsync();
                if (empresa == null)
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "Empresa no encontrado o su estado no es activo."
                    });

                var user = context.AspNetUsers.Where(u => u.Document.Equals(changedPassword.Document) && u.Email.Equals(changedPassword.Email) && empresa.Id == u.IdCompany).FirstOrDefault();//Si el usuario existe
                if (user == null)
                    return BadRequest(new General()
                    {
                        title = "usuario",
                        status = 400,
                        message = "Usuario no encontrado"
                    });

                string code = await userManager.GeneratePasswordResetTokenAsync(user);
                var Url = configuration["UrlService"];

                var urlCompleted = string.Format(Url + "/account/reset-password?userId={0}&codePassword={1}", user.Id, code);

                var message = new Message(new string[] { user.Email }, "Cambio de contraseña. ", "cambia tu contraseña,  ingresando al siguiente link: <br/><br/>" + urlCompleted + "<br/> <br/>Cordialmente: <br/> <br/>" + "Sirpsi");
                await emailSender.SendEmailAsync(message);

                return Ok(new General()
                {
                    title = "usuario",
                    status = 200,
                    message = "Se ha enviado un link a tu correo electrónico, para reinicar tu contraseña."
                });
            }
            catch (Exception ex)
            {
                logger.LogError("usuario " + ex.Message + ex.StackTrace);

                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Usuario no encontrado"
                });
            }


        }
        #endregion

        #region Token
        private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials)
        {

            var user = await context.AspNetUsers.Where(x => x.Document.Equals(userCredentials.Document)).FirstOrDefaultAsync();

            var roles = await context.AspNetUserRoles.Where(x => x.UserId.Equals(user.Id)).Select(x => x.RoleId).ToListAsync();

            var rolesConcatenados = "";

            if (roles != null || roles.Count != 0)
            {
                rolesConcatenados = string.Join(", ", roles);
            }

            var email = userCredentials.Email;

            var claims = new List<Claim>()
            {
                new Claim("documento", user.Document != null ?user.Document : ""),
                new Claim("email", user.Email != null ? user.Email: ""),
                new Claim("rol", rolesConcatenados != null ? rolesConcatenados : ""),
                new Claim("estado", user.Status != null ? user.Status : ""),
                new Claim("empresa", userCredentials.IdCompany != null ? userCredentials.IdCompany: "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["KeyJwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddDays(5);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);

            userCredentials.Password = null;
            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration,
                RoleId = userCredentials.IdRol,
                Id = user.Id,
                User = userCredentials,
                EstadoId = userCredentials.IdEstado,
                RoleName = userCredentials.Role,
                EmpresaId = userCredentials.IdCompany,
                Empresa = await context.empresas.Where(x => x.Id == userCredentials.IdCompany).FirstOrDefaultAsync(),
                RutasAsignadas = (context.moduloUserRole.Where(x => x.RoleId == userCredentials.IdRol))
                                   .Select(x => new RutasAsignadas()
                                   {
                                       Id = x.Id,
                                       ModuloId = x.ModuloId,
                                       Ruta = context.modulo.Where(d => d.Id == x.ModuloId).First().Ruta
                                   }).ToList(),
            };
        }
        #endregion

        #region Recuperar constraseña

        [HttpPost("RecoverPassword")]
        public async Task<ActionResult<General>> RecoverPassword(RecoverPassword recoverPassword)
        {
            if (recoverPassword.NewPassword != recoverPassword.ConfirmPassword)
            {
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Contraseñas ingresadas no coinciden"
                });
            }
            var user = context.AspNetUsers.Where(u => u.Id.Equals(recoverPassword.UserId)).FirstOrDefault();//Si el usuario existe

            if (user == null)
            {
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Usuario no encontrado"
                });
            }
            var validationNewPassword = new Microsoft.AspNetCore.Identity.PasswordHasher<IdentityUser>().VerifyHashedPassword(user, user.PasswordHash, recoverPassword.NewPassword);
            if (validationNewPassword == PasswordVerificationResult.Success)
            {
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "La contraseña debe ser diferente a la registrada anteriomente."
                });
            }

            //var actualUser = await userManager.FindByEmailAsync(user.Email);
            var actualUser = await context.AspNetUsers.Where(x => x.Email == user.Email).FirstOrDefaultAsync();

            var isCorrectPwd = new IdentityResult();

            if (actualUser != null)
            {
                isCorrectPwd = await userManager.ResetPasswordAsync(actualUser, recoverPassword.CodePassword, recoverPassword.NewPassword);
            }

            if (isCorrectPwd.Succeeded)
            {
                var message = new Message(new string[] { user.Email }, "Cambio de contraseña. ", "Se ha realizado exitosamente el cambio de contraseña.");
                await emailSender.SendEmailAsync(message);

                return Ok(new General()
                {
                    title = "usuario",
                    status = 200,
                    message = "Cambio de contraseña, ¡exitoso!"

                });
            }
            else
            {
                logger.LogError(isCorrectPwd.Errors.Select(x => x.Description).First());
                var errorsDescripcion = "";
                var index = 0;
                foreach (var item in isCorrectPwd.Errors)
                    errorsDescripcion += (index != 0 ? "\n\n" : "") + item.Description;

                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Ha ocurrido un error con el cambio de contraseña: " + errorsDescripcion
                });
            }
        }
        #endregion

        #region Activar usuario

        [HttpPost("ActivateUser")]
        public async Task<ActionResult<General>> ActivateUser(ActivateUserRequest activateUserRequest)
        {
            var empresa = await context.empresas.Where(x => x.Documento.Equals(activateUserRequest.Company) && x.IdEstado == statusSettings.Activo).FirstOrDefaultAsync();
            if (empresa == null)
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Empresa no encontrado o su estado no es activo."
                });

            var user = await context.AspNetUsers.Where(x => x.Document == activateUserRequest.Document && empresa.Id == x.IdCompany).FirstOrDefaultAsync();
            if (user == null)
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "No se ha encontrado un usuario con ese numero de documentos."
                });

            if (user.Status != statusSettings.Inactivo)
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Este codigo ya fue utilizado y el usuario ya se encuentra activo."
                });

            if (user.CodeActivation == null || user.CodeActivation != activateUserRequest.Code)
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "El usuario ya fue activado o el codigo es incorrecto, por favor verificar."
                });

            user.Status = statusSettings.Activo;
            var updateUser = context.Update(user);
            await context.SaveChangesAsync();
            return Ok(new General()
            {
                title = "usuario",
                status = 200,
                message = "El usuario ha sido activado exitosamente."
            });
        }
        #endregion

        #region ChangeStatusUser
        [HttpPut("ChangeStatusUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangeStatus([FromBody] ChangeStatusUser changeStatusUser)
        {
            try
            {
                //Claims de usuario - Enviados por token
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                }

                var documento = identity.FindFirst("documento").Value.ToString();
                //Consulta de usuario
                var usuario = context.AspNetUsers.Where(u => u.Document.Equals(documento)).FirstOrDefault();



                if (usuario == null)
                {
                    return NotFound(new General()
                    {
                        title = "Usuario",
                        status = 404,
                        message = "Usuario no encontrado"
                    });
                }

                var usuarioLogin = context.AspNetUsers.Where(u => u.Id.Equals(changeStatusUser.Id)).FirstOrDefault();

                if (usuarioLogin.Document.Equals(usuario.Document))
                {
                    return NotFound(new General()
                    {
                        title = "Usuario",
                        status = 404,
                        message = "No te puedes eliminar a ti mismo."
                    });
                }

                //Consultar estados
                var estados = await context.estados.ToListAsync();

                if (estados == null)
                {
                    return NotFound(new General()
                    {
                        title = "Usuario",
                        status = 404,
                        message = "Estados no encontrados"
                    });
                }
                usuarioLogin.UserModify = usuario.Document;
                usuarioLogin.ModifiedDate = DateTime.Now.ToDateTimeZone().DateTime;
                usuarioLogin.Status = changeStatusUser.IdEstado == statusSettings.Activo ? statusSettings.Inactivo : statusSettings.Activo;
                context.Update(usuarioLogin);
                await context.SaveChangesAsync();
                return Ok(new General()
                {
                    //Visualizacion de mensajes al usuario del aplicativo
                    title = "usuario",
                    status = 200,
                    message = changeStatusUser.IdEstado == statusSettings.Activo ? "Usuario inactivado" : "Usuario activado"
                });
            }
            catch (Exception ex)
            {
                //Registro de errores
                logger.LogError("usuario " + ex.Message + ex.StackTrace);
                return BadRequest(new General()
                {
                    title = "usuario",
                    status = 400,
                    message = "Contacte con el administrador del sistema"
                });
            }

        }
        #endregion
    }
}
