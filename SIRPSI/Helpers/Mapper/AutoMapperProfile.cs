using AutoMapper;
using DataAccess.Models.Companies;
using DataAccess.Models.Country;
using DataAccess.Models.Documents;
using DataAccess.Models.Estados;
using DataAccess.Models.Ministry;
using DataAccess.Models.Module;
using DataAccess.Models.ModuleUserRole;
using DataAccess.Models.OccupationalLicense;
using DataAccess.Models.Permissions;
using DataAccess.Models.PsychosocialEvaluation;
using DataAccess.Models.RepresentativeCompany;
using DataAccess.Models.Rols;
using DataAccess.Models.Status;
using DataAccess.Models.Tests;
using DataAccess.Models.Users;
using DataAccess.Models.Variables;
using DataAccess.Models.WorkPlace;
using SIRPSI.DTOs.Companies;
using SIRPSI.DTOs.Country;
using SIRPSI.DTOs.Document;
using SIRPSI.DTOs.Ministry;
using SIRPSI.DTOs.Module;
using SIRPSI.DTOs.ModuleUserRole;
using SIRPSI.DTOs.OccupationalLicense;
using SIRPSI.DTOs.PsychosocialEvaluation;
using SIRPSI.DTOs.RepresentativeCompany;
using SIRPSI.DTOs.Status;
using SIRPSI.DTOs.Tests;
using SIRPSI.DTOs.User;
using SIRPSI.DTOs.User.Roles;
using SIRPSI.DTOs.User.RolesUsuario;
using SIRPSI.DTOs.UserPermissions;
using SIRPSI.DTOs.Variables;
using SIRPSI.DTOs.WorkPlace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SIRPSI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Usuario
            //Mapeo de la clase usuario
            CreateMap<AspNetUsers, UserCredentials>().ReverseMap();
            CreateMap<AspNetUsers, User>().ReverseMap();

            CreateMap<PermisosXUsuario, ConsultarPermisosUsuario>().ReverseMap();
            CreateMap<PermisosXUsuario, RegistrarPermisosUsuario>().ReverseMap();
            CreateMap<PermisosXUsuario, ActualizarPermisosUsuario>().ReverseMap();
            CreateMap<PermisosXUsuario, EliminarPermisosUsuario>().ReverseMap();

            #endregion

            #region Roles

            CreateMap<Roles, ConsultarRoles>().ReverseMap();
            CreateMap<Roles, RegistrarRol>().ReverseMap();

            #endregion

            #region Roles de usuario

            CreateMap<UserRoles, ConsultarRolesUsuario>().ReverseMap();
            CreateMap<UserRoles, RegistrarRolesUsuario>().ReverseMap();
            CreateMap<UserRoles, ActualizarRolesUsuario>().ReverseMap();
            CreateMap<UserRoles, EliminarRolesUsuario>().ReverseMap();

            #endregion

            #region Estados

            CreateMap<Estados, ConsultarEstados>().ReverseMap();
            CreateMap<Estados, RegistrarEstados>().ReverseMap();
            CreateMap<Estados, ActualizarEstados>().ReverseMap();
            CreateMap<Estados, EliminarEstados>().ReverseMap();

            #endregion

            #region Empresas

            CreateMap<Empresas, ConsultarEmpresas>().ReverseMap();
            CreateMap<Empresas, RegistrarEmpresas>().ReverseMap();
            CreateMap<Empresas, EmpresasAct>().ReverseMap();
            CreateMap<Empresas, ActualizarEmpresas>().ReverseMap();
            CreateMap<Empresas, EliminarEmpresas>().ReverseMap();

            CreateMap<TiposEmpresa, ConsultarTipoEmpresa>().ReverseMap();
            CreateMap<TiposEmpresa, RegistrarTipoEmpresa>().ReverseMap();
            CreateMap<TiposEmpresa, ActualizarTipoEmpresa>().ReverseMap();
            CreateMap<TiposEmpresa, EliminarTipoEmpresa>().ReverseMap();

            #endregion

            #region País
            CreateMap<Pais, ConsultarPaises>().ReverseMap();
            CreateMap<Pais, RegistrarPais>().ReverseMap();
            CreateMap<Pais, ActualizarPais>().ReverseMap();
            CreateMap<Pais, EliminarPais>().ReverseMap();
            #endregion

            #region Documentos
            CreateMap<TiposDocumento, ConsultarTiposDocumento>().ReverseMap();
            CreateMap<TiposDocumento, RegistrarTipoDocumento>().ReverseMap();
            CreateMap<TiposDocumento, ActualizarTipoDocumento>().ReverseMap();
            CreateMap<TiposDocumento, EliminarTipoDocumento>().ReverseMap();
            #endregion 

            #region Modulo
            CreateMap<Modulo, ConsultarModulo>().ReverseMap();
            CreateMap<Modulo, RegistrarModulo>().ReverseMap();
            CreateMap<Modulo, ActualizarModulo>().ReverseMap();
            CreateMap<Modulo, EliminarModulo>().ReverseMap();
            #endregion

            #region Variables
            CreateMap<Variables, ConsultarVariable>().ReverseMap();
            CreateMap<Variables, RegistrarVariable>().ReverseMap();
            CreateMap<Variables, ActualizarVariable>().ReverseMap();
            CreateMap<Variables, EliminarVariable>().ReverseMap();
            #endregion

            #region ModuloUserRole
            CreateMap<ModuloUserRole, ConsultarModuloUserRole>().ReverseMap();
            CreateMap<ModuloUserRole, RegistrarModuloUserRole>().ReverseMap();
            CreateMap<ModuloUserRole, ActualizarModuloUserRole>().ReverseMap();
            CreateMap<ModuloUserRole, EliminarModuloUserRole>().ReverseMap();
            #endregion

            #region Ministerios

            CreateMap<Ministerio, ConsultarMinisterios>().ReverseMap();
            CreateMap<Ministerio, RegistrarMinisterio>().ReverseMap();

            #endregion

            #region Centros de trabajo
            CreateMap<CentroTrabajo, ConsultarCentroTrabajo>().ReverseMap();
            CreateMap<CentroTrabajo, RegistrarCentroTrabajo>().ReverseMap();
            CreateMap<CentroTrabajo, CentroTrabajoAct>().ReverseMap();
            CreateMap<UserWorkPlace, RegistrarCentroTrabajoUsuario>().ReverseMap();
            #endregion

            #region Evaluacion
            CreateMap<EvaluacionPsicosocialUsuario, ConsultarEvaluacionPsicosocial>().ReverseMap();
            CreateMap<EvaluacionPsicosocialUsuario, RegistrarEvaluacionPsicosocial>().ReverseMap();
            CreateMap<EvaluacionPsicosocialUsuario, RegistrarEvaluacionPsicosocial>().ReverseMap();
            CreateMap<EvaluacionPsicosocialUsuario, RegistrarEvaluacionPsicosocial>().ReverseMap();
            CreateMap<DetalleEvaluacionPsicosocial, DetalleEvaluacionPsicosocialDto>().ReverseMap();
            CreateMap<Employees, EmployeeDataDto>().ReverseMap();
            #endregion

            #region Licencia ocupacional
            CreateMap<LicenciaOcupacional, ConsultarLicenciaOcupacional>().ReverseMap();
            CreateMap<LicenciaOcupacional, ActualizarLicenciaOcupacional>().ReverseMap();
            #endregion

            #region Representante de empresa
            CreateMap<RepresentanteEmpresa, ConsultarRepresentanteEmpresa>().ReverseMap();
            CreateMap<RepresentanteEmpresa, RegistrarRepresentanteEmpresa>().ReverseMap();
            #endregion
        }
    }
}
