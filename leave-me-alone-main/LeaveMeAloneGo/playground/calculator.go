package playground

import (
	"LeaveMeAloneGo/playground/calculator"
	"fmt"
)

func CalculatorDemo() {
	demoEvaluatePrefixExpression()
}

func printTokens(input string) {
	lexer := calculator.NewLexer(input)

	for {
		token := lexer.NextToken()
		fmt.Println(token)

		if token.Type == calculator.EOF {
			break
		}
	}
}

func demoPrintTokens() {
	printTokens("3 + 5 * (10 - 2) / 4")
}

func demoPrintOperators() {
	printTokens("+-*/()")
}

func demoNewParser() {
	lexer := calculator.NewLexer("2+3")
	parser := calculator.NewParser(lexer)

	fmt.Println(parser)
}

func demoParseExpression() {
	lexer := calculator.NewLexer("42")
	parser := calculator.NewParser(lexer)
	expression := parser.ParseExpression(calculator.Lowest)

	// %v - default format
	// %#v - Go-syntax representation of the value
	fmt.Printf("%#v\n", expression)
}

func demoParseGroupedExpression() {
	lexer := calculator.NewLexer("1 + 2 * 3")
	parser := calculator.NewParser(lexer)
	expression := parser.ParseExpression(calculator.Lowest)

	fmt.Printf("%#v\n", expression)
	// Print the expression in a human-readable format
	fmt.Println(expression)
}

func demoParsePrefixExpression() {
	lexer := calculator.NewLexer("-2 * 3")
	parser := calculator.NewParser(lexer)
	expression := parser.ParseExpression(calculator.Lowest)

	fmt.Printf("%#v\n", expression)
	// Print the expression in a human-readable format
	fmt.Println(expression)
}

func demoEvaluateNumberLiteral() {
	lexer := calculator.NewLexer("42")
	parser := calculator.NewParser(lexer)
	expression := parser.ParseExpression(calculator.Lowest)

	fmt.Printf("%#v\n", expression)
	fmt.Println("Evaluated Result:", calculator.Evaluate(expression))
}

func demoEvaluateBinaryExpression() {
	lexer := calculator.NewLexer("1 + 2 * 3") // (1 + (2 * 3)) = 7
	parser := calculator.NewParser(lexer)
	expression := parser.ParseExpression(calculator.Lowest)

	fmt.Printf("%#v\n", expression)
	fmt.Printf("%s = %d\n", expression, calculator.Evaluate(expression))
}

func demoEvaluatePrefixExpression() {
	lexer := calculator.NewLexer("-5 + 3*2") // ((-5) + (3 * 2)) = 1
	parser := calculator.NewParser(lexer)
	expression := parser.ParseExpression(calculator.Lowest)

	fmt.Printf("%#v\n", expression)
	fmt.Printf("%s = %d\n", expression, calculator.Evaluate(expression))
}
