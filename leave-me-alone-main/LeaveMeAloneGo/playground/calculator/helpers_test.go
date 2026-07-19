package calculator

import "testing"

func parse(t *testing.T, input string) Expression {
	t.Helper() // Mark this function as a test helper

	lexer := NewLexer(input)
	parser := NewParser(lexer)

	expression := parser.ParseExpression(Lowest)

	if len(parser.Errors()) != 0 {
		t.Fatalf("parser returned %d errors", len(parser.Errors()))
	}

	return expression
}
