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
            // determination of the type of the variable(s)
            var typeSymbol = (ITypeSymbol)this.SemanticModel.GetSymbolInfo(node.Type).Symbol;            
            
            bool changed = false;
            
            // you could declare more than one variable with one expression
            SeparatedSyntaxList<VariableDeclaratorSyntax> vs = node.Variables;
            // we create a space to improve readability
            SyntaxTrivia space = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");                       

            for (var i = 0; i < node.Variables.Count; i++)
            {
                // there is not an initialization
                if (this.SemanticModel.GetSymbolInfo(node.Type).Symbol.ToString() == "int" &&
                node.Variables[i].Initializer == null)                            
                {                                        
                    // we create a new espression "42"
                    // preceded by the space we create earlier
                    ExpressionSyntax es = SyntaxFactory.ParseExpression("42")
                                                       .WithLeadingTrivia(Space);                                        

                    // basically we create an assignment to the espression we just created
                    EqualsValueClauseSyntax evc = SyntaxFactory.EqualsValueClause(es)
                                                               .WithLeadingTrivia(space);                

                    // we replace the null initializer with ours
                    vs = vs.Replace(vs.ElementAt(i), vs.ElementAt(i).WithInitializer(evc));                                

                    changed = true;                 
                }

                // there is an initialization but it's not to 42
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