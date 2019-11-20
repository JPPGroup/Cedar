using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jpp.Cedar.Core
{
    public interface ISharedParameterManager
    {
        void BindParameters(Document document, Definition parameter, BuiltInParameterGroup group = BuiltInParameterGroup.INVALID);

        Definition RegisterParameter(string groupName, string parameterName, ParameterType parameterType, bool editable, string description);
    }
}
