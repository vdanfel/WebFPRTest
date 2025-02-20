﻿using WebFPRTest.Areas.Interno.Models.ListAcreditacion;

namespace WebFPRTest.Areas.Interno.Interface.ListAcreditacion
{
    public interface IListAcreditacionService
    {
        Task<List<ComprobanteTabla>> Comprobante_Bandeja(ComprobanteFiltroViewModel comprobanteFiltroViewModel);
    }
}
