using cmn.std.Log;
using Comun.Peticiones.Interfaces;
using Microsoft.Extensions.Options;
using ServDocumentos.Core.Configuraciones;
using ServDocumentos.Core.Contratos.Factories.Mambu;
using ServDocumentos.Core.Enumeradores;
using System;

namespace ServDocumentos.Servicios.Comun.Factories
{
    /// <summary>
    /// Factory de <see cref="IUnitOfWorkApi"/>
    /// </summary>
    public class MambuRepositoryFactory : IMambuRepositoryFactory
    {
        /// <summary>
        /// Gestor log
        /// </summary>
        private readonly GestorLog _gestorLog;
        /// <summary>
        /// Gestor peticiones
        /// </summary>
        private readonly IGestorPeticiones _gestorPeticiones;
        /// <summary>
        /// Configuracion de MAMBU
        /// </summary>
        private IOptionsSnapshot<MambuConfig> _mambuOptions;
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="gestorLog">Gestor log</param>
        /// <param name="gestorPeticiones">Gestor peticiones</param>
        /// <param name="mambuOptions"><see cref="IOptionsSnapshot{TOptions}"/> inyectado</param>
        public MambuRepositoryFactory(
            GestorLog gestorLog,
            IGestorPeticiones gestorPeticiones,
            IOptionsSnapshot<MambuConfig> mambuOptions)
        {
            _gestorLog = gestorLog;
            _gestorPeticiones = gestorPeticiones;
            _mambuOptions = mambuOptions;
        }
        /// <summary>
        /// Crear una instancia de <see cref="IUnitOfWorkApi"/>
        /// </summary>
        /// <param name="core">Empresa de la que se va crear la instancia</param>
        /// <returns>Instancia de <see cref="IUnitOfWorkApi"/></returns>
        public IUnitOfWorkApi Crear(Empresa core)
        {
            switch (core)
            {
                case Empresa.CAME:
                    return new Repositorios.CAMEDIGITAL.UnitOfWorkApi(_gestorLog, _gestorPeticiones, _mambuOptions);
                case Empresa.TCR:
                    return new Repositorios.TCR.UnitOfWorkApi(_gestorLog, _gestorPeticiones, _mambuOptions);
                default:
                    throw new NotImplementedException($"No existe repositorio implementado para la empresa: {core}");
            }
        }
        /// <summary>
        /// Crear una instancia de <see cref="IUnitOfWorkApi"/>
        /// </summary>
        /// <param name="core">Empresa de la que se va crear la instancia</param>
        /// <returns>Instancia de <see cref="IUnitOfWorkApi"/></returns>
        public IUnitOfWorkApi Crear(string core)
        {
            if (!Enum.TryParse(core, true, out Empresa coreType))
                throw new ArgumentException($"El core {core} no es valido");
            return Crear(coreType);
        }
    }
}
