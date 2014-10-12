using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TemplateLoader {
    class DynamicLinqExecutionEngine {

        public Delegate Compilation<T>(string input, params ParameterExpression[] parameters) {
            var expression = System.Linq.Dynamic.DynamicExpression.ParseLambda(parameters, 
                typeof(T), input);
            var comp = expression.Compile();
            return comp;
        }
    }
}
