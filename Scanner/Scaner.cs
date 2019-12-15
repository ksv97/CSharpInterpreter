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
        /// Variables list that's received as a result of text scan.
        /// </summary>
        public readonly List<Variable> Variables = new List<Variable>();

        /// <summary>
        /// A text to be scanned
        /// </summary>
        public readonly string TextToScan;

        private StringBuilder currentChain = new StringBuilder();
        private int currentPosition = 0;
        private int currentLine = 0;

        private int currentTokenToSend = 0;

        private char currentChar
        {
            get
            {
                try
                {
                    return TextToScan[currentPosition];
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the next token from result list. Returns NULL if end of tokens list is reached.
        /// </summary>
        public Token NextToken
        {
            get
            {
                return currentTokenToSend == ResultTokens.Count ? null : ResultTokens[currentTokenToSend++];
            }
        }

        public Token PreviousToken
        {
            get
            {
                return currentTokenToSend == 0 ? null : ResultTokens[--currentTokenToSend];
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
                SkipComments();

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
            if (currentChar == '\r')
            {
                currentPosition++;
                if (currentChar == '\n')
                {
                    currentPosition++;
                    currentLine++;
                }
            }
        }

        private void SkipComments()
        {
            if (currentChar == '/')
            {
                currentPosition++;
                if (currentChar == '/')
                {
                    this.SkipSigleLineComment();
                    return;
                }

                if (currentChar == '*')
                {
                    this.SkipMultilineComment();
                    return;
                }

                //ThrowParseErrorException("Invalid token '/'");
                currentPosition--; // maybe it's not a comment, so return back
            }
        }

        private void SkipSigleLineComment()
        {
            while (currentChar != '\n')
            {
                currentPosition++;
            }
            currentPosition++;
        }

        private void SkipMultilineComment()
        {
            bool commentEnd = false;
            do
            {
                currentPosition++;
                if (currentChar == '*')
                {
                    currentPosition++;
                    if (currentChar == '/')
                    {
                        commentEnd = true;
                    }
                }
            } while (!commentEnd);
            currentPosition++;
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
                ThrowParseErrorException("After '-' sign there's no digit. It's not possible");
            }
        }

        private void CreateNumericConstToken ()
        {
            AddAllDigitsToCurrentChain();

            if (currentChar.IsDoubleConstDelimeter())
            {
                this.AddCharToChainAndAdvance();
                AddAllDigitsToCurrentChain();
                AddTokenFromCurrentChainValue(TokenType.DOUBLE_CONST);

                if (currentChar.IsDoubleConstDelimeter())
                {
                    ThrowParseErrorException("Second double const delimeter ('.') in one numer is not possible.");
                }
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
                            ResultTokens.Add(new Token(TokenType.LOGICAL_OR, "||", currentPosition, currentLine));

                        }
                        else ThrowParseErrorException("'|' expected");
                        break;
                    }
                case '&':
                    {
                        currentPosition++;
                        if (currentChar == '&')
                        {
                            ResultTokens.Add(new Token(TokenType.LOGICAL_AND, "&&", currentPosition, currentLine));
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

                Token languageKeyword = Dictionaries.LanguageKeywords.SingleOrDefault(x => x.Value == chain);
                if (languageKeyword != null)
                {
                    AddTokenFromCurrentChainValue(languageKeyword.TokenType);

                    CheckAllowedCharacterAfterKeyword();
                    return;
                }

                AddTokenFromCurrentChainValue(TokenType.VARIABLE);
                if (!this.Variables.Any(x => x.Name == chain))
                {
                    this.Variables.Add(new Variable(chain));
                }
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
                ResultTokens.Add(new Token(caseTwoChars, tokenValue, currentPosition, currentLine));
            }
            else
            {
                ResultTokens.Add(new Token(caseOneChar, firstChar.ToString(), currentPosition, currentLine));
                currentPosition--; // rollback position change in case we've found non-two characters delimeter.
            }
        }

        /// <summary>
        /// Add the token with current char value to results token set
        /// </summary>
        /// <param name="tokenType"></param>
        private void AddTokenWithCurrentCharValue (TokenType tokenType)
        {
            Token newToken = new Token(tokenType, currentChar.ToString(), currentPosition, currentLine);
            ResultTokens.Add(newToken);
        }

        /// <summary>
        /// Add the token with current chain value to results token set and clears the chain.
        /// </summary>
        /// <param name="tokenType"></param>
        private void AddTokenFromCurrentChainValue(TokenType tokenType)
        {
            Token newToken = new Token(tokenType, currentChain.ToString(), currentPosition, currentLine);
            ResultTokens.Add(newToken);
            currentChain.Clear();
        }

        private void CheckAllowedCharacterAfterKeyword ()
        {
            char[] charsAllowedAfterKeyword = { '(', '{', ';', ' ', '/', ')' };

            if (charsAllowedAfterKeyword.Any(x => x == currentChar))
            {
                return;
            }
            else ThrowParseErrorException("Bad character after keyword/identifier declaration");
        }

        private void ThrowParseErrorException (string message)
        {
            throw new ParseErrorException(currentPosition, message);
        }
    }
}