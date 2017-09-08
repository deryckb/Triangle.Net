using Triangle.Compiler.SyntaxTrees.Actuals;
using Triangle.Compiler.SyntaxTrees.Declarations;
using Triangle.Compiler.SyntaxTrees.Formals;
using Triangle.Compiler.SyntaxTrees.Visitors;

namespace Triangle.Compiler.ContextualAnalyzer
{
    public partial class Checker : IActualParameterVisitor<FormalParameter, Void>,
            IActualParameterSequenceVisitor<FormalParameterSequence, Void>,
            IFormalParameterVisitor,
            IFormalParameterSequenceVisitor
    {
        // Formal Parameters

        // Always returns null. Does not use the given object.

        public Void VisitConstFormalParameter(ConstFormalParameter ast, Void arg)
        {
            ast.Type = ast.Type.Visit(this);
            _idTable.Enter(ast.Identifier, ast);
            CheckAndReportError(!ast.Duplicated, "duplicated formal parameter \"%\"",
                ast.Identifier, ast);
            return null;
        }

        public Void VisitFuncFormalParameter(FuncFormalParameter ast, Void arg)
        {
            _idTable.OpenScope();
            ast.Formals.Visit(this);
            _idTable.CloseScope();
            ast.Type = ast.Type.Visit(this);
            _idTable.Enter(ast.Identifier, ast);
            CheckAndReportError(!ast.Duplicated, "duplicated formal parameter \"%\"",
                ast.Identifier, ast);
            return null;
        }

        public Void VisitProcFormalParameter(ProcFormalParameter ast, Void arg)
        {
            _idTable.OpenScope();
            ast.Formals.Visit(this);
            _idTable.CloseScope();
            _idTable.Enter(ast.Identifier, ast);
            CheckAndReportError(!ast.Duplicated, "duplicated formal parameter \"%\"",
                ast.Identifier, ast);
            return null;
        }

        public Void VisitVarFormalParameter(VarFormalParameter ast, Void arg)
        {
            ast.Type = ast.Type.Visit(this);
            _idTable.Enter(ast.Identifier, ast);
            CheckAndReportError(!ast.Duplicated, "duplicated formal parameter \"%\"",
                ast.Identifier, ast);
            return null;
        }

        public Void VisitEmptyFormalParameterSequence(EmptyFormalParameterSequence ast, Void arg)
        {
            return null;
        }

        public Void VisitMultipleFormalParameterSequence(MultipleFormalParameterSequence ast, Void arg)
        {
            ast.Formal.Visit(this);
            ast.Formals.Visit(this);
            return null;
        }

        public Void VisitSingleFormalParameterSequence(SingleFormalParameterSequence ast, Void arg)
        {
            ast.Formal.Visit(this);
            return null;
        }

        // Actual Parameters

        // Always returns null. Uses the given FormalParameter.

        public Void VisitConstActualParameter(ConstActualParameter ast, FormalParameter arg)
        {
            var expressionType = ast.Expression.Visit(this);
            var param = arg as ConstFormalParameter;
            if (param != null)
            {
                CheckAndReportError(expressionType.Equals(param.Type),
                    "wrong type for const actual parameter", ast.Expression);
            }
            else
            {
                ReportError("const actual parameter not expected here", ast);
            }
            return null;
        }

        public Void VisitFuncActualParameter(FuncActualParameter ast, FormalParameter arg)
        {
            var binding = ast.Identifier.Visit(this);
            var function = binding as IFunctionDeclaration;
            if (function != null)
            {
                var formals = function.Formals;
                var functionType = function.Type;
                if (arg is FuncFormalParameter)
                {
                    var param = (FuncFormalParameter)arg;
                    if (!formals.Equals(param.Formals))
                    {
                        ReportError("wrong signature for function \"%\"", ast.Identifier);
                    }
                    else if (!functionType.Equals(param.Type))
                    {
                        ReportError("wrong type for function \"%\"", ast.Identifier);
                    }
                }
                else
                {
                    ReportError("func actual parameter not expected here", ast);
                }
            }
            else
            {
                ReportUndeclaredOrError(binding, ast.Identifier, "\"%\" is not a function identifier");
            }
            return null;
        }

        public Void VisitProcActualParameter(ProcActualParameter ast, FormalParameter arg)
        {
            var binding = ast.Identifier.Visit(this);
            var procedure = binding as IProcedureDeclaration;
            if (procedure != null)
            {
                var formals = procedure.Formals;
                if (arg is ProcFormalParameter)
                {
                    var param = (ProcFormalParameter)arg;
                    CheckAndReportError(formals.Equals(param.Formals),
                        "wrong signature for procedure \"%\"", ast.Identifier);
                }
                else
                {
                    ReportError("proc actual parameter not expected here", ast);
                }
            }
            else
            {
                ReportUndeclaredOrError(binding, ast.Identifier, "\"%\" is not a procedure identifier");
            }
            return null;
        }

        public Void VisitVarActualParameter(VarActualParameter ast, FormalParameter arg)
        {
            var actualType = ast.Vname.Visit(this);
            if (!ast.Vname.IsVariable)
            {
                ReportError("actual parameter is not a variable", ast.Vname);
            }
            else if (arg is VarFormalParameter)
            {
                var parameter = (VarFormalParameter)arg;
                CheckAndReportError(actualType.Equals(parameter.Type),
                    "wrong type for var actual parameter", ast.Vname);
            }
            else
            {
                ReportError("var actual parameter not expected here", ast.Vname);
            }
            return null;
        }

        public Void VisitEmptyActualParameterSequence(EmptyActualParameterSequence ast,
                FormalParameterSequence arg)
        {
            CheckAndReportError(arg is EmptyFormalParameterSequence, "too few actual parameters",
                ast);
            return null;
        }

        public Void VisitMultipleActualParameterSequence(MultipleActualParameterSequence ast,
                FormalParameterSequence arg)
        {
            var formals = arg as MultipleFormalParameterSequence;
            if (formals != null)
            {
                ast.Actual.Visit(this, formals.Formal);
                ast.Actuals.Visit(this, formals.Formals);
            }
            else
            {
                ReportError("too many actual parameters", ast);
            }
            return null;
        }

        public Void VisitSingleActualParameterSequence(SingleActualParameterSequence ast,
                FormalParameterSequence arg)
        {
            var formal = arg as SingleFormalParameterSequence;
            if (formal != null)
            {
                ast.Actual.Visit(this, formal.Formal);
            }
            else
            {
                ReportError("incorrect number of actual parameters", ast);
            }
            return null;
        }

    }
}