package calculator

import "fmt"

type TokenType int

// TokenType represents the type of a lexical token.
const (
	Illegal TokenType = iota // iota - start from 0 and increment by 1 for each constant
	EOF

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
