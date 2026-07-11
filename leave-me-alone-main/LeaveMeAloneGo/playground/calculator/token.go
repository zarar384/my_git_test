package calculator

import "fmt"

type TokenType int

// TokenType represents the type of a lexical token.
const (
	Illegal TokenType = iota // iota - start from 0 and increment by 1 for each constant
	EOF                      // End of File

	// Literals
	Number

	// Operators
	Plus
	Minus
	Multiply
	Divide

	// Parentheses ()
	LeftParen
	RightParen
)

type Token struct {
	Type    TokenType
	Literal string
}

func (tt TokenType) String() string {
	switch tt {
	case Illegal:
		return "Illegal"
	case EOF:
		return "EOF"
	case Number:
		return "Number"
	case Plus:
		return "Plus"
	case Minus:
		return "Minus"
	case Multiply:
		return "Multiply"
	case Divide:
		return "Divide"
	case LeftParen:
		return "LeftParen"
	case RightParen:
		return "RightParen"
	default:
		return "Unknown"
	}
}

// String returns a string representation of the Token.
// example: "Plus('+')"
// $s - string
// $q - quoted string
func (t Token) String() string {
	return fmt.Sprintf("%s(%q)", t.Type, t.Literal)
}

// newToken creates a new Token with the given type and character.
func newToken(tokenType TokenType, ch byte) Token {
	return Token{
		Type:    tokenType,
		Literal: string(ch),
	}
}

// NextToken returns the next token from the input.
func (l *Lexer) NextToken() Token {
	var token Token

	switch l.ch {
	case '+':
		token = newToken(Plus, l.ch)
	case '-':
		token = newToken(Minus, l.ch)
	case '*':
		token = newToken(Multiply, l.ch)
	case '/':
		token = newToken(Divide, l.ch)
	case '(':
		token = newToken(LeftParen, l.ch)
	case ')':
		token = newToken(RightParen, l.ch)
	case 0:
		token = newToken(EOF, l.ch)
	default:
		token = Token{
			Type:    Illegal,
			Literal: string(l.ch),
		}
		return token
	}

	l.readChar()

	return token
}
