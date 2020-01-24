using Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHarpInterpreter
{
    class ReversePolishNotation
    {
        List<LexemPriority> priorityTable = new List<LexemPriority>();
        Stack<Token> rpnStack = new Stack<Token>();
        List<Token> markTable = new List<Token>();

        public List<Token> rpn = new List<Token>();

        string buff_st;
        int buff_int;
        Token buff_lex, buff_lex2, pc;
        LexemPriority l_p, l_p1;
        int ind = 0;
        List<string> print_op = new List<string> { "+", "-", "*", "/", "=", "==", "!=", ">", "<", ">=", "<=" };
        Token token = new Token();
        public List<Token> my_lexx = new List<Token>();

        public void CreatePriorTable()
        {
            //0 приоритет
            l_p = new LexemPriority();
            l_p.Priority = 0;
            l_p.Name = "(";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 0;
            l_p.Name = "{";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 0;
            l_p.Name = "if";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 0;
            l_p.Name = "for";
            priorityTable.Add(l_p);

            //1 приоритет
            l_p = new LexemPriority();
            l_p.Priority = 1;
            l_p.Name = ")";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 1;
            l_p.Name = "}";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 1;
            l_p.Name = ";";
            priorityTable.Add(l_p);

            //2 приоритет
            l_p = new LexemPriority();
            l_p.Priority = 2;
            l_p.Name = "=";
            priorityTable.Add(l_p);

            //3 приоритет
            l_p = new LexemPriority();
            l_p.Priority = 3;
            l_p.Name = "||";
            priorityTable.Add(l_p);

            //4 приоритет
            l_p = new LexemPriority();
            l_p.Priority = 4;
            l_p.Name = "&&";
            priorityTable.Add(l_p);

            //5 приоритет
            l_p = new LexemPriority();
            l_p.Priority = 5;
            l_p.Name = "==";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 5;
            l_p.Name = "!=";
            priorityTable.Add(l_p);

            //5 приоритет
            l_p = new LexemPriority();
            l_p.Priority = 6;
            l_p.Name = ">";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 6;
            l_p.Name = "<";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 6;
            l_p.Name = ">=";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 6;
            l_p.Name = "<=";
            priorityTable.Add(l_p);

            //7 приоритет
            l_p = new LexemPriority();
            l_p.Priority = 7;
            l_p.Name = "+";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 7;
            l_p.Name = "-";
            priorityTable.Add(l_p);

            //8 приоритет
            l_p = new LexemPriority();
            l_p.Priority = 8;
            l_p.Name = "*";
            priorityTable.Add(l_p);

            l_p = new LexemPriority();
            l_p.Priority = 8;
            l_p.Name = "/";
            priorityTable.Add(l_p);
        }

        void GetNextLexem()
        {
            if(my_lexx.Count > ind)
            {
                do
                {
                    token = my_lexx.ElementAt(ind);
                    ind++;
                }
                while (my_lexx[ind - 1].TokenType == TokenType.COMMENT);
            }
        }

        public void StartOPZ(List<Token> lexx, List<Variable> lexx_id_con)
        {
            int i = 0;
            my_lexx = lexx;

            CreatePriorTable();

            int start_index = 0;
            start_index = my_lexx.FindIndex(a => a.Value == "{");
            ind = start_index;

            for(i = start_index + 1; i < lexx.Count - 1; i++)
            {
                GetNextLexem();
                if(token.TokenType == TokenType.VARIABLE || token.TokenType == TokenType.VAR_CONST)//Если лексема - идентификатор или переменная, то добавляем её в список
                {
                    rpn.Add(token);
                }
                else//А если нет - то нет
                {
                    l_p = new LexemPriority();
                    l_p = priorityTable.Find(a => a.Name == token.Value);
                    l_p1 = new LexemPriority();

                    if(rpnStack.Count != 0)
                    {
                        buff_lex2 = new Token();
                        buff_lex2 = rpnStack.Peek();

                        l_p1 = priorityTable.Find(a => a.Name == buff_lex2.Value);
                    }

                    if(l_p.Priority == 0 || rpnStack.Count == 0 || l_p.Priority > l_p1.Priority)
                    {
                        rpnStack.Push(token);

                        if(token.Value == "for")//создаём метку для цикла
                        {
                            buff_lex = new Token();
                            buff_lex.Id = markTable.Count();
                            buff_lex.Value = "m" + markTable.Count();
                            buff_lex.TokenType = TokenType.MARK;
                            buff_lex.Type = "metka";
                            markTable.Add(buff_lex);

                            buff_lex2 = new Token();
                            buff_lex2 = rpnStack.Peek();
                            buff_lex2.Type = buff_lex2.TokenType.ToString() + " " + buff_lex.Value;

                            buff_lex = new Token();
                            buff_lex.Id = markTable.Count() - 1;
                            buff_lex.Value = "m" + (markTable.Count() - 1);
                            buff_lex.Type = "metka";
                            buff_lex.Value = buff_lex.Value + ":";
                            rpn.Add(buff_lex);
                        }
                    }
                    else
                    {
                        do
                        {
                            buff_lex = rpnStack.Peek();
                            l_p1 = new LexemPriority();
                            l_p1 = priorityTable.Find(a => a.Name == buff_lex.Value);
                            if (l_p.Priority < l_p1.Priority)
                            {
                                buff_lex = rpnStack.Pop();
                                if (print_op.Contains(buff_lex.Value))
                                {
                                    rpn.Add(buff_lex);
                                }

                                l_p1 = new LexemPriority();
                                l_p1 = priorityTable.Find(a => a.Name == buff_lex.Value);
                            }
                        }
                        while (l_p.Priority <= l_p1.Priority);

                        if (token.Value == ")")
                        {
                            buff_lex = new Token();
                            buff_lex = rpnStack.Peek();
                            if(buff_lex.Value == "(")
                            {
                                rpnStack.Pop();
                            }
                        }

                        if(token.Value == ";")
                        {
                            buff_lex2 = rpnStack.Peek();
                            if(buff_lex2.Value == "if")
                            {
                                int buf1, buf2;
                                buf1 = buff_lex2.Type.LastIndexOf(" ");
                                buf2 = buff_lex2.Type.Length - 1;
                                buff_st = buff_lex2.Type.Substring(buf1 + 1, buf2 - buf1);
                                buff_lex = new Token();
                                buff_lex = markTable.Find(a => a.Value == buff_st);
                                buff_lex2 = new Token();
                                buff_lex2.Id = buff_lex.Id;
                                buff_lex2.Value = buff_lex.Value;
                                buff_lex2.Type = buff_lex.Type;
                                buff_lex2.Value = buff_lex2.Value + ":";
                                rpn.Add(buff_lex2);
                                rpnStack.Pop();
                            }

                            if(buff_lex.Value == "for")
                            {
                                rpn.Add(pc);
                                rpn.Add(pc);


                            }
                        }
                    }
                }
            }
        }
    }
}
