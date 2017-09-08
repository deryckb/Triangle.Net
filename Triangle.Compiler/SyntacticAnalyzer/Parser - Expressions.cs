using Triangle.Compiler.SyntaxTrees.Aggregates;
using Triangle.Compiler.SyntaxTrees.Expressions;

namespace Triangle.Compiler.SyntacticAnalyzer
{
    public partial class Parser
    {
        ///////////////////////////////////////////////////////////////////////////////
        //
        // EXPRESSIONS
        //
        ///////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Parses the expression, and constructs an AST to represent its phrase
        /// structure.
        /// </summary>
        /// <returns>
        /// an <link>Triangle.SyntaxTrees.Expressions.Expression</link>
        /// </returns> 
        /// <throws type="SyntaxError">
        /// a syntactic error
        /// </throws>
        Expression ParseExpression()
        {

            var startLocation = _currentToken.Start;

            switch (_currentToken.Kind)
            {

                case TokenKind.Let:
                    {
                        AcceptIt();
                        var declaration = ParseDeclaration();
                        Accept(TokenKind.In);
                        var expression = ParseExpression();
                        var expressionPos = new SourcePosition(startLocation, _currentToken.Finish);
                        return new LetExpression(declaration, expression, expressionPos);
                    }

                case TokenKind.If:
                    {
                        AcceptIt();
                        var expression1 = ParseExpression();
                        Accept(TokenKind.Then);
                        var expression2 = ParseExpression();
                        Accept(TokenKind.Else);
                        var expression3 = ParseExpression();
                        var expressionPos = new SourcePosition(startLocation, _currentToken.Finish);
                        return new IfExpression(expression1, expression2, expression3, expressionPos);
                    }

                default:
                    {
                        return ParseSecondaryExpression();
                    }
            }
        }

        /// <summary>
        // Parses the secondary expression, and constructs an AST to represent its
        /// phrase structure.
        /// </summary>
        /// <returns>
        /// an <link>Triangle.SyntaxTrees.Expressions.Expression</link>
        /// </returns>
        /// <throws type="SyntaxError">
        /// a syntactic error
        /// </throws>
        Expression ParseSecondaryExpression()
        {

            var startLocation = _currentToken.Start;
            var expression = ParsePrimaryExpression();
            while (_currentToken.Kind == TokenKind.Operator)
            {
                var op = ParseOperator();
                var expression2 = ParsePrimaryExpression();
                var expressionPos = new SourcePosition(startLocation, _currentToken.Finish);
                expression = new BinaryExpression(expression, op, expression2, expressionPos);
            }
            return expression;
        }

        /// <summary>
        /// Parses the primary expression, and constructs an AST to represent its
        /// phrase structure.
        /// </summary>
        /// <returns>
        /// an <link>Triangle.SyntaxTrees.Expressions.Expression</link>
        /// </returns>
        /// <throws type="SyntaxError">
        /// a syntactic error
        /// </throws>
        Expression ParsePrimaryExpression()
        {

            var startlocation = _currentToken.Start;
            switch (_currentToken.Kind)
            {

                case TokenKind.IntLiteral:
                    {
                        var integerLiteral = ParseIntegerLiteral();
                        var expressionPos = new SourcePosition(startlocation, _currentToken.Finish);
                        return new IntegerExpression(integerLiteral, expressionPos);
                    }

                case TokenKind.CharLiteral:
                    {
                        var characterLiteral = ParseCharacterLiteral();
                        var expressionPos = new SourcePosition(startlocation, _currentToken.Finish);
                        return new CharacterExpression(characterLiteral, expressionPos);
                    }

                case TokenKind.LeftBracket:
                    {
                        AcceptIt();
                        var arrayAggregate = ParseArrayAggregate();
                        Accept(TokenKind.RightBracket);
                        var expressionPos = new SourcePosition(startlocation, _currentToken.Finish);
                        return new ArrayExpression(arrayAggregate, expressionPos);
                    }

                case TokenKind.LeftCurly:
                    {
                        AcceptIt();
                        var recordAggregate = ParseRecordAggregate();
                        Accept(TokenKind.RightCurly);
                        var expressionPos = new SourcePosition(startlocation, _currentToken.Finish);
                        return new RecordExpression(recordAggregate, expressionPos);
                    }

                case TokenKind.Identifier:
                    {
                        var identifier = ParseIdentifier();
                        if (_currentToken.Kind == TokenKind.LeftParen)
                        {
                            AcceptIt();
                            var actualParameterSequence = ParseActualParameterSequence();
                            Accept(TokenKind.RightParen);
                            var expressionPos = new SourcePosition(startlocation, _currentToken.Finish);
                            return new CallExpression(identifier, actualParameterSequence, expressionPos);
                        }
                        else
                        {
                            var vname = ParseRestOfVname(identifier);
                            var expressionPos = new SourcePosition(startlocation, _currentToken.Finish);
                            return new VnameExpression(vname, expressionPos);
                        }
                    }

                case TokenKind.Operator:
                    {
                        var op = ParseOperator();
                        var expression = ParsePrimaryExpression();
                        var expressionPos = new SourcePosition(startlocation, _currentToken.Finish);
                        return new UnaryExpression(op, expression, expressionPos);
                    }

                case TokenKind.LeftParen:
                    {
                        AcceptIt();
                        var expression = ParseExpression();
                        Accept(TokenKind.RightParen);
                        return expression;
                    }

                default:
                    {
                        RaiseSyntacticError("\"%\" cannot start an expression", _currentToken);
                        return null;
                    }
            }
        }

        /// <summary>
        /// Parses the record aggregate, and constructs an AST to represent its phrase
        /// structure.
        /// </summary>
        /// @return a {@link triangle.compiler.syntax.trees.aggregates.RecordAggregate}
        ///
        /// @throws SyntaxError
        ///           a syntactic error
        ///
        RecordAggregate ParseRecordAggregate()
        {

            var startLocation = _currentToken.Start;
            var identifier = ParseIdentifier();
            Accept(TokenKind.Is);
            var expression = ParseExpression();

            if (_currentToken.Kind == TokenKind.Comma)
            {
                AcceptIt();
                var recordAggregate = ParseRecordAggregate();
                var aggregatePosition = new SourcePosition(startLocation, _currentToken.Finish);
                return new MultipleRecordAggregate(identifier, expression, recordAggregate, aggregatePosition);
            }
            else
            {
                var aggregatePosition = new SourcePosition(startLocation, _currentToken.Finish);
                return new SingleRecordAggregate(identifier, expression, aggregatePosition);
            }
        }

        /**
         * Parses the array aggregate, and constructs an AST to represent its phrase
         * structure.
         * 
         * @return an {@link triangle.compiler.syntax.trees.aggregates.ArrayAggregate}
         * 
         * @throws SyntaxError
         *           a syntactic error
         * 
         */
        ArrayAggregate ParseArrayAggregate()
        {

            var startLocation = _currentToken.Start;
            var expression = ParseExpression();
            if (_currentToken.Kind == TokenKind.Comma)
            {
                AcceptIt();
                var arrayAggregate = ParseArrayAggregate();
                var aggregatePosition = new SourcePosition(startLocation, _currentToken.Finish);
                return new MultipleArrayAggregate(expression, arrayAggregate, aggregatePosition);
            }
            else
            {
                var aggregatePosition = new SourcePosition(startLocation, _currentToken.Finish);
                return new SingleArrayAggregate(expression, aggregatePosition);
            }
        }
    }
}