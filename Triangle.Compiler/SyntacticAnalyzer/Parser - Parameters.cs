using Triangle.Compiler.SyntaxTrees.Actuals;
using Triangle.Compiler.SyntaxTrees.Formals;

namespace Triangle.Compiler.SyntacticAnalyzer
{
    public partial class Parser
    {

        ///////////////////////////////////////////////////////////////////////////////
        //
        // PARAMETERS
        //
        ///////////////////////////////////////////////////////////////////////////////

        /**
         * Parses the formal parameter sequence, and constructs an AST to represent
         * its phrase structure.
         * 
         * @return a
         *         {@link triangle.compiler.syntax.trees.formals.FormalParameterSequence}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        FormalParameterSequence ParseFormalParameterSequence()
        {

            var startLocation = _currentToken.Position.Start;
            if (_currentToken.Kind == TokenKind.RightParen)
            {
                var formalsPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                return new EmptyFormalParameterSequence(formalsPosition);
            }
            return ParseProperFormalParameterSequence();
        }

        /**
         * Parses the proper (non-empty) formal parameter sequence, and constructs an
         * AST to represent its phrase structure.
         * 
         * @return a
         *         {@link triangle.compiler.syntax.trees.formals.FormalParameterSequence}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        FormalParameterSequence ParseProperFormalParameterSequence()
        {

            var startLocation = _currentToken.Position.Start;
            var formalParameter = ParseFormalParameter();
            if (_currentToken.Kind == TokenKind.Comma)
            {
                AcceptIt();
                var formalParameterSequence = ParseProperFormalParameterSequence();
                var formalsPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                return new MultipleFormalParameterSequence(formalParameter, formalParameterSequence,
                    formalsPosition);
            }
            else
            {
                var formalsPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                return new SingleFormalParameterSequence(formalParameter, formalsPosition);
            }

        }

        /**
         * Parses the formal parameter, and constructs an AST to represent its phrase
         * structure.
         * 
         * @return a {@link triangle.compiler.syntax.trees.formals.FormalParameter}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        FormalParameter ParseFormalParameter()
        {

            var startLocation = _currentToken.Position.Start;
            switch (_currentToken.Kind)
            {

                case TokenKind.Identifier:
                    {
                        var identifier = ParseIdentifier();
                        Accept(TokenKind.Colon);
                        var typeDenoter = ParseTypeDenoter();
                        var formalPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                        return new ConstFormalParameter(identifier, typeDenoter, formalPosition);
                    }

                case TokenKind.Var:
                    {
                        AcceptIt();
                        var identifier = ParseIdentifier();
                        Accept(TokenKind.Colon);
                        var typeDenoter = ParseTypeDenoter();
                        var formalPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                        return new VarFormalParameter(identifier, typeDenoter, formalPosition);
                    }

                case TokenKind.Proc:
                    {
                        AcceptIt();
                        var identifier = ParseIdentifier();
                        Accept(TokenKind.LeftParen);
                        var formalParameterSequence = ParseFormalParameterSequence();
                        Accept(TokenKind.RightParen);
                        var formalPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                        return new ProcFormalParameter(identifier, formalParameterSequence, formalPosition);
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
                        var formalPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                        return new FuncFormalParameter(identifier, formals, typeDenoter, formalPosition);
                    }

                default:
                    {
                        RaiseSyntacticError("\"%\" cannot start a formal parameter", _currentToken);
                        return null;
                    }

            }

        }

        /**
         * Parses the actual parameter sequence, and constructs an AST to represent
         * its phrase structure.
         * 
         * @return an
         *         {@link triangle.compiler.syntax.trees.actuals.ActualParameterSequence}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        ActualParameterSequence ParseActualParameterSequence()
        {

            var startLocation = _currentToken.Position.Start;
            if (_currentToken.Kind == TokenKind.RightParen)
            {
                var actualsPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                return new EmptyActualParameterSequence(actualsPosition);
            }
            return ParseProperActualParameterSequence();
        }

        /**
         * Parses the proper (non-empty) actual parameter sequence, and constructs an
         * AST to represent its phrase structure.
         * 
         * @return an
         *         {@link triangle.compiler.syntax.trees.actuals.ActualParameterSequence}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        ActualParameterSequence ParseProperActualParameterSequence()
        {

            var startLocation = _currentToken.Position.Start;
            var actualParameter = ParseActualParameter();
            if (_currentToken.Kind == TokenKind.Comma)
            {
                AcceptIt();
                var actualParameterSequence = ParseProperActualParameterSequence();
                var actualsPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                return new MultipleActualParameterSequence(actualParameter, actualParameterSequence,
                    actualsPosition);
            }
            else
            {
                var actualsPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                return new SingleActualParameterSequence(actualParameter, actualsPosition);
            }

        }

        /**
         * Parses the actual parameter, and constructs an AST to represent its phrase
         * structure.
         * 
         * @return an {@link triangle.compiler.syntax.trees.actuals.ActualParameter}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        ActualParameter ParseActualParameter()
        {

            var startLocation = _currentToken.Position.Start;
            switch (_currentToken.Kind)
            {

                case TokenKind.Identifier:
                case TokenKind.IntLiteral:
                case TokenKind.CharLiteral:
                case TokenKind.Operator:
                case TokenKind.Let:
                case TokenKind.If:
                case TokenKind.LeftParen:
                case TokenKind.LeftBracket:
                case TokenKind.LeftCurly:
                    {
                        var expression = ParseExpression();
                        var actualPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                        return new ConstActualParameter(expression, actualPosition);
                    }

                case TokenKind.Var:
                    {
                        AcceptIt();
                        var vname = ParseVname();
                        var actualPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                        return new VarActualParameter(vname, actualPosition);
                    }

                case TokenKind.Proc:
                    {
                        AcceptIt();
                        var identifier = ParseIdentifier();
                        var actualPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                        return new ProcActualParameter(identifier, actualPosition);
                    }

                case TokenKind.Func:
                    {
                        AcceptIt();
                        var identifier = ParseIdentifier();
                        var actualPosition = new SourcePosition(startLocation, _currentToken.Position.Finish);
                        return new FuncActualParameter(identifier, actualPosition);
                    }

                default:
                    {
                        RaiseSyntacticError("\"%\" cannot start an actual parameter", _currentToken);
                        return null;
                    }

            }

        }
    }
}