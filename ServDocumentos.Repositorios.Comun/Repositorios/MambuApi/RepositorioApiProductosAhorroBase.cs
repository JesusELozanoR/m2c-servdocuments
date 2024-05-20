﻿using cmn.std.Log;
using Comun.Peticiones.Extensores;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Contratos.Repositorios.Mambu;
using ServDocumentos.Core.Dtos.DatosMambu;
using ServDocumentos.Core.Enumeradores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServDocumentos.Repositorios.Comun.Repositorios.MambuApi
{
    /// <summary>
    /// Abstraccion del repositorios de productos de ahorro
    /// </summary>
    public abstract class RepositorioApiProductosAhorroBase : IRepositorioProductosAhorroApi
    {
        /// <summary>
        /// Nombre de la configuracion
        /// </summary>
        public abstract Empresa Core { get; }
        /// <summary>
        /// Url para consultar los productos
        /// </summary>
        private const string GetProductsUrl = "depositproducts?&detailsLevel=FULL";
        /// <summary>
        /// Url para obtener un producto por id o encoded key
        /// </summary>
        private const string GetByIdUrl = "depositproducts/{0}?detailsLevel=FULL";
        /// <summary>
        /// Gestor log
        /// </summary>
        private readonly GestorLog _gestorLog;
        /// <summary>
        /// Nombre del header necesario para las peticiones de mambu
        /// </summary>
        private const string AcceptsHeaderName = "Accept";
        /// <summary>
        /// Nombre del valor del header necesario en mambu
        /// </summary>
        private const string AcceptsHeaderValue = "application/vnd.mambu.v2+json";
        /// <summary>
        /// Gestor peticiones
        /// </summary>
        private readonly IGestorPeticiones _gestorPeticiones;
        /// <summary>
        /// Configuracion de mambu
        /// </summary>
        private readonly MambuConfig _config;
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="gestorLog">Gestor log</param>
        /// <param name="gestorPeticiones">Gestor peticiones</param>
        /// <param name="mambuOptions"><see cref="IOptionsSnapshot{TOptions}"/> inyectado</param>
        public RepositorioApiProductosAhorroBase(GestorLog gestorLog, IGestorPeticiones gestorPeticiones, IOptionsSnapshot<MambuConfig> mambuOptions)
        {
            _gestorLog = gestorLog;
            _gestorPeticiones = gestorPeticiones;
            _config = mambuOptions.Get(Core.ToString());
        }
        /// <summary>
        /// Consultar productos
        /// </summary>
        /// <returns>Coleccion de productos</returns>
        public async Task<IEnumerable<ProductoDto>> ObtenerProductos()
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl($"{_config.Url}/{GetProductsUrl}")
                    .ComoGet()
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .Retornar<IEnumerable<ProductoDto>>();
            }
            catch (Exception ex)
            {
                _gestorLog.RegistrarError(ex);
                return new List<ProductoDto>();
            }
            finally
            {
                _gestorLog.Salir();
            }
        }
        /// <summary>
        /// Obtener producto en base a su id o encoded key
        /// </summary>
        /// <param name="id">Id o encoded key del producto</param>
        /// <returns>Informacion del producto</returns>
        public async Task<ProductoDto> GetById(string id)
        {
            try
            {
                _gestorLog.Entrar();
                return await _gestorPeticiones.CrearPeticion()
                    .ConUrl(string.Format($"{_config.Url}/{GetByIdUrl}", id))
                    .ComoGet()
                    .ConAutenticacionBasica(_config.Usuario, _config.Password)
                    .ConHeader(AcceptsHeaderName, AcceptsHeaderValue)
                    .Retornar<ProductoDto>();
            }
            catch (Exception ex)
            {
                _gestorLog.RegistrarError(ex);
                return null;
            }
            finally
            {
                _gestorLog.Salir();
            }
        }
    }
}
