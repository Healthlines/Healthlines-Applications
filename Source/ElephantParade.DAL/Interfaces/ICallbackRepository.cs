using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSD.ElephantParade.DAL.EntityModels;
using NHSD.ElephantParade.DAL.Interfaces;

namespace NHSD.ElephantParade.DAL.Interfaces
{
    public interface ICallbackRepository : IRepository<Callback>
    {
        IQueryable<CallEvent> CallEvent_List();

        void CallEvent_Add(CallEvent ce);
    }
}