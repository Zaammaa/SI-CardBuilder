using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Fixers
{
    public class FixerResult
    {
        public enum FixResult {
            FixError,
            FixFailed,
            FixSucceeded,
            UpdateEffect,
            RemoveEffect,
        }

        public FixResult result;
        public object resultObj;

        public FixerResult(FixResult result, object obj)
        {
            this.result = result;
            this.resultObj = obj;
        }
    }
}
