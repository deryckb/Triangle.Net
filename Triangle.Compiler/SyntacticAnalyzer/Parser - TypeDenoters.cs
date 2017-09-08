using Triangle.Compiler.SyntaxTrees.Types;

namespace Triangle.Compiler.SyntacticAnalyzer
{
    public partial class Parser
    {

        // /////////////////////////////////////////////////////////////////////////////
        //
        // TYPE-DENOTERS
        //
        // /////////////////////////////////////////////////////////////////////////////

        /**
         * Parses the type denoter, and constructs an AST to represent its phrase
         * structure.
         * 
         * @return a {@link triangle.compiler.syntax.trees.types.TypeDenoter}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        TypeDenoter ParseTypeDenoter()
        {

            var startLocation = _currentToken.Start;
            switch (_currentToken.Kind)
            {

                case TokenKind.Identifier:
                    {
                        var identifier = ParseIdentifier();
                        var typePosition = new SourcePosition(startLocation, _currentToken.Finish);
                        return new SimpleTypeDenoter(identifier, typePosition);
                    }

                case TokenKind.Array:
                    {
                        AcceptIt();
                        var integerLiteral = ParseIntegerLiteral();
                        Accept(TokenKind.Of);
                        var typeDenoter = ParseTypeDenoter();
                        var typePosition = new SourcePosition(startLocation, _currentToken.Finish);
                        return new ArrayTypeDenoter(integerLiteral, typeDenoter, typePosition);
                    }

                case TokenKind.Record:
                    {
                        AcceptIt();
                        var fieldTypeDenoter = ParseFieldTypeDenoter();
                        Accept(TokenKind.End);
                        var typePosition = new SourcePosition(startLocation, _currentToken.Finish);
                        return new RecordTypeDenoter(fieldTypeDenoter, typePosition);
                    }

                default:
                    {
                        RaiseSyntacticError("\"%\" cannot start a type denoter", _currentToken);
                        return null;
                    }

            }

        }

        /**
         * Parses the field type denoter, and constructs an AST to represent its
         * phrase structure.
         * 
         * @return a {@link triangle.compiler.syntax.trees.types.FieldTypeDenoter}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        FieldTypeDenoter ParseFieldTypeDenoter()
        {

            var startLocation = _currentToken.Start;
            var identifier = ParseIdentifier();
            Accept(TokenKind.Colon);
            var typeDenoter = ParseTypeDenoter();
            if (_currentToken.Kind == TokenKind.Comma)
            {
                AcceptIt();
                var fieldTypeDenoter = ParseFieldTypeDenoter();
                var fieldPosition = new SourcePosition(startLocation, _currentToken.Finish);
                return new MultipleFieldTypeDenoter(identifier, typeDenoter, fieldTypeDenoter, fieldPosition);
            }
            else
            {
                var fieldPosition = new SourcePosition(startLocation, _currentToken.Finish);
                return new SingleFieldTypeDenoter(identifier, typeDenoter, fieldPosition);
            }

        }
    }
}