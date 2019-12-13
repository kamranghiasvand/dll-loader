using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DllService.app.Model;
using DllService.app.Validators;
using DllService.Model;
using DllService.service;
using Microsoft.AspNetCore.Mvc;

namespace DllService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassLoaderController : ControllerBase
    {
        private readonly IDllContext context;
        private readonly DllValidator dllValidator;
        private readonly TypeLoadValidator typeLoadValidator;

        public ClassLoaderController(IDllContext context, DllValidator dllValidator, TypeLoadValidator typeLoadValidator)
        {
            this.context = context;
            this.dllValidator = dllValidator;
            this.typeLoadValidator = typeLoadValidator;
        }

        [HttpPost]
        [Route("load-dll")]
        public ActionResult<DllEntity> LoadDll([FromBody]DllEntity entity)
        {
            dllValidator.validate(entity);
            context.LoadDll(entity);
            return entity;
        }

        [HttpPost]
        [Route("call-method")]
        public ActionResult<Object> CallMethod([FromBody]TypeLoadEntity entity)
        {
            typeLoadValidator.validate(entity);
            var resp = context.CallMethod(entity);
            return resp;
        }

    }
}
