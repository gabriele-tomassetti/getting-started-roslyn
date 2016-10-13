using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ConsoleApplication
{
    public class InitializerRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel SemanticModel;

        public InitializerRewriter(SemanticModel semanticModel)
        {
            this.SemanticModel = semanticModel;
        }

        public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            var typeSymbol = (ITypeSymbol)this.SemanticModel.GetSymbolInfo(node.Type).Symbol;            
            
            bool changed = false;
            
            SeparatedSyntaxList<VariableDeclaratorSyntax> vs = node.Variables;
            SyntaxTrivia space = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");                       

            for (var i = 0; i < node.Variables.Count; i++)
            {
                if (this.SemanticModel.GetSymbolInfo(node.Type).Symbol.ToString() == "int" &&
                node.Variables[i].Initializer == null)                            
                {                                        
                    ExpressionSyntax es = SyntaxFactory.ParseExpression("42")
                                                       .WithLeadingTrivia(Space);                                        
                    
                    EqualsValueClauseSyntax evc = SyntaxFactory.EqualsValueClause(es)
                                                               .WithLeadingTrivia(space);                
                    
                    vs = vs.Replace(vs.ElementAt(i), vs.ElementAt(i).WithInitializer(evc));                                

                    changed = true;                 
                }
                
                if (this.SemanticModel.GetSymbolInfo(node.Type).Symbol.ToString() == "int" &&
                    node.Variables[i].Initializer != null &&
                    !node.Variables[i].Initializer.Value.IsEquivalentTo(SyntaxFactory.ParseExpression("42")))
                {                    
                    ExpressionSyntax es = SyntaxFactory.ParseExpression("42")
                                                       .WithLeadingTrivia(Space);
                   
                    EqualsValueClauseSyntax evc = SyntaxFactory.EqualsValueClause(es);

                    vs = vs.Replace(vs.ElementAt(i), vs.ElementAt(i).WithInitializer(evc));

                    changed = true;
                }
            }                        
            
            if(changed == true)                
                return node.WithVariables(vs); 

            return base.VisitVariableDeclaration(node);
        }          
    }
}