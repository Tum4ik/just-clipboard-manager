using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tum4ik.Attributes;
using Tum4ik.SorceGenerators.Exceptions;
using Tum4ik.SorceGenerators.Templates;

namespace Tum4ik.SorceGenerators;

[Generator]
public class StaticClassInterfaceAndImplementationGenerator : ISourceGenerator
{
  public void Initialize(GeneratorInitializationContext context)
  {
    //System.Diagnostics.Debugger.Launch();
  }


  public void Execute(GeneratorExecutionContext context)
  {
    var components = GetComponents(context.Compilation);
    foreach (var component in components)
    {
      var source = Template.StaticClassInterfaceAndImplementation(component);
      context.AddSource($"{component.InterfaceName}.g.cs", source);
    }
  }


  private static IEnumerable<ComponentInfo> GetComponents(Compilation compilation)
  {
    var attributeName = nameof(IIForStaticClassAttribute).Replace("Attribute", string.Empty);
    var interfaces = compilation.SyntaxTrees
      .SelectMany(st => st.GetRoot().DescendantNodes())
      .Where(n => n.IsKind(SyntaxKind.InterfaceDeclaration))
      .OfType<InterfaceDeclarationSyntax>();

    foreach (var @interface in interfaces)
    {
      var iiAttribute = @interface
        .AttributeLists
        .SelectMany(al => al.Attributes)
        .FirstOrDefault(a => a.Name.ToString() == attributeName);
      if (iiAttribute is null)
      {
        continue;
      }

      var staticClassTypeOfExpressionSyntax = iiAttribute
        .ArgumentList?
        .Arguments[0]
        .Expression as TypeOfExpressionSyntax;
      var implementationClassLiteralExpressionSyntax = iiAttribute
        .ArgumentList?
        .Arguments[1]
        .Expression as LiteralExpressionSyntax;
      if (staticClassTypeOfExpressionSyntax is null)
      {
        throw new SourceGenerationException("Static class type argument is not provided");
      }
      if (implementationClassLiteralExpressionSyntax is null)
      {
        throw new SourceGenerationException("Implementation class name argument is not provided");
      }

      var staticClassTypeSyntax = staticClassTypeOfExpressionSyntax.Type;
      var staticClassFullName = compilation
        .GetSemanticModel(staticClassTypeSyntax.SyntaxTree)
        .GetSymbolInfo(staticClassTypeSyntax)
        .Symbol?
        .ToString();
      if (staticClassFullName is null)
      {
        throw new SourceGenerationException("Can not retrieve static class full name");
      }

      var implementationClassName = implementationClassLiteralExpressionSyntax.Token.ValueText;

      var @namespace = compilation
        .GetSemanticModel(@interface.SyntaxTree)
        .GetDeclaredSymbol(@interface)?
        .ContainingNamespace
        .ToString();
      if (@namespace is null)
      {
        throw new SourceGenerationException("Can not retrieve interface namespace");
      }

      var accessModifier = @interface.Modifiers
        .FirstOrDefault(st => st.IsKind(SyntaxKind.PublicKeyword) || st.IsKind(SyntaxKind.InternalKeyword))
        .ValueText;
      var interfaceName = @interface.Identifier.ValueText;

      var staticClassType = compilation.GetTypeByMetadataName(staticClassFullName);
      if (staticClassType is null)
      {
        throw new SourceGenerationException($"Can not get metadata for {staticClassType}");
      }

      var methods = staticClassType.GetMembers()
        .Select(m => m as IMethodSymbol)
        .Where(ms => ms is not null)
        .Select(ms => new ComponentMethodInfo(
          ms!.ReturnType.ToString(),
          ms.Name,
          ms.Parameters.Select(p => new MethodParameterInfo(p.Type.ToString(), p.Name))
        ));

      yield return new ComponentInfo(
        staticClassFullName, @namespace, accessModifier, interfaceName, implementationClassName, methods
      );
    }
  }
}


internal class ComponentInfo
{
  public ComponentInfo(string staticClassFullName,
                       string @namespace,
                       string accessModifier,
                       string interfaceName,
                       string implementationClassName,
                       IEnumerable<ComponentMethodInfo> methods)
  {
    StaticClassFullName = staticClassFullName;
    Namespace = @namespace;
    AccessModifier = accessModifier;
    InterfaceName = interfaceName;
    ImplementationClassName = implementationClassName;
    Methods = methods;
  }

  public string StaticClassFullName { get; }
  public string Namespace { get; }
  public string AccessModifier { get; }
  public string InterfaceName { get; }
  public string ImplementationClassName { get; }
  public IEnumerable<ComponentMethodInfo> Methods { get; }
}


internal class ComponentMethodInfo
{
  public ComponentMethodInfo(string returnTypeFullName,
                             string methodName,
                             IEnumerable<MethodParameterInfo> parameters)
  {
    ReturnTypeFullName = returnTypeFullName;
    MethodName = methodName;
    Parameters = parameters;
  }

  public string ReturnTypeFullName { get; }
  public string MethodName { get; }
  public IEnumerable<MethodParameterInfo> Parameters { get; }
}


internal class MethodParameterInfo
{
  public MethodParameterInfo(string typeFullName, string name)
  {
    TypeFullName = typeFullName;
    Name = name;
  }

  public string TypeFullName { get; }
  public string Name { get; }
}
