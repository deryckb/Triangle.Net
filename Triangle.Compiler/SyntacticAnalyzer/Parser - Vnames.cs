using Triangle.Compiler.SyntaxTrees.Terminals;
using Triangle.Compiler.SyntaxTrees.Vnames;

namespace Triangle.Compiler.SyntacticAnalyzer
{
    public partial class Parser
    {

        // /////////////////////////////////////////////////////////////////////////////
        //
        // VALUE-OR-VARIABLE NAMES
        //
        // /////////////////////////////////////////////////////////////////////////////

        /**
         * Parses the v-name, and constructs an AST to represent its phrase structure.
         * 
         * @return a {@link triangle.compiler.syntax.trees.vnames.Vname}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        Vname ParseVname()
        {
            var identifier = ParseIdentifier();
            return ParseRestOfVname(identifier);
        }

        /**
         * Parses the rest of a v-name, and constructs an AST to represent its phrase
         * structure.
         * 
         * @param firstIdentifier
         *          the {@link triangle.compiler.syntax.trees.terminals.Identifier}
         *          that is the start of the
         *          {@link triangle.compiler.syntax.trees.vnames.Vname}
         * 
         * @return a {@link triangle.compiler.syntax.trees.vnames.Vname}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        Vname ParseRestOfVname(Identifier firstIdentifier)
        {

            var startLocation = firstIdentifier.Start;
            Vname vname = new SimpleVname(firstIdentifier, firstIdentifier.Position);

            while (_currentToken.Kind == TokenKind.Dot
                || _currentToken.Kind == TokenKind.LeftBracket)
            {

                if (_currentToken.Kind == TokenKind.Dot)
                {
                    AcceptIt();
                    var identifier = ParseIdentifier();
                    var vnamePosition = new SourcePosition(startLocation, _currentToken.Finish);
                    vname = new DotVname(vname, identifier, vnamePosition);
                }
                else
                {
                    AcceptIt();
                    var expression = ParseExpression();
                    Accept(TokenKind.RightBracket);
                    var vnamePosition = new SourcePosition(startLocation, _currentToken.Finish);
                    vname = new SubscriptVname(vname, expression, vnamePosition);
                }
            }

            return vname;

        }
    }
}