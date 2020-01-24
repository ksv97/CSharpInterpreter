using Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHarpInterpreter
{
    class SyntaxAnalyzer
    {
        public readonly List<Token> Tokens;
        public readonly List<Variable> ConstsAndVariables;

        int currentIndex = 0;
        Token token;

        public SyntaxAnalyzer(List<Token> tokens, List<Variable> constsAndVariables)
        {
            this.Tokens = tokens;
            this.ConstsAndVariables = constsAndVariables;
        }

        public void GetNextToken()
        {
            currentIndex++;
            token = Tokens[currentIndex];
        }

        private void NormalizeTokenList()
        {
            Tokens.RemoveAll(t => t.TokenType == TokenType.COMMENT);
        }

        public string StartSyntaxAnalysis()
        {
            NormalizeTokenList();
            currentIndex = 0;
            token = Tokens[currentIndex];
            var result = Program();
            return result;
        }

        string ConsoleLog()
        {
            string errorMessage = "";
            GetNextToken();
            if (token.TokenType == TokenType.PARANTHESIS_START)
            {
                GetNextToken();
                if (token.TokenType == TokenType.VAR_CONST || token.TokenType == TokenType.VARIABLE)
                {
                    GetNextToken();
                    if (token.TokenType == TokenType.PARANTHESIS_END)
                    {
                        GetNextToken();
                        if (token.TokenType == TokenType.SEMICOLON)
                        {
                            GetNextToken();
                            errorMessage = "";
                        }
                        else
                        {
                            errorMessage = "Ожидалась ; после console.log (" + currentIndex + ")";
                        }
                    }
                    else
                    {
                        errorMessage = "Ожидалась ) в инструкции console.log (" + currentIndex + ")";
                    }
                }
                else
                {
                    errorMessage = "Ожидалась переменная (" + currentIndex + ")";
                }
            }
            else
            {
                errorMessage = "Ожидася символ ( после console.log (" + currentIndex + ")";
            }

            return errorMessage;
        }

        bool Factor()
        {
            bool valid = false;

            if (token.TokenType == TokenType.VARIABLE || token.TokenType == TokenType.VAR_CONST)
            {
                valid = true;
                GetNextToken();
            }
            else if (token.TokenType == TokenType.PARANTHESIS_START)
            {
                GetNextToken();

                if (CheckMathExpression())
                {
                    valid = true;
                    GetNextToken();
                }
            }

            return valid;
        }

        bool Term()
        {
            bool valid = false;
            if (Factor())
            {
                //Нельзя 2 переменные подряд
                if (token.TokenType == TokenType.VARIABLE || token.TokenType == TokenType.VAR_CONST)
                {
                    valid = false;
                    return valid;
                }
                valid = true;
                while ((token.TokenType == TokenType.MULTIPLY || token.TokenType == TokenType.DIV) && token.TokenType != TokenType.SEMICOLON && valid)
                {
                    GetNextToken();
                    if (Factor() == false)
                    {
                        valid = false;
                    }
                }
            }

            return valid;
        }

        bool CheckMathExpression()
        {
            bool valid = false;

            if (Term())
            {
                valid = true;
                while ((token.TokenType == TokenType.PLUS || token.TokenType == TokenType.MINUS) && valid && token.TokenType != TokenType.SEMICOLON)
                {
                    GetNextToken();
                    if (Term() == false)
                    {
                        valid = false;
                    }
                }
            }

            if (valid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        string Assign()
        {
            string errorMessage = "";

            GetNextToken();
            if (token.TokenType == TokenType.VARIABLE)
            {
                GetNextToken();
                if (token.TokenType == TokenType.ASSIGN)
                {
                    GetNextToken();
                    if (CheckMathExpression())
                    {
                        GetNextToken();
                    }
                    else
                    {
                        errorMessage = "Ошибка в математическом выражении (" + currentIndex + ")";
                    }
                }
                else
                {
                    errorMessage = "Ожидался символ = (" + currentIndex + ")";
                }
            }
            else
            {
                errorMessage = "Ожидался идентификатор переменной или константы (" + currentIndex + ")";
            }

            return errorMessage;
        }

        /// <summary>
        /// Checks bool exps like a==b && c==d
        /// </summary>
        /// <returns></returns>
        string CheckBoolExpression()
        {
            string errorMessage = "";

            GetNextToken();
            if (token.TokenType == TokenType.VARIABLE || token.TokenType == TokenType.VAR_CONST)
            {
                while (token.TokenType != TokenType.PARANTHESIS_END || token.TokenType != TokenType.SEMICOLON)
                {
                    GetNextToken();
                    if (token.TokenType == TokenType.MORE ||
                        token.TokenType == TokenType.LESS ||
                        token.TokenType == TokenType.MORE_OR_EQUAL ||
                        token.TokenType == TokenType.LESS_OR_EQUAL ||
                        token.TokenType == TokenType.EQUALS ||
                        token.TokenType == TokenType.NON_EQUALS)
                    {
                        GetNextToken();
                        if (token.TokenType == TokenType.VARIABLE || token.TokenType == TokenType.VAR_CONST)
                        {
                            GetNextToken();
                            if (token.TokenType == TokenType.PARANTHESIS_END || token.TokenType == TokenType.SEMICOLON)
                            {
                                return errorMessage;
                            }
                            else if (token.TokenType == TokenType.LOGICAL_AND || token.TokenType == TokenType.LOGICAL_OR)
                            {
                                GetNextToken();
                                if (token.TokenType == TokenType.VARIABLE || token.TokenType == TokenType.VAR_CONST)
                                {
                                    GetNextToken();
                                }
                                else
                                {
                                    errorMessage = "Ожидалась переменная или константа (" + currentIndex + ")";
                                    return errorMessage;
                                }
                            }
                            else
                            {
                                errorMessage = "Ожидалось завершение логического выражения или && или || (" + currentIndex + ")";
                                return errorMessage;
                            }
                        }
                        else
                        {
                            errorMessage = "Ожидалась переменная или кнстанта (" + currentIndex + ")";
                            break;
                        }
                    }
                    else
                    {
                        errorMessage = "Ожидался знак <, >, <=, >=, ==, или != (" + currentIndex + ")";
                        break;
                    }
                }
            }
            else
            {
                errorMessage = "Ожидалась константа или идентификатор (" + currentIndex + ")";
            }

            return errorMessage;
        }

        string IfCheck()
        {
            string errorMessage = "";

            GetNextToken();
            if (token.TokenType == TokenType.PARANTHESIS_START)
            {
                errorMessage = CheckBoolExpression();
                if (errorMessage.Equals(""))
                {
                    if (token.TokenType == TokenType.PARANTHESIS_END)
                    {
                        GetNextToken();
                        if (token.TokenType == TokenType.CODEBLOCK_START)
                        {
                            errorMessage = CodeBlock();//ТОкен }
                            if (errorMessage.Equals(""))
                            {
                                GetNextToken();
                            }
                        }
                        else
                        {
                            errorMessage = "Ожидался символ { (" + currentIndex + ")";
                        }
                    }
                    else
                    {
                        errorMessage = "Ожиадлась ) (" + currentIndex + ")";
                    }
                }
                else
                {
                    return errorMessage;
                }
            }
            else
            {
                errorMessage = "Ожидалась ( (" + currentIndex + ")";
            }

            return errorMessage;
        }

        string ForCheck()
        {
            var errorMessage = "";

            GetNextToken();
            if (token.TokenType == TokenType.PARANTHESIS_START)
            {
                GetNextToken();
                if (token.TokenType == TokenType.KEYWORD_LANG && token.Value == "var")
                {
                    GetNextToken();
                    if (token.TokenType == TokenType.VARIABLE)
                    {
                        currentIndex--; //Уменьшаем currentIndexб потому что дальше в Assign он будет сразу увеличен
                        errorMessage = Assign();
                        if (errorMessage.Equals(""))
                        {
                            currentIndex--;//Уменьшаем currentIndex, потому что дальше он будет увеличен в CheckBoolExpression
                            errorMessage = CheckBoolExpression();
                            if (errorMessage.Equals(""))
                            {
                                if (token.TokenType == TokenType.SEMICOLON)
                                {
                                    errorMessage = Assign();
                                    if (errorMessage.Equals(""))
                                    {
                                        if (Tokens[currentIndex - 1].TokenType == TokenType.PARANTHESIS_END)
                                        {
                                            if (token.TokenType == TokenType.CODEBLOCK_START)
                                            {
                                                errorMessage = CodeBlock();//Символ }
                                                GetNextToken();
                                            }
                                            else
                                            {
                                                errorMessage = "Ожидался символ { после объявления цикла for (" + currentIndex + ")";
                                            }
                                        }
                                        else
                                        {
                                            errorMessage = "Ожидался символ ) в цикле for (" + currentIndex + ")";
                                        }
                                    }
                                    else
                                    {
                                        errorMessage = "Ошибка в записи инкремента/декремента for (" + currentIndex + ")";
                                    }
                                }
                                else
                                {
                                    errorMessage = "Ожидался символ ; в цикле for (" + currentIndex + ")";
                                }
                            }
                        }
                        else
                        {
                            return errorMessage + currentIndex;
                        }
                    }
                    else
                    {
                        errorMessage = "Ожидался идентификатор переменной (" + currentIndex + ")";
                    }
                }
                else
                {
                    errorMessage = "Ожидалось объявление переменной в цикле for (" + currentIndex + ")";
                }
            }
            else
            {
                errorMessage = "Ожидался символ ( после for (" + currentIndex + ")";
            }

            return errorMessage;
        }

        string KeywordLang(Token tok)
        {
            string errorMessage = "";
            switch (tok.Value)
            {
                case "if": errorMessage = IfCheck();
                    break;
                case "for": errorMessage = ForCheck(); break;
                case "var": errorMessage = Assign(); break;//После 
                case "console.log": errorMessage = ConsoleLog(); break;

            }

            return errorMessage;
        }

        string CodeBlock()
        {
            string errorMessage = "";
            GetNextToken();
            while (token.TokenType != TokenType.CODEBLOCK_END)
            {
                switch (token.TokenType)
                {
                    case TokenType.KEYWORD_LANG:
                        {
                            errorMessage = KeywordLang(token);
                            if (!errorMessage.Equals(""))
                                return errorMessage;
                            break;
                        }
                    case TokenType.VARIABLE:
                        {
                            //Уменьшаем currentIndex, потому что потом увеличим его в Assign
                            currentIndex--;
                            errorMessage = Assign();
                            if (!errorMessage.Equals("")) { return errorMessage; }
                            break;
                        }
                    default:
                        {
                            return "Неожиданный символ после { (" + currentIndex + ")";
                        }
                }
            }

            return errorMessage;
        }

        public string Program()
        {
            string errorMessage = "";
            bool valid = false;
            if (token.Value.Equals("function") && token.TokenType == TokenType.KEYWORD_LANG)
            {
                GetNextToken();
                if (token.Value.Equals("main") && token.TokenType == TokenType.VARIABLE)
                {
                    GetNextToken();
                    if (token.Value.Equals("(") && token.TokenType == TokenType.PARANTHESIS_START)
                    {
                        GetNextToken();
                        if (token.TokenType == TokenType.VARIABLE)
                        {
                            GetNextToken();
                            if (token.TokenType == TokenType.PARANTHESIS_END)
                            {
                                GetNextToken();
                                if (token.TokenType == TokenType.CODEBLOCK_START)
                                {
                                    errorMessage = CodeBlock();
                                    if (errorMessage.Equals(""))
                                        errorMessage = "Синтаксический разбор выполнен без ошибок";
                                }
                                else
                                {
                                    valid = false;
                                    errorMessage = "Ожидалось начало функции и символ { (" + currentIndex + ")";
                                }
                            }
                            else
                            {
                                errorMessage = "Ожидался символ ) (" + currentIndex + ")";
                                valid = false;
                            }
                        }
                        else if (token.TokenType == TokenType.PARANTHESIS_END)
                        {
                            GetNextToken();
                            if (token.TokenType == TokenType.CODEBLOCK_START)
                            {
                                errorMessage = CodeBlock();
                                valid = true;
                                if(errorMessage.Equals(""))
                                    errorMessage = "Синтаксический разбор выполнен без ошибок";
                            }
                            else
                            {
                                errorMessage = "Ожидалось начало функции и символ { (" + currentIndex + ")";
                                valid = false;
                            }
                        }
                        else
                        {
                            errorMessage = "Ожидался параметр, или ) (" + currentIndex + ")";
                            valid = false;
                        }
                    }
                    else
                    {
                        errorMessage = "Ождалась ( (" + currentIndex + ")";
                        valid = false;
                    }
                }
                else
                {
                    errorMessage = "Ожидался идентификатор функции main (" + currentIndex + ")";
                    valid = false;
                }
            }
            else
            {
                errorMessage = "Ожидалось ключевое слово function (" + currentIndex + ")";
                valid = false;
            }

            return errorMessage;
        }
    }
}
