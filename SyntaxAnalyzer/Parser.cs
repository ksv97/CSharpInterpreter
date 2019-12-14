using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scanner;

namespace SyntaxAnalyzer
{
    public class Parser
    {
        private Scaner scaner;

        private Token currentToken;

        private TokenType CurrentTokenType => this.currentToken.TokenType;

        public Parser(Scaner scaner)
        {
            this.scaner = scaner;
            if (scaner.ResultTokens.Count == 0)
            {
                scaner.ScanText();
            }
            currentToken = scaner.NextToken;
        }

        /// <summary>
        /// Parses the 
        /// </summary>
        public void Parse()
        {
            while (currentToken != null)
            {
                if (Node() == false)
                {
                    ThrowInvalidTokenException();
                }
            }
        }

        /// <summary>
        /// Parse node statement and advance in case of success
        /// </summary>
        private bool Node() // DONE
        {
            if (DeclareStatement() == true)
            {
                return true;
            }

            if (AssignStatement() == true)
            {
                return true;
            }

            if (IfStatement() == true)
            {
                return true;
            }

            if (Cycle() == true)
            {
                return true;
            }

            if (Block() == true)
            {
                return true;                
            }

            return false;
        }

        private bool DeclareStatement() // DONE
        {
            if (DeclareNumStatement() == true)
            {
                return true;
            }

            if (DeclareBooleanStatement() == true)
            {
                return true;
            }

            if (DeclareStringStatement() == true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check for declare statement and advances in case of success.
        /// </summary>
        /// <returns></returns>
        private bool DeclareNumStatement() // DONE
        {
            if (CurrentTokenType != TokenType.INT && CurrentTokenType != TokenType.DOUBLE)
            {
                return false;
            }

            NextToken(); // advance to variable
            if (CurrentTokenType != TokenType.VARIABLE)
            {
                return false;
            }

            NextToken(); // advance to assignment or semicolon
            if (CurrentTokenType == TokenType.ASSIGN)
            {
                NextToken(); // advance to expression
                if (NumExpression() == false)
                {
                    return false;
                }
            }

            if (CurrentTokenType == TokenType.SEMICOLON)
            {
                NextToken();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks num expression and advances to the next token in case of success
        /// </summary>
        private bool NumExpression() // DONE
        {
            if (NumTerm() == false)
            {
                return false;
            }      
            
            while (CurrentTokenType == TokenType.PLUS || CurrentTokenType == TokenType.MINUS)
            {
                if (NumTerm() == false)
                {
                    return false;
                }
            }

            return true;            
        }

        /// <summary>
        /// Checks the num term and advance in case of success.
        /// </summary>
        /// <returns></returns>
        private bool NumTerm()
        {
            if (NumFactor() == false)
            {
                return false;
            }

            while (CurrentTokenType == TokenType.DIV || CurrentTokenType == TokenType.MULTIPLY || CurrentTokenType == TokenType.MOD)
            {                
                if (NumFactor() == false)
                {
                    return false;
                }                
            }

            return true;
        }

        /// <summary>
        /// Advances
        /// </summary>
        /// <returns></returns>
        private bool NumFactor() // DONE
        {
            if (CurrentTokenType == TokenType.PARANTHESIS_START)
            {
                NextToken();
                bool numExpressionResult = NumExpression();
                if (numExpressionResult == false)
                {
                    return false;
                }

                NextToken();
                if (CurrentTokenType == TokenType.PARANTHESIS_END)
                {
                    NextToken();
                    return true;
                }
            }

           if (Sqr() == true)
            {
                NextToken();
                return true;
            }

           if (Sqrt() == true)
            {
                NextToken();
                return true;
            }

            if (CurrentTokenType == TokenType.INT_CONST
                || CurrentTokenType == TokenType.DOUBLE_CONST
                || CurrentTokenType == TokenType.VARIABLE)
            {
                NextToken();
                return true;
            }

            return false;            
        }

        private bool Sqr() // DONE
        {
            if (CurrentTokenType != TokenType.SQR)
            {
                return false;
            }

            NextToken(); // advance to paranthesis
            if (CurrentTokenType != TokenType.PARANTHESIS_START)
            {
                return false;
            }

            NextToken();
            if (NumExpression() == false)
            {
                return false;
            }


            if (CurrentTokenType == TokenType.PARANTHESIS_END)
            {
                return true;
            }

            return false;
        }

        private bool Sqrt() // DONE
        {
            if (CurrentTokenType != TokenType.SQRT)
            {
                return false;
            }

            NextToken(); // advance to paranthesis
            if (CurrentTokenType != TokenType.PARANTHESIS_START)
            {
                return false;
            }

            NextToken();
            if (NumExpression() == false)
            {
                return false;
            }


            if (CurrentTokenType == TokenType.PARANTHESIS_END)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check for declare bool statement and advances in case of success
        /// </summary>
        /// <returns></returns>
        private bool DeclareBooleanStatement()
        {
            if (CurrentTokenType != TokenType.BOOLEAN)
            {
                return false;
            }

            NextToken();
            if (CurrentTokenType != TokenType.VARIABLE)
            {
                return false;
            }
           
            NextToken(); // advance to assignment or semicolon
            if (CurrentTokenType == TokenType.ASSIGN)
            {
                NextToken();
                if ( BoolExpression() == false)
                {
                    return false;
                }
            }

            if (CurrentTokenType == TokenType.SEMICOLON)
            {
                NextToken();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks bool expression and advances to the next token in case of success
        /// </summary>
        private bool BoolExpression()
        {
            if (BoolTerm() == false)
            {
                return false;
            }

            while (CurrentTokenType == TokenType.LOGICAL_OR)
            {
                if (BoolTerm() == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks bool term and advances in case of success.
        /// </summary>
        /// <returns></returns>
        private bool BoolTerm()
        {
            if (BoolNotFactor() == false)
            {
                return false;
            }

            NextToken();
            while (CurrentTokenType == TokenType.LOGICAL_AND)
            {
                if (BoolNotFactor() == false)
                {
                    return false;
                }

                NextToken();
            }

            return true;
        }

        private bool BoolNotFactor()
        {
            if (CurrentTokenType == TokenType.NOT)
            {
                NextToken();
                return BoolNotFactor();
            }

            return BoolFactor();
        }

        private bool BoolFactor()
        {
            if (CurrentTokenType == TokenType.VARIABLE
                || CurrentTokenType == TokenType.TRUE
                || CurrentTokenType == TokenType.FALSE)
            {
                return true;
            }

            if (CurrentTokenType == TokenType.PARANTHESIS_START)
            {
                NextToken();
                if (BoolExpression() == false)
                {
                    return false;
                }

                if (CurrentTokenType == TokenType.PARANTHESIS_END)
                {
                    return true;
                }
                else return false;                
            }

            return BoolOp();
        }

        private bool BoolOp()
        {
            if (NumExpression() == true)
            {
                if (EqualityOperator() == true)
                {
                    return NumExpression();
                }
                else if (MoreLessOperator() == true)
                {
                    return NumExpression();
                }
            }

            if (BoolExpression() == true)
            {
                if (EqualityOperator() == true)
                {
                    return BoolExpression();
                }
            }

            if (StringExpression() == true)
            {
                if (EqualityOperator() == true)
                {
                    return StringExpression();
                }
            }

            return false;
        }

        /// <summary>
        /// Checks current token for being equality operation and advances in case of success.
        /// </summary>
        private bool EqualityOperator()
        {
            if (CurrentTokenType == TokenType.EQUALS || CurrentTokenType == TokenType.NON_EQUALS)
            {
                NextToken();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks current token for being more/less operation and advances in case of success.
        /// </summary>
        private bool MoreLessOperator()
        {
            if (CurrentTokenType == TokenType.MORE
                || CurrentTokenType == TokenType.MORE_OR_EQUAL
                || CurrentTokenType == TokenType.LESS
                || CurrentTokenType == TokenType.LESS_OR_EQUAL)
            {
                NextToken();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check for string statement and advances in case of success.
        /// </summary>
        /// <returns></returns>
        private bool DeclareStringStatement()
        {
            if (CurrentTokenType != TokenType.STRING)
            {
                return false;
            }

            NextToken();
            if (CurrentTokenType != TokenType.VARIABLE)
            {
                return false;
            }

            NextToken();
            if (CurrentTokenType == TokenType.ASSIGN)
            {
                NextToken();
                if (StringExpression() == false)
                {
                    return false;
                }
            }

            if (CurrentTokenType == TokenType.SEMICOLON)
            {
                NextToken();
                return true;
            }

            return false;
        }
   
        /// <summary>
        /// Checks whether expression is a string and jumps to the next token in case of success.
        /// </summary>
        /// <returns></returns>
        private bool StringExpression()
        {
            if (StringTerm() == false)
            {
                return false;
            }

            NextToken();
            while (CurrentTokenType == TokenType.PLUS)
            {
                if (StringTerm() == false)
                {
                    return false;
                }
                NextToken();
            }

            return true;
        }

        private bool StringTerm()
        {
            if (CurrentTokenType == TokenType.STRING_CONST || CurrentTokenType == TokenType.VARIABLE)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check for assignment statement and advances in case of success.
        /// </summary>
        /// <returns></returns>
        private bool AssignStatement()
        {
            if (CurrentTokenType != TokenType.VARIABLE)
            {
                return false;
            }

            NextToken();

            if (CurrentTokenType != TokenType.ASSIGN)
            {
                return false;
            }

            NextToken();

            if (NumExpression() == true)
            {                
                return true;
            }
            else if (StringExpression() == true)
            {
                return true;
            }
            else if (BoolExpression() == true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for if statement and advances in case of success.
        /// </summary>
        /// <returns></returns>
        private bool IfStatement()
        {
            if (CurrentTokenType != TokenType.IF)
            {
                return false;
            }

            NextToken();
            if (CurrentTokenType != TokenType.PARANTHESIS_START)
            {
                return false;
            }

            NextToken();
            if (BoolExpression() == false)
            {
                return false;
            }

            if (CurrentTokenType != TokenType.PARANTHESIS_END)
            {
                return false;
            }

            NextToken();
            if (AfterCondition() == false)
            {
                return false;
            }

            if (CurrentTokenType == TokenType.ELSE)
            {
                NextToken();
                return AfterCondition();
            }
            else return true;

        }

        /// <summary>
        /// Checks the correctness of block after 'if' or 'else' statements and advance in case of success.
        /// </summary>
        /// <returns></returns>
        private bool AfterCondition()
        {
            if (AssignStatement() == true
                || IfStatement() == true
                || Cycle() == true
                || Block() == true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for 'do' and 'while' cycles and advances in case of success.
        /// </summary>
        /// <returns></returns>
        private bool Cycle()
        {
            if (PreConditionalCycle() == true)
            {
                return true;
            }
            else return PostConditionalCycle();
        }

        /// <summary>
        /// Checks for 'while' loop and advances in case of success.
        /// </summary>
        /// <returns></returns>
        private bool PreConditionalCycle()
        {
            if (CurrentTokenType != TokenType.WHILE)
            {
                return false;
            }

            NextToken();
            if (CurrentTokenType != TokenType.PARANTHESIS_START)
            {
                return false;
            }

            NextToken();
            if (BoolExpression() == false)
            {
                return false;
            }

            if (CurrentTokenType != TokenType.PARANTHESIS_END)
            {
                return false;
            }
            NextToken();

            return AfterCondition();
        }

        /// <summary>
        /// Checks for 'do' loop and advances in case of success.
        /// </summary>
        /// <returns></returns>
        private bool PostConditionalCycle()
        {
            if (CurrentTokenType != TokenType.DO)
            {
                return false;
            }

            NextToken();

            if (AfterCondition() == false)
            {
                return false;
            }

            if (CurrentTokenType != TokenType.WHILE)
            {
                return false;
            }

            NextToken();
            if (CurrentTokenType != TokenType.PARANTHESIS_START)
            {
                return false;
            }

            NextToken();
            if (BoolExpression() == false)
            {
                return false;
            }
            
            if (CurrentTokenType != TokenType.PARANTHESIS_END)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for block of code and advances in case of success.
        /// </summary>
        /// <returns></returns>
        private bool Block()
        {
            if (CurrentTokenType != TokenType.CODEBLOCK_START)
            {
                return false;
            }

            NextToken();
            while (Node() == true);
            if (CurrentTokenType != TokenType.CODEBLOCK_END)
            {
                return false;
            }

            NextToken();
            return true;
        }

        private void NextToken() => this.currentToken = scaner.NextToken;

        private void ThrowInvalidTokenException()
        {
            throw new InvalidOperationException($"Error ({currentToken.Line}, {currentToken.Position}): Invalid token: {currentToken.Value}");
        }
    }
}
