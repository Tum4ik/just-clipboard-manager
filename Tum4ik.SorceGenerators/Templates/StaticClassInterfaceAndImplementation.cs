using System;
using System.Collections.Generic;
using System.Linq;

namespace Tum4ik.SorceGenerators.Templates
{
  internal static partial class Template
  {
    public static string StaticClassInterfaceAndImplementation(ComponentInfo component)
    {
      var interfaceMethodsSeparator = Environment.NewLine + new string(' ', 4);
      var implementationMethodsSeparator = Environment.NewLine + Environment.NewLine;
      var interfaceMethods = new List<string>();
      var implementationMethods = new List<string>();

      foreach (var method in component.Methods)
      {
        var parametersList = method.Parameters.ToList();
        var parameters = string.Join(", ", parametersList.Select(p => $"{p.TypeFullName} {p.Name}"));
        var parameterNames = string.Join(", ", parametersList.Select(p => p.Name));

        interfaceMethods.Add($"{method.ReturnTypeFullName} {method.MethodName}({parameters});");

        implementationMethods.Add($@"public {method.ReturnTypeFullName} {method.MethodName}({parameters})
    {{
      {(method.ReturnTypeFullName == "void" ? string.Empty : "return ")}{component.StaticClassFullName}.{method.MethodName}({parameterNames});
    }}
");
      }

      return $@"namespace {component.Namespace}
{{
  {component.AccessModifier} partial interface {component.InterfaceName}
  {{
    {string.Join(interfaceMethodsSeparator, interfaceMethods)}
  }}


  {component.AccessModifier} class {component.ImplementationClassName} : {component.InterfaceName}
  {{
    {string.Join(implementationMethodsSeparator, implementationMethods)}
  }}
}}
";
    }
  }
}
