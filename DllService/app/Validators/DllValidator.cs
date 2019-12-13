using DllService.app.Model;
using DllService.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DllService.app.Validators
{
    public class DllValidator : ValidationContext<DllEntity>
    {
        public override void init()
        {
            validations.Add(RuleFactory.emptyRule<DllEntity>(m => m.Name, "Name"));
            validations.Add(RuleFactory.emptyRule<DllEntity>(m => m.Path, "Path"));
        }
    }
}
