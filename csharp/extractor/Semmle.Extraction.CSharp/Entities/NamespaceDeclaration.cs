
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Semmle.Extraction.Entities;
using System.Linq;

namespace Semmle.Extraction.CSharp.Entities
{
    class NamespaceDeclaration : FreshEntity
    {
        public NamespaceDeclaration(Context cx, NamespaceDeclarationSyntax node, NamespaceDeclaration parent)
            : base(cx)
        {
            var ns = Namespace.Create(cx, (INamespaceSymbol)cx.Model(node).GetSymbolInfo(node.Name).Symbol);
            cx.Emit(Tuples.namespace_declarations(this, ns));
            cx.Emit(Tuples.namespace_declaration_location(this, cx.Create(node.Name.GetLocation())));

            var visitor = new Populators.TypeOrNamespaceVisitor(cx, this);

            foreach (var member in node.Members.Cast<CSharpSyntaxNode>().Concat(node.Usings))
            {
                member.Accept(visitor);
            }

            if (parent != null)
            {
                cx.Emit(Tuples.parent_namespace_declaration(this, parent));
            }
        }

        public static NamespaceDeclaration Create(Context cx, NamespaceDeclarationSyntax decl, NamespaceDeclaration parent) => new NamespaceDeclaration(cx, decl, parent);

        public override TrapStackBehaviour TrapStackBehaviour => TrapStackBehaviour.NoLabel;
    }
}
