package calculator

// package calculator // only public functions and types are exported

// -v - verbose output
// go test -v playground/calculator/lexer_test.go
// OR
// go test -v  ./...

import "testing"

func TestSimpleLexerNextToken(t *testing.T) {
	tests := []struct {
		expectedType    TokenType
		expectedLiteral string
	}{
		{Number, "3"},
		{Plus, "+"},
		{Number, "5"},
		{Multiply, "*"},
		{Number, "10"},
		{EOF, ""},
	}

	lexer := NewLexer("3 + 5 * 10")

	for i, tt := range tests {
		token := lexer.NextToken()
		if token.Type != tt.expectedType {
			// %d - decimal integer
			t.Fatalf("tests[%d] - token type wrong. expected=%s, got=%s", i, tt.expectedType, token.Type)
		}

		if token.Literal != tt.expectedLiteral {
			t.Fatalf("tests[%d] - token literal wrong. expected=%s, got=%s", i, tt.expectedLiteral, token.Literal)
		}
	}
}

func TestLexerNextToken(t *testing.T) {
	tests := []struct {
		name   string
		input  string
		tokens []Token
	}{
		{
			name:  "addition",
			input: "3 + 5",
			tokens: []Token{
				{Type: Number, Literal: "3"},
				{Type: Plus, Literal: "+"},
				{Type: Number, Literal: "5"},
				{Type: EOF, Literal: ""},
			},
		},
		{
			name:  "precedence",
			input: "3 + 5 * 10",
			tokens: []Token{
				{Type: Number, Literal: "3"},
				{Type: Plus, Literal: "+"},
				{Type: Number, Literal: "5"},
				{Type: Multiply, Literal: "*"},
				{Type: Number, Literal: "10"},
				{Type: EOF, Literal: ""},
			},
		},
		{
			name:  "parentheses",
			input: "(3 + 5) * 10",
			tokens: []Token{
				{Type: LeftParen, Literal: "("},
				{Type: Number, Literal: "3"},
				{Type: Plus, Literal: "+"},
				{Type: Number, Literal: "5"},
				{Type: RightParen, Literal: ")"},
				{Type: Multiply, Literal: "*"},
				{Type: Number, Literal: "10"},
				{Type: EOF, Literal: ""},
			},
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			lexer := NewLexer(tt.input)

			for i, expectedToken := range tt.tokens {
				token := lexer.NextToken()

				if token.Type != expectedToken.Type {
					t.Fatalf("tests[%d] - token type wrong. expected=%s, got=%s", i, expectedToken.Type, token.Type)
				}

				if token.Literal != expectedToken.Literal {
					t.Fatalf("tests[%d] - token literal wrong. expected=%s, got=%s", i, expectedToken.Literal, token.Literal)
				}
			}
		})
	}
}
