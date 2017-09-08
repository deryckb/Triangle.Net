using Triangle.Compiler.SyntaxTrees.Declarations;
using Triangle.Compiler.SyntaxTrees.Visitors;

namespace Triangle.Compiler.ContextualAnalyzer
{
    public partial class Checker : IDeclarationVisitor
    {
        // Declarations

        public Void VisitBinaryOperatorDeclaration(BinaryOperatorDeclaration ast, Void arg)
        {
            return null;
        }

        public Void VisitConstDeclaration(ConstDeclaration ast, Void arg)
        {
            ast.Expression.Visit(this);
            _idTable.Enter(ast.Identifier, ast);
            CheckAndReportError(!ast.Duplicated, "identifier \"%\" already declared",
                ast.Identifier, ast);
            return null;
        }

        public Void VisitFuncDeclaration(FuncDeclaration ast, Void arg)
        {
            ast.Type = ast.Type.Visit(this);
            // permits recursion
            _idTable.Enter(ast.Identifier, ast);
            CheckAndReportError(!ast.Duplicated, "identifier \"%\" already declared",
                ast.Identifier, ast);
            _idTable.OpenScope();
            ast.Formals.Visit(this);
            var expressionType = ast.Expression.Visit(this);
            _idTable.CloseScope();
            CheckAndReportError(ast.Type.Equals(expressionType),
                "body of function \"%\" has wrong type", ast.Identifier, ast.Expression);
            return null;
        }

        public Void VisitProcDeclaration(ProcDeclaration ast, Void arg)
        {
            // permits recursion
            _idTable.Enter(ast.Identifier, ast);
            CheckAndReportError(!ast.Duplicated, "identifier \"%\" already declared",
                ast.Identifier, ast);
            _idTable.OpenScope();
            ast.Formals.Visit(this);
            ast.Command.Visit(this);
            _idTable.CloseScope();
            return null;
        }

        public Void VisitSequentialDeclaration(SequentialDeclaration ast, Void arg)
        {
            ast.FirstDeclaration.Visit(this);
            ast.SecondDeclaration.Visit(this);
            return null;
        }

        public Void VisitTypeDeclaration(TypeDeclaration ast, Void arg)
        {
            ast.Type = ast.Type.Visit(this);
            _idTable.Enter(ast.Identifier, ast);
            CheckAndReportError(!ast.Duplicated, "identifier \"%\" already declared",
                ast.Identifier, ast);
            return null;
        }

        public Void VisitUnaryOperatorDeclaration(UnaryOperatorDeclaration ast, Void arg)
        {
            return null;
        }

        public Void VisitVarDeclaration(VarDeclaration ast, Void arg)
        {
            ast.Type = ast.Type.Visit(this);
            _idTable.Enter(ast.Identifier, ast);
            CheckAndReportError(!ast.Duplicated, "identifier \"%\" already declared",
                ast.Identifier, ast);
            return null;
        }

    }
}