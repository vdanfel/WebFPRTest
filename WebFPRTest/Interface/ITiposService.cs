﻿using WebFPRTest.Result;

namespace WebFPRTest.Interface
{
    public interface ITiposService
    {
        Task<List<ParametrosTipoResult>> ParametroTipo_Listar(int parametroTipo);
        Task<List<EquipoListResult>> Equipo_Listar();
        Task<string> TipoArchivo_Descripcion(int Id_003_TipoArchivo);
        Task<bool> ControladorTipoUsuario(int Id_Controlador, int Id_011_TipoUsuario);
    }
}
