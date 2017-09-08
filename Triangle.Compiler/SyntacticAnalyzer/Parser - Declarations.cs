using Triangle.Compiler.SyntaxTrees.Declarations;

namespace Triangle.Compiler.SyntacticAnalyzer
{
    public partial class Parser
    {

        ///////////////////////////////////////////////////////////////////////////////
        //
        // DECLARATIONS
        //
        ///////////////////////////////////////////////////////////////////////////////

        /**
         * Parses the declaration, and constructs an AST to represent its phrase
         * structure.
         * 
         * @return a {@link triangle.compiler.syntax.trees.declarations.Declaration}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        Declaration ParseDeclaration()
        {
            var startLocation = _currentToken.Start;
            var declaration = ParseSingleDeclaration();
            while (_currentToken.Kind == TokenKind.Semicolon)
            {
                AcceptIt();
                var declaration2 = ParseSingleDeclaration();
                var declarationPosition = new SourcePosition(startLocation, _currentToken.Finish);
                declaration = new SequentialDeclaration(declaration, declaration2, declarationPosition);
            }
            return declaration;
        }

        /**
         * Parses the single declaration, and constructs an AST to represent its
         * phrase structure.
         * 
         * @return a {@link triangle.compiler.syntax.trees.declarations.Declaration}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        Declaration ParseSingleDeclaration()
        {
            var startLocation = _currentToken.Start;
            switch (_currentToken.Kind)
            {

                case TokenKind.Const:
                    {
                        AcceptIt();
                        var identifier = ParseIdentifier();
                        Accept(TokenKind.Is);
                        var expression = ParseExpression();
                        var declarationPosition = new SourcePosition(startLocation, _currentToken.Finish);
                        return new ConstDeclaration(identifier, expression, declarationPosition);
                    }

                case TokenKind.Var:
                    {
                        AcceptIt();
                        var identifier = ParseIdentifier();
                        Accept(TokenKind.Colon);
                        var typeDenoter = ParseTypeDenoter();
                        var declarationPosition = new SourcePosition(startLocation, _currentToken.Finish);
                        return new VarDeclaration(identifier, typeDenoter, declarationPosition);
                    }

                case TokenKind.Proc:
                    {
                        AcceptIt();
                        var identifier = ParseIdentifier();
                        Accept(TokenKind.LeftParen);
                        var formalParameterSequence = ParseFormalParameterSequence();
                        Accept(TokenKind.RightParen);
                        Accept(TokenKind.Is);
                        var command = ParseSingleCommand();
                        var declarationPosition = new SourcePosition(startLocation, _currentToken.Finish);
                        return new ProcDeclaration(identifier, formalParameterSequence, command, declarationPosition);
                    }

                case TokenKind.Func:
                    {
                        AcceptIt();
                        var identifier = ParseIdentifier();
                        Accept(TokenKind.LeftParen);
                        var formals = ParseFormalParameterSequence();
                        Accept(TokenKind.RightParen);
                        Accept(TokenKind.Colon);
                        var typeDenoter = ParseTypeDenoter();
                        Accept(TokenKind.Is);
                        var expression = ParseExpression();
                        var declarationPosition = new SourcePosition(startLocation, _currentToken.Finish);
                        return new FuncDeclaration(identifier, formals, typeDenoter, expression, declarationPosition);
                    }

                case TokenKind.Type:
                    {
                        AcceptIt();
                        var identifier = ParseIdentifier();
                        Accept(TokenKind.Is);
                        var typeDenoter = ParseTypeDenoter();
                        var declarationPosition = new SourcePosition(startLocation, _currentToken.Finish);
                        return new TypeDeclaration(identifier, typeDenoter, declarationPosition);
                    }

                default:
                    {
                        RaiseSyntacticError("\"%\" cannot start a declaration", _currentToken);
                        return null;
                    }

            }

        }
    }
}