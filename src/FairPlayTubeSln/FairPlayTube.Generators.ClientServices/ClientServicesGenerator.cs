using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace FairPlayTube.Generators.ClientServices
{
    /// <summary>
    /// Currently this is a draft, the purpose is to automatically generate the 
    /// client services at compile time.
    /// Code based on: https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/
    /// </summary>
    [Generator]
    public class ClientServicesGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if DEBUG
            Debugger.Launch();
#endif
            //context.RegisterPostInitializationOutput(context => 
            //{
            //    StringBuilder stringBuilder = new StringBuilder();
            //    stringBuilder.AppendLine("/// <summary>");
            //    stringBuilder.AppendLine("/// ");
            //    stringBuilder.AppendLine("/// </summary>");
            //    stringBuilder.AppendLine("public class Test123 {}");
            //    context.AddSource("Test123.g.cs", stringBuilder.ToString());
            //});

            // Do a simple filter for enums
            IncrementalValuesProvider<ClassDeclarationSyntax> enumDeclarations =
                context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // sect the enum with the [EnumExtensions] attribute
                .Where(static m => m is not null)!; // filter out attributed enums that we don't care about

            // Combine the selected enums with the `Compilation`
            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndEnums
                = context.CompilationProvider.Combine(enumDeclarations.Collect());

            // Generate the source using the compilation and enums
            context.RegisterSourceOutput(compilationAndEnums,
                static (spc, source) => Execute(source.Item1, source.Item2, spc));

        }
        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

        private const string GenerateClientAttribute =
            "FairPlayTube.Controllers.CustomAttributes.GenerateClientAttribute";

        static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            // we know the node is a EnumDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var enumDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            // loop through all the attributes on the method
            foreach (AttributeListSyntax attributeListSyntax in enumDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();
                    if (fullName.EndsWith("Controller"))
                        return enumDeclarationSyntax;
                    // Is the attribute the [EnumExtensions] attribute?
                    if (fullName == GenerateClientAttribute)
                    {
                        // return the enum
                        return enumDeclarationSyntax;
                    }
                }
            }

            // we didn't find the attribute we were looking for
            return null;
        }

        static void Execute(Compilation compilation,
            ImmutableArray<ClassDeclarationSyntax> enums, SourceProductionContext context)
        {
            if (enums.IsDefaultOrEmpty)
            {
                // nothing to do yet
                return;
            }

            // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
            IEnumerable<ClassDeclarationSyntax> distinctEnums = enums.Distinct();

            foreach (var item in enums)
            {
            }
            // Convert each EnumDeclarationSyntax to an EnumToGenerate
            //List<EnumToGenerate> enumsToGenerate = GetTypesToGenerate(compilation, distinctEnums, context.CancellationToken);

            //// If there were errors in the EnumDeclarationSyntax, we won't create an
            //// EnumToGenerate for it, so make sure we have something to generate
            //if (enumsToGenerate.Count > 0)
            //{
            //    // generate the source code and add it to the output
            //    string result = SourceGenerationHelper.GenerateExtensionClass(enumsToGenerate);
            //    context.AddSource("EnumExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
            //}
        }
        //static List<EnumToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<EnumDeclarationSyntax> enums, CancellationToken ct)
        //{
        //    // Create a list to hold our output
        //    var enumsToGenerate = new List<EnumToGenerate>();
        //    // Get the semantic representation of our marker attribute 
        //    INamedTypeSymbol? enumAttribute = compilation.GetTypeByMetadataName(GenerateClientAttribute);

        //    if (enumAttribute == null)
        //    {
        //        // If this is null, the compilation couldn't find the marker attribute type
        //        // which suggests there's something very wrong! Bail out..
        //        return enumsToGenerate;
        //    }

        //    foreach (EnumDeclarationSyntax enumDeclarationSyntax in enums)
        //    {
        //        // stop if we're asked to
        //        ct.ThrowIfCancellationRequested();

        //        // Get the semantic representation of the enum syntax
        //        SemanticModel semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
        //        if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
        //        {
        //            // something went wrong, bail out
        //            continue;
        //        }

        //        // Get the full type name of the enum e.g. Colour, 
        //        // or OuterClass<T>.Colour if it was nested in a generic type (for example)
        //        string enumName = enumSymbol.ToString();

        //        // Get all the members in the enum
        //        ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
        //        var members = new List<string>(enumMembers.Length);

        //        // Get all the fields from the enum, and add their name to the list
        //        foreach (ISymbol member in enumMembers)
        //        {
        //            if (member is IFieldSymbol field && field.ConstantValue is not null)
        //            {
        //                members.Add(member.Name);
        //            }
        //        }

        //        // Create an EnumToGenerate for use in the generation phase
        //        enumsToGenerate.Add(new EnumToGenerate(enumName, members));
        //    }

        //    return enumsToGenerate;
        //}
    }
}
