using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// A text to be scanned
        /// </summary>
        public readonly string TextToScan;

        private StringBuilder currentChain = new StringBuilder();
        private int currentPosition = 0;

        private char currentChar
        {
            get
            {
                return TextToScan[currentPosition];
            }
        }

        public Scaner(string text)
        {
            this.TextToScan = text;
        }

        public void ScanText ()
        {
            while (currentPosition < TextToScan.Length)
            {
                SkipWhitespaces();
                if (currentChar == '-' || currentChar.IsDigit()) // done
                {
                    ScanForNumericConstant();
                }
                else if (char.IsLetter(currentChar)) // done
                {
                    ScanForKeywordsAndIdentifiers();
                }
                else if (currentChar == '"') // done
                {
                    ScanForStringConstants();
                }
                else // done
                {
                    ScanForMeaningfulDelimeters();
                }
            }
        }

        /// <summary>
        /// Adds current char to chain and advance position
        /// </summary>
        private void AddCharToChainAndAdvance ()
        {
            currentChain.Append(currentChar);
            currentPosition++;
        }

        private void SkipWhitespaces()
        {
            while (currentChar.IsWhiteSpace())
            {
                currentPosition++;
            }
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
                throw new Exception("Parse error: after '-' sign there's no digit. It's not possible");
            }
        }

        private void CreateNumericConstToken ()
        {
            AddAllDigitsToCurrentChain();

            if (currentChar.IsDoubleConstDelimeter())
            {
                AddAllDigitsToCurrentChain();
                AddTokenFromCurrentChainValue(TokenType.DOUBLE_CONST);
            }
            else
            {
                AddTokenFromCurrentChainValue(TokenType.INT_CONST);
            }

            if (char.IsLetter(currentChar))
            {
                ThrowParseErrorException("Letters are not allowed in numeric constants declaration");
            }
        }

        private void AddAllDigitsToCurrentChain()
        {
            while (currentChar.IsDigit())
            {
                AddCharToChainAndAdvance();
            }
        }

        private void ScanForMeaningfulDelimeters ()
        {
            switch (currentChar)
            {
                case '{': ; AddTokenWithCurrentCharValue(TokenType.CODEBLOCK_START); break;
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

                        }
                        else ThrowParseErrorException("'|' expected");
                        break;
                    }
                case '&':
                    {
                        currentPosition++;
                        if (currentChar == '&')
                        {
                            ResultTokens.Add(new Token(TokenType.LOGICAL_AND, "&&"));
                            break;
                        }
                        else ThrowParseErrorException("'&' expected");
                        break;
                    }
            }
            currentPosition++;
        }

        /// <summary>
        /// Scans for keywords (including '<see langword="true"/>' and '<see langword="false"/>) and identifiers
        /// </summary>
        private void ScanForKeywordsAndIdentifiers()
        {
            while (currentChar <= 'z' && currentChar >= 'a')
            {
                AddCharToChainAndAdvance();
            }

            if (currentChain.Length > 0)
            {
                string chain = currentChain.ToString();
                switch (chain)
                {
                    case "true":
                        {
                            AddTokenFromCurrentChainValue(TokenType.TRUE);
                            return;
                        }
                    case "false":
                        {
                            AddTokenFromCurrentChainValue(TokenType.FALSE);
                            return;
                        }
                }

                Token languageKeyword = Dictionaries.LanguageKeywords.SingleOrDefault(x => x.Value == chain);
                if (languageKeyword != null)
                {
                    AddTokenFromCurrentChainValue(languageKeyword.TokenType);
                    return;
                }

                AddTokenFromCurrentChainValue(TokenType.VARIABLE);
            }
        }

        private void ScanForStringConstants()
        {
            do
            {
                AddCharToChainAndAdvance();
            }
            while (currentChar != '"');
            AddCharToChainAndAdvance();
            AddTokenFromCurrentChainValue(TokenType.STRING_CONST);
        }

        /// <summary>
        /// Adds delimeter with 2 characters or 1 (e.g. = or ==, ! or !=)
        /// </summary>
        private void AddMeaningfulDelimetersWithTwoCharacters (char firstChar, char secondChar, TokenType caseOneChar, TokenType caseTwoChars)
        {
            currentPosition++;
            if (currentChar == secondChar)
            {
                string tokenValue = firstChar.ToString() + secondChar.ToString();
                ResultTokens.Add(new Token(caseTwoChars, tokenValue));
            }
            else ResultTokens.Add(new Token(caseOneChar, firstChar.ToString()));
        }

        /// <summary>
        /// Add the token with current char value to results token set
        /// </summary>
        /// <param name="tokenType"></param>
        private void AddTokenWithCurrentCharValue (TokenType tokenType)
        {
            Token newToken = new Token(tokenType, currentChar.ToString());
            ResultTokens.Add(newToken);
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

        private void ThrowParseErrorException (string message)
        {
            throw new ParseErrorException(currentPosition, message);
        }
    }
}