using System;
using System.Collections.Generic;
using System.Text;

namespace Scanner
{
    /// <summary>
    /// Class that scans text
    /// </summary>
    public class Scaner
    {
        /// <summary>
        /// Token list that's received as a result of text scan
        /// </summary>
        public readonly List<Token> ResultTokens = new List<Token>();
        public readonly List<Variable> constsAndVariables = new List<Variable>();

        /// <summary>
        /// A text to be scanned
        /// </summary>
        public readonly string TextToScan;

        private readonly StringBuilder currentChain = new StringBuilder();
        private int currentPosition = 0;

        /// <summary>
        /// Currently searched char
        /// </summary>
        private char currentChar
        {
            get
            {
                return TextToScan[currentPosition];
            }
        }

        /// <summary>
        /// Creating Scanner object
        /// </summary>
        /// <param name="text">Text to parse</param>
        public Scaner(string text)
        {
            this.TextToScan = text;
        }

        public string ScanText ()
        {
            try
            {
                for (currentPosition = 0; currentPosition < TextToScan.Length; currentPosition++)
                {
                    if (!Dictionaries.UnresolvedSymbols.Contains(currentChar.ToString()))
                    {
                        SkipWhitespaces();
                        if (!ScanForComment())
                        {
                            ScanForMeaningfulDelimeters();
                            ScanForNumericConstant();
                            ScanForKeywordsAndIdentifiers();
                            ScanForStringConstants();
                        }
                    }
                    else
                    {
                        throw new UnsupportedSymbolException("Unsupported symbol on position " + currentPosition);
                    }
                }
                return "";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Adds current char to chain and advance position
        /// </summary>
        private void AddCharToChainAndAdvance()
        {
            currentChain.Append(currentChar);
            currentPosition++;
        }

        /// <summary>
        /// Add all digits to current chain
        /// </summary>
        private void AddAllDigitsToCurrentChain()
        {
            while (currentChar.IsDigit())
            {
                AddCharToChainAndAdvance();
            }
        }

        /// <summary>
        /// Adding dot to chain
        /// </summary>
        private void AddDotToCurrentChain()
        {
            currentChain.Append(".");
            currentPosition++;
        }

        /// <summary>
        /// Adding comment to chain
        /// </summary>
        private void AddCommentToChain()
        {
            while (currentChar != '\n')
            {
                currentChain.Append(currentChar);
                currentPosition++;
            }

            Token newToken = new Token(TokenType.COMMENT, currentChain.ToString());
            ResultTokens.Add(newToken);
            currentChain.Clear();
        }

        /// <summary>
        /// Skipping whitespaces
        /// </summary>
        private void SkipWhitespaces()
        {
            while (currentChar.IsWhiteSpace())
            {
                currentPosition++;
            }
        }

        /// <summary>
        /// Scan For comment token
        /// </summary>
        private bool ScanForComment()
        {
            if(currentChar == '/' && TextToScan[currentPosition+1] == '/')
            {
                AddCommentToChain();
                return true;
            }

            return false;
        }

        private void ScanForNumericConstant()
        {
            bool isMinusEncountered = false;
            if (currentChar == '-')
            {
                AddCharToChainAndAdvance();
                isMinusEncountered = true;
            }

            if (currentChar.IsDigit())
            {
                CreateNumericConstToken();
            }
            else if (isMinusEncountered) // if we encountered minus sign, digits should follow them. If that's not happening, it's parse error in terms of language.
            {
                throw new IncorrectSyntaxException("Parse error: after '-' sign there's no digit. It's not possible");
            }
        }

        private void CreateNumericConstToken()
        {
            AddAllDigitsToCurrentChain();

            if (currentChar.IsDoubleConstDelimeter())
            {
                AddDotToCurrentChain();
                AddAllDigitsToCurrentChain();
                if(currentChar == '.')
                {
                    throw new UnrecognizedDoubleException("Unrecognized double variable on position " + currentPosition);
                }
                AddTokenFromCurrentChainValue(TokenType.VAR_CONST);
                
                currentPosition--;
            }
            else
            {
                AddTokenFromCurrentChainValue(TokenType.VAR_CONST);
                currentPosition--;
            }
        }

        /// <summary>
        /// Search meaningful delimiters
        /// </summary>
        private void ScanForMeaningfulDelimeters()
        {
            switch (currentChar)
            {
                case '{': AddTokenWithCurrentCharValue(TokenType.CODEBLOCK_START); break;
                case '}': AddTokenWithCurrentCharValue(TokenType.CODEBLOCK_END); break;
                case '/': AddTokenWithCurrentCharValue(TokenType.DIV); break;
                case '%': AddTokenWithCurrentCharValue(TokenType.MOD); break;
                case '*': AddTokenWithCurrentCharValue(TokenType.MULTIPLY);break;
                case '+': AddTokenWithCurrentCharValue(TokenType.PLUS); break;
                case '-': AddTokenWithCurrentCharValue(TokenType.MINUS); break;
                case ';': AddTokenWithCurrentCharValue(TokenType.SEMICOLON); break;
                case '(': AddTokenWithCurrentCharValue(TokenType.PARANTHESIS_START); break;
                case ')': AddTokenWithCurrentCharValue(TokenType.PARANTHESIS_END); break;
                case '=':
                    {
                        AddMeaningfulDelimetersWithTwoCharacters('=', '=', TokenType.ASSIGN, TokenType.EQUALS);
                        break;
                    }
                case '!':
                    {
                        AddMeaningfulDelimetersWithTwoCharacters('!', '=', TokenType.NOT, TokenType.NON_EQUALS);
                        break;
                    }
                case '<':
                    {
                        AddMeaningfulDelimetersWithTwoCharacters('<', '=', TokenType.LESS, TokenType.LESS_OR_EQUAL);
                        break;
                    }
                case '>':
                    {
                        AddMeaningfulDelimetersWithTwoCharacters('>', '=', TokenType.MORE, TokenType.MORE_OR_EQUAL);
                        break;
                    }
                case '|':
                    {
                        currentPosition++;
                        if (currentChar == '|')
                        {
                            ResultTokens.Add(new Token(TokenType.LOGICAL_OR, "||"));
                            break;
                        }
                        else throw new IncorrectSyntaxException("Error: unallowed symbol after '|' at position " + currentPosition);
                    }
                case '&':
                    {
                        currentPosition++;
                        if (currentChar == '&')
                        {
                            ResultTokens.Add(new Token(TokenType.LOGICAL_AND, "&&"));
                            break;
                        }
                        else throw new IncorrectSyntaxException("Error: unallowed symbol after '&' at position " + currentPosition);
                    }
            }
        }

        /// <summary>
        /// Scans for keywords (including '<see langword="true"/>' and '<see langword="false"/>) and identifiers
        /// </summary>
        private void ScanForKeywordsAndIdentifiers()
        {
            if ('a' <= currentChar && currentChar <= 'z')
            {
                while ('a' <= currentChar && currentChar <= 'z' || currentChar == '.' || '0' <=currentChar && currentChar <= '9')
                {
                    AddCharToChainAndAdvance();
                }

                if (currentChain.Length > 0)
                {
                    string chain = currentChain.ToString();

                    //Check for bool values
                    if (chain.Length >= 4)
                    {
                        switch (chain)
                        {
                            case "true":
                                {
                                    AddTokenFromCurrentChainValue(TokenType.BOOLEAN_TRUE);
                                    currentPosition--;
                                    return;
                                }
                            case "false":
                                {
                                    AddTokenFromCurrentChainValue(TokenType.BOOLEAN_FALSE);
                                    currentPosition--;
                                    return;
                                }
                        }
                    }

                    //Check for language keywords
                    if (Dictionaries.LanguageKeywords.Contains(chain))
                    {
                        AddTokenFromCurrentChainValue(TokenType.KEYWORD_LANG);
                        currentPosition--;
                        return;
                    }

                    //Variable in other cases
                    constsAndVariables.Add(new Variable(chain, false));
                    AddTokenFromCurrentChainValue(TokenType.VARIABLE);
                    currentPosition--;
                }
            }
        }

        /// <summary>
        /// Search currentChain for text constants
        /// </summary>
        private void ScanForStringConstants()
        {
            if (currentChar == '"')
            {
                do
                {
                    AddCharToChainAndAdvance();
                }
                while (currentChar != '"' || TextToScan[currentPosition-1] == '\\');
                AddCharToChainAndAdvance();
                currentPosition--;
                AddTokenFromCurrentChainValue(TokenType.VAR_CONST);
            }
        }

        /// <summary>
        /// Adds delimeter with 2 characters or 1 (e.g. = or ==, ! or !=)
        /// </summary>
        private void AddMeaningfulDelimetersWithTwoCharacters(char firstChar, char secondChar, TokenType caseOneChar, TokenType caseTwoChars)
        {
            //currentPosition++;
            if (TextToScan[currentPosition + 1] == secondChar)
            {
                string tokenValue = firstChar.ToString() + secondChar.ToString();
                ResultTokens.Add(new Token(caseTwoChars, tokenValue));
                currentPosition++;
            }
            else ResultTokens.Add(new Token(caseOneChar, firstChar.ToString()));
        }

        /// <summary>
        /// Add the token with current char value to results token set
        /// </summary>
        /// <param name="tokenType"></param>
        private void AddTokenWithCurrentCharValue(TokenType tokenType)
        {
            Token newToken = new Token(tokenType, currentChar.ToString());
            ResultTokens.Add(newToken);
            if(TextToScan[currentPosition +1].IsWhiteSpace())
                currentPosition++;
        }

        /// <summary>
        /// Add the token with current chain value to results token set and clears the chain.
        /// </summary>
        /// <param name="tokenType"></param>
        private void AddTokenFromCurrentChainValue(TokenType tokenType)
        {
            Token newToken = new Token(tokenType, currentChain.ToString());
            ResultTokens.Add(newToken);
            currentChain.Clear();
        }
    }
}
